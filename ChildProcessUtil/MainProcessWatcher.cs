using System;
using System.Diagnostics;
using System.Threading;

namespace ChildProcessUtil
{
    internal class MainProcessWatcher
    {
        private Timer Ticker;

        internal MainProcessWatcher(int processId)
        {
            ProcessId = processId;
            Ticker = new Timer(TimerMethod, this, 1000, 1000);
        }

        internal MainProcessWatcher(int processId, Timer timer)
        {
            ProcessId = processId;
        }

        public int ProcessId { get; private set; }

        public static void TimerMethod(object state)
        {
            var exit = HandleMainprocessStatus((MainProcessWatcher) state);
            if (exit)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        internal static bool HandleMainprocessStatus(MainProcessWatcher state)
        {
            var isMainAlive = IsMainProcessAlive(state);
            if (!isMainAlive)
                KillChildProcesses();
            return !isMainAlive;
        }

        private static bool IsMainProcessAlive(MainProcessWatcher watcher)
        {
            try
            {
                Process.GetProcessById(watcher.ProcessId);
                Console.WriteLine("MainProcess is alive");
                return true;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("MainProcess exited");
                return false;
            }
        }

        private static void KillChildProcesses()
        {
            foreach (var childProcessId in ProcessModule.ActiveProcesses)
            {
                try
                {
                    Process.GetProcessById(childProcessId).Kill();
                }
                catch (ArgumentException)
                {
                    // ignored, as Process not found 
                }
            }
        }
    }
}