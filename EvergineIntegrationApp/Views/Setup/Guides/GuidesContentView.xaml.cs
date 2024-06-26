using EvergineIntegrationApp.Models;

namespace EvergineIntegrationApp.Views.Setup.Guides
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuidesContentView : ContentView
    {
        private List<GuidesItem> guideItems;

        private bool dontShow;
        private bool dontShowDialog;
        private ColorScheme currentColorScheme;
        public ColorScheme CurrentColorScheme { get => currentColorScheme; }

        public enum ColorScheme { Colored, Grey }

        public GuidesContentView(List<GuidesItem> itemList, bool HasDontShowDialog = false, ColorScheme colorScheme = ColorScheme.Colored, bool hasIntro = false)
        {
            InitializeComponent();

            dontShowDialog = HasDontShowDialog;
            guideItems = itemList;
            MainCarouselView.ItemsSource = guideItems;

            this.currentColorScheme = colorScheme;
            ChangeColorScheme(colorScheme);
            MainCarouselView.CurrentItemChanged += CarouselView_CurrentItemChanged;

            //TODO: Remove addition of TopBorder, this needs to happen because of a Bug in MAUI and how it handles Modal Pages
            //NavBarBuffer.Height = ((App)Application.Current).BottomBorder;// + ((App)Application.Current).TopBorder;

            CheckboxCheckmark.IsVisible = dontShow;

            Task.Delay(100).ContinueWith(t =>
            {
                LottieBG.ScaleX = 1;
                LottieBG.ScaleX = 1.5;
            });

            MessagingCenter.Subscribe<object>(this, "NEXT_SLIDE", Next_Slide);
            MessagingCenter.Subscribe<object>(this, "LAST_SLIDE", Last_Slide);

            MessagingCenter.Subscribe<object>(this, "BLACK_OUT_ON", ShowBlackOut);
            MessagingCenter.Subscribe<object>(this, "BLACK_OUT_OFF", HideBlackOut);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent == null)
            {
                MessagingCenter.Unsubscribe<object>(this, "NEXT_SLIDE");
                MessagingCenter.Unsubscribe<object>(this, "LAST_SLIDE");

                MessagingCenter.Unsubscribe<object>(this, "BLACK_OUT_ON");
                MessagingCenter.Unsubscribe<object>(this, "BLACK_OUT_OFF");
            }            
        }

        private void ChangeColorScheme(ColorScheme scheme)
        {
            switch (scheme)
            {
                case ColorScheme.Colored:
                    {
                        Resources["BackgroundBase"] = Resources["BackgroundColored"];
                        Resources["BackButtonBase"] = Resources["BackButtonColored"];
                        Resources["BackAccentBase"] = Resources["BackAccentColored"];
                        Resources["TextBase"] = Resources["TextColored"];
                        Resources["AccentBase"] = Resources["AccentColored"];
                        Resources["GlowBase"] = Resources["GlowColored"];
                        Resources["LottieBackgroundBase"] = Resources["LottieBackgroundColored"];
                        Resources["ButtonTextBase"] = Resources["ButtonTextColored"];
                        break;
                    }
                case ColorScheme.Grey:
                    {
                        Resources["BackgroundBase"] = Resources["BackgroundGray"];
                        Resources["BackButtonBase"] = Resources["BackButtonGray"];
                        Resources["BackAccentBase"] = Resources["BackAccentGray"];
                        Resources["TextBase"] = Resources["TextGray"];
                        Resources["AccentBase"] = Resources["AccentGray"];
                        Resources["GlowBase"] = Resources["GlowGray"];
                        Resources["LottieBackgroundBase"] = Resources["LottieBackgroundGray"];
                        Resources["ButtonTextBase"] = Resources["ButtonTextGray"];
                        break;
                    }
            }
        }

        private void Skip_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<object>(this, "LOAD_NEXT_VIEW");
        }

        private void Next_Slide(object obj) 
        {
            Next_Clicked(null, null);
        }

        private void Last_Slide(object obj) 
        {
            Back_Clicked(null, null);
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            if (MainCarouselView.Position < guideItems.Count - 1)
            {
                MainCarouselView.Position = MainCarouselView.Position + 1;
                return;
            }

            if (dontShow)
                MessagingCenter.Send<object>(this, "DONT_SHOW_AGAIN");

            MessagingCenter.Send<object>(this, "LOAD_NEXT_VIEW");
        }

        private void Back_Clicked(object sender, EventArgs e)
        {
            if (MainCarouselView.Position > 0)
            {
                MainCarouselView.Position = MainCarouselView.Position - 1;
                return;
            }
            MessagingCenter.Send<object>(this, "LOAD_LAST_VIEW");
        }

        private void CarouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            Back_Button.IsVisible = guideItems[0].BackButtonVisible;
            PrimaryActionButton.Text = IsLastPage(0) && guideItems[0].ButtonText == "Next" ? "Close" : guideItems[0].ButtonText;
            MainCarouselView.IsSwipeEnabled = !guideItems[0].DisableSwipe;
            PrimaryActionButton.IsEnabled = !guideItems[0].DisableSwipe;
            PrimaryActionButton.WidthRequest = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density * 0.65;
        }

        private void CarouselView_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            Back_Button.IsVisible = guideItems[e.CurrentPosition].BackButtonVisible;
            PrimaryActionButton.Text = IsLastPage(e.CurrentPosition) && guideItems[e.CurrentPosition].ButtonText == "Next" ? "Close" : guideItems[e.CurrentPosition].ButtonText;
            MainCarouselView.IsSwipeEnabled = !guideItems[e.CurrentPosition].DisableSwipe;
            PrimaryActionButton.IsEnabled = !guideItems[e.CurrentPosition].DisableSwipe;

            ShowDontShowCheckbox(e.CurrentPosition == guideItems.Count - 1 || guideItems.Count == 1);
        }

        private bool IsLastPage(int currentPageNumber) 
        {
            return guideItems.Count -1 == currentPageNumber;
        }

        private void ShowDontShowCheckbox(bool show) 
        {
            if (!dontShowDialog)
                return;

            DontShowFrame.IsVisible = show;
        }

        private void DontShowFrame_Tapped(object sender, EventArgs e)
        {
            dontShow = !dontShow;
            CheckboxCheckmark.IsVisible = dontShow;
        }

        private void ShowBlackOut(object obj) 
        {
            BlackOutImage.IsVisible = true;
        }

        private void HideBlackOut(object obj)
        {
            BlackOutImage.IsVisible = false;
        }
    }
}
