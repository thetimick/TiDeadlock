using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TiDeadlock.Services.RunLoop;

public interface IRunLoopService
{
    Task Run();
}

public class RunLoopService(ILogger<RunLoopService> logger): IRunLoopService
{
    public async Task Run()
    {
        logger.LogInformation("[Run] Start");
        
        while (true)
        {
            var process = Process.GetProcessesByName("project8").FirstOrDefault();
            if (process == null)
            {
                Thread.Sleep(1000);
                continue;
            }
            
            logger.LogInformation("[Run] WaitForExitAsync");
            
            await process.WaitForExitAsync();
            
            
        }
        
        // ReSharper disable once FunctionNeverReturns
    }
}