using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Renac.ReminderService
{
    public class Worker : BackgroundService
    {

        private ServiceBusProcessor _contractReminderProcessor;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _azureServiceBusConnectionString;

        private readonly ILogger<Worker> _logger;
        private readonly IEnumerable<IScheduledTask> _tasks;
        private readonly SemaphoreSlim _executionSemaphore;
        private readonly ConcurrentBag<string> _runningTasks;
        private Timer _oneAmTimer;
        private Timer _minuteTimer;
        private bool _isDisposed;


        public Worker(
              ILogger<Worker> logger,
               IConfiguration configuration,
               IEnumerable<IScheduledTask> tasks,

               HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
            //_azureServiceBusConnectionString = _configuration.GetValue<string>("azureServiceBus");

            _tasks = tasks;
            _executionSemaphore = new SemaphoreSlim(10, 10);
            _runningTasks = new ConcurrentBag<string>();
            _isDisposed = false;


        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Task Scheduler Service started at: {Time}", DateTime.Now);

            var taskList = _tasks.ToList();
            _logger.LogInformation("Loaded {TaskCount} tasks", taskList.Count);

            foreach (var task in taskList)
            {
                _logger.LogInformation("Registered task: {TaskName} (1AM: {IsOneAm}, Minute: {IsMinute})",
                    task.Name, task.IsOneAmTask, task.IsMinuteTask);
            }

            ScheduleOneAmTasks();
            ScheduleMinuteTasks();

            return Task.CompletedTask;

        }

        private void ScheduleOneAmTasks()
        {
            var now = DateTime.Now;
            var nextOneAm = GetNextOneAm(now);
            var initialDelay = nextOneAm - now;

            _logger.LogInformation("Next 1 AM execution scheduled at: {NextOneAm} (in {DelayMinutes:F1} minutes)",
                nextOneAm, initialDelay.TotalMinutes);

            _oneAmTimer = new Timer(
                async _ => await ExecuteOneAmTasks(),
                null,
                initialDelay,
                TimeSpan.FromHours(24)
            );
        }

        private void ScheduleMinuteTasks()
        {
            var now = DateTime.Now;
            var secondsUntilNextMinute = 60 - now.Second;
            var initialDelay = TimeSpan.FromSeconds(secondsUntilNextMinute + 1);

            _logger.LogInformation("Next minute execution scheduled at: {NextMinute}",
                now.AddSeconds(secondsUntilNextMinute + 1).ToString("HH:mm:ss"));

            _minuteTimer = new Timer(
                async _ => await ExecuteMinuteTasks(),
                null,
                initialDelay,
                TimeSpan.FromMinutes(1)
            );
        }

        private async Task ExecuteOneAmTasks()
        {
            _logger.LogInformation("Starting 1 AM task batch at: {Time}", DateTime.Now);

            var oneAmTasks = _tasks.Where(t => t.IsOneAmTask).ToList();
            _logger.LogInformation("Executing {TaskCount} 1 AM tasks", oneAmTasks.Count);

            await ExecuteTasksInParallel(oneAmTasks);

            _logger.LogInformation("1 AM task batch completed at: {Time}", DateTime.Now);
        }

        private async Task ExecuteMinuteTasks()
        {
            try
            {
                var minuteTasks = _tasks.Where(t => t.IsMinuteTask).ToList();

                if (minuteTasks.Any())
                {
                    _logger.LogDebug("Executing {TaskCount} minute tasks at: {Time:HH:mm:ss}",
                        minuteTasks.Count, DateTime.Now);
                }

                await ExecuteTasksInParallel(minuteTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing minute tasks");
            }
        }

        private async Task ExecuteTasksInParallel(IEnumerable<IScheduledTask> tasks)
        {
            var taskList = tasks.ToList();
            if (!taskList.Any())
                return;

            var tasksToRun = taskList.Select(async task =>
            {
                if (!await _executionSemaphore.WaitAsync(0))
                {
                    _logger.LogWarning("Task '{TaskName}' skipped - maximum concurrent tasks limit reached", task.Name);
                    return;
                }

                try
                {
                    _runningTasks.Add(task.Name);
                    _logger.LogDebug("Starting task: {TaskName}", task.Name);

                    await task.ExecuteAsync(CancellationToken.None);

                    _logger.LogDebug("Completed task: {TaskName}", task.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing task: {TaskName}", task.Name);
                }
                finally
                {
                    _runningTasks.TryTake(out _);
                    _executionSemaphore.Release();
                }
            });

            await Task.WhenAll(tasksToRun);
        }

        private DateTime GetNextOneAm(DateTime currentTime)
        {
            var nextOneAm = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 1, 0, 0);
            return currentTime >= nextOneAm ? nextOneAm.AddDays(1) : nextOneAm;
        }

        public async Task<List<string>> GetRunningTasksAsync()
        {
            return _runningTasks.ToList();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task Scheduler Service stopping at: {Time}", DateTime.Now);

            _oneAmTimer?.Dispose();
            _minuteTimer?.Dispose();

            if (_runningTasks.Any())
            {
                _logger.LogInformation("Waiting for {TaskCount} running tasks to complete...", _runningTasks.Count);
                var maxWaitTime = TimeSpan.FromSeconds(30);
                var startTime = DateTime.Now;

                while (_runningTasks.Any() && DateTime.Now - startTime < maxWaitTime)
                {
                    await Task.Delay(100, cancellationToken);
                }

                if (_runningTasks.Any())
                {
                    _logger.LogWarning("{TaskCount} tasks did not complete within timeout", _runningTasks.Count);
                }
            }

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                _oneAmTimer?.Dispose();
                _minuteTimer?.Dispose();
                _executionSemaphore?.Dispose();
                _isDisposed = true;
            }
            base.Dispose();
        }

    }
}
