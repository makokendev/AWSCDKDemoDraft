using System;

namespace CodingChallenge.Application.Exceptions;
public class NFTTokenNotFoundException : Exception
{
    public NFTTokenNotFoundException(string message)
        : base(message)
    {
    }
}
public class NFTTokenAlreadyExistsException : Exception
{
    public NFTTokenAlreadyExistsException(string message)
        : base(message)
    {
    }
}
