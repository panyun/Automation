using System;
using System.Threading;

namespace Example.Tool.Proto2CS
{
    class Program
    {
        private const string outputNS = "RPCBus.Protos";
        private const string outputPath = "../RPCBus/Server/RPCBus.Server.Model/Proto/";

        static void Main(string[] args)
        {
            var t1 = DateTime.Now.Ticks;
            Thread.Sleep(2);
            var t2 = DateTime.Now.Ticks;
            var t3 = (t2- t1)/10000;
            int t = (int)0xFFFF;

            ET.InnerProto2CS.Proto2CS(outputNS, "../RPCBus/Proto/InnerMessage.proto", outputPath, "InnerOpcode", 60000);
            ET.InnerProto2CS.Proto2CS(outputNS, "../RPCBus/Proto/OuterMessage.proto", outputPath, "OuterOpcode", 61000);
            //ET.InnerProto2CS.Proto2CS("Example.Protos.Map", "../Example/Proto/OuterMessage_Designer.proto", outputPath, "OuterOpcode_Designer", 21000);
            //ET.InnerProto2CS.Proto2CS("Example.Protos.Battle", "../Example/Proto/OuterMessage_Controller.proto", outputPath, "OuterOpcode_Controller", 22000);
            //ET.InnerProto2CS.Proto2CS("Example.Protos.Client", "../Example/Proto/OuterMessage_Robot.proto", outputPath, "OuterOpcode_Robot", 23000);

            Console.WriteLine("导出完毕!");
            Environment.Exit(0);
        }
    }
}
