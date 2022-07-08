using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.BrokerFeeApplier.Jobs;
using Service.BrokerFeeApplier.Subscribers;
using Service.Fireblocks.Webhook.ServiceBus;
using Service.Fireblocks.Webhook.ServiceBus.Deposits;

namespace Service.BrokerFeeApplier.Modules
{
    public class ServiceBusModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(
                () => Program.Settings.SpotServiceBusHostPort,
                Program.LogFactory);

            builder.RegisterType<FeeApplierJob>().AsSelf().SingleInstance().AutoActivate();

            builder.RegisterMyServiceBusSubscriberSingle<FireblocksWithdrawalSignal>(serviceBusClient, 
                Topics.FireblocksWithdrawalSignalTopic,
              "broker-fee-applier",
              TopicQueueType.Permanent);

            builder.RegisterType<SignalFireblocksTransferSubscriber>().AsSelf().SingleInstance().AutoActivate();
        }
    }
}