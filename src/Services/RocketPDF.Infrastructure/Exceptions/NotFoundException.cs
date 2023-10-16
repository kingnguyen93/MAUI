using System.Runtime.Serialization;

namespace RocketPDF.Infrastructure.Exceptions
{
    /// <summary>
    /// Base exception type for those are thrown by Abp system for Abp specific exceptions.
    /// </summary>
    [Serializable]
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="NotFoundException"/> object.
        /// </summary>
        public NotFoundException() : this("Not found")
        {
        }

        /// <summary>
        /// Creates a new <see cref="NotFoundException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public NotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="NotFoundException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a new <see cref="NotFoundException"/> object.
        /// </summary>
        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}