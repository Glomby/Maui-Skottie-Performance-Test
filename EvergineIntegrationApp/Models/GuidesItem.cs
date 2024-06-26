using SkiaSharp.Extended.UI.Controls;
using SkiaSharp.Extended.UI.Controls.Converters;

namespace EvergineIntegrationApp.Models
{
    public enum GuideDataTemplate
    {
        Default,
        TitleCard,
        ProfileSelect,
        MagCalibration,
        OriCalibration,
        Headphones3D
    }

    public class GuidesItem
    {
        public string? HeroBanner { get; set; }
        public string? Header { get; set; }
        public string? Text { get; set; }
        public string? ButtonText { get; set; }
        public bool BackButtonVisible { get; set; }

        //public SKLottieRepeatMode RepeatMode { get => loopAnimation ? SKLottieRepeatMode.Restart : SKLottieRepeatMode.Restart; }
        public int RepeatCount { get => loopAnimation ? -1 : 0; }

        private GuideDataTemplate guideDataTemplate = GuideDataTemplate.Default;
        public GuideDataTemplate GuideDataTemplate { get => guideDataTemplate; set => guideDataTemplate = value; }

        private bool disableSwipe = false;
        public bool DisableSwipe { get => disableSwipe; set => disableSwipe = value; }
    
        private bool loopAnimation = true;
        public bool LoopAnimation { get => loopAnimation; set => loopAnimation = value; }

        private SKLottieImageSourceConverter _lottieConverter = new SKLottieImageSourceConverter();
        public SKLottieImageSource? HeroBannerLottie => (SKLottieImageSource?)_lottieConverter.ConvertFromString(HeroBanner);
    }
}
