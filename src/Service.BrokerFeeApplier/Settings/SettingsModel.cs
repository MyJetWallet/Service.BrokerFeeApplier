using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.BrokerFeeApplier.Settings
{
    public class SettingsModel
    {
        [YamlProperty("BrokerFeeApplier.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BrokerFeeApplier.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("BrokerFeeApplier.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("BrokerFeeApplier.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("BrokerFeeApplier.ChangeBalanceGatewayGrpcServiceUrl")]
        public string ChangeBalanceGatewayGrpcServiceUrl { get; set; }

        [YamlProperty("BrokerFeeApplier.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }

        [YamlProperty("BrokerFeeApplier.ProcessingIntervalSec")]
        public int ProcessingIntervalSec { get; set; }

        [YamlProperty("BrokerFeeApplier.BrokerFeeId")]
        public string BrokerFeeId { get; set; }

        [YamlProperty("BrokerFeeApplier.ClientWalletsGrpcServiceUrl")]
        public string ClientWalletsGrpcServiceUrl { get; internal set; }
    }
}

