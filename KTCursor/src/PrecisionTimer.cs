using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

// Peter A. Bromberg. High-Precision Code Timing in .NET. 
// http://www.eggheadcafe.com/articles/20021111.asp

namespace KinTemplates
{

    public class HiPerfTimer
        {
            [DllImport("Kernel32.dll")]
            private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
            [DllImport("Kernel32.dll")]
            private static extern bool QueryPerformanceFrequency(out long lpFrequency);

            private long startTime;
            private long stopTime;
            private long freq;
            /// <summary>
            /// ctor
            /// </summary>
            public HiPerfTimer()
            {
                startTime = 0;
                stopTime = 0;
                freq = 0;
                if (QueryPerformanceFrequency(out freq) == false)
                {
                    throw new Win32Exception(); // timer not supported
                }
            }
            /// <summary>
            /// Start the timer
            /// </summary>
            /// <returns>long - tick count</returns>
            public long Start()
            {
                QueryPerformanceCounter(out startTime);
                return startTime;
            }
            /// <summary>
            /// Stop timer
            /// </summary>
            /// <returns>long - tick count</returns>
            public long Stop()
            {
                QueryPerformanceCounter(out stopTime);
                return stopTime;
            }
            /// <summary>
            /// Return the duration of the timer (in seconds)
            /// </summary>
            /// <returns>double - duration</returns>
            public double Duration
            {
                get
                {
                    return (double)(stopTime - startTime) / (double)freq;
                }
            }

            public long DurationInterval
            {
                get
                {
                    return (stopTime - startTime);
                }
            }

            /// <summary>
            /// Frequency of timer (no counts in one second on this machine)
            /// </summary>
            ///<returns>long - Frequency</returns>
            public long Frequency
            {
                get
                {
                    QueryPerformanceFrequency(out freq);
                    return freq;
                }
            }
        }
    
}
