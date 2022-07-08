using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Service.ChangeBalanceGateway.Client;
using Service.ClientWallets.Client;

namespace Service.BrokerFeeApplier.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterSpotChangeBalanceGatewayClient(Program.Settings.ChangeBalanceGatewayGrpcServiceUrl);

            builder.RegisterClientWalletsClientsWithoutCache(Program.Settings.ClientWalletsGrpcServiceUrl);
        }
    }
}