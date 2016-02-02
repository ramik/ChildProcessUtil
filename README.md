ChildProcessUtil
================

This package is used to control, how child processes are closed when parent
process is killed. This is in production use (quite many installs within WPF
application).

How it works
------------

When main process starts, it loads child process util. This util watches, if
main process is still alive. When main process starts a child process, it
registers it's process ID to the this utility. If main process is killed, the
utility signals kill() to children and the terminates itself.

Interprocess communication
--------------------------

This utility uses named pipes to pass information between processes.

Why?
----

This is needed in our case, because child processes are doing long lasting
"computation". If main process dies (like using task manager), the results are
no longer needed. In windows, there is no way to tell (at least I'm not aware)
that when parent exists -- childs must exit, too.

Usage
-----

In our project, we have bundled the utility (exe).

### Starting the application 

In Application start we have simply:

AddingPipe and DeletingPipe are simply strings like:

public const string AddingPipe = "ProductProcessPipeAdd";

public const string DeletingPipe = "ProductProcessPipeDelete";

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
private static void StartProcessWatcher()
        {
            var assemblyInfo = Assembly.GetExecutingAssembly();
            var assemblyLocation = Path.GetDirectoryName(assemblyInfo.Location);
            var arguments = "" + Process.GetCurrentProcess().Id + " " + ProcessWatcherConnection.AddingPipe + " " +
                            ProcessWatcherConnection.DeletingPipe;
            var watcherProcess = new Process
            {
                StartInfo =
                    new ProcessStartInfo(assemblyLocation + @"\ChildProcessUtil_1_0_7.exe",
                        arguments)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    }
            };
            watcherProcess.Start();
        }
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

### In registering child process 

See tests for usage scenario:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
internal static string AddProcess(int processId)
        {
            return SendReceiveProcessToWatcher(processId, AddingPipe);
        }

        internal static string DeleteProcess(int processId)
        {
            return SendReceiveProcessToWatcher(processId, DeletingPipe);
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
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

>   Now when starting the process, call:  
>   var result = ProcessWatcherConnection.AddProcess(Process.Id);  
>   

>   After process completes, call:  
>   var result = ProcessWatcherConnection.DeleteProcess(Process.Id);

 

Background information
----------------------

[Stackoverflow thread][1]

[1]: <http://stackoverflow.com/questions/3342941/kill-child-process-when-parent-process-is-killed>

 
