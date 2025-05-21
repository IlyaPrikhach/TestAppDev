namespace TestAppDev.Models;

public class SecureException : Exception
{
    public SecureException(string message) : base(message) { }
}
