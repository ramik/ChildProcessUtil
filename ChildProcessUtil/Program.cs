using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace ChildProcessUtil
{
    public class Program
    {
        private static void Main(string[] args)
        {
            new MainProcessWatcher(int.Parse(args[0]));
            StartAddPipe(args[1]);
            StartDeletePipe(args[2]);    
            Task.Delay(Timeout.Infinite).Wait();
        }

        internal static void StartAddPipe(string pipe)
        {
            StartServer(pipe, ProcessModule.AddProcess);
        }

        internal static void StartDeletePipe(string pipe)
        {
            StartServer(pipe, ProcessModule.DeletePRocess);
        }

        private static void StartServer(string pipeName, Func<int, string> handleFunc)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    using (var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut))
                    {
                        server.WaitForConnection();
                        using (var reader = new StreamReader(server))
                        using (var writer = new StreamWriter(server))
                        {
                            var line = reader.ReadLine();
                            writer.WriteLine(handleFunc(int.Parse(line)));
                        }
                    }
                }
            });
        }
    }
}