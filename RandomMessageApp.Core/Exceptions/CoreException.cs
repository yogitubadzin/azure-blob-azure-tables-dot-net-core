namespace RandomMessageApp.Core.Exceptions;

public class CoreException : Exception
{
    public virtual int StatusCode => 500;

    public CoreException(string message)
        : base(message)
    {
    }
}