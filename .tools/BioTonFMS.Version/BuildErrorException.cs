namespace BioTonFMS.Version
{
    using System;

    /// <summary>
    /// Thrown when a build error occurs, and the message (but not the stacktrace) should be
    /// displayed in the Visual Studio "Error List" view.
    /// </summary>
    public class BuildErrorException : Exception
    {
        /// <inheritdoc />
        public BuildErrorException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public BuildErrorException()
        {
        }

        /// <inheritdoc />
        public BuildErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}