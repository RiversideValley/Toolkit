using Cube.UI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Cube.UI.Controls
{
    public sealed partial class VoiceControl : UserControl
    {
        // LEGACY
        public event EventHandler<string> SpeechSubmitted; // Text captured from speech
        public event EventHandler<string> SpeechUnavailable; // No mic or permission disabled
        private SpeechRecognizer Recognizer;

        public VoiceControl() => this.InitializeComponent();

        private async void CaptureSpeech()
        {
            /*if (await VoiceHelper.RequestMicrophonePermission())
            {
                SpeechText.Text = string.Empty;
                SpeechProgress.IsActive = true;
                StopRecordingButton.Visibility = Visibility.Visible;
                Recognizer = new SpeechRecognizer();
                await Recognizer.CompileConstraintsAsync();

                Recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
                Recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);
                Recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

                var Session = Recognizer.ContinuousRecognitionSession;
                Session.ResultGenerated += Session_ResultGenerated;
                Session.Completed += Session_Completed;
                await Session.StartAsync();
            }
            else
            {
                RecordingContent.Visibility = Visibility.Collapsed;
                ErrorContent.Visibility = Visibility.Visible;
            }*/
        }

        private async void Session_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (SpeechSubmitted is not null)
                    SpeechSubmitted(this, SpeechText.Text);
                SpeechProgress.IsActive = false;
                StopRecordingButton.Visibility = Visibility.Collapsed;
            });
        }

        private async void Session_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            SpeechRecognitionResult Result = args.Result;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {    
               if (Result != null)
                {
                    SpeechText.Text = Result.Text;

                    AlternateList.ItemsSource = Result.GetAlternates(3);
                }
            });
        }
        private async void Stop_Click(object sender, RoutedEventArgs e) => await Recognizer.ContinuousRecognitionSession.StopAsync();

        private void VoiceControl_Loaded(object sender, RoutedEventArgs e) => CaptureSpeech();

        private async void VoiceControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await Recognizer.ContinuousRecognitionSession.StopAsync();
                    Recognizer.Dispose();
                });
            }
            catch { }
        }
    }
}
