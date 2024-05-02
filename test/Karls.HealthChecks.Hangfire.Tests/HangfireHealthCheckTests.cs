using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Karls.HealthChecks.Hangfire.Core.Tests;

public class HangfireHealthCheckTests {
    [Fact]
    public async Task CheckHealth_WhenCalledWithoutAnyConfiguredLimitations_ShouldReturnHealthyAsync() {
        var monitoringApi = A.Dummy<IMonitoringApi>();

        var hangfireOptions = new HangfireOptions();

        var healthCheck = new HangfireHealthCheck(hangfireOptions, monitoringApi);

        var result = await healthCheck.CheckHealthAsync(A.Dummy<HealthCheckContext>(), CancellationToken.None);

        result.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealth_WhenCalledWithMaximumJobsFailed_ShouldReturnUnhealthyAsync() {
        var monitoringApi = A.Dummy<IMonitoringApi>();

        var hangfireOptions = new HangfireOptions {
            MaximumJobsFailed = 1
        };

        var context = new HealthCheckContext {
            Registration = new HealthCheckRegistration("Hangfire", A.Dummy<IHealthCheck>(), HealthStatus.Unhealthy, null)
        };

        A.CallTo(() => monitoringApi.FailedCount()).Returns(2);

        var healthCheck = new HangfireHealthCheck(hangfireOptions, monitoringApi);

        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        result.Status.ShouldBe(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task CheckHealth_WhenCalledWithMinimumAvailableServers_ShouldReturnUnhealthyAsync() {
        var monitoringApi = A.Dummy<IMonitoringApi>();

        var hangfireOptions = new HangfireOptions {
            MinimumAvailableServers = 2
        };

        var context = new HealthCheckContext {
            Registration = new HealthCheckRegistration("Hangfire", A.Dummy<IHealthCheck>(), HealthStatus.Unhealthy, null)
        };

        A.CallTo(() => monitoringApi.Servers())
            .Returns([
                A.Dummy<ServerDto>()
            ]
        );

        var healthCheck = new HangfireHealthCheck(hangfireOptions, monitoringApi);

        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        result.Status.ShouldBe(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task CheckHealth_WhenCalledWithQueues_ShouldReturnUnhealthyAsync() {
        var monitoringApi = A.Dummy<IMonitoringApi>();

        var hangfireOptions = new HangfireOptions();
        hangfireOptions.AddQueueCheck("queue1", 1);

        var context = new HealthCheckContext {
            Registration = new HealthCheckRegistration("Hangfire", A.Dummy<IHealthCheck>(), HealthStatus.Degraded, null)
        };

        A.CallTo(() => monitoringApi.EnqueuedCount("queue1")).Returns(2);

        var healthCheck = new HangfireHealthCheck(hangfireOptions, monitoringApi);

        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        result.Status.ShouldBe(HealthStatus.Degraded);
    }

    [Fact]
    public async Task CheckHealth_WhenCalledWithMaximumTotalQueuedJobs_ShouldReturnUnhealthyAsync() {
        var monitoringApi = A.Dummy<IMonitoringApi>();

        var hangfireOptions = new HangfireOptions {
            MaximumTotalQueuedJobs = 10
        };

        var context = new HealthCheckContext {
            Registration = new HealthCheckRegistration("Hangfire", A.Dummy<IHealthCheck>(), HealthStatus.Unhealthy, null)
        };

        A.CallTo(() => monitoringApi.Queues())
            .Returns([
                new QueueWithTopEnqueuedJobsDto {
                    Name = "queue1",
                    Length = 2
                },
                new QueueWithTopEnqueuedJobsDto {
                    Name = "queue2",
                    Length = 6
                },
                new QueueWithTopEnqueuedJobsDto {
                    Name = "queue3",
                    Length = 6
                }
            ]
        );

        var healthCheck = new HangfireHealthCheck(hangfireOptions, monitoringApi);

        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        result.Status.ShouldBe(HealthStatus.Unhealthy);
    }
}
