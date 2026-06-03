using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Abstractions;

public static class DefaultConfiguration
{
    public static IConfiguration Create() => DefaultConfigurationBuilder.Create().Build();
}
