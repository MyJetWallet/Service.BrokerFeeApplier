using Service.BrokerFeeApplier.Domain.Models.FireblocksWithdrawals;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.BrokerFeeApplier.Grpc.Models
{
    [DataContract]
    public class GetFeeApplicationsResponse
    {
        [DataMember(Order = 1)] public bool Success { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
        [DataMember(Order = 3)] public long IdForNextQuery { get; set; }
        [DataMember(Order = 4)] public List<FireblocksFeeApplication> Collection { get; set; }
    }
}