using System.Runtime.Serialization;

namespace Service.BrokerFeeApplier.Grpc.Models
{
    [DataContract]
    public class GetFeeApplicationsRequest
    {
        [DataMember(Order = 1)] public long LastId { get; set; }
        [DataMember(Order = 2)] public int BatchSize { get; set; }
    }
}