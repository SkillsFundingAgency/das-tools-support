using System;

namespace SFA.DAS.Tools.Support.Core;

public class ValidationException(string message) : Exception(message);