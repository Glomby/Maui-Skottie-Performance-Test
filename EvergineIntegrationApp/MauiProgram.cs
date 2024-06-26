using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using CommunityToolkit.Maui;
using EvergineIntegrationApp.Views;
using EvergineIntegrationApp.ViewModels;

namespace EvergineIntegrationApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseMauiCommunityToolkit()
                .AddPages()
                .AddViewModels()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("911Fonts.com_CenturyGothicRegular__-_911fonts.com_fonts_mhpY.ttf", "CenturyGothicRegular");
                    fonts.AddFont("GOTHICB.TTF", "CenturyGothicBold");
                    fonts.AddFont("fontello.ttf", "Icons");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        static MauiAppBuilder AddPages(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<StatusPage>();
            //builder.Services.AddTransient<GuideView>();
            return builder;
        }

        static MauiAppBuilder AddViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<StatusViewModel>();
            return builder;
        }

    }
}
