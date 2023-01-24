using System;

namespace BioTonFMS.Common.Testable
{
    public static class SystemTime
    {
        private static DateTime _date = DateTime.MinValue;

        public static void Set(DateTime custom)
        { 
            _date = custom; 
        }
        
        public static void Reset()
        { 
            _date = DateTime.MinValue;
        }
        
        public static DateTime Now
        {
            get 
            {
                if (_date != DateTime.MinValue)
                {
                    return _date;
                }
                return DateTime.Now;
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                if (_date != DateTime.MinValue)
                {
                    return _date.ToUniversalTime();
                }
                return DateTime.UtcNow;
            }
        }

        public static DateTime Today
        {
            get
            {
                if (_date != DateTime.MinValue)
                {
                    return _date;
                }
                return DateTime.Today;
            }
        }

    }
}
