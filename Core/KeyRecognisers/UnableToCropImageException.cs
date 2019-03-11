using System;

namespace AlexNoddings.Infinit3.Core.KeyRecognisers
{
    public class UnableToCropImageException : Exception
    {
        public UnableToCropImageException()
        {
        }

        public UnableToCropImageException(string message)
            : base(message)
        {
        }

        public UnableToCropImageException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}