using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Animation;

namespace Marv
{
    public class VisibilityAnimation : DependencyObject
    {
        private const int DurationMs = 5000;

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached("IsActive", typeof (bool), typeof (VisibilityAnimation), new FrameworkPropertyMetadata(false, OnIsActivePropertyChanged));

        public static readonly DependencyProperty IsScaleYActiveProperty =
            DependencyProperty.Register("IsScaleYActive", typeof (bool), typeof (VisibilityAnimation), new PropertyMetadata(true, OnIsActivePropertyChanged));

        private static readonly Hashtable HookedElements = new Hashtable();

        static VisibilityAnimation()
        {
            UIElement.VisibilityProperty.AddOwner(typeof (FrameworkElement), new FrameworkPropertyMetadata(Visibility.Visible, VisibilityChanged, CoerceVisibility));
        }

        public static bool GetIsActive(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool) element.GetValue(IsActiveProperty);
        }

        public static bool GetIsScaleYActive(UIElement element)
        {
            return (bool) element.GetValue(IsScaleYActiveProperty);
        }

        public static void SetIsActive(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsActiveProperty, value);
        }

        public static void SetIsScaleYActive(UIElement element, bool value)
        {
            element.SetValue(IsScaleYActiveProperty, value);
        }

        private static bool CheckAndUpdateAnimationStartedFlag(FrameworkElement fe)
        {
            var hookedElement = HookedElements.Contains(fe);
            if (!hookedElement)
            {
                return true; // don't need to animate unhooked elements.
            }

            var animationStarted = (bool) HookedElements[fe];
            HookedElements[fe] = !animationStarted;

            return animationStarted;
        }

        private static object CoerceVisibility(DependencyObject d, object baseValue)
        {
            var fe = d as FrameworkElement;

            if (fe == null)
            {
                return baseValue;
            }

            if (CheckAndUpdateAnimationStartedFlag(fe))
            {
                return baseValue;
            }
            // If we get here, it means we have to start fade in or fade out
            // animation. In any case return value of this method will be
            // Visibility.Visible. 

            var visibility = (Visibility) baseValue;

            var da = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(DurationMs))
            };

            da.Completed += (o, e) =>
            {
                // This will trigger value coercion again
                // but CheckAndUpdateAnimationStartedFlag() function will reture true
                // this time, and animation will not be triggered.
                fe.Visibility = visibility;
                // NB: Small problem here. This may and probably will brake 
                // binding to visibility property.
            };

            if (visibility == Visibility.Collapsed || visibility == Visibility.Hidden)
            {
                da.From = 1.0;
                da.To = 0.0;
            }
            else
            {
                da.From = 0.0;
                da.To = 1.0;
            }

            fe.BeginAnimation(UIElement.OpacityProperty, da);

            return Visibility.Visible;
        }

        private static void HookVisibilityChanges(FrameworkElement fe)
        {
            if (!HookedElements.Contains(fe))
            {
                HookedElements.Add(fe, false);
            }
        }

        private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = d as FrameworkElement;
            if (fe == null)
            {
                return;
            }

            if (GetIsActive(fe))
            {
                HookVisibilityChanges(fe);
            }
            else
            {
                UnHookVisibilityChanges(fe);
            }
        }

        private static void UnHookVisibilityChanges(FrameworkElement fe)
        {
            if (HookedElements.Contains(fe))
            {
                HookedElements.Remove(fe);
            }
        }

        private static void VisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // So what? Ignore.
        }
    }
}