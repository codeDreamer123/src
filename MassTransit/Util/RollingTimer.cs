// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Util
{
    using System;
    using System.Threading;


    /// <summary>
    /// Thread safe timer that allows efficient restarts by rolling the due time further into the future.
    /// Will roll over once every 43~ days of continuous runtime without a restart.
    /// </summary>
    public class RollingTimer :
        IDisposable
    {
        readonly TimerCallback _callback;
        readonly TimeSpan _dueTime;
        readonly object _lock = new object();
        Timer _timer;
        int _triggered;

        public RollingTimer(TimerCallback callback, TimeSpan dueTime)
        {
            _callback = o =>
            {
                Interlocked.CompareExchange(ref _triggered, 1, 0);
                callback(o);
            };
            _dueTime = dueTime;
        }

        public bool Triggered => Interlocked.CompareExchange(ref _triggered, int.MinValue, int.MinValue) == 1;

        public void Dispose()
        {
            lock (_lock)
            {
                Interlocked.CompareExchange(ref _triggered, 0, 1);
                _timer?.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Creates a new timer and starts it.
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                _timer?.Dispose();
                StartInternal();
            }
        }

        void StartInternal()
        {
            Interlocked.CompareExchange(ref _triggered, 0, 1);
            _timer = new Timer(_callback, null, _dueTime, TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Restarts the existing timer, creates and starts a new timer if it does not exist.
        /// </summary>
        public void Restart()
        {
            lock (_lock)
            {
                if (_timer == null)
                    StartInternal();
                else
                {
                    Interlocked.CompareExchange(ref _triggered, 0, 1);
                    _timer.Change(_dueTime, TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        /// <summary>
        /// Stops and disposes the existing timer.
        /// </summary>
        public void Stop()
        {
            Dispose();
        }
    }
}