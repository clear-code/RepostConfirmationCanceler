/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.

Copyright (c) 2025 ClearCode Inc.
*/
using System;

namespace RepostConfirmationCanceler
{
    internal class RuntimeContext
    {
        private readonly object lockObject = new object();

        internal Logger Logger { get; }

        internal Config Config { get; }

        internal RuntimeContext(RunTimeMode mode)
        {
            FinishTime = DateTime.Now.AddMinutes(2);
            // 設定の読み込みに失敗した場合もログを出力できるよう、Loggerを先に初期化する。
            Logger = new Logger(mode);
            Config = LoadConfigOrDefault();
        }

        private Config LoadConfigOrDefault()
        {
            try
            {
                return ConfigLoader.LoadConfig();
            }
            catch (Exception ex)
            {
                // ルールファイルはGPOの基本設定「ファイル」でアクション「置換」により各端末へ
                // 配布する運用を案内しているため、グループポリシーの適用タイミングでファイルが
                // 一時的に存在しない、または他プロセスにロックされていることがある。
                // ここで例外を送出するとプロセスが動作できなくなるため、既定の設定で継続する。
                // 既定の設定ではダイアログのキャンセル後に警告を表示しないだけで、
                // キャンセル自体は従来通り行われる。
                Logger.Log("Failed to load the configuration. Continue with the default configuration.");
                Logger.Log(ex);
                return new Config();
            }
        }

        internal DateTime FinishTime
        {
            get
            {
                lock (lockObject)
                {
                    return _finishTime;
                }
            }
            set
            {
                lock (lockObject)
                {
                    _finishTime = value;
                }
            }
        }
        private DateTime _finishTime = DateTime.MinValue;

        internal bool IsEndTime
        {
            get
            {
                lock (lockObject)
                {
                    return _finishTime <= DateTime.Now;
                }
            }
        }
    }
}
