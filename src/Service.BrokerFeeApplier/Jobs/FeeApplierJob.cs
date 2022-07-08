using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using Service.BrokerFeeApplier.Postgres;
using Service.ChangeBalanceGateway.Grpc;
using Service.ChangeBalanceGateway.Grpc.Models;
using Service.ClientWallets.Grpc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Service.BrokerFeeApplier.Jobs
{
    public class FeeApplierJob : IDisposable
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ISpotChangeBalanceService _spotChangeBalanceService;
        private readonly IClientWalletService _clientWalletService;
        private readonly ILogger<FeeApplierJob> _logger;
        private readonly MyTaskTimer _timer;


        public FeeApplierJob(ILogger<FeeApplierJob> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ISpotChangeBalanceService spotChangeBalanceService,
            IClientWalletService clientWalletService)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _spotChangeBalanceService = spotChangeBalanceService;
            _clientWalletService = clientWalletService;
            _timer = new MyTaskTimer(typeof(FeeApplierJob),
                TimeSpan.FromSeconds(Program.Settings.ProcessingIntervalSec),
                logger, DoTime);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task DoTime()
        {
            using var activity = MyTelemetry.StartActivity("Handle new FeeApplication");
            try
            {
                var list = await _clientWalletService.GetWalletsByClient(new MyJetWallet.Domain.JetClientIdentity
                {
                    BrokerId = "jetwallet",
                    ClientId = Program.Settings.BrokerFeeId
                });

                var defaultWallet = list.Wallets.FirstOrDefault(e => e.IsDefault) ?? list.Wallets.FirstOrDefault();

                if (defaultWallet == null)
                {
                    throw new Exception($"Cannot found default wallet for Client: {Program.Settings.BrokerFeeId}");
                }

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var sw = new Stopwatch();
                sw.Start();

                var applications = await context.FeeApplication.Where(e =>
                    e.Status == Domain.Models.FireblocksWithdrawals.FireblocksFeeApplicationStatus.InProgress
                    ).Take(500).ToListAsync();

                foreach (var feeApplication in applications)
                    try
                    {
                        var request = new BlockchainFeeApplyGrpcRequest()
                        {
                            AssetSymbol = feeApplication.FeeAssetSymbol,
                            BrokerId = "jetwallet",
                            FeeAmount = feeApplication.FeeAmount,
                            FeeWalletId = defaultWallet.WalletId,
                            TransactionId = feeApplication.FeeApplicationIdInMe,
                            //WalletId = ,
                        };
                        var response = await _spotChangeBalanceService.BlockchainWithdrawalFeeApplyOnBrokerFeeAsync(request);

                        if (response.ErrorCode != ChangeBalanceGrpcResponse.ErrorCodeEnum.Ok)
                        {
                            throw new Exception("Unable to apply fees to broker acc due to: " + response.ErrorMessage + " " + $"Error code {response.ErrorCode} ");
                        }

                        feeApplication.Status = Domain.Models.FireblocksWithdrawals.FireblocksFeeApplicationStatus.Completed;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Can't apply broker fees. {application}", feeApplication.ToJson());
                    }

                context.UpdateRange(applications);
                await context.SaveChangesAsync();

                applications.Count.AddToActivityAsTag("FeeApplication-count");

                sw.Stop();
                if (applications.Count > 0)
                    _logger.LogInformation("Handled {countTrade} new FeeApplication. Time: {timeRangeText}",
                        applications.Count,
                        sw.Elapsed.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot Handle new FeeApplications");
                ex.FailActivity();

                throw;
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
