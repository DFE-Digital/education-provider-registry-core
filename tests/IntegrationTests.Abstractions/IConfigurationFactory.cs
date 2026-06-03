using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Abstractions;

public static class IConfigurationFactory
{
    public static IConfiguration CreateEmpty() => IConfigurationBuilderFactory.CreateDefault().Build();
}
