using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Service.ChangeBalanceGateway.Client;

namespace Service.BrokerFeeApplier.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterSpotChangeBalanceGatewayClient(Program.Settings.ChangeBalanceGatewayGrpcServiceUrl);
        }
    }
}