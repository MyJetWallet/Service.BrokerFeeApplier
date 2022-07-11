using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.BrokerFeeApplier.Domain.Withdrawals
{
    public class FireblocksWithdrawalNoteService
    {
        private static Regex _regex = new Regex(@"manual[ ]*withdrawal:?[ ]*(\d+)", RegexOptions.IgnoreCase);
        public string GenerateManualNote(long withdrawalId)
        {
            return $"Manual Withdrawal: {withdrawalId}";
        }

        public long? GetWithdrawalIdFromNote(string note)
        {
            var matches = _regex.Matches(note);

            var groups = matches.FirstOrDefault()?.Groups?.Values?.LastOrDefault();

            if (groups == null)
                return null;

            if (long.TryParse(groups.Value, out var withdrawalId))
                return withdrawalId;

            return null;
        }
    }
}
