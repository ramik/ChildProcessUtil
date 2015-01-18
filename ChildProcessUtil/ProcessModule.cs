using System.Collections.Generic;
using Nancy;

namespace ChildProcessUtil
{
    public class ProcessModule : NancyModule
    {
        static ProcessModule()
        {
            ActiveProcesses = new List<int>();
        }

        public ProcessModule() : base("/process")
        {
            Get["/list"] = _ => ActiveProcesses;
            Post["/{process:int}"] = parameters =>
            {
                ActiveProcesses.Add(parameters.process);
                return "ok";
            };
            Delete["/{process:int}"] = parameters =>
            {
                ActiveProcesses.RemoveAll(x => x == parameters.process);
                return "ok";
            };
        }

        public static List<int> ActiveProcesses { get; private set; }
    }
}