namespace BioTonFMS.Common.Testable
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class SystemGuid
    {
        private static Guid _guid;

        public static void Set(Guid custom)
        {
            _guid = custom;
        }

        public static void Reset()
        {
            _guid = new Guid();
        }

        public static Guid NewGuid()
        {
            if (_guid != new Guid())
            {
                return _guid;
            }
            return Guid.NewGuid();
        }
    }
}
