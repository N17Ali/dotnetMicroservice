namespace PlatformsService.Exceptions;

public class DuplicateResourceException : Exception
{
    public DuplicateResourceException(string Message) : base(Message) { }
    public DuplicateResourceException(string Message, Exception InnerException) : base(Message, InnerException) { }
}