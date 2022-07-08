using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.TcpClient;
using Service.BrokerFeeApplier.Jobs;

namespace Service.BrokerFeeApplier.Modules
{
    public class ServiceBusModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusLogger = Program.LogFactory.CreateLogger(nameof(MyServiceBusTcpClient));

            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(
                () => Program.Settings.SpotServiceBusHostPort,
                Program.LogFactory);
            serviceBusClient.Log.AddLogException(ex => serviceBusLogger.LogInformation(ex, "Exception in MyServiceBusTcpClient"));
            serviceBusClient.Log.AddLogInfo(info => serviceBusLogger.LogDebug($"MyServiceBusTcpClient[info]: {info}"));
            serviceBusClient.SocketLogs.AddLogInfo((context, msg) => serviceBusLogger.LogInformation($"MyServiceBusTcpClient[Socket {context?.Id}|{context?.ContextName}|{context?.Inited}][Info] {msg}"));
            serviceBusClient.SocketLogs.AddLogException((context, exception) => serviceBusLogger.LogInformation(exception, $"MyServiceBusTcpClient[Socket {context?.Id}|{context?.ContextName}|{context?.Inited}][Exception] {exception.Message}"));
            builder.RegisterInstance(serviceBusClient).AsSelf().SingleInstance();

            builder.RegisterType<FeeApplierJob>().AsSelf().SingleInstance().AutoActivate();
        }
    }
}