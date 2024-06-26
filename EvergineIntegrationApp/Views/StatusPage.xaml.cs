using SkiaSharp.Extended.UI.Controls;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;
using EvergineIntegrationApp.ViewModels;
//using EvergineIntegrationApp.Interfaces;
using EvergineIntegrationApp.Helpers;

namespace EvergineIntegrationApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusPage : ContentPage
    {
        private Random randomGenerator;
        private List<QueueAnimation> animationQueue = new List<QueueAnimation>();
        private QueueAnimation? nextAdapterAnimation = null;
        private QueueAnimation? nextTrackerAnimation = null;

        private List<QueueWidth> changeWidthQueue = new List<QueueWidth>();

        private bool expandAnimationPlaying;

        private bool firstStartUp;

        private struct QueueAnimation
        {
            public View expandView;
            public Label opacityLabel;
            public double opacityTo;
            public double expandTo;

            public QueueAnimation(View expandView, Label opacityLabel, double opacityTo, double expandTo)
            {
                this.expandView = expandView;
                this.opacityLabel = opacityLabel;
                this.opacityTo = opacityTo;
                this.expandTo = expandTo;
            }
        }

        private struct QueueWidth
        {
            public View expandView;
            public Label Label;

            public QueueWidth(View expandView, Label Label)
            {
                this.expandView = expandView;
                this.Label = Label;
            }
        }

        private static Timer pageTimer;
        private static Timer aTimer;

        public StatusPage(StatusViewModel vm)
        {
            BindingContext = vm;
            InitializeComponent();

            randomGenerator = new Random();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var topBorder = ((App)Application.Current).TopBorder;
            var bottomBorder = ((App)Application.Current).BottomBorder;

            if (aTimer != null && aTimer.Enabled)
                aTimer.Stop();

            if (changeWidthQueue.Count > 0)
            {
                foreach (var view in changeWidthQueue)
                {
                    view.expandView.WidthRequest = !string.IsNullOrEmpty(view.Label?.Text) ? ((View)view.expandView.Parent).Width : view.expandView.Height;
                }
                changeWidthQueue.Clear();
            }

            AdapterFrame.WidthRequest = !string.IsNullOrEmpty(GetChildLabel(AdapterFrame).Text) ? ((View)AdapterFrame.Parent).Width : AdapterFrame.Height;
            AdapterLabel.Opacity = !string.IsNullOrEmpty(GetChildLabel(AdapterFrame).Text) ? 1 : 0;
            TrackerFrame.WidthRequest = !string.IsNullOrEmpty(GetChildLabel(TrackerFrame).Text) ? ((View)TrackerFrame.Parent).Width : TrackerFrame.Height;
            TrackerLabel.Opacity = !string.IsNullOrEmpty(GetChildLabel(AdapterFrame).Text) ? 1 : 0;

            await Task.Delay(500);
            firstStartUp = true;
            PlayQueue();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public void TimedAppearing()
        {
#if DEBUG
            Debug.WriteLine("TimedAppearing()");
#endif
            aTimer = new Timer(1500);
            aTimer.Elapsed += SwitchMainPage;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
            aTimer.Start();
        }

        private void SwitchMainPage(Object source, ElapsedEventArgs e)
        {
#if DEBUG
            Debug.WriteLine("SwitchMainPage()");
#endif

            MainThread.BeginInvokeOnMainThread(OnAppearing);
            aTimer.Elapsed -= SwitchMainPage;
        }

        private async void LoopRevearsAnimationView_OnFinishedAnimation(object sender, EventArgs e)
        {
            if (((SKLottieView)sender) == null)
                return;
            ((SKLottieView)sender).IsAnimationEnabled = false;
            ((SKLottieView)sender).RepeatMode = SKLottieRepeatMode.Reverse; 
            var randomDelay = randomGenerator.Next(0, 5000);
            await Task.Delay(randomDelay);
            ((SKLottieView)sender).Progress = ((SKLottieView)sender).Duration - TimeSpan.FromMilliseconds(10);
            ((SKLottieView)sender).IsAnimationEnabled = true;
            //PlayLottieAnimationSafe((SKLottieView)sender);
        }

        private void PlayExpandAnimation(QueueAnimation queueAnimation)
        {
            for (int i = animationQueue.Count - 1; i>=0; i--)
            {
                if (animationQueue[i].expandView.Id == queueAnimation.expandView.Id)
                {
                    if (i == 0)
                        this.AbortAnimation("AnimationQueue");
                    animationQueue.RemoveAt(i);
                }
            }

            animationQueue.Add(queueAnimation);

            if (!expandAnimationPlaying)
                PlayQueue();
        }

        private void PlayQueue(bool keepPlaying = false)
        {
            if (!firstStartUp || (expandAnimationPlaying && !keepPlaying) || animationQueue.Count <= 0)
                return;

            Animation animation;
            Animation expand;
            Animation textFade;

            double shouldHaveWidth;

            if (nextTrackerAnimation.HasValue)
            {
                shouldHaveWidth = !string.IsNullOrEmpty(GetChildLabel((Border)nextTrackerAnimation.Value.expandView).Text) ? ((View)nextTrackerAnimation.Value.expandView.Parent).Width : nextTrackerAnimation.Value.expandView.Height;
                if (shouldHaveWidth != nextTrackerAnimation.Value.expandTo)
                {
                    nextTrackerAnimation = null;
                    if (nextAdapterAnimation.HasValue)
                        PlayQueue(true);
                    else
                        expandAnimationPlaying = false;
                    return;
                }

                expandAnimationPlaying = true;
                this.AbortAnimation("AnimationQueue");

                animation = new Animation();
                expand = new Animation(v => nextTrackerAnimation.Value.expandView.WidthRequest = v, nextTrackerAnimation.Value.expandView.Width, nextTrackerAnimation.Value.expandTo, Easing.CubicOut);
                textFade = new Animation(v => nextTrackerAnimation.Value.opacityLabel.Opacity = v, nextTrackerAnimation.Value.opacityLabel.Opacity, nextTrackerAnimation.Value.opacityTo, Easing.CubicOut);

                animation.Add(0, .8, expand);
                animation.Add(0.5, 1, textFade);

                animation.Commit(this, "AnimationQueue", 16, 1000, null, (v, cancled) =>
                {
                    if (!cancled)
                    {
                        nextTrackerAnimation.Value.expandView.WidthRequest = !string.IsNullOrEmpty(nextTrackerAnimation.Value.opacityLabel?.Text) ? ((View)nextTrackerAnimation.Value.expandView.Parent).Width : nextTrackerAnimation.Value.expandView.Height;

                        nextTrackerAnimation = null;
                        if (nextAdapterAnimation.HasValue)
                            PlayQueue(true);
                        else
                            expandAnimationPlaying = false;
                    }
                });
            }
            else if (nextAdapterAnimation.HasValue)
            {
                shouldHaveWidth = !string.IsNullOrEmpty(GetChildLabel((Border)nextAdapterAnimation.Value.expandView).Text) ? ((View)nextAdapterAnimation.Value.expandView.Parent).Width : nextAdapterAnimation.Value.expandView.Height;
                if (shouldHaveWidth != nextAdapterAnimation.Value.expandTo)
                {
                    nextAdapterAnimation = null;
                    if (nextTrackerAnimation.HasValue)
                        PlayQueue(true);
                    else
                        expandAnimationPlaying = false;
                    return;
                }

                expandAnimationPlaying = true;
                nextAdapterAnimation.Value.expandView.AbortAnimation("AnimationQueue");

                animation = new Animation();
                expand = new Animation(v => nextAdapterAnimation.Value.expandView.WidthRequest = v, nextAdapterAnimation.Value.expandView.Width, nextAdapterAnimation.Value.expandTo, Easing.CubicOut);
                textFade = new Animation(v => nextAdapterAnimation.Value.opacityLabel.Opacity = v, nextAdapterAnimation.Value.opacityLabel.Opacity, nextAdapterAnimation.Value.opacityTo, Easing.CubicOut);

                animation.Add(0, .8, expand);
                animation.Add(0.5, 1, textFade);

                animation.Commit(nextAdapterAnimation.Value.expandView, "AnimationQueue", 16, 1000, null, (v, cancled) =>
                {
                    if (!cancled)
                    {
                        nextAdapterAnimation.Value.expandView.WidthRequest = !string.IsNullOrEmpty(nextAdapterAnimation.Value.opacityLabel?.Text) ? ((View)nextAdapterAnimation.Value.expandView.Parent).Width : nextAdapterAnimation.Value.expandView.Height;

                        nextAdapterAnimation = null;
                        if (nextTrackerAnimation.HasValue)
                            PlayQueue(true);
                        else
                            expandAnimationPlaying = false;
                    }
                });

            }
            else
                return;
        }

        private void PlayLottieAnimationSafe(SKLottieView animation)
        {
            if (animation == null)
                return;

            try
            {
                animation.IsAnimationEnabled = true;
                //animation?.PlayAnimation();
            }
            catch (NullReferenceException exc)  //This can happen but needs no extra handling.
            {
                Console.WriteLine(exc?.Message);
            }
            catch (ObjectDisposedException oexc)
            {
                Console.WriteLine(oexc?.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e?.Message);
            }
        }

        private void StopLottieAnimationSafe(SKLottieView animation)
        {
            if (animation == null)
                return;

            try
            {
                animation.IsAnimationEnabled = false;
                //animation?.StopAnimation();
            }  //This can happen but needs no extra handling.
            catch (NullReferenceException exc)
            {
                Console.WriteLine(exc?.Message);
            }
            catch (ObjectDisposedException oexc)
            {
                Console.WriteLine(oexc?.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e?.Message);
            }
        }

        private Label GetChildLabel(Border frame)
        {
            var parent = frame.Parent as Grid;
            Label label = null;

            if (parent != null)
            {
                foreach (View view in parent.Children)
                {
                    Label childLabel = view as Label;
                    if (childLabel != null)
                    {
                        label = childLabel;
                        break;
                    }
                }
            }
            return label;
        }

        private void Text_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Parent")
            {
                var label = sender as Label;
                Border statusBarFrame = null;

                var parent = label.Parent as Grid;
                foreach (View view in parent.Children)
                {
                    Border frame = view as Border;
                    if (frame != null)
                    {
                        statusBarFrame = frame;
                        break;
                    }
                }

                if (statusBarFrame == null)
                    return;

                changeWidthQueue.Add(new QueueWidth(statusBarFrame, label));

                var toWidth = !string.IsNullOrEmpty(label?.Text) ? ((View)statusBarFrame.Parent).Width : statusBarFrame.Height;

                if (toWidth < 0)
                    WaitForWidth(statusBarFrame, !string.IsNullOrEmpty(label?.Text));
                else
                    statusBarFrame.WidthRequest = toWidth;
            }

            if (e.PropertyName == Label.TextProperty.PropertyName)
            {
                Label label = sender as Label;
                bool emptyText = string.IsNullOrEmpty(label?.Text);

                ExpandStatusBar(!emptyText, label);
            }
        }

        private void ExpandStatusBar(bool shouldExpand, Label label)
        {
            Border? statusBarFrame = null;

            Grid? parent = label.Parent as Grid;
            foreach (View view in parent.Children)
            {
                Border? border = view as Border;
                if (border != null)
                {
                    statusBarFrame = border;
                    break;
                }
            }

            if (statusBarFrame == null)
                return;

            var toWidth = shouldExpand ? ((View)statusBarFrame.Parent).Width : statusBarFrame.Height;

            if (toWidth < 0)
            {
                WaitForWidth(statusBarFrame, shouldExpand);
                return;
            }

            if (toWidth == statusBarFrame.WidthRequest)
            {
                statusBarFrame.WidthRequest = toWidth;
                return;
            }

            if (statusBarFrame == AdapterFrame)
                nextAdapterAnimation = new QueueAnimation(statusBarFrame, label, shouldExpand ? 1 : 0, toWidth);

            if (statusBarFrame == TrackerFrame)
                nextTrackerAnimation = new QueueAnimation(statusBarFrame, label, shouldExpand ? 1 : 0, toWidth);

            PlayExpandAnimation(new QueueAnimation(statusBarFrame, label, shouldExpand ? 1 : 0, toWidth));
        }

        private async void WaitForWidth(View view, bool shouldExpand)
        {
            await Task.Delay(500);
            var toWidth = shouldExpand ? ((View)view.Parent).Width : view.Height;

            if (toWidth < 0)
            {
                WaitForWidth(view, shouldExpand);
                return;
            }

            if (toWidth == view.WidthRequest)
                return;
            
            var animation = new Animation(v => view.WidthRequest = v, view.Width, toWidth, Easing.CubicInOut);
            animation.Commit(this, view.ToString() + "ExpandAnimation", 16, 1000, null);
        }

        private void CircleAnimation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((SKLottieView)sender != null)
            {
                var anim = sender as SKLottieView;
                if (anim == null)
                    return;

                Debug.WriteLine("CircleAnimation_PropertyChanged: " + e.PropertyName);

                if (anim.Source != null)
                {
                    var source_old = anim.Source;
                    anim.Source = null;
                    anim.Source = source_old;
                }

                if (anim.Source?.ToString() == "connectPage_ReadyCheckmark.json")
                {
                    StopLottieAnimationSafe(anim);
                    anim.RepeatMode = SKLottieRepeatMode.Restart;
                    anim.RepeatCount = 1;
                    anim.Progress = TimeSpan.Zero;
                    PlayLottieAnimationSafe(anim);
                }
                else
                {
                    anim.RepeatMode = SKLottieRepeatMode.Restart;
                    //anim.Progress = 0;
                    PlayLottieAnimationSafe(anim);
                }
            }
        }

        private void CircleLottie_AnimationLoaded(object sender, SKLottieAnimationLoadedEventArgs e)
        {

        }

        private void CircleLottie_AnimationFailed(object sender, SKLottieAnimationFailedEventArgs e)
        {

        }
    }
}