namespace RandomMessageApp.Core.Exceptions;

public class InfrastructureException : CoreException
{
    public override int StatusCode => 500;

    public InfrastructureException(string message)
        : base(message)
    {
    }
}
