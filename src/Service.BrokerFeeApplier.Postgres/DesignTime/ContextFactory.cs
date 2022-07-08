using MyJetWallet.Sdk.Postgres;
using Service.BrokerFeeApplier.Postgres;

namespace Service.BrokerFeeApplier.Postgres.DesignTime
{
    public class ContextFactory : MyDesignTimeContextFactory<DatabaseContext>
    {
        public ContextFactory() : base(options => new DatabaseContext(options))
        {
        }
    }
}