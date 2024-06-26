#if ANDROID
using Android.Content;
using Android.Views;
using Android.Runtime;
#elif IOS
using UIKit;
#endif

namespace EvergineIntegrationApp.Helpers
{
    public static class CrossPlatformFunctions
    {

        //void RestartApp();

        public static float GetBottomBorder()
        {
#if ANDROID
            return Android.App.Application.Context.Resources.GetDimensionPixelSize(Android.App.Application.Context.Resources.GetIdentifier("navigation_bar_height", "dimen", "android"));
#elif IOS
            float bottomPadding = 20;
            MainThread.BeginInvokeOnMainThread(() => 
            { 
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    UIWindow window = UIApplication.SharedApplication.Delegate.GetWindow();
                    if (window != null)
                        bottomPadding = (int)window.SafeAreaInsets.Bottom;
                }
            });
            return bottomPadding;
#else
            return 0;
#endif
        }

        public static float GetTopBorder() 
        {
#if ANDROID
            return Android.App.Application.Context.Resources.GetDimensionPixelSize(Android.App.Application.Context.Resources.GetIdentifier("status_bar_height", "dimen", "android"));
#elif IOS
            float topPadding = 20;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    UIWindow window = UIApplication.SharedApplication.Delegate.GetWindow();
                    if (window != null)
                        topPadding = (int)window.SafeAreaInsets.Top;
                }
            });
            return topPadding;
#else
            return 0;
#endif
        }
    }
}
