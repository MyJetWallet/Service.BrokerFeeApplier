using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Service.BrokerFeeApplier.Domain.Models.FireblocksWithdrawals
{
    [DataContract]
    public class FireblocksFeeApplication
    {
        [DataMember(Order = 1)]
        public string ExternalId
        {
            get;
            set;
        }

        [DataMember(Order = 2)]
        public string TransactionId
        {
            get;
            set;
        }

        [DataMember(Order = 3)]
        public decimal Amount
        {
            get;
            set;
        }

        [DataMember(Order = 4)]
        public string AssetSymbol
        {
            get;
            set;
        }

        [DataMember(Order = 5)]
        public string Comment
        {
            get;
            set;
        }

        [DataMember(Order = 7)]
        public DateTime EventDate
        {
            get;
            set;
        }

        [DataMember(Order = 8)]
        public decimal FeeAmount
        {
            get;
            set;
        }

        [DataMember(Order = 9)]
        public string FeeAssetSymbol
        {
            get;
            set;
        }

        [DataMember(Order = 10)]
        public string Network
        {
            get;
            set;
        }

        [DataMember(Order = 11)]
        public string InternalNote
        {
            get;
            set;
        }

        [DataMember(Order = 12)]
        public string DestinationAddress
        {
            get;
            set;
        }

        [DataMember(Order = 13)]
        public string DestinationTag
        {
            get;
            set;
        }

        [DataMember(Order = 14)]
        public FireblocksFeeApplicationStatus Status { get; set; }

        [DataMember(Order = 15)]
        public string FeeApplicationIdInMe { get; set; }

        [DataMember(Order = 16)]
        public FireblocksFeeApplicationType Type { get; set; }

    }
}
