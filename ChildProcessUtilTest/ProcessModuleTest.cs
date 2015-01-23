using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using ChildProcessUtil;
using NUnit.Framework;

namespace ChildProcessUtilTest
{
    public class ProcessModuleTest
    {
        private const string Testpipe = "TestPipe";
        private const string Testpipe2 = "TestPipe2";

        [SetUp]
        public void Init()
        {
            // ProcessModule.ActiveProcesses = new List<int>();
            Task.Factory.StartNew(() => Program.StartAddPipe(Testpipe));
            Task.Factory.StartNew(() => Program.StartDeletePipe(Testpipe2));
        }

        [TestCase]
        public void DeleteFromProcessesRemovesFromProcessList()
        {
            Assert.AreEqual("1", AddProcess(1));
            Assert.AreEqual("1,2", AddProcess(2));
            Assert.AreEqual("1", DeleteProcess(2));
            Assert.AreEqual("", DeleteProcess(1));
        }

        [TestCase]
        public void AddToProcessesInsertsToProcessList()
        {
            Assert.AreEqual("1", AddProcess(1));
            Assert.AreEqual("1,2", AddProcess(2));
            Assert.AreEqual("2", DeleteProcess(1));
            Assert.AreEqual("", DeleteProcess(2));
        }

        internal static string AddProcess(int processId)
        {
            return SendReceiveProcessToWatcher(processId, Testpipe);
        }

        internal static string DeleteProcess(int processId)
        {
            return SendReceiveProcessToWatcher(processId, Testpipe2);
        }

        private static string SendReceiveProcessToWatcher(int processId, string pipe)
        {
            using (var add = new NamedPipeClientStream(pipe))
            {
                add.Connect();
                using (var streamReader = new StreamReader(add))
                using (var writer = new StreamWriter(add))
                {
                    writer.WriteLine(processId.ToString());
                    writer.Flush();
                    return streamReader.ReadLine();
                }
            }
        }
    }
}