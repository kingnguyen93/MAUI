using System.Runtime.Serialization;

namespace RocketPDF.Infrastructure.Exceptions
{
    [Serializable]
    public class ConflictException : Exception
    {
        public ConflictException() : this("Conflict")
        {
        }

        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}