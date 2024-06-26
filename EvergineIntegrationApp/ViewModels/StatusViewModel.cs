using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using EvergineIntegrationApp.Views.Setup;
using EvergineIntegrationApp.Views.Setup.Guides;
using SkiaSharp.Extended.UI.Controls;
using SkiaSharp.Extended.UI.Controls.Converters;

namespace EvergineIntegrationApp.ViewModels
{
    public partial class StatusViewModel : ObservableObject
    {
        private App? appRef;
        private SKLottieImageSourceConverter _lottieConverter = new SKLottieImageSourceConverter();
        private bool canDirectConnect;

        public StatusViewModel() //IIMUBLE iMUBLE
        {
            appRef = (App)Application.Current;

            TrackerLottieCircleAnimation = (SKLottieImageSource?)_lottieConverter.ConvertFromString("waiting_dots.json");
            AdapterLottieCircleAnimation = (SKLottieImageSource?)_lottieConverter.ConvertFromString("waiting_dots.json");
        }

        public double TopBorder => appRef.TopBorder;

        public double BottomBorder => appRef.BottomBorder;

        [ObservableProperty]
        private string? adapterStatus;

        [ObservableProperty]
        private string? trackerStatus;

        [ObservableProperty]
        private double adapterStatusOpacity;

        [ObservableProperty]
        private double trackerStatusOpacity;

        [ObservableProperty]
        private SKLottieImageSource? adapterLottieCircleAnimation;

        [ObservableProperty]
        private int adapterLottieRepeatCount = -1;

        [ObservableProperty]
        private SKLottieImageSource? trackerLottieCircleAnimation;

        [ObservableProperty]
        private bool trackerLottieAnimationEnabled = true;

        [ObservableProperty]
        private int trackerLottieRepeatCount = -1;

        [ObservableProperty]
        private TimeSpan batteryPercent;

        [ObservableProperty]
        private bool batteryVisibility;

        [ObservableProperty]
        private double batteryOpacity;

        [RelayCommand(CanExecute = nameof(CanDirectConnect))]
        private void DirectConnect(object obj)
        {
            var setupViews = new List<View>();
            //setupViews.Add(new DirectConnectView());
            var setupPage = new SetupPage(setupViews);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(setupPage);
            });
        }
        private bool CanDirectConnect(object arg)
        {
            return canDirectConnect;
        }

        [RelayCommand]
        private void OpenGuide(object obj)
        {
            var setupViews = new List<View>();
            setupViews.Add(new GuidesContentView(Guides.FirstSteps));
            var setupPage = new SetupPage(setupViews);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(setupPage);
            });
        }
    }
}
