using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Cube.UI.Buttons
{
    [INotifyPropertyChanged]
    public partial class AnimatedButton : Button
    {
        private IAnimatedVisualSource icon;

        public IAnimatedVisualSource AnimatedIcon
        {
            get => icon;
            set
            {
                SetProperty(ref icon, value);
                if(Player is not null)
                    Player.Source = value;
            }
        }

        protected AnimatedVisualPlayer Player
        {
            get { return (AnimatedVisualPlayer)GetTemplateChild("Icon"); }
        }

        public AnimatedButton() => this.DefaultStyleKey = typeof(AnimatedButton);

        protected async override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Click += async delegate { await Player.PlayAsync(0, 1, false); };
            if (AnimatedIcon is not null)
                Player.Source = AnimatedIcon;
        }
    }
}
