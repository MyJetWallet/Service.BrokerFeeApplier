using System.Runtime.Serialization;

namespace Service.BrokerFeeApplier.Grpc.Models
{
    [DataContract]
    public class HelloMessage 
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}