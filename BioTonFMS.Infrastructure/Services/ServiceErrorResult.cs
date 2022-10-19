namespace BioTonFMS.Infrastructure.Services
{
    public class ServiceErrorResult
    {
        /// <summary>
        ///  Вложенный объект с сообщением об ошибке
        /// </summary>
        public ErrorWithMessage Error { get; set; }

        public ServiceErrorResult(string message)
        {
            Error = new ErrorWithMessage(message);
        }

        public class ErrorWithMessage
        {
            /// <summary>
            /// Сообщение об ошибке
            /// </summary>
            public string Message { get; set; } = "";

            public ErrorWithMessage(string message)
            {
                Message = message;
            }
        }
    }
}
