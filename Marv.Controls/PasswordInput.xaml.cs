using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.MaskedInput;
using Marv;
using NLog;

namespace Marv.Controls
{
    public partial class PasswordInput : RadMaskedTextInput
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(PasswordInput), new PropertyMetadata(""));

        public PasswordInput()
        {
            InitializeComponent();
            this.ValueChanging += PasswordInput_ValueChanging;
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
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