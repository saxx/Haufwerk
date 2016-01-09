using System;

namespace Haufwerk.Client
{
    public class HaufwerkException : Exception
    {
        public HaufwerkException(Exception innerException) : base("Unable to post to Haufwerk: " + innerException.Message, innerException) { }
    }
}
