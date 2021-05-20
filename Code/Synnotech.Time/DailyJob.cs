using System;
using System.Timers;

namespace Synnotech.Time
{
    /// <summary>
    /// Represents a job that executes at the same time every day. You can use this
    /// class by inheriting from it and overriding the <see cref="Execute"/> method.
    /// </summary>
    public abstract class DailyJob : IDisposable
    {
        private readonly IClock _clock;
        private readonly Timer _timer;

        /// <summary>
        /// Gets the target time of day when the task is executed.
        /// The date part of this value is ignored.
        /// </summary>
        public readonly DateTime StartTime;

        /// <summary>
        /// Initializes a new instance of <see cref="DailyJob" />.
        /// </summary>
        /// <param name="startTime">The time of day when the job should start. The date part of this value is ignored.</param>
        /// <param name="clock">The object that is used to retrieve the current time.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="clock" /> is null.</exception>
        protected DailyJob(DateTime startTime, IClock clock)
        {
            StartTime = startTime;
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _timer = new Timer { AutoReset = false };
            _timer.Elapsed += OnTimerTick;
        }

        /// <summary>
        /// Disposes of the internal timer instance.
        /// </summary>
        public void Dispose() => _timer.Dispose();

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            Execute();
            Start();
        }

        /// <summary>
        /// Starts execution of this job. It will execute on the next day for the first time.
        /// </summary>
        public void Start()
        {
            var interval = _clock.GetTime().CalculateIntervalForSameTimeNextDay(StartTime);
            _timer.Interval = interval.TotalMilliseconds;
            _timer.Start();
        }

        /// <summary>
        /// Stops execution of this job.
        /// </summary>
        public void Stop() => _timer.Stop();

        /// <summary>
        /// Execute this job immediately.
        /// </summary>
        public abstract void Execute();
    }
}