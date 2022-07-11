using System.ServiceModel;
using System.Threading.Tasks;
using Service.BrokerFeeApplier.Grpc.Models;

namespace Service.BrokerFeeApplier.Grpc
{
    [ServiceContract]
    public interface IFeeApplicationService
    {

        [OperationContract]
        Task<GetFeeApplicationsResponse> GetFeeApplications(GetFeeApplicationsRequest request);
    }
}