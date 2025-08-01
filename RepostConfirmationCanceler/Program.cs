using Microsoft.Win32;
using RepostConfirmationCanceler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static Mutex MyMutex { get; } = new Mutex(false, @"Global\RepostConfirmationCancelerMutex");

    [STAThread]
    static void Main()
    {
        bool isMutexAcquired = false;
        try
        {
            isMutexAcquired = MyMutex.WaitOne(1000, false);
        }
        catch (AbandonedMutexException)
        {
            // 既に起動中のプロセスが強制終了した。Mutexの所有権は取得できている。
            // 本プログラムは、単なる同時起動の排他のためだけにMutexを使用しているので、この場合もそのまま
            // 処理を続けて問題ない。
            isMutexAcquired = true;
        }

        if (isMutexAcquired)
        {
            // Mutexの獲得に成功した
            // * NamedPipeサーバーを起動し、後続のプロセスによるメッセージ受付を開始
            // * ダイアログの監視を開始
            try
            {
                var runtimeContext = new RuntimeContext(RunTimeMode.Server);
                Task serverTask = Task.Run(() => ProcessCommunicator.RunNamedPipedServer(runtimeContext));
                Task watchTask = Task.Run(() => EdgeConfirmationDialogCanceler.WatchDialog(runtimeContext));
                Task.WhenAll(serverTask, watchTask).Wait();
            }
            finally
            {
                MyMutex.ReleaseMutex();
            }
        }
        else
        {
            // Mutexの獲得に失敗した場合、既にサーバーが起動中なので、先行のプロセスの実行時間を延ばす。
            var runtimeContext = new RuntimeContext(RunTimeMode.Client);
            ProcessCommunicator.SendKeepAliveMessage(runtimeContext);

        }
    }

    public static class RepostConfirmationCancelerUtils
    {
        public static string GetRepostConfirmationCancelerRulefile()
        {
            const string registryPath = @"SOFTWARE\RepostConfirmationCanceler";
            const string valueName = "Rulefile";
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    if (key == null)
                    {
                        Console.Error.WriteLine($"cannot read {registryPath}: key not found");
                        return null;
                    }
                    object value = key.GetValue(valueName);
                    if (value is string rulefile)
                    {
                        return rulefile;
                    }
                    else
                    {
                        Console.Error.WriteLine($"cannot read {registryPath}: 'Rulefile' not found or not string");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"cannot read {registryPath}: {ex.Message}");
                return null;
            }
        }
    }

// 補助クラス Section
public class Section
{
    public string Name;
    public bool Disabled = false;
    public bool WarningWhenCloseDialog = false;
    public StringBuilder ExcludeGroups = new StringBuilder();
    public StringBuilder Excludes = new StringBuilder();
    public StringBuilder Patterns = new StringBuilder();

    public Section(string name)
    {
        Name = name;
    }
}

// Configクラス
public class Config
{
    public bool IgnoreQueryString = false;
    public bool OnlyMainFrame = false;
    public List<Section> SectionList = new List<Section>();
}

public static class ConfParser
{

}

}
