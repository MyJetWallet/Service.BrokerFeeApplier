using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Newtonsoft.Json;
using Service.BrokerFeeApplier.Domain.Models.FireblocksWithdrawals;
using Service.BrokerFeeApplier.Domain.Withdrawals;
using Service.BrokerFeeApplier.Postgres;
using Service.BrokerFeeApplier.Postgres.Models;
using Service.ChangeBalanceGateway.Grpc;
using Service.Fireblocks.Webhook.ServiceBus.Deposits;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Service.BrokerFeeApplier.Subscribers
{
    public class SignalFireblocksTransferSubscriber
    {
        private readonly ILogger<SignalFireblocksTransferSubscriber> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly FireblocksWithdrawalNoteService _fireblocksWithdrawalNoteService;

        public SignalFireblocksTransferSubscriber(ISubscriber<FireblocksWithdrawalSignal> subscriber,
            ILogger<SignalFireblocksTransferSubscriber> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            FireblocksWithdrawalNoteService fireblocksWithdrawalNoteService)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _fireblocksWithdrawalNoteService = fireblocksWithdrawalNoteService;
            subscriber.Subscribe(HandleSignal);
        }

        private async ValueTask HandleSignal(FireblocksWithdrawalSignal signal)
        {
            using var activity = MyTelemetry.StartActivity("Handle event FireblocksWithdrawalSignal");
            signal.AddToActivityAsJsonTag("fireblocks-withdrawal-signal");

            var logContext = JsonConvert.SerializeObject(signal);
            _logger.LogInformation("FireblocksWithdrawalSignal is received: {jsonText}", logContext);

            if (signal.Status == Fireblocks.Webhook.Domain.Models.Withdrawals.FireblocksWithdrawalStatus.Failed)
                return;

            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                long transferId = 0;
                var containsExternalId = signal.ExternalId != null && signal.ExternalId.Contains("fire_tx_");
                var containsNote = !string.IsNullOrEmpty(signal.InternalNote);
                var withdrawalIdExtracted = false;
                var idIsFromNote = false;
                string feeApplicationId = null;

                var type = FireblocksFeeApplicationType.TransferBetweenAccounts;
                if (signal.ExternalId.Contains("settl_"))
                {
                    type = FireblocksFeeApplicationType.Settlement;
                    var step1 = signal.ExternalId.Replace("settl_", "");
                    var ids = step1.Split('_', StringSplitOptions.RemoveEmptyEntries);

                    if (long.TryParse(ids[0], out transferId))
                        withdrawalIdExtracted = true;

                    feeApplicationId = $"Settlement|{transferId}|{Guid.NewGuid()}";
                }
                else if (containsExternalId || containsNote)
                {
                    if (containsExternalId)
                    {
                        var step1 = signal.ExternalId.Replace("fire_tx_", "");
                        var ids = step1.Split('_', StringSplitOptions.RemoveEmptyEntries);

                        if (long.TryParse(ids[0], out transferId))
                            withdrawalIdExtracted = true;
                    }

                    if (containsNote)
                    {
                        var extractedId = _fireblocksWithdrawalNoteService.GetWithdrawalIdFromNote(signal.InternalNote);

                        if (extractedId != null)
                        {
                            withdrawalIdExtracted = true;
                            transferId = extractedId.Value;
                            idIsFromNote = true;
                        }
                    }

                    if (withdrawalIdExtracted)
                    {
                        type = FireblocksFeeApplicationType.Withdrawal;
                        feeApplicationId = $"Withdrawal|{transferId}|{Guid.NewGuid()}";
                    }
                }

                if (string.IsNullOrEmpty(feeApplicationId))
                {
                    feeApplicationId = Guid.NewGuid().ToString();
                }

                var newApp = new FireblocksFeeApplicationEntity
                {
                    TransactionId = signal.TransactionId,
                    Amount = signal.Amount,
                    AssetSymbol = signal.AssetSymbol,
                    Comment = signal.Comment,
                    DestinationAddress = signal.DestinationAddress,
                    DestinationTag = signal.DestinationTag,
                    EventDate = signal.EventDate,
                    ExternalId = signal.ExternalId,
                    FeeAmount = signal.FeeAmount,
                    FeeAssetSymbol = signal.FeeAssetSymbol,
                    InternalNote = signal.InternalNote,
                    Network = signal.Network,
                    Status = Domain.Models.FireblocksWithdrawals.FireblocksFeeApplicationStatus.InProgress,
                    FeeApplicationIdInMe = feeApplicationId,
                    Type = type,
                };

                await context.FeeApplication.Upsert(newApp).On(x => x.TransactionId).NoUpdate().RunAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to handle FireblocksWithdrawalSignal; {logContext}", logContext);
            }
        }
    }
}
