namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;


public sealed class InvalidGroupIdentifierException : ArgumentException
{
    public InvalidGroupIdentifierException(string value)
        : base($"Invalid GroupId: {value}")
    {
    }
}
