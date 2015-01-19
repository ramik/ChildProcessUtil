using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChildProcessUtil;
using NUnit.Framework;
using ProcessModule = ChildProcessUtil.ProcessModule;

namespace ChildProcessUtilTest
{
    public class MainProcessWatcherTest
    {
        private Process mainProcess;

        [SetUp]
        public void Init()
        {
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
            ProcessModule.ActiveProcesses = new List<int> {process1.Id, process2.Id};
            var watch = new MainProcessWatcher(mainProcess.Id, null);
            Assert.IsFalse(process1.HasExited);
            Assert.IsFalse(process1.HasExited);
            mainProcess.Kill();
            MainProcessWatcher.HandleMainprocessStatus(watch);
            Assert.IsTrue(process1.HasExited);
            Assert.IsTrue(process1.HasExited);
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