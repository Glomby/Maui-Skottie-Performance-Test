using EvergineIntegrationApp.Helpers;
using EvergineIntegrationApp.ViewModels;
using EvergineIntegrationApp.Views;

namespace EvergineIntegrationApp
{
    public partial class App : Application
    {
        private double screenDensity = 1;
        public double BottomBorder { get => CrossPlatformFunctions.GetBottomBorder() / screenDensity; }
        public double TopBorder { get => CrossPlatformFunctions.GetTopBorder() / screenDensity; }

        public App()
        {
            InitializeComponent();

            MainPage = new StatusPage(ServiceHelper.GetService<StatusViewModel>());
        }
    }
}
