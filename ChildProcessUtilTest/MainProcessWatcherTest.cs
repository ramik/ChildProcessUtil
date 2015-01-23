using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ChildProcessUtil;
using NUnit.Framework;

namespace ChildProcessUtilTest
{
    public class MainProcessWatcherTest
    {
        private const string Testpipe = "TestPipe";
        private const string Testpipe2 = "TestPipe2";
        private Process mainProcess;

        [SetUp]
        public void Init()
        {
            Task.Factory.StartNew(() => Program.StartAddPipe(Testpipe));
            Task.Factory.StartNew(() => Program.StartDeletePipe(Testpipe2));
            mainProcess = StartNotepadProcess();
        }

        [TearDown]
        public void CleanUp()
        {
            if (mainProcess.HasExited == false)
                mainProcess.Kill();
        }

        [TestCase]
        public void WhenMainprocessExistsKillChilds()
        {
            var process1 = StartNotepadProcess();
            var process2 = StartNotepadProcess();
            ProcessModuleTest.AddProcess(process1.Id);
            ProcessModuleTest.AddProcess(process2.Id);
            var watch = new MainProcessWatcher(mainProcess.Id, null);
            Assert.IsFalse(process1.HasExited);
            Assert.IsFalse(process1.HasExited);
            mainProcess.Kill();
            MainProcessWatcher.HandleMainprocessStatus(watch);
            Assert.IsTrue(process1.HasExited);
            Assert.IsTrue(process1.HasExited);
            ProcessModuleTest.DeleteProcess(process1.Id);
            ProcessModuleTest.DeleteProcess(process2.Id);
        }

        private static Process StartNotepadProcess()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "notepad.exe",
                WorkingDirectory = Environment.SystemDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };
            return Process.Start(psi);
        }
    }
}