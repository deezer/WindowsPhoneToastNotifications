using System;
using System.Diagnostics;
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
        private Border _toastControlMainBorder;
        private TranslateTransform _translate;
        private DispatcherTimer _dismissTimer;
        private ToastNotificationManager _toastNotificationManager;

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


        public string Id { get; set; }
        public Brush BackgroundBrush { get; set; }
        public double ToastShowDuration { get; set; }

        public void Show(ToastNotificationManager toastNotificationManager)
        {
            if(toastNotificationManager == null)
                throw new ArgumentNullException("toastNotificationManager");

            _toastNotificationManager = toastNotificationManager;

            _toastControlMainBorder = new Border();
            _translate = new TranslateTransform();
            _toastControlMainBorder.RenderTransform = _translate;
            _toastControlMainBorder.ManipulationStarted += OnMainBorderManipulationStarted;
            _toastControlMainBorder.ManipulationDelta += OnMainBorderManipulationDelta;
            _toastControlMainBorder.ManipulationCompleted += OnMainBorderManipulationCompleted;
            // Setting the background brush (from property or default)
            if (BackgroundBrush != null)
            {
                _toastControlMainBorder.Background = BackgroundBrush;
            }
            else
            {
                // This resource (PhoneAccentColor) is provided by the system, and therefore can't be null.
                Color currentAccentColorHex = (Color)Application.Current.Resources["PhoneAccentColor"];
                _toastControlMainBorder.Background = new SolidColorBrush(currentAccentColorHex);
            }

            _toastControlMainBorder.VerticalAlignment = VerticalAlignment.Top;
            _toastControlMainBorder.Tap += OnToastBorderTapped;

            // TODO: Show the entry storyboard
            // Loading the display storyboard
            //Storyboard enteringStoryboard = XamlReader.Load(SwivelInStoryboard) as Storyboard;
            //_toastControlMainBorder.Projection = new PlaneProjection();

            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    ParentControl.Children.Add(_toastControlMainBorder);
            //    foreach (Timeline t in enteringStoryboard.Children)
            //    {
            //        Storyboard.SetTarget(t, _toastControlMainBorder);
            //    }
            //    enteringStoryboard.Begin();
            //});

            _dismissTimer = new DispatcherTimer();
            _dismissTimer.Interval = TimeSpan.FromMilliseconds(ToastShowDuration);
            _dismissTimer.Tick += OnDismissTimerTicked;
            _dismissTimer.Start();
        }

        private void OnDismissTimerTicked(object sender, EventArgs e)
        {
            if (_dismissTimer != null)
            {
                _dismissTimer.Tick -= OnDismissTimerTicked;
                _dismissTimer.Stop();
            }
            DismissToast(_toastControlMainBorder, false);
        }

        private void DismissToast(Border toastControlMainBorder, bool b)
        {
            _toastControlMainBorder.ManipulationStarted -= OnMainBorderManipulationStarted;
            _toastControlMainBorder.ManipulationDelta -= OnMainBorderManipulationDelta;
            _toastControlMainBorder.ManipulationCompleted -= OnMainBorderManipulationCompleted;

            Storyboard leavingStoryboard = XamlReader.Load(SwivelOutStoryboard) as Storyboard;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (Timeline t in leavingStoryboard.Children)
                {
                    Storyboard.SetTarget(t, _toastControlMainBorder);
                }

                leavingStoryboard.Begin();
                leavingStoryboard.Completed += delegate
                {
                    _toastNotificationManager.RootGrid.Children.Remove(_toastControlMainBorder);
                    RaiseToastDismissed();
                };
            });

            _dismissTimer = null;
        }

        private void RaiseToastDismissed()
        {
            throw new NotImplementedException();
            //TODO: Call _toastNotificationManager.CompleteToast();
        }

        private void OnToastBorderTapped(object sender, GestureEventArgs e)
        {
            Border toastBorder = sender as Border;
            if (toastBorder == null)
                return;

            toastBorder.Tap -= OnToastBorderTapped;
            DismissToast(toastBorder, true);
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
                DismissToast(_toastControlMainBorder, false);
            }
            else if (e.TotalManipulation.Translation.X > -10 && e.TotalManipulation.Translation.X < 10)
            {
                DismissToast(_toastControlMainBorder, true);
            }
            else
            {
                _translate.X = 0;
                _dismissTimer.Start();
            }
        }
    }
}
