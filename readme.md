# Synnotech.Time

*Provides time-related abstractions and implementations for .NET.*

[![Synnotech Logo](synnotech-large-logo.png)](https://www.synnotech.de/)

[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](https://github.com/Synnotech-AG/Synnotech.Time/blob/main/LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg?style=for-the-badge)](https://www.nuget.org/packages/Synnotech.Time/)

# How to install

Synnotech.Time is compiled against [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) and thus supports all major plattforms like .NET 5, .NET Core, .NET Framework 4.6.1 or newer, Mono, Xamarin, UWP, or Unity.

Synnotech.Time is available as a [NuGet package](https://www.nuget.org/packages/Synnotech.Time/) and can be installed via:

- **Package Reference in csproj**: `<PackageReference Include="Synnotech.Time" Version="1.0.0" />`
- **dotnet CLI**: `dotnet add package Synnotech.Time`
- **Visual Studio Package Manager Console**: `Install-Package Synnotech.Time`

# What does Synnotech.Time offer you?

Synnotech.Time offers an `IClock` abstraction with several clock implementations as well as an implementation for a `DailyJob` that you can use in long running services, e.g. Windows Services.

# Abstract from DateTime with IClock

Synnotech.Time provides the `IClock` interface that abstracts calls to `DateTime.Now` and `DateTime.UtcNow`. This is usually required when testing your code and you want to supply dedicated `DateTime` values to better control your tests. `IClock` has a method called `GetTime` that you can use to obtain the current time stamp.

There are three implementations for `IClock`:

- `UtcClock` will return `DateTime.UtcNow` when calling `GetTime`. This should be the default clock that you use as the resulting value is unambiguous.
- `LocalClock` will return the local time. Be aware that this might lead to ambiguous time stamps, e.g. when a change from standard time to daylight saving time happens.
- `TestClock` can be used in unit test scenarios to control the time programmatically.

You typically register the clock as a singleton with the DI container:

```csharp
services.AddSingleton<IClock>(new UtcClock());
```

The clock can then be injected into a client:

```csharp
public sealed class UdpateJob
{
    public UpdateJob(IClock clock, INotificationService notificationService)
    {
        Clock = clock;
        NotificationService = notificationService;
    }

    private IClock Clock { get; }
    private INotificationService NotificationService { get; }

    public async Task ExecuteAsync()
    {
        var now = Clock.GetTime();

        var finished = Clock.GetTime();
        if ((finished - now) >= TimeSpan.FromMinutes(2))
            await NotificationService.CreateMessage("The update took unusually long - please check the log files for irregularities.");
    }
}
```

The usage of `IClock` in your production code lets us now write the tests way easier:

```csharp
public sealed class UpdateJobTests
{
    [Fact]
    public async Task CreateNotificationOnLongExecutionTime()
    {
        var initialTime = DateTime.UtcNow;
        var secondTime = initialTime.AddMinutes(2);
        var testClock = new TestClock(initialTime, secondTime);
        var notificationService = new NotificationServiceMock();
        var job = new UpdateJob(testClock, notificationService);

        await job.ExecuteAsync();

        notificationService.CreateMessageMusrHaveBeenCalled();
    }
}
```

In the example above, two `DateTime` instances are created, where the second one is two minutes later than the initial one. They are passed to the test clock which will return them on subsequent calls to `GetTime`. This allows us to easily check if the notification service is called properly by our job implementation.

`TestClock` also provides you with a `AdvanceTime` method that will change the current time. This can be used in scenarios where flow control returns to the test method in between calls to `GetTime`.

# Daily Job

The abstract class `DailyJob` represents a job that executes at the same time every day. It respects time changes (e.g. from standard time to daylight saving time). The class uses a `System.Timers.Timer` internally to trigger execution. To use it, you can simply derive from it:

```csharp
public sealed class MyJob : DailyJob
{
    // The date part of startTime will be ignored
    public MyJob(DateTime startTime, IClock clock) : base(startTime, clock) { }

    public override void Execute()
    {
        // The actual stuff you want to do in your job goes here
    }
}
```

To start the job, simply instantiate it and call `Start`, usually in your [Composition Root](https://freecontent.manning.com/dependency-injection-in-net-2nd-edition-understanding-the-composition-root/):

```csharp
var myJob = new MyJob(startTime, clock);
container.RegisterInstance(myJob); // DailyJob is disposable, so your DI container should dispose of it when the app shuts down
myJob.Start(); // The timer will queue the task for next day
// myJob.Execute(); // optional: if you want to execute the task right at startup, then call Execute
```

If your start time is configurable (e.g. in appsettings.json), then it is not unlikely that it will be deserialized as a `TimeSpan`. You can use the `TryConvertToTimeOfDay` (or `TryConvertToUtcTimeOfDay`) extension method to convert this `TimeSpan` to a `DateTime` instance:

```csharp
var jobSettings = configuration.GetSection("job").Get<JobSettings>();
if (!jobSettings.StartTime.TryConvertToUtcTimeOfDay(out DateTime timeOfDay))
    throw new InvalidConfigurationException($"The start time {jobSettings.StartTime} of job settings is invalid.");
var myJob = new MyJob(timeOfDay, clock);
```

# General recommendations

- Prefer UTC time stamps, especially in services and when saving date and time values. They are unambiguous, especially when it comes to changes in daylight saving time or to political decisions. You can convert your UTC time stamp to local time in the UI layer.
- Web apps should not host long-running jobs (like `DailyJob`). Many web servers, including Microsoft's IIS, will unload your website after a few minutes of not receiving HTTP requests (in IIS, this is 20 minutes by default). And this is wanted behavior: you want to host as many websites as possible next to each other on the same VM. This unloading mechanism will result in your jobs not being triggered because the underlying timer is disposed. Resort to things like the [Windows Task Scheduler](https://en.wikipedia.org/wiki/Windows_Task_Scheduler) or [Linux Crontab Files](https://www.howtogeek.com/101288/how-to-schedule-tasks-on-linux-an-introduction-to-crontab-files/) to trigger work.
