using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Threading;

namespace MPlayerOne
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool UserIsDragginSlider = false;
        private bool MediaPlayerIsPlaying = false;
        
        double beforeMuting;


        private bool Fullscreen = false;

        

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_tick;
            timer.Start();

            this.inCaseOfMusicOrImage.Source = new BitmapImage(new Uri("ImagesFolder\\Music.png", UriKind.Relative));
            inCaseOfMusicOrImage.Visibility = Visibility.Collapsed;
            beforeMuting = mediaPlayerMain.Volume;

            
        }

        private void timer_tick(object sender, EventArgs e)
        {
            if ((mediaPlayerMain.Source != null) && (mediaPlayerMain.NaturalDuration.HasTimeSpan) && (!UserIsDragginSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = mediaPlayerMain.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = mediaPlayerMain.Position.TotalSeconds;
            }
        }

        private void OpenBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            inCaseOfMusicOrImage.Source = null;
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "Media files (*.mp3;*.mpg;*.mpeg;*.Wav;*.mkv)|*.mp3;*.mpg;*.mpeg;*.wav*.avi*.mkv|All files (*.*)|*.*";
            ofd.Filter = "All files (*.*)|*.*";
            

            if (ofd.ShowDialog() == true)
            {

                if (ofd.FileName.ToLower().Contains(".mp3") || ofd.FileName.ToLower().Contains(".mpg") || ofd.FileName.ToLower().Contains(".wav"))
                {
                    mediaPlayerMain.Source = new Uri(ofd.FileName);
                    MediaPlayerIsPlaying = true;
                    this.inCaseOfMusicOrImage.Source = new BitmapImage(new Uri("ImagesFolder\\Music.png", UriKind.Relative));
                    inCaseOfMusicOrImage.Visibility = Visibility.Visible;
                    mediaPlayerMain.Play();

                }
                else if (ofd.FileName.ToLower().Contains(".jpeg")|| ofd.FileName.ToLower().Contains(".png") || ofd.FileName.ToLower().Contains(".jpg"))
                {
                    inCaseOfMusicOrImage.Source = new BitmapImage(new Uri(ofd.FileName));
                    inCaseOfMusicOrImage.Visibility = Visibility.Visible;

                }
                else
                {
                    mediaPlayerMain.Source = new Uri(ofd.FileName);
                    MediaPlayerIsPlaying = true;
                    if (inCaseOfMusicOrImage.Visibility == Visibility.Visible)
                    {
                        inCaseOfMusicOrImage.Visibility = Visibility.Collapsed;
                    }
                    mediaPlayerMain.Play();
                }
            }
            
        }

        private void PlayBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mediaPlayerMain != null) && (mediaPlayerMain.Source != null);
        }

        private void PlayBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayerMain.Play();
            MediaPlayerIsPlaying = true;
        }

        private void PauseBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaPlayerIsPlaying;
        }

        private void PauseBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayerMain.Pause();
        }

        private void StopBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaPlayerIsPlaying;
        }

        private void StopBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayerMain.Stop();
            MediaPlayerIsPlaying = false;
        }

        private void sliProgress_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            UserIsDragginSlider = false;
            mediaPlayerMain.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            UserIsDragginSlider = true;
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds((int)sliProgress.Value).ToString();
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mediaPlayerMain.Volume += (e.Delta > 0) ? 0.1 : -0.1;
            beforeMuting = mediaPlayerMain.Volume;
        }

        private void muteBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if(mediaPlayerMain.Volume != 0)
            {
                mediaPlayerMain.Volume = 0;
            }
            else
            {
                mediaPlayerMain.Volume = beforeMuting;
            }
        }

        private void OnButtonDownListener(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if(mediaPlayerMain.Source != null && mediaPlayerMain != null)
                    {
                        mediaPlayerMain.Position = TimeSpan.FromSeconds(sliProgress.Value - 5);
                    }
                    break;
                case Key.Right:
                    if (mediaPlayerMain.Source != null && mediaPlayerMain != null)
                    {
                        mediaPlayerMain.Position = TimeSpan.FromSeconds(sliProgress.Value + 5);
                    }
                    break;
                case Key.OemPlus:
                    mediaPlayerMain.Volume +=0.1;
                    break;
                case Key.OemMinus:
                    mediaPlayerMain.Volume -= 0.1;
                    break;
                case Key.Space:
                    switch (MediaPlayerIsPlaying)
                    {
                        case true:
                            mediaPlayerMain.Pause();
                            MediaPlayerIsPlaying = false;
                            break;
                        case false:
                            mediaPlayerMain.Play();
                            MediaPlayerIsPlaying = true;
                            break;
                    }                    
                    break;
                case Key.F11:
                    ScreenResizer();
                    break;
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ScreenResizer();
        }

        private void ScreenResizer()
        {
            var desktopScreen = SystemParameters.WorkArea;
            
            switch (Fullscreen)
            {

                case false:
                    mainMenu.Visibility = Visibility.Collapsed;
                    mainToolBarStackPanel.Visibility = Visibility.Collapsed;
                    this.WindowState = WindowState.Maximized;
                    this.WindowStyle = WindowStyle.None;
                    this.Left = desktopScreen.Left;
                    this.Top = desktopScreen.Top;
                    Fullscreen = true;
                    break;
                case true:
                    mainMenu.Visibility = Visibility.Visible;
                    mainToolBarStackPanel.Visibility = Visibility.Visible;
                    this.WindowState = WindowState.Normal;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    Fullscreen = false;
                    break;
            }
        }

        private void mediaPlayerMain_MediaEnded(object sender, RoutedEventArgs e)
        {
            if(repeatBox.IsChecked == true)
            {
                mediaPlayerMain.Position = TimeSpan.FromSeconds(0);
                mediaPlayerMain.Play();
            }
            else
            {
                mediaPlayerMain.Source = null;
            }
        }
    }
}
