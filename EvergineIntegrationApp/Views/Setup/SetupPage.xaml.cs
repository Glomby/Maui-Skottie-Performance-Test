using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace EvergineIntegrationApp.Views.Setup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupPage : ContentPage
    {
        List<View>? views;
        int viewIndex = 0;
        string? setupName;

        bool directConnectClosed;

        public SetupPage(List<View> setupViews, string? SetupName = null)
        {
            InitializeComponent();
            InitViews(setupViews);
            setupName = SetupName;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<object>(this, "LOAD_NEXT_VIEW", LoadNextView);
            MessagingCenter.Subscribe<object>(this, "LOAD_LAST_VIEW", LoadPrevView);
            MessagingCenter.Subscribe<object>(this, "DONT_SHOW_AGAIN", WritePropertyDontShow);
            MessagingCenter.Subscribe<object>(this, "DIRECT_CONNECT_CLOSED", DirectConnectClosed);
        }

        private void DirectConnectClosed(object obj)
        {
            directConnectClosed = true;
            LoadNextView(obj);
        }

        private void WritePropertyDontShow(object obj)
        {
            if (!string.IsNullOrEmpty(setupName))
            {
                Preferences.Set(setupName, true);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<object>(this, "LOAD_NEXT_VIEW");
            MessagingCenter.Unsubscribe<object>(this, "LOAD_LAST_VIEW");
            MessagingCenter.Unsubscribe<object>(this, "DONT_SHOW_AGAIN");
            MessagingCenter.Unsubscribe<object>(this, "DIRECT_CONNECT_CLOSED");

//            if (!directConnectClosed && ((App)Application.Current).connectPageRef != null && ((App)Application.Current).MainPage == ((App)Application.Current).connectPageRef)
//            {
//                try
//                {
//                    ((App)Application.Current).connectPageRef.TimedAppearing();
//                }
//                catch (Exception ex)
//                {
//#if DEBUG
//                    Console.Out.WriteLine(ex.ToString());
//#endif
//                }
//            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            foreach (View element in viewStackLayout.Children.ToList())
            {
                element.HeightRequest = height;
            }
        }

        public void InitViews(List<View> setupViews)
        {
            views = setupViews;
            viewIndex = 0;
            SetControls();
        }

        public void LoadNextView(object obj)
        {
            if ((viewIndex + 1) < views.Count)
            {
                viewIndex++;
                SetControls();
            }
            else
            {
                Pop();
            }
        }

        private void LoadPrevView(object obj)
        {
            if (viewIndex > 0)
            {
                viewIndex--;
                SetControls();
            }
        }

        public void SetControls()
        {
            viewStackLayout.Children.Clear();
            viewStackLayout.Children.Add(views[viewIndex]);

            foreach (View element in viewStackLayout.Children.ToList())
            {
                element.HeightRequest = this.Height;
            }
        }

        private async void Pop()
        {
            if (Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
            else if (Navigation.NavigationStack.Count > 0)
            {
                await Navigation.PopAsync();
            }
        }
    }
}