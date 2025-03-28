﻿using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Tools.Support.Infrastructure.Configuration;

public class ToolsSupportOuterApiConfiguration : IApimClientConfiguration
{
    public string ApiBaseUrl { get; set; }

    public string SubscriptionKey { get; set; }

    public string ApiVersion { get; set; }
}