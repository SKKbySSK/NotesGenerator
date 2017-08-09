using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NotesGenerator
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
#if RELEASE
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
#endif
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string stacktrace = "\n\n---エラー情報---\nType:" + e.Exception.GetType() + "\n\nMessage:\n" + e.Exception.Message + "\n\nStackTrace:\n" + e.Exception.StackTrace;
            System.Windows.Forms.MessageBox.Show("エラーによりメッセージ後にソフトが強制終了します。できれば以下のエラーメッセージを砂賀まで報告してください（自動でコピーされるのでLINE等にペーストしてください）\n" +
                    stacktrace);
            Clipboard.SetText(stacktrace);
            e.Handled = true;

            Current.Shutdown(-1);
        }
    }
}
