namespace Application.Exceptions;

public class CreateRoleException : Exception
{
    public CreateRoleException(string message) : base(message)
    {
    }
}