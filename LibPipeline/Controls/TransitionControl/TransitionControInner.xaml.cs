using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Telerik.Windows.Controls.TransitionEffects;

namespace LibPipeline
{
    public partial class TransitionControlInner : Grid
    {
        public static readonly DependencyProperty SelectedElementProperty =
        DependencyProperty.Register("SelectedElement", typeof(FrameworkElement), typeof(TransitionControlInner), new PropertyMetadata(null));

        public TransitionControlInner()
        {
            InitializeComponent();
        }

        public FrameworkElement SelectedElement
        {
            get { return (FrameworkElement)GetValue(SelectedElementProperty); }
            set { SetValue(SelectedElementProperty, value); }
        }
    }
}