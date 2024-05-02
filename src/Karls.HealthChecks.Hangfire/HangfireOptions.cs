namespace Karls.HealthChecks.Hangfire;

public class HangfireOptions {
    public Int32? MaximumJobsFailed { get; internal set; }

    public Int32? MinimumAvailableServers { get; internal set; }

    public Int32? MaximumTotalQueuedJobs { get; internal set; }

    public IList<HangfireQueueOptions>? Queues { get; private set; }

    internal void AddQueueCheck(string queueName, Int32 maximumQueuedJobs) {
        Queues ??= [];
        Queues.Add(new HangfireQueueOptions(queueName, maximumQueuedJobs));
    }
}
