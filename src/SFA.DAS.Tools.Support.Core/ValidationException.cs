using System;

namespace SFA.DAS.Tools.Support.Core;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}