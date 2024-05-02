using Karls.HealthChecks.Hangfire;

namespace Microsoft.Extensions.DependencyInjection;

public class HangfireOptionsBuilder {
    private readonly HangfireOptions _options;

    internal HangfireOptionsBuilder() {
        _options = new();
    }

    public HangfireOptionsBuilder MaximumJobsFailed(Int32 maximumJobsFailed) {
        _options.MaximumJobsFailed = maximumJobsFailed;
        return this;
    }

    public HangfireOptionsBuilder MinimumAvailableServers(Int32 minimumAvailableServers) {
        _options.MinimumAvailableServers = minimumAvailableServers;
        return this;
    }

    public HangfireOptionsBuilder MaximumTotalQueuedJobs(Int32 maximumTotalQueuedJobs) {
        _options.MaximumTotalQueuedJobs = maximumTotalQueuedJobs;
        return this;
    }

    public HangfireOptionsBuilder AddJobQueueCheck(string queueName, Int32 maximumQueuedJobs) {
        _options.AddQueueCheck(queueName, maximumQueuedJobs);
        return this;
    }

    public HangfireOptions Build() {
        return _options;
    }
}
