using System.IO;
using System.Net;
using ChildProcessUtil;
using NUnit.Framework;

namespace ChildProcessUtilTest
{
    public class ProcessModuleTest
    {
        [SetUp]
        public void Init()
        {
            Program.StartServer(40001);
        }

        [TearDown]
        public void CleanUp()
        {
            Program.StopServer(40001);
        }

        [TestCase]
        public void WhenProcessArrayIsEmpty()
        {
            Assert.AreEqual("[]", GetProcessList());
        }

        [TestCase]
        public void DeleteFromProcessesRemovesFromProcessList()
        {
            Assert.AreEqual("ok", AddProcess(1));
            Assert.AreEqual("ok", AddProcess(2));
            Assert.AreEqual("ok", DeleteProcess(2));
            Assert.AreEqual("[1]", GetProcessList());
        }

        [TestCase]
        public void PostToProcessesGetsAddsToProcessList()
        {
            Assert.AreEqual("ok", AddProcess(1));
            Assert.AreEqual("ok", AddProcess(2));
            Assert.AreEqual("[1,2]", GetProcessList());
        }

        private static string GetProcessList()
        {
            var request = (HttpWebRequest) WebRequest.Create(@"http://localhost:40001/process/list");
            request.Method = "GET";
            request.Accept = "*/*";

            return ReadResultFromRequest(request);
        }

        private static string AddProcess(int processId)
        {
            var request = (HttpWebRequest) WebRequest.Create(@"http://localhost:40001/process/" + processId);
            request.Method = "POST";
            request.Accept = "*/*";
            request.ContentLength = 0;
            return ReadResultFromRequest(request);
        }

        private static string DeleteProcess(int processId)
        {
            var request = (HttpWebRequest) WebRequest.Create(@"http://localhost:40001/process/" + processId);
            request.Method = "DELETE";
            request.Accept = "*/*";
            request.ContentLength = 0;
            return ReadResultFromRequest(request);
        }

        private static string ReadResultFromRequest(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}