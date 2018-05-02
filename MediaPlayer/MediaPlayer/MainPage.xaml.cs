using System;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MediaPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DisplayRequest appDisplayRequest = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        //void start_Click()
        //{
        //    myMediaElement.Play();
        //    InitializePropertyValues();
        //}
        // error: start_Click 没有与委托"RoutedEventHandler"匹配的重载
        // answer：start_Click 是重载，但是参数不对，下面是修正
        //void start_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    myMediaElement.Play();
        //    InitializePropertyValues();
        //}
        void start_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            myMediaElement.Play();
            InitializePropertyValues();
        }

        void pause_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            myMediaElement.Pause();
        }

        void stop_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }
        private void display_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isInFullScreenMode = view.IsFullScreenMode;
            if (isInFullScreenMode)
            {
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/背景12.jpg", UriKind.Absolute));
                MyGrid.Background = imageBrush;
                MyGrid.Background.Opacity = 0.5;
                view.ExitFullScreenMode();
            }
            else
            {
                MyGrid.Background = new SolidColorBrush(Colors.Black);// Windows.UI.Xaml.Media.Brush
                view.TryEnterFullScreenMode();
            }
            //if(myMediaElement.IsFullWindow)
            //{
            //    myMediaElement.IsFullWindow = false;
            //}
            //else
            //{
            //    myMediaElement.IsFullWindow = true;
            //}
        }
        private async void add_Click(object sender, RoutedEventArgs e)
        {
            await SetLocalMedia();
        }

        async private System.Threading.Tasks.Task SetLocalMedia()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            var file = await openPicker.PickSingleFileAsync();

            // mediaPlayer is a MediaElement defined in XAML
            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                myMediaElement.SetSource(stream, file.ContentType);

                myMediaElement.Play();
            }
        }

        private void Manual(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
        private void Stop(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
        
        private void ChangeMediaVolume(object sender, RangeBaseValueChangedEventArgs e)
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }
        private void ChangeMediaSpeedRatio(object sender, RangeBaseValueChangedEventArgs e)
        {
            //myMediaElement.SpeedRatio = (double)speedRarionSlider.Value;
        }
        private void Element_MediaOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            timelineSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
        }
        private void Element_MediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }
        private void SeekToMediaPosition(object sender, RangeBaseValueChangedEventArgs e)
        {
            int SliderValue = (int)timelineSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            myMediaElement.Position = ts;
        }
        void InitializePropertyValues()
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
            //myMediaElement.SpeedRatio = (double)speedRarionSlider.Value;
        }

        private void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = sender as MediaElement;
            if (mediaElement != null && mediaElement.IsAudioOnly == false)
            {
                if (mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
                {
                    if (appDisplayRequest == null)
                    {
                        // This call creates an instance of the DisplayRequest object. 
                        appDisplayRequest = new DisplayRequest();
                        appDisplayRequest.RequestActive();
                    }
                }
                else // CurrentState is Buffering, Closed, Opening, Paused, or Stopped. 
                {
                    if (appDisplayRequest != null)
                    {
                        // Deactivate the display request and set the var to null.
                        appDisplayRequest.RequestRelease();
                        appDisplayRequest = null;
                    }
                }
            }
        }
    }
}
