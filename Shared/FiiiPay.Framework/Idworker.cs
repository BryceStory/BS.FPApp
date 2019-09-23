using System;

namespace FiiiPay.Framework
{
    internal class IdWorker
    {
        private readonly object _lock = new object();

        private readonly long _workerId;

        private const long Twepoch = 12888349746579L;

        private static readonly int workerIdBits = 10;
        private static readonly int sequenceBits = 12;        

        private long _lastTimestamp = -1L;

        private long _sequence;

        private readonly long maxWorkerId = -1L ^ -1L << workerIdBits;
        private readonly long sequenceMask = -1L ^ -1L << sequenceBits;

        private readonly int workerIdShift = sequenceBits;
        private readonly int timestampLeftShift = sequenceBits + workerIdBits;

        private static readonly DateTime DateTime197011 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public IdWorker(long workerId)
        {
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(workerId));
            }

            _workerId = workerId;
        }
        
        public long Id()
        {
            lock (_lock)
            {
                var timestamp = CurrentTimestamp();
                if (timestamp < _lastTimestamp)
                {
                    throw new InvalidSystemClockException($"Clock moved backwards. Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");
                }
                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1L) & sequenceMask;
                    if (_sequence == 0L)
                    {
                        timestamp = NextTimestamp(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0L;
                }
                _lastTimestamp = timestamp;
                return timestamp - Twepoch << timestampLeftShift | _workerId << workerIdShift | _sequence;
            }
        }

        private static long NextTimestamp(long lastTimestamp)
        {
            var timestamp = CurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = CurrentTimestamp();
            }
            return timestamp;
        }

        private static long CurrentTimestamp()
        {
            return (long)(DateTime.UtcNow - DateTime197011).TotalMilliseconds;
        }
    }
}
