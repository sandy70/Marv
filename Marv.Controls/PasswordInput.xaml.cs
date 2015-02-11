using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.MaskedInput;

namespace Marv.Controls
{
    public partial class PasswordInput : RadMaskedTextInput
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof (string), typeof (PasswordInput), new PropertyMetadata(""));

        public string Password
        {
            get
            {
                return (string) GetValue(PasswordProperty);
            }
            set
            {
                SetValue(PasswordProperty, value);
            }
        }

        public PasswordInput()
        {
            InitializeComponent();
            this.ValueChanging += PasswordInput_ValueChanging;
        }

        private void PasswordInput_ValueChanging(object sender, RadMaskedInputValueChangingEventArgs e)
        {
            if (e.NewValue != null)
            {
                var newString = e.NewValue as string;

                if (newString.Length > 0)
                {
                    this.Password += newString.Last();
                    this.Value += "*";
                }
            }
        }
    }
}