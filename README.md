# Karls Hangfire Health Checks [![NuGet Badge](https://buildstats.info/nuget/Karls.Hangfire.HealthChecks)](https://www.nuget.org/packages/Karls.Hangfire.HealthChecks/)

This is a ASP.NET Core health check for use with [Hangfire](https://www.hangfire.io/).

It supports a few more health checks then the one found in the
[AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
package.

## Installation

### .Net CLI

```sh
> dotnet add package Karls.Hangfire.HealthChecks
```

## Usage

This assumes that you have already setup Hangfire in your application and that you have
an endpoint for health checks.

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddHealthChecks(builder =>
        builder
            .AddHangfireCheck("Hangfire", optionsBuilder =>
                optionsBuilder
                    .MinimumAvailableServers(1)
                    .MaximumJobsFailed(1)
                    .AddJobQueueCheck("SpecialQueue", maximumQueuedJobs: 10),
                HealthStatus.Degraded);
    );
}
```
