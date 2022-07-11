using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.BrokerFeeApplier.Client;
using Service.BrokerFeeApplier.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new BrokerFeeApplyerClientFactory("http://localhost:5001");
            var client = factory.GetFeeApplicationService();

            var resp = await  client.GetFeeApplications(new (){BatchSize = 10});
            //Console.WriteLine(resp?.ToJson());

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
