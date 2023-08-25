namespace BioTonFMS.Infrastructure.Services
{
    public class ServiceErrorResult
    {
        private readonly List<string> _errors = new();
        /// <summary>
        /// Сообщения об ошибке
        /// </summary>
        public string[] Messages
        {
            get
            {
                return _errors.ToArray();
            }
        }

        public ServiceErrorResult()
        {
        }

        public ServiceErrorResult(string message)
        {
            _errors.Add(message);
        }

        public void AddError(string message) => _errors.Add(message);
    }
}
