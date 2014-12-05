using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Deezer.WindowsPhone.UI
{
    [DebuggerDisplay("ToastNotificationBase({Id})")]
    public abstract class ToastNotificationBase
    {
        private TranslateTransform _translate;
        private DispatcherTimer _dismissTimer;
        private ToastNotificationManager _toastNotificationManager;
        private double _toastShowDuration = 4000;
        private TaskCompletionSource<DismissStatus> _showAsyncTaskCompletionSource;

	// This storyboard source is from http://blogs.claritycon.com/blog/2010/10/wp7-page-transitions-sample/
        protected const string SwivelInStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimation 
				To="".5""
                Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.CenterOfRotationY)"" />
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationX)"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""-30""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.35"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseOut"" Exponent=""6""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"">
                <DiscreteDoubleKeyFrame KeyTime=""0"" Value=""1"" />
            </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        protected const string SwivelOutStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimation BeginTime=""0:0:0"" Duration=""0"" 
                                Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.CenterOfRotationY)"" 
                                To="".5""/>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationX)"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""45"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseIn"" Exponent=""6""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"">
                <DiscreteDoubleKeyFrame KeyTime=""0"" Value=""1"" />
                <DiscreteDoubleKeyFrame KeyTime=""0:0:0.267"" Value=""0"" />
            </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        protected const string SlideOutStoryboard =
           @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""1"" />
                <EasingDoubleKeyFrame KeyTime=""0:0:0.267"" Value=""0"" />
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimation Duration=""0:0:0.267"" To=""480"" Storyboard.TargetProperty=""(UIElement.RenderTransform).(TranslateTransform.X)"">
            </DoubleAnimation >
        </Storyboard>";

        public string Id { get; set; }

        public Brush BackgroundBrush { get; set; }

        public double ToastShowDuration
        {
            get { return _toastShowDuration; }
            set { _toastShowDuration = value; }
        }

        internal Border ToastControlMainBorder { get; set; }

        public event EventHandler<ToastCompletedEventArgs> Completed;

        public Task<DismissStatus> EnqueueAndShow(ToastNotificationManager manager)
        {
            if(manager == null)
                throw new ArgumentNullException("manager");

            _showAsyncTaskCompletionSource = new TaskCompletionSource<DismissStatus>();
            Completed += OnAsyncToastCompleted;
            manager.Enqueue(this);

            return _showAsyncTaskCompletionSource.Task;
        }

        private void OnAsyncToastCompleted(object sender, ToastCompletedEventArgs e)
        {
            this.Completed -= OnAsyncToastCompleted;
            _showAsyncTaskCompletionSource.TrySetResult(e.HasBeenDismissed);
        }

        public void Show(ToastNotificationManager toastNotificationManager)
        {
            if(toastNotificationManager == null)
                throw new ArgumentNullException("toastNotificationManager");

            _toastNotificationManager = toastNotificationManager;

            ToastControlMainBorder = new Border();
            _translate = new TranslateTransform();
            ToastControlMainBorder.RenderTransform = _translate;
            ToastControlMainBorder.ManipulationStarted += OnMainBorderManipulationStarted;
            ToastControlMainBorder.ManipulationDelta += OnMainBorderManipulationDelta;
            ToastControlMainBorder.ManipulationCompleted += OnMainBorderManipulationCompleted;
            ToastControlMainBorder.UseOptimizedManipulationRouting = false;

            // Setting the background brush (from property or default)
            if (BackgroundBrush != null)
            {
                ToastControlMainBorder.Background = BackgroundBrush;
            }
            else
            {
                // This resource (PhoneAccentColor) is provided by the system, and therefore can't be null.
                Color currentAccentColorHex = (Color)Application.Current.Resources["PhoneAccentColor"];
                ToastControlMainBorder.Background = new SolidColorBrush(currentAccentColorHex);
            }

            ToastControlMainBorder.VerticalAlignment = VerticalAlignment.Top;
            ToastControlMainBorder.Tap += OnToastBorderTapped;

            ToastControlMainBorder.Child = GetNotificationContent();

            // Loading the display storyboard
            Storyboard enteringStoryboard = XamlReader.Load(SwivelInStoryboard) as Storyboard;
            ToastControlMainBorder.Projection = new PlaneProjection();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _toastNotificationManager.AddNotificiationToVisualTree(this);;
                foreach (Timeline t in enteringStoryboard.Children)
                {
                    Storyboard.SetTarget(t, ToastControlMainBorder);
                }
                enteringStoryboard.Begin();
            });

            _dismissTimer = new DispatcherTimer();
            _dismissTimer.Interval = TimeSpan.FromMilliseconds(ToastShowDuration);
            _dismissTimer.Tick += OnDismissTimerTicked;
            _dismissTimer.Start();
        }

        internal void AbortNotification()
        {
            _dismissTimer.Stop();
            Deployment.Current.Dispatcher.BeginInvoke(() => _toastNotificationManager.RemoveNotificationFromVisualTree(this));
        }

        protected abstract ContentPresenter GetNotificationContent();

        protected virtual void RaiseCompleted(ToastCompletedEventArgs e)
        {
            EventHandler<ToastCompletedEventArgs> handler = Completed;
            if (handler != null) handler(this, e);
        }

        private void OnDismissTimerTicked(object sender, EventArgs e)
        {
            if (_dismissTimer != null)
            {
                _dismissTimer.Tick -= OnDismissTimerTicked;
                _dismissTimer.Stop();
            }
            DismissToast(DismissStatus.TimerDismissed);
        }

        private void DismissToast(DismissStatus dismissStatus, bool continueGestureAnimation = false)
        {
            ToastControlMainBorder.ManipulationStarted -= OnMainBorderManipulationStarted;
            ToastControlMainBorder.ManipulationDelta -= OnMainBorderManipulationDelta;
            ToastControlMainBorder.ManipulationCompleted -= OnMainBorderManipulationCompleted;
            
            Storyboard leavingStoryboard;
            if (continueGestureAnimation)
            {
                leavingStoryboard = XamlReader.Load(SlideOutStoryboard) as Storyboard;
            }
            else
            {
                leavingStoryboard = XamlReader.Load(SwivelOutStoryboard) as Storyboard;
            }
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {

                foreach (Timeline t in leavingStoryboard.Children)
                {
                    Storyboard.SetTarget(t, ToastControlMainBorder);
                }

                leavingStoryboard.Begin();
                leavingStoryboard.Completed += delegate
                {
                    _toastNotificationManager.RemoveNotificationFromVisualTree(this);
                    CompleteToast(dismissStatus);
                };
            });

            _dismissTimer = null;
        }

        internal void CompleteToast(DismissStatus dismissStatus, bool notifyManager = true)
        {
            if (notifyManager)
            {
                _toastNotificationManager.CompleteToast(this);
            }
            RaiseCompleted(new ToastCompletedEventArgs(dismissStatus));
        }

        private void OnToastBorderTapped(object sender, GestureEventArgs e)
        {
            Border toastBorder = sender as Border;
            if (toastBorder == null)
                return;

            toastBorder.Tap -= OnToastBorderTapped;
            DismissToast(DismissStatus.Tapped);
        }

        private void OnMainBorderManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            _dismissTimer.Stop();
            e.Handled = true;
        }

        private void OnMainBorderManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            e.Handled = true;
            _translate.X += e.DeltaManipulation.Translation.X;

            if (_translate.X < 0)
            {
                _translate.X = 0;
            }
        }

        private void OnMainBorderManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            e.Handled = true;
            if (e.TotalManipulation.Translation.X > 200 || e.FinalVelocities.LinearVelocity.X > 1000)
            {
                DismissToast(DismissStatus.Dismissed, true);
            }
            else if (e.TotalManipulation.Translation.X > -10 && e.TotalManipulation.Translation.X < 10)
            {
                DismissToast(DismissStatus.Tapped);
            }
            else
            {
                _translate.X = 0;
                _dismissTimer.Start();
            }
        }
    }
}
