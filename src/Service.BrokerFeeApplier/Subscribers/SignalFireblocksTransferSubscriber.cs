using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Newtonsoft.Json;
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

        public SignalFireblocksTransferSubscriber(ISubscriber<FireblocksWithdrawalSignal> subscriber,
            ILogger<SignalFireblocksTransferSubscriber> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
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

                var newApp = new FireblocksFeeApplicationEntity { 
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
                    FeeApplicationIdInMe = Guid.NewGuid().ToString(),
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
