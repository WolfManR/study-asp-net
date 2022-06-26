using MetricsManagement.Agent.Data;

using Quartz;

using System.Diagnostics;

namespace MetricsManagement.Agent.Jobs;

[DisallowConcurrentExecution]
public class ProcessorTimeJob : IJob
{
    private readonly Repository _repository;
    private readonly ILogger<ProcessorTimeJob> _logger;
    private readonly PerformanceCounter _listener;

    public ProcessorTimeJob(Repository repository, ILogger<ProcessorTimeJob> logger)
    {
        _repository = repository;
        _repository.TableName = "processorTime";
        _logger = logger;
        _listener = new("Processor", "% Processor Time", "_total");
    }

    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            var value = Convert.ToInt32(_listener.NextValue());
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _repository.Create(value, time);
        }
        catch (OverflowException e)
        {
            _logger.LogError("Cant create processor time metric, metric value overflow limit of integer", e);
        }
        catch (Exception e)
        {
            _logger.LogError("Cant save processor time metric", e);
        }
        return Task.CompletedTask;
    }
}