using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.BrokerFeeApplier.Grpc;

namespace Service.BrokerFeeApplier.Client
{
    [UsedImplicitly]
    public class BrokerFeeApplyerClientFactory: MyGrpcClientFactory
    {
        public BrokerFeeApplyerClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IFeeApplicationService GetFeeApplicationService() => CreateGrpcService<IFeeApplicationService>();
    }
}
