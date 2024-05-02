namespace Karls.HealthChecks.Hangfire;

public class HangfireQueueOptions {
    public string QueueName { get; }

    public Int32 MaximumQueuedJobs { get; }

    internal HangfireQueueOptions(string queueName, Int32 maximumQueuedJobs) {
        QueueName = queueName;
        MaximumQueuedJobs = maximumQueuedJobs;
    }
}
