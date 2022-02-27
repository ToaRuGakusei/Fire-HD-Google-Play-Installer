using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Fire_HD_Google_Play_Installer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private int i = 0;
        private Encoding enc = Encoding.GetEncoding("Shift_JIS");
        private int n = 0;
        private System.Net.WebClient wc = new System.Net.WebClient();
        System.Net.WebClient wc2 = new System.Net.WebClient();//なぜ分けた？？？？
        private ProcessStartInfo si;
        private bool finish;
        private void Install_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc2 = new WebClient();
            if (five.IsChecked == true)
            {
                int i = 0;
                Directory.CreateDirectory(@".\Download");
                string[] Urls = new string[] { "https://d-01.aabstatic.com/1017/google_account_manager_5.1-1743759_androidapksbox.apk", "https://d-01.aabstatic.com/1017/google_services_framework_5.1-1743759_androidapksbox.apk", "https://www.apkmirror.com/wp-content/themes/APKMirror/download.php?id=2200822", "https://d-01.aabstatic.com/1020/google_play_store_22.4.25-21_androidapksbox.apk" };
                foreach (string url in Urls)
                {

                    downloadFileAsync(url, $".\\Download\\{i}.apk");
                    i++;
                }

            }

        }

        private void adb()
        {
            StreamReader sm = new StreamReader(Path.GetTempPath() + "\\" + "Path.txt");
            string saveFolder = sm.ReadToEnd();
            StreamReader sm3 = new StreamReader(Path.GetTempPath() + "\\" + "switch.txt");
            string mp4_mkv = sm3.ReadToEnd();
            sm.Close();
            sm3.Close();
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (five.IsChecked == true)
                {
                    MessageBox.Show("タブレットをパソコンに接続してください。\n※固まってしまった場合は強制終了させてください。","Infomation", MessageBoxButton.OK,MessageBoxImage.Information);
                    si = new ProcessStartInfo(@"adb.exe", $"install .\\Download\\0.apk");
                }


            }));

            // ウィンドウ表示を完全に消したい場合
            si.CreateNoWindow = true;
            si.RedirectStandardError = true;
            si.RedirectStandardOutput = true;
            si.UseShellExecute = false;
            using (var proc = new Process())
            using (var ctoken = new CancellationTokenSource())
            {

                proc.EnableRaisingEvents = true;
                proc.StartInfo = si;
                // コールバックの設定
                proc.Exited += (s, ev) =>
                {
                    Console.WriteLine($"exited");
                    this.Dispatcher.Invoke((Action)(() =>
                    {

                    }));
                    // プロセスが終了すると呼ばれる
                    ctoken.Cancel();
                };
                // プロセスの開始
                proc.Start();
                Task.WaitAll(
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            var l = proc.StandardOutput.ReadLine();
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                adb_Log.Content = l;

                            }));
                            if (l == null)
                            {
                                break;
                            }
                            try
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {

                                }));
                            }
                            catch (Exception)
                            {

                            }
                        }
                        n = 0;

                    }),
                    Task.Run(() =>
                    {
                        ctoken.Token.WaitHandle.WaitOne();
                        proc.WaitForExit();
                    })
                );
            }





        }



        private async void downloadFileAsync(string uri, string outputPath)
        {
            var client = new HttpClient();
            HttpResponseMessage res = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

            using (var fileStream = File.Create(outputPath))
            {
                using (var httpStream = await res.Content.ReadAsStreamAsync())
                {
                    httpStream.CopyTo(fileStream);
                    fileStream.Flush();
                    n++;
                }
            }
            if (n == 3)
            {
                finish = true;
                Debug.WriteLine("完了");
                adb();

            }
        }


    }
}

