using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.BrokerFeeApplier.Jobs;

namespace Service.BrokerFeeApplier
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly ServiceBusLifeTime _myServiceBusTcpClient;
        private readonly FeeApplierJob _feeApplierJob;

        public ApplicationLifetimeManager(
            IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeManager> logger,
            ServiceBusLifeTime myServiceBusTcpClient,
            FeeApplierJob feeApplierJob)
            : base(appLifetime)
        {
            _logger = logger;
            _myServiceBusTcpClient = myServiceBusTcpClient;
            this._feeApplierJob = feeApplierJob;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called");
            _myServiceBusTcpClient.Start();
            _logger.LogInformation("ServiceBusLifeTime is started");
            _feeApplierJob.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called");
            _myServiceBusTcpClient.Stop();
            _logger.LogInformation("ServiceBusLifeTime is stopped");
            _feeApplierJob.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called");
        }
    }
}
