﻿using System.Windows;
using System.Windows.Media;

namespace Marv.Controls
{
    public partial class SliderProgressBar
    {
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof (bool), typeof (SliderProgressBar), new PropertyMetadata(false));

        public static readonly DependencyProperty SliderForegroundProperty =
            DependencyProperty.Register("SliderForeground", typeof (Brush), typeof (SliderProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty IsTextVisibleProperty =
            DependencyProperty.Register("IsTextVisible", typeof (bool), typeof (SliderProgressBar), new PropertyMetadata(true));

        public SliderProgressBar()
        {
            InitializeComponent();
        }

        public bool IsEditable
        {
            get
            {
                return (bool) GetValue(IsEditableProperty);
            }
            set
            {
                SetValue(IsEditableProperty, value);
            }
        }

        public bool IsTextVisible
        {
            get
            {
                return (bool) GetValue(IsTextVisibleProperty);
            }
            set
            {
                SetValue(IsTextVisibleProperty, value);
            }
        }

        public Brush SliderForeground
        {
            get
            {
                return (Brush) GetValue(SliderForegroundProperty);
            }
            set
            {
                SetValue(SliderForegroundProperty, value);
            }
        }
    }
}