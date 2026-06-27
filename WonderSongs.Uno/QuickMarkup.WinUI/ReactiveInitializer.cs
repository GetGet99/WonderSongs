#if UWP
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
#else
using Microsoft.UI.Dispatching;
#endif

namespace QuickMarkup.WinUI;

public static class ReactiveInitializer
{
    /// <summary>
    /// Initialize Reactive Scheduler
    /// </summary>
    /// <remarks>Must be call on UI thread with dispatcher</remarks>
    public static void InitReactiveScheduler()
    {
#if UWP
        InitReactiveScheduler(CoreApplication.GetCurrentView().Dispatcher);
#else
        var dispatcher = DispatcherQueue.GetForCurrentThread();
        InitReactiveScheduler(dispatcher);
#endif
    }
    /// <summary>
    /// Initialize Reactive Scheduler
    /// </summary>
    /// <param name="dispatcher">Dispatcher to use to schedule callback</param>
#if UWP
    public static void InitReactiveScheduler(CoreDispatcher dispatcher)
    {
        ReactiveScheduler.AddTickCallbackForCurrentThread(delegate
        {
            _ = dispatcher.TryRunAsync(CoreDispatcherPriority.High, ReactiveScheduler.Tick);
        });
    }
#else
    public static void InitReactiveScheduler(DispatcherQueue dispatcher)
    {
        ReactiveScheduler.AddTickCallbackForCurrentThread(() =>
        {
            dispatcher.TryEnqueue(ReactiveScheduler.Tick);
        });
    }
#endif
}