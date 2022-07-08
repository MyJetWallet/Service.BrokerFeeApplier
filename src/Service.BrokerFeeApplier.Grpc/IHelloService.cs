using System.ServiceModel;
using System.Threading.Tasks;
using Service.BrokerFeeApplier.Grpc.Models;

namespace Service.BrokerFeeApplier.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}