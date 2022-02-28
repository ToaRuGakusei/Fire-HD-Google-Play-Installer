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
        private void Install_Click(object sender, RoutedEventArgs e)
        {
            n = 0;
            WebClient wc2 = new WebClient();
            if (five.IsChecked == true)
            {
                int i = 0;
                Directory.CreateDirectory(@".\Download");
                string[] Urls = new string[] { "https://d-01.aabstatic.com/1017/google_account_manager_5.1-1743759_androidapksbox.apk", "https://d-01.aabstatic.com/1017/google_services_framework_5.1-1743759_androidapksbox.apk", "https://r2-static-assets.androidapksfree.com/sdata/5e3f9bb672ebf370bd6b3faf7e661843/com.google.android.gms_v22.02.21_(000300-428111784)-220221000_Android-4.4.apk", "https://d-01.aabstatic.com/1020/google_play_store_22.4.25-21_androidapksbox.apk" };
                foreach (string url in Urls)
                {

                    downloadFileAsync(url, $".\\Download\\{i}.apk");
                    i++;
                }

            }
            else if (seven.IsChecked == true)
            {
                /*int i = 0;
                Directory.CreateDirectory(@".\Download");
                string[] Urls = new string[] { "https://appszx.com/wp-content/uploads/2019/08/com.google.android.gsf_.login_7.1.2-25_minAPI23nodpi_appszx.com_.apk", "https://github.com/jjqqkk/android-vpn/releases/download/1903/google-services-framework-9.apk", "https://r2-static-assets.androidapksfree.com/sdata/5e3f9bb672ebf370bd6b3faf7e661843/com.google.android.gms_v22.02.21_(000300-428111784)-220221000_Android-4.4.apk", "https://us.softpedia-secure-download.com/dl/d39f328a3f8691003f3b2dbc927918d9/621c4cb7/800000089/apk/Google%20Play%20Store-16.8.19-all%20[0]%20[PR]%20270615440.apk" };
                foreach (string url in Urls)
                {

                    downloadFileAsync(url, $".\\Download\\{i}.apk");
                    i++;
                }*/
                adb();

            }

        }

        private void adb()
        {
            int c = 0;
            MessageBox.Show("タブレットをパソコンに接続してください。\n※固まってしまった場合は強制終了させてください。", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
            si = new ProcessStartInfo(@"adb.exe", $"device");
            MessageBox.Show("USBデバッグを許可してください。", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
            for (c = 0; c <= 3; c++)
            {

                if (seven.IsChecked == true)
                {
                    si = new ProcessStartInfo(@"adb.exe", $"install .\\Fire_OS_7\\{c}.apk");
                }
                else if (six.IsChecked == true)
                {
                    si = new ProcessStartInfo(@"adb.exe", $"install .\\Fire_OS_6\\{c}.apk");
                }
                else if(five.IsChecked == true)
                {
                    si = new ProcessStartInfo(@"adb.exe", $"install .\\Download\\{c}.apk");
                }
                


                // ウィンドウ表示を完全に消したい場合
                // si.CreateNoWindow = true;
                si.RedirectStandardError = true;
                si.RedirectStandardOutput = true;
                si.UseShellExecute = false;
                using (var proc = new Process())
                using (var ctoken = new CancellationTokenSource())
                {
                    proc.EnableRaisingEvents = true;
                    proc.StartInfo = si;
                    // コールバックの設定
                    proc.Exited += (sender, ev) =>
                    {
                        Console.WriteLine($"exited");

                        n = 0;
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
                                if (l == null)
                                {
                                    break;
                                }
                                Console.WriteLine($"stdout = {l}");

                            }
                        }),
                        Task.Run(() =>
                        {
                            ctoken.Token.WaitHandle.WaitOne();
                            proc.WaitForExit();
                        })
                    );
                }
            }


            MessageBox.Show("処理が終了しました！\n一度再起動してください。", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);




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
                Debug.WriteLine("完了");
                adb();


                DirectoryInfo di = new DirectoryInfo(@".\Download");
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
                Console.WriteLine("Files deleted successfully");


            }

        }


    }
}

