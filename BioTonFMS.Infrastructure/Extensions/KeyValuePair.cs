namespace BioTonFMS.Infrastructure.Extensions
{
    /// <summary>
    /// Пара ключ-значение
    /// </summary>
    public struct KeyValuePair
    {
        /// <summary>
        /// Установка пары ключ-значение
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="value">значение</param>
        public KeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; } = "";
        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; } = "";
    }
}
