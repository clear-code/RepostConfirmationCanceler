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
            FinishTime = DateTime.Now.AddMinutes(1);
            Logger = new Logger(mode);
            Config = ConfigLoader.LoadConfig();
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
        private static DateTime _finishTime = DateTime.MinValue;

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
