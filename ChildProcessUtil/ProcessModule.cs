using System.Collections.Generic;
using System.Linq;

namespace ChildProcessUtil
{
    public class ProcessModule
    {
        static ProcessModule()
        {
            ActiveProcesses = new List<int>();
        }

        public static List<int> ActiveProcesses { get; set; }

        public static string AddProcess(int process)
        {
            ActiveProcesses.Add(process);
            return string.Join(",", ActiveProcesses.OrderBy(x => x));
        }

        public static string DeletePRocess(int process)
        {
            ActiveProcesses.RemoveAll(x => x == process);
            return string.Join(",", ActiveProcesses.OrderBy(x => x));
        }
    }
}