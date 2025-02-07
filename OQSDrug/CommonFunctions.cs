using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OQSDrug
{
    public static class CommonFunctions
    {
        // グローバル変数の定義
        public static string DBProvider = "";
        public static string connectionOQSdata = "";
        public static bool DataDbLock = false;

        public static List<string[]> RSBDI = new List<string[]>();
        // Korodata Dictionary
        public static Dictionary<string, string> ReceptToMedisCodeMap = new Dictionary<string, string>();


        public static void SnapToScreenEdges(Form form, int snapDistance, int snapCompPixel)
        {
            Rectangle workingArea = Screen.FromControl(form).WorkingArea;

            int newX = form.Left;
            int newY = form.Top;

            if (Math.Abs(form.Left - workingArea.Left) <= snapDistance)
            {
                newX = workingArea.Left - snapCompPixel;
            }
            else if (Math.Abs(form.Right - workingArea.Right) <= snapDistance)
            {
                newX = workingArea.Right - form.Width + snapCompPixel;
            }

            if (Math.Abs(form.Top - workingArea.Top) <= snapDistance)
            {
                newY = workingArea.Top;
            }
            else if (Math.Abs(form.Bottom - workingArea.Bottom) <= snapDistance)
            {
                newY = workingArea.Bottom - form.Height + snapCompPixel;
            }

            form.Location = new Point(newX, newY);
        }


        public static async Task<bool> RetryClipboardSetTextAsync(string text)
        {
            const int maxRetries = 10;
            const int delayBetweenRetries = 50; // ミリ秒

            for (int attempts = 0; attempts < maxRetries; attempts++)
            {
                try
                {
                    // STA スレッドで Clipboard.SetText を実行
                    await Task.Run(() =>
                    {
                        var thread = new Thread(() =>
                        {
                            try
                            {
                                //Clipboard.SetText(text);
                                Clipboard.Clear(); // クリア
                                Clipboard.SetDataObject(text, true); // SetText の代わりに SetDataObject を利用
                            }
                            catch (Exception)
                            {
                                // 他の予期しない例外をキャッチして再スロー
                                throw;
                            }
                        });
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();
                    });

                    return true; // 成功
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    // 他のプロセスがクリップボードを使用している場合
                    await Task.Delay(delayBetweenRetries); // 少し待機してリトライ
                }
                catch (Exception ex)
                {
                    // 他の予期せぬ例外
                    MessageBox.Show($"エラー: {ex.Message}", "クリップボードエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return false; // 最大リトライ回数を超えた場合は失敗
        }

        public static async Task<bool> WaitForDbUnlock(int maxWaitms)
        {
            int interval = 10;
            int retry = maxWaitms / interval;
            for (int i = 0; i < retry; i++)
            {
                if (DataDbLock)
                {
                    await Task.Delay(interval);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }


}
