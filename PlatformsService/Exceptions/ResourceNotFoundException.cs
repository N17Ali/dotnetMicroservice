namespace PlatformsService.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string Message) : base(Message) { }
    public ResourceNotFoundException(string Message, Exception InnerException) : base(Message, InnerException) { }
}