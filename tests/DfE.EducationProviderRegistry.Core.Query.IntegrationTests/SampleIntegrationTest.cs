using System;
using System.Collections.Generic;
using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests;

public sealed class SampleIntegrationTest : UseCaseIntegrationTestBase
{
    public SampleIntegrationTest(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    [Fact]
    public void IsTrue()
    {
        Assert.True(true);
    }
}
