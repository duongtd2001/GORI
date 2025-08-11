using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GORI.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            Storyboard fadeIn = (Storyboard)this.Resources["FadeInStoryboard"];
            fadeIn.Begin(this);
            this.StateChanged += Window_StateChanged;
        }
        private void pnControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void pnControlBar_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        }

        public void ShutdownWithFade()
        {
            Storyboard fadeOut = (Storyboard)this.Resources["FadeOutStoryboard"];
            fadeOut.Completed += (s, e) =>
            {
                Application.Current.Shutdown();
            };
            fadeOut.Begin(this);
        }

        public void MinimizeWithFade()
        {
            Storyboard fadeOut = (Storyboard)this.Resources["FadeOutStoryboard"];
            fadeOut.Completed += (s, e) =>
            {
                this.WindowState = WindowState.Minimized;
                this.Opacity = 1;
            };
            fadeOut.Begin(this);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            Storyboard fadeIn = (Storyboard)this.Resources["FadeInStoryboard"];
            fadeIn.Begin(this);
        }
    }
}
