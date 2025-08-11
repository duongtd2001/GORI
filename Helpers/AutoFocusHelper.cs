using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace GORI.Helpers
{
    public class AutoFocusHelper
    {
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused",
            typeof(bool),
            typeof(AutoFocusHelper),
            new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }
        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && (bool)e.NewValue)
            {
                textBox.Dispatcher.BeginInvoke((System.Action)(() =>
                {
                    textBox.Focus();
                    Keyboard.Focus(textBox);
                }));
            }
        }
    }
}
