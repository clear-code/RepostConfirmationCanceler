/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.

Copyright (c) 2025 ClearCode Inc.
*/
using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepostConfirmationCanceler
{
    internal static class ProcessCommunicator
    {
        private const string NAMED_PIPE_NAME_BASE = "RepostConfirmationCancelerNamedPipe";

        // 通信エラーが連続して発生した場合に、CPUを占有し続けないようにするための待ち時間。
        private static readonly TimeSpan RETRY_INTERVAL = TimeSpan.FromMilliseconds(500);

        private static string GeneratePipeName()
        {
            WindowsIdentity user = WindowsIdentity.GetCurrent();
            string sid = user.User.Value;
            return $"{NAMED_PIPE_NAME_BASE}_{sid}";
        }

        internal static void LogThreadStatus(RuntimeContext context)
        {
            try
            {
                ThreadPool.GetAvailableThreads(out int workerThreads, out int completionPortThreads);
                ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);

                int usedWorkerThreads = maxWorkerThreads - workerThreads;
                int usedCompletionPortThreads = maxCompletionPortThreads - completionPortThreads;

                context.Logger.Log($"Max worker threads: {maxWorkerThreads}");
                context.Logger.Log($"Used worker threads: {usedWorkerThreads}");
                context.Logger.Log($"Max completion port threads: {maxCompletionPortThreads}");
                context.Logger.Log($"Used completion port threads: {usedCompletionPortThreads}");
            }
            catch
            {
                //Do nothing
            }
        }

        internal static async Task RunNamedPipedServer(RuntimeContext context)
        {
            PipeSecurity ps = new PipeSecurity();
            ps.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.FullControl, AccessControlType.Allow));

            context.Logger.Log("Start server");
            LogThreadStatus(context);
            // FinishTime > DateTime.Nowではなく、trueでも良いが、念のため。
            while (!context.IsEndTime)
            {
                try
                {
                    // NamedPipeServerStreamの生成とWaitForConnectionAsyncの呼び出しは、awaitした時ではなく
                    // その場で同期的に例外を投げることがあるため、tryブロックの内側で実行する。
                    using (var pipeServer = new NamedPipeServerStream(GeneratePipeName(), PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024, ps))
                    {
                        var cancellationTokenSource = new CancellationTokenSource();
                        Task waitTask = pipeServer.WaitForConnectionAsync(cancellationTokenSource.Token);
                        TimeSpan waitDuration = context.FinishTime - DateTime.Now;
                        waitDuration = waitDuration < TimeSpan.Zero ? TimeSpan.Zero : waitDuration;
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます
                        Task.Delay(waitDuration).ContinueWith(t => cancellationTokenSource.Cancel());
#pragma warning restore CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます
                        context.Logger.Log("Start to wait client access");
                        //受信待ち。
                        await waitTask;
                        context.Logger.Log("Client connected");
                        using (var reader = new StreamReader(pipeServer, Encoding.UTF8, true, 1024, true))
                        {
                            context.Logger.Log($"Start receive");
                            var receiveString = await reader.ReadLineAsync();
                            context.Logger.Log($"Received string: {receiveString ?? "null"}");
                            if (string.IsNullOrEmpty(receiveString))
                            {
                                continue;
                            }
                            if (receiveString.ToLowerInvariant().Contains("keep-alive"))
                            {
                                context.FinishTime = DateTime.Now.AddMinutes(2);
                                continue;
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    context.Logger.Log("WaitForConnectionAsync was cancelled");
                    break;
                }
                catch (IOException ex)
                {
                    // サーバーが接続の受け付けを開始する前に、クライアントが接続・送信・切断まで完了した
                    // 場合(ERROR_NO_DATA)などに発生する。クライアントの依頼自体は送信済みであり、
                    // パイプを作り直して待ち受けを継続すればよいので、サーバーは終了させない。
                    context.Logger.Log(ex);
                    LogThreadStatus(context);
                    await Task.Delay(RETRY_INTERVAL);
                }
                catch (Exception ex)
                {
                    context.Logger.Log(ex);
                    break;
                }
            }
            context.Logger.Log("Stop server");
        }

        internal static void SendKeepAliveMessage(RuntimeContext context)
        {
            context.Logger.Log("Start to send keep-alive");
            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", GeneratePipeName(), PipeDirection.Out))
                {
                    pipeClient.Connect(15000);
                    using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                    {
                        writer.WriteLine("keep-alive");
                        context.Logger.Log("Sent keep-alive");
                    }
                }
            }
            catch (TimeoutException)
            {
                context.Logger.Log("Failed to connect to the named pipe server within the timeout period.");
                LogThreadStatus(context);
            }
            catch (Exception ex)
            {
                context.Logger.Log(ex);
            }
        }
    }
}
