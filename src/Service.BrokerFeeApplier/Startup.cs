using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using MyJetWallet.Sdk.GrpcMetrics;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Service;
using Prometheus;
using ProtoBuf.Grpc.Server;
using Service.BrokerFeeApplier.Grpc;
using Service.BrokerFeeApplier.Modules;
using Service.BrokerFeeApplier.Services;
using SimpleTrading.BaseMetrics;
using SimpleTrading.ServiceStatusReporterConnector;
using Service.BrokerFeeApplier.Postgres;
using MyJetWallet.Sdk.Postgres;

namespace Service.BrokerFeeApplier
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
             services.ConfigureJetWallet<ApplicationLifetimeManager>(Program.Settings.ZipkinUrl);
            DatabaseContext.LoggerFactory = Program.LogFactory;
            services.AddDatabase(DatabaseContext.Schema, Program.Settings.PostgresConnectionString,
                o => new DatabaseContext(o));
            DatabaseContext.LoggerFactory = null;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureJetWallet(env, endpoints =>
            {
                endpoints.MapGrpcSchema<FeeApplicationService, IFeeApplicationService>();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureJetWallet();
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<ServiceBusModule>();
        }
    }
}
