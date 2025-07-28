using RepostConfirmationCanceler;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static Mutex MyMutex { get; } = new Mutex(false, @"Global\RepostConfirmationCancelerMutex");

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
                Task watchTask = Task.Run(() => ConfirmationDialogCanceler.WatchEdgeDialog(runtimeContext));
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
}
