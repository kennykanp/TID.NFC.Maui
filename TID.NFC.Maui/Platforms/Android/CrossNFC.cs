using Android.App;
using Android.Content;
using Android.OS;
using System;

namespace TID.NFC.Maui;

/// <summary>
/// Cross NFC (Android specific)
/// </summary>
public static partial class CrossNFC
{
    internal static ActivityLifecycleContextListener lifecycleListener;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="application">Android <see cref="Application"/></param>
    public static void Init(Android.App.Application application)
    {
        if (lifecycleListener != null) return;
        lifecycleListener = new ActivityLifecycleContextListener();
        application.RegisterActivityLifecycleCallbacks(lifecycleListener);
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="activity">Android <see cref="Activity"/></param>
    public static void Init(Activity activity)
    {
        Init(activity.Application);
        lifecycleListener.Activity = activity;
    }

    /// <summary>
    /// Overrides Activity.OnNewIntent()
    /// </summary>
    /// <param name="intent">Android <see cref="Intent"/></param>
    public static void OnNewIntent(Intent intent) => ((NFCImplementation)Current).HandleNewIntent(intent);

    /// <summary>
    /// Overrides Activity.OnResume()
    /// </summary>
    public static void OnResume() => ((NFCImplementation)Current).HandleOnResume();

    /// <summary>
    /// Returns the current Android <see cref="Context"/>
    /// </summary>
    internal static Context AppContext => Android.App.Application.Context;

    /// <summary>
    /// Returns the current Android <see cref="Activity"/>
    /// </summary>
    /// <param name="throwError"></param>
    /// <returns></returns>
    internal static Activity GetCurrentActivity(bool throwError)
    {
        var activity = lifecycleListener?.Activity;
        if (throwError && activity == null)
            throw new NullReferenceException("The current Activity can not be detected. Ensure that you have called Init in your Activity or Application class.");
        return activity;
    }
}

/// <summary>
/// James Montemagno's ActivityLifecycleContextListener from CurrentActivityPlugin
/// <see href="https://github.com/jamesmontemagno/CurrentActivityPlugin"/>
/// </summary>
class ActivityLifecycleContextListener : Java.Lang.Object, Android.App.Application.IActivityLifecycleCallbacks
{
    WeakReference<Activity> _currentActivity = new WeakReference<Activity>(null);

    internal Context Context => Activity ?? Android.App.Application.Context;

    internal Activity Activity
    {
        get => _currentActivity.TryGetTarget(out var a) ? a : null;
        set => _currentActivity.SetTarget(value);
    }

    void Android.App.Application.IActivityLifecycleCallbacks.OnActivityCreated(Activity activity, Bundle savedInstanceState) => Activity = activity;
    void Android.App.Application.IActivityLifecycleCallbacks.OnActivityDestroyed(Activity activity) { }
    void Android.App.Application.IActivityLifecycleCallbacks.OnActivityPaused(Activity activity) => Activity = activity;
    void Android.App.Application.IActivityLifecycleCallbacks.OnActivityResumed(Activity activity) => Activity = activity;
    void Android.App.Application.IActivityLifecycleCallbacks.OnActivitySaveInstanceState(Activity activity, Bundle outState) { }
    void Android.App.Application.IActivityLifecycleCallbacks.OnActivityStarted(Activity activity) { }
    void Android.App.Application.IActivityLifecycleCallbacks.OnActivityStopped(Activity activity) { }
}
