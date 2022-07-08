using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using Service.BrokerFeeApplier.Postgres;
using Service.ChangeBalanceGateway.Grpc;
using Service.ChangeBalanceGateway.Grpc.Models;
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
        private readonly ILogger<FeeApplierJob> _logger;
        private readonly MyTaskTimer _timer;


        public FeeApplierJob(ILogger<FeeApplierJob> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ISpotChangeBalanceService spotChangeBalanceService)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            this._spotChangeBalanceService = spotChangeBalanceService;
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
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var sw = new Stopwatch();
                sw.Start();

                var applications = await context.FeeApplication.Where(e =>
                    e.Status == Domain.Models.FireblocksWithdrawals.FireblocksFeeApplicationStatus.InProgress
                    ).Take(100).ToListAsync();

                foreach (var feeApplication in applications)
                    try
                    {
                        var request = new BlockchainFeeApplyGrpcRequest()
                        {
                            AssetSymbol = feeApplication.FeeAssetSymbol,
                            BrokerId = "jetwallet",
                            FeeAmount = feeApplication.FeeAmount,
                            FeeWalletId = Program.Settings.BrokerFeeId,
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
