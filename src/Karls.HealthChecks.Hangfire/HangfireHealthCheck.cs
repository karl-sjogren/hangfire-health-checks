using Hangfire.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Karls.HealthChecks.Hangfire;

internal class HangfireHealthCheck : IHealthCheck {
    private readonly HangfireOptions _hangfireOptions;
    private readonly IMonitoringApi _monitoringApi;

    public HangfireHealthCheck(HangfireOptions hangfireOptions, IMonitoringApi monitoringApi) {
        ArgumentNullException.ThrowIfNull(hangfireOptions);
        ArgumentNullException.ThrowIfNull(monitoringApi);

        _hangfireOptions = hangfireOptions;
        _monitoringApi = monitoringApi;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        try {
            var options = _hangfireOptions;
            List<string>? errors = null;

            if(options.MaximumJobsFailed.HasValue) {
                var failedCount = _monitoringApi.FailedCount();
                if(failedCount >= options.MaximumJobsFailed) {
                    errors ??= [];
                    errors.Add($"Hangfire has #{failedCount} failed jobs and the maximum allowed is {options.MaximumJobsFailed}.");
                }
            }

            if(options.MinimumAvailableServers.HasValue) {
                var serversCount = _monitoringApi.Servers().Count;
                if(serversCount < options.MinimumAvailableServers) {
                    errors ??= [];
                    errors.Add($"{serversCount} server registered. Expected minimum {options.MinimumAvailableServers}.");
                }
            }

            if(options.Queues?.Count > 0) {
                foreach(var queue in options.Queues) {
                    var enqueuedJobs = _monitoringApi.EnqueuedCount(queue.QueueName);
                    if(enqueuedJobs > queue.MaximumQueuedJobs) {
                        errors ??= [];
                        errors.Add($"Hangfire has {enqueuedJobs} queded jobs for the queue \"{queue.QueueName}\" and the maximum allowed is {queue.MaximumQueuedJobs}.");
                    }
                }
            }

            if(options.MaximumTotalQueuedJobs.HasValue) {
                var enqueuedJobs = _monitoringApi.Queues().Sum(queue => queue.Length);
                if(enqueuedJobs > options.MaximumTotalQueuedJobs) {
                    errors ??= [];
                    errors.Add($"Hangfire has a total of {enqueuedJobs} queued jobs and the maximum allowed is {options.MaximumTotalQueuedJobs}.");
                }
            }

            if(errors?.Count > 0) {
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, description: string.Join(" + ", errors)));
            }

            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "Successfully checked Hangfire and found no discrepancies."));
        } catch(Exception ex) {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
        }
    }
}
