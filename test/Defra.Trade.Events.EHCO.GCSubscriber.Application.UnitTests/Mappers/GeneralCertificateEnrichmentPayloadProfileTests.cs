// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Mappers;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Mappers;

public class GeneralCertificateEnrichmentPayloadProfileTests
{
    private readonly IFixture _fixture;

    public GeneralCertificateEnrichmentPayloadProfileTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void AutoMapper_Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<GeneralCertificateEnrichmentPayloadProfile>());
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void GeneralCertificateEnrichmentPayloadMapper_Should_MappAsExpected()
    {
        // Arrange
        var message = _fixture.Create<GeneralCertificateRequest>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<GeneralCertificateEnrichmentPayloadProfile>());
        var mapper = config.CreateMapper();

        // Act
        var result = mapper.Map<GeneralCertificateEnrichmentPayload>(message);

        // Assert
        result.GcId.ShouldBe(message.ExchangedDocument.Id);
        result.Applicant.ShouldBe(message.ExchangedDocument.Applicant);
        result.Consignee.ShouldBe(message.SupplyChainConsignment.Consignee);
        result.Consignor.ShouldBe(message.SupplyChainConsignment.Consignor);
        result.DispatchLocation.ShouldBe(message.SupplyChainConsignment.DispatchLocation);
        result.DestinationLocation.ShouldBe(message.SupplyChainConsignment.DestinationLocation);
    }
}
