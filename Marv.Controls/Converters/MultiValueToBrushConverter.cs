using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Marv.Controls.Converters
{
    [ValueConversion(typeof (bool[]), typeof (Brush))]
    internal class MultiValueToBrushConverter : DependencyObject, IMultiValueConverter
    {
        public static readonly DependencyProperty FalseBrushProperty =
            DependencyProperty.Register("FalseBrush", typeof (Brush), typeof (MultiValueToBrushConverter), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty PartialTrueBrushProperty =
            DependencyProperty.Register("PartialTrueBrush", typeof (Brush), typeof (MultiValueToBrushConverter), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty TrueBrushProperty =
            DependencyProperty.Register("TrueBrush", typeof (Brush), typeof (MultiValueToBrushConverter), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush FalseBrush
        {
            get { return (Brush) this.GetValue(FalseBrushProperty); }

            set { this.SetValue(FalseBrushProperty, value); }
        }

        public Brush PartialTrueBrush
        {
            get { return (Brush) this.GetValue(PartialTrueBrushProperty); }

            set { this.SetValue(PartialTrueBrushProperty, value); }
        }

        public Brush TrueBrush
        {
            get { return (Brush) this.GetValue(TrueBrushProperty); }

            set { this.SetValue(TrueBrushProperty, value); }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var allTrueResult = true;
            var partialTrueResult = false;
            foreach (var value in values)
            {
                allTrueResult = allTrueResult && (bool)value;
                partialTrueResult = partialTrueResult || (bool)value;
            }
            if (allTrueResult)
            {
                return this.TrueBrush;
            }
            return partialTrueResult ? this.PartialTrueBrush : this.FalseBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}