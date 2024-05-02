using Hangfire.Storage;
using Karls.HealthChecks.Hangfire;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

public static class HealthCheckBuilderExtensions {
    private const string _defaultName = "Hangfire";

    /// <summary>
    /// Add a health check for Hangfire.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="setup">The action to configure the Hangfire parameters.</param>
    /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'hangfire' will be used for the name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
    /// <returns>The specified <paramref name="builder"/>.</returns>
    public static IHealthChecksBuilder AddHangfireCheck(
            this IHealthChecksBuilder builder,
            string? name = null,
            Action<HangfireOptionsBuilder>? setup = null,
            HealthStatus? failureStatus = null,
            IEnumerable<string>? tags = null,
            TimeSpan? timeout = null) {
        var hangfireOptionsBuilder = new HangfireOptionsBuilder();
        setup?.Invoke(hangfireOptionsBuilder);

        return builder.Add(new HealthCheckRegistration(
            name ?? _defaultName,
            sp => new HangfireHealthCheck(hangfireOptionsBuilder.Build(), sp.GetService<IMonitoringApi>()!),
            failureStatus,
            tags,
            timeout));
    }
}
