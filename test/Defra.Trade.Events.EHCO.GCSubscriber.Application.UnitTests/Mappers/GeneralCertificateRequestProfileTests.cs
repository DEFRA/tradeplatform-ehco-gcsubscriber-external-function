// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.API.CertificatesStore.V1.ApiClient.Model;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Mappers;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Mappers;

public class GeneralCertificateRequestProfileTests
{
    private readonly IFixture _fixture;

    public GeneralCertificateRequestProfileTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void AutoMapper_Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<GeneralCertificateRequestProfile>());
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void AutoMapper_ConvertFrom_IsValid()
    {
        // Arrange
        var generalCertificateInbound = new GeneralCertificateInbound
        {
            ExchangedDocument = _fixture.Create<ExchangedDocument>(),
            SupplyChainConsignment = _fixture.Create<SupplyChainConsignment>()
        };

        var config = new MapperConfiguration(cfg => cfg.AddProfile<GeneralCertificateRequestProfile>());
        var mapper = config.CreateMapper();
        var result = mapper.Map<GeneralCertificateInbound, GeneralCertificateInbound>(generalCertificateInbound);

        // Assert
        result.ShouldBe(generalCertificateInbound);
    }
}
