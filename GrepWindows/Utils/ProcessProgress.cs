using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;
using Smithgeek.Extensions;

namespace Smithgeek.IO
{
    public class ProcessProgress
    {
        public Action<ProcessProgress> Notify { get; set; }

        private Double mLastNotificationSentSeconds;
        private Timer mTimer;

        private bool mSuspended;
        public void Suspend()
        {
            mSuspended = true;
        }

        public void Resume()
        {
            mSuspended = false;
        }

        private bool mCanceled;
        public bool Canceled 
        {
            get
            {
                return mCanceled;
            }
            set
            {
                if (value != mCanceled)
                {
                    mCanceled = value;
                    mStopwatch.Stop();
                    sendNotification();
                }
            }
        }

        private bool mCompleted;
        public bool Completed
        {
            get
            {
                return mCompleted;
            }
            set
            {
                if (value != mCompleted)
                {
                    mCompleted = value;
                    sendNotification();
                    if (mStopwatch != null)
                    {
                        mStopwatch.Stop();
                    }
                    if (mTimer != null)
                    {
                        mTimer.Stop();
                    }
                }
            }
        }
  
        private Stopwatch mStopwatch;

        private String mText;
        public String Text
        {
            get
            {
                return mText;
            }
            set
            {
                if (value != mText)
                {
                    mText = value;
                    sendNotification();
                }
            }
        }

        private long mTotal;
        public long Total 
        {
            get
            {
                return mTotal;
            }
            set
            {
                if (value != mTotal)
                {
                    mTotal = value;
                    if (mTotal > Processed && mStopwatch != null)
                    {
                        mStopwatch.Start();
                        mTimer.Start();
                    }
                    sendNotification();
                }
            }
        }
        
        private long mProcessed;
        
        public long Processed 
        {
            get
            {
                return mProcessed;
            }
            set
            {
                if (mProcessed != value)
                {
                    mProcessed = value;
                    if (mProcessed >= Total && !Completed)
                    {
                        Completed = true;
                    }
                    else
                    {
                        sendNotification();
                    }
                }
            }
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                return mStopwatch.Elapsed;
            }
        }

        public Double Rate
        {
            get
            {
                return (double)Processed / (double)ElapsedTime.TotalSeconds;
            }
        }

        public TimeSpan EstimatedTimeRemaining
        {
            get
            {
                return new TimeSpan(0, 0, (int)((double)(Total - Processed) / (double)Rate));
            }
        }

        public int Percent
        {
            get
            {
                if (Total == 0)
                    return 100;
                else
                    return Convert.ToInt32((double)Processed / (double)Total * 100);
            }
        }

        public ProcessProgress()
        {
            init();
            Processed = 0;
            Total = 0;
        }

        public ProcessProgress(int aTotal)
        {
            init();
            Processed = 0;
            Total = aTotal;
        }

        public ProcessProgress(int aTotal, Action<ProcessProgress> handler)
        {
            init();
            Processed = 0;
            Total = aTotal;
            Notify = handler;
        }

        public ProcessProgress(ProcessProgress aFileProgress)
        {
            if (aFileProgress == null)
            {
                init();
                Total = 0;
                Processed = 0;
            }
            else
            {
                mTimer = new Timer();
                mStopwatch = new Stopwatch();
                Total = aFileProgress.Total;
                Processed = aFileProgress.Processed;
                Canceled = aFileProgress.Canceled;
                Notify = aFileProgress.Notify;
            }
        }

        private void init()
        {
            mSuspended = false;
            mStopwatch = new Stopwatch();
            Notify = null;
            Canceled = false;
            mCompleted = false;
            mLastNotificationSentSeconds = -1;
            mTimer = new Timer();
            mTimer.Interval = 1000;
            mTimer.Stop();
            mTimer.Elapsed += new ElapsedEventHandler(mTimer_Tick);
        }

        void mTimer_Tick(object sender, EventArgs e)
        {
            if (mLastNotificationSentSeconds != ElapsedTime.TotalSeconds)
            {
                sendNotification();
            }
        }

        private void sendNotification()
        {
            if (Notify != null && !mSuspended)
            {
                mLastNotificationSentSeconds = ElapsedTime.TotalSeconds;
                Notify(this);
            }
        }

        public static bool operator ==(ProcessProgress prog1, ProcessProgress prog2)
        {
            if (prog1.isNull() || prog2.isNull())
            {
                if (prog1.isNull() && prog2.isNull())
                    return true;
                else
                    return false;
            }
            return prog1.Total == prog2.Total && prog1.Processed == prog2.Processed &&
                prog1.Canceled == prog2.Canceled;
        }

        public static bool operator !=(ProcessProgress prog1, ProcessProgress prog2)
        {
            if (prog1.isNull() || prog2.isNull())
            {
                if (prog1.isNull() && prog2.isNull())
                    return false;
                else
                    return true;
            }
            return prog1.Total != prog2.Total || prog1.Processed != prog2.Processed || 
                prog1.Canceled != prog2.Canceled;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            try
            {
                return (bool)(this == (ProcessProgress)obj);
            }
            catch
            {
                return false;
            }
        }
    }
}
