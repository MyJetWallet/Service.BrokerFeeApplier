using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.BrokerFeeApplier.Settings
{
    public class SettingsModel
    {
        [YamlProperty("BrokerFeeApplyer.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BrokerFeeApplyer.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("BrokerFeeApplyer.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("BrokerFeeApplyer.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("BrokerFeeApplyer.ChangeBalanceGatewayGrpcServiceUrl")]
        public string ChangeBalanceGatewayGrpcServiceUrl { get; set; }

        [YamlProperty("BrokerFeeApplyer.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }

        [YamlProperty("BrokerFeeApplyer.ProcessingIntervalSec")]
        public int ProcessingIntervalSec { get; set; }

        [YamlProperty("BrokerFeeApplyer.BrokerFeeId")]
        public string BrokerFeeId { get; set; }
    }
}

