using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using WonderSongs.Droid;
#endif
namespace WonderSongs.UI;
static class ClockFactory
{
    public static TextBlock CreateClock()
    {
        var clock = new TextBlock
        {
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 10, 0)
        };

        var timer = new DispatcherTimer();
        timer.Tick += (s, e) =>
        {
            ScheduleNextTick(clock, timer);
        };

        clock.Loaded += (s, e) =>
        {
#if ANDROID
            MainActivity.Resume += ScheduleAndStartTimer;
            MainActivity.Pause += timer.Stop;
#endif
            ScheduleAndStartTimer();
        };

        clock.Unloaded += (s, e) =>
        {
#if ANDROID
            MainActivity.Resume -= ScheduleAndStartTimer;
            MainActivity.Pause -= timer.Stop;
#endif
            timer.Stop();
        };
        void ScheduleAndStartTimer()
        {
            ScheduleNextTick(clock, timer);
            timer.Start();
        }
        return clock;
    }

    private static void ScheduleNextTick(TextBlock clock, DispatcherTimer timer)
    {
        // Update clock text
        clock.Text = DateTime.Now.ToString("hh:mm tt");

        // Calculate time until next full minute
        var now = DateTime.Now;
        var nextMinute = now.AddMinutes(1)
                            .AddSeconds(-now.Second)
                            .AddMilliseconds(-now.Millisecond);

        timer.Interval = nextMinute - now;
    }
}
