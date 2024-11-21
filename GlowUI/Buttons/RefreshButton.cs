using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cube.UI.Animations;
using Cube.UI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Cube.UI.Buttons
{
    public partial class RefreshButton : AnimatedButton
    {
        protected bool isRefreshing = false;
        public event EventHandler RefreshClicked;
        public event EventHandler CancelClicked;

        protected UIElement CancelIcon
        {
            get { return (UIElement)GetTemplateChild("CancelIcon"); }
        }

        public RefreshButton()
        {
            this.DefaultStyleKey = typeof(RefreshButton);
            this.AnimatedIcon = new RefreshAnimation();
            this.Click += RefreshButton_Click;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e) => Refresh();

        protected virtual async void Refresh()
        {
            if (isRefreshing) // Cancel refresh
            {
                CancelIcon.Visibility = Visibility.Collapsed;
                Player.Opacity = 1;
                if (CancelClicked is not null)
                    CancelClicked(this, new EventArgs());
                isRefreshing = false;
            }
            else
            {
                if (RefreshClicked is not null)
                    RefreshClicked(this, new EventArgs());
                isRefreshing = true;
                await Task.Delay(500); // wait for refresh animation to finish
                if (isRefreshing)
                {
                    CancelIcon.Visibility = Visibility.Visible;
                    Player.Opacity = 0;
                }
            }
        }
    }
}
