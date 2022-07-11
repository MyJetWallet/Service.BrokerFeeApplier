using Autofac;
using Service.BrokerFeeApplier.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.BrokerFeeApplier.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBrokerFeeApplyerClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new BrokerFeeApplyerClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetFeeApplicationService()).As<IFeeApplicationService>().SingleInstance();
        }
    }
}
