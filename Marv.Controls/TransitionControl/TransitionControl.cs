using Marv;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Telerik.Windows.Controls.TransitionControl;
using Telerik.Windows.Controls.TransitionEffects;

namespace Marv.Controls
{
    [ContentProperty("Elements")]
    public class TransitionControl : UserControl
    {
        public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.RegisterAttached("DisplayName", typeof(string), typeof(TransitionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ElementsProperty =
        DependencyProperty.Register("Elements", typeof(ObservableCollection<FrameworkElement>), typeof(TransitionControl), new PropertyMetadata(new ObservableCollection<FrameworkElement>()));

        public static readonly DependencyProperty ElementStackProperty =
        DependencyProperty.Register("ElementStack", typeof(Stack<FrameworkElement>), typeof(TransitionControl), new PropertyMetadata(new Stack<FrameworkElement>()));

        public static readonly DependencyProperty IsHeaderVisibleProperty =
        DependencyProperty.RegisterAttached("IsHeaderVisible", typeof(bool), typeof(TransitionControl), new UIPropertyMetadata(false));

        private TransitionControlInner transitionControlInner = new TransitionControlInner();

        public event EventHandler<TransitionStatusChangedEventArgs> StatusChanged;

        public ObservableCollection<FrameworkElement> Elements
        {
            get { return (ObservableCollection<FrameworkElement>)GetValue(ElementsProperty); }
            set { SetValue(ElementsProperty, value); }
        }

        public Stack<FrameworkElement> ElementStack
        {
            get { return (Stack<FrameworkElement>)GetValue(ElementStackProperty); }
            set { SetValue(ElementStackProperty, value); }
        }

        [AttachedPropertyBrowsableForChildren]
        public static string GetDisplayName(DependencyObject obj)
        {
            return (string)obj.GetValue(DisplayNameProperty);
        }

        [AttachedPropertyBrowsableForChildren]
        public static bool GetIsHeaderVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHeaderVisibleProperty);
        }

        public static void SetDisplayName(DependencyObject obj, string value)
        {
            obj.SetValue(DisplayNameProperty, value);
        }

        public static void SetIsHeaderVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHeaderVisibleProperty, value);
        }

        public void SelectElement(string name)
        {
            this.ElementStack.Push(this.transitionControlInner.SelectedElement);

            this.transitionControlInner.RadTransitionControl.Transition = new SlideAndZoomTransition
            {
                MinZoom = 1,
                SlideDirection = FlowDirection.RightToLeft
            };

            var elements = this.Elements.Where(x => x.Name.Equals(name)).ToList();

            if (elements.Count() > 0)
            {
                this.transitionControlInner.SelectedElement = elements.First();
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            foreach (var element in this.Elements)
            {
                var nameScope = NameScope.GetNameScope(this.Parent.GetParent<ContentControl>());
                NameScope.SetNameScope(element, NameScope.GetNameScope(this.Parent.GetParent<ContentControl>()));
            }

            this.Content = this.transitionControlInner;

            this.Loaded += TransitionControl_Loaded;
        }

        private void TransitionControl_Loaded(object sender, RoutedEventArgs e)
        {
            var transitionControl = this;

            transitionControl.Elements.CollectionChanged += (s1, e1) =>
            {
                if (transitionControl.Elements.Count > 0)
                {
                    transitionControlInner.SelectedElement = transitionControl.Elements.First();
                    transitionControl.ElementStack.Push(transitionControlInner.SelectedElement);
                }
            };

            if (transitionControl.Elements.Count > 0)
            {
                transitionControlInner.SelectedElement = transitionControl.Elements.First();
                transitionControl.ElementStack.Push(transitionControlInner.SelectedElement);
            }

            this.transitionControlInner.BackButton.Click += (s1, e1) =>
                {
                    this.transitionControlInner.RadTransitionControl.Transition = new SlideAndZoomTransition
                    {
                        MinZoom = 1,
                        SlideDirection = FlowDirection.LeftToRight
                    };

                    if (this.ElementStack.Count > 0)
                    {
                        this.transitionControlInner.SelectedElement = this.ElementStack.Pop();
                    }
                };

            this.transitionControlInner.RadTransitionControl.TransitionStatusChanged += (s2, e2) =>
                {
                    if (this.StatusChanged != null)
                    {
                        this.StatusChanged(this, e2);
                    }
                };
        }
    }
}