using Service.BrokerFeeApplier.Domain.Models.FireblocksWithdrawals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.BrokerFeeApplier.Postgres.Models
{
    public class FireblocksFeeApplicationEntity : FireblocksFeeApplication
    {
        public long Id { get; set; }
    }
}
