using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace Riverside.Toolkit.UserControls
{
    /// <summary>
    /// A UserControl that represents an ImageFrame.
    /// </summary>
    public sealed partial class ImageFrame : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFrame"/> class.
        /// </summary>
        public ImageFrame()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
        "Source", // The name of the property
        typeof(string), // The type of the property
        typeof(ImageFrame), // The type of the owner class
        new PropertyMetadata(null) // Default value
        );

        /// <summary>
        /// Gets or sets the source of the content of the ImageFrame.
        /// </summary>
        [Browsable(true)]
        [Category("Common")]
        [Description("The source of the content of the ImageFrame")]
        public string Source
        {
            get
            {
                if (GetValue(SourceProperty).ToString().Contains("ms-appx://"))
                {
                    return (string)GetValue(SourceProperty);
                }
                else
                {
                    return BaseUri + (string)GetValue(SourceProperty);
                }
            }
            set { SetValue(SourceProperty, value); }
        }
    }
}
