using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using static System.Net.WebRequestMethods;

namespace audio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsEnabled = true;
        public List<string> playlist = new List<string>();
        public MediaPlayer player = new MediaPlayer();
        public string filename;
        public bool isPaused = false;
        public bool isRepeat = false;
        public bool isRandom = false;
        private void StartThread()
        {
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        audioSlider.Value = media.Position.Ticks;
                        time.Text = TimeSpan.FromTicks((long)audioSlider.Value).ToString(@"hh\:mm\:ss");
                        ostatok.text =(media.naturalduration.timespan.ticks - media.position.ticks).tostring();
                        if (media.NaturalDuration.HasTimeSpan && media.Position.Ticks == media.NaturalDuration.TimeSpan.Ticks)
                        {
                            if (isRepeat)
                                media.Position = new TimeSpan(Convert.ToInt64(0));
                            else
                            {
                                if (Spisok.SelectedIndex == Spisok.Items.Count - 1)
                                {
                                    Spisok.SelectedIndex = 0;
                                }
                                else
                                {
                                    Spisok.SelectedIndex += 1;
                                }
                            }

                        }
                    }));

                }
                catch
                {
                    Environment.Exit(0);
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                filename = dialog.FileName;
                playlist = Directory.GetFiles(dialog.FileName).ToList();
                foreach (string file in playlist)
                {
                    if (file.EndsWith("mp3") || file.EndsWith("m4a") || file.EndsWith("wav"))
                    {
                        Spisok.Items.Add(file.Split("\\")[^1]);
                    }
                }
                Spisok.SelectedIndex = 0;
                Thread t = new Thread(_ =>
                {
                    StartThread();
                });
                t.Start();
                playMusic();
            }
        }
        private void playMusic()
        {
            media.Source = new Uri(playlist[Spisok.SelectedIndex]);
            media.Play();
        }

        private void playclick_Click(object sender, RoutedEventArgs e)
        {
            if (IsEnabled == false)
            {
                IsEnabled = true;
                playMusic();

            }
            else if (IsEnabled == true)
            {
                IsEnabled = false;
                stopMusic();
            }
        }

        private void stopMusic()
        {
            if (isPaused == false)
                media.Pause();
            else
                media.Play();
            isPaused = !isPaused;
        }

        private void backclick_Click(object sender, RoutedEventArgs e)
        {
            if (Spisok.SelectedIndex == 0)
            {
                Spisok.SelectedIndex = Spisok.Items.Count - 1;
            }
            else
            {
                Spisok.SelectedIndex -= 1;
            }
        }

        private void nextclick_Click(object sender, RoutedEventArgs e)
        {
            if (Spisok.SelectedIndex == Spisok.Items.Count - 1)
            {
                Spisok.SelectedIndex = 0;
            }
            else
            {
                Spisok.SelectedIndex += 1;
            }
        }

        private void replayclick_Click(object sender, RoutedEventArgs e)
        {
            isRepeat = !isRepeat;
        }

        private void randomclick_Click(object sender, RoutedEventArgs e)
        {
            isRandom = !isRandom;
            Spisok.Items.Clear();
            if (isRandom)
            {
                Random rnd = new Random();
                playlist = playlist.OrderBy(x => rnd.Next()).ToList();
                
            }
            else
            {
                playlist = Directory.GetFiles(filename).ToList();
            }
            foreach (string file in playlist)
            {
                if (file.EndsWith("mp3") || file.EndsWith("m4a") || file.EndsWith("wav"))
                {
                    Spisok.Items.Add(file.Split("\\")[^1]);
                }
            }
        }
        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            audioSlider.Maximum = media.NaturalDuration.TimeSpan.Ticks;
        }
        private void audioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = new TimeSpan(Convert.ToInt64(audioSlider.Value));
        }

        private void Spisok_Changed(object sender, RoutedEventArgs e)
        {
            if (Spisok.SelectedIndex == -1) return;
            else playMusic();
        }
    }
}
