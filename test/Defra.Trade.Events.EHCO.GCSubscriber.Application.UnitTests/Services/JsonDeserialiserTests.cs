// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Text.Json;
using Defra.Trade.API.CertificatesStore.V1.ApiClient.Model;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Services;

public class JsonDeserialiserTests
{
    private readonly JsonDeserialiser _sut = new();
    private readonly IFixture _fixture;

    public JsonDeserialiserTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Deserialise_WhenInputInvalidObject_ShouldThrowArgumentException()
    {
        // Act
        var act = () => _sut.Deserialise<GeneralCertificateRequest>("Invalid-json");

        // Assert
        act.ShouldThrow<JsonException>("Unable to parse json data");
    }

    [Theory]
    [InlineData("")]
    public void Deserialise_WhenGivenValidJson_ShouldParseAsExpected(string json)
    {
        // Act
        var act = () => _sut.Deserialise<GeneralCertificateRequest>(json);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void Deserialise_GivenValidObject_ShouldParseAsExpected()
    {
        // Arrange
        var mockedObject = new GeneralCertificateRequest
        {
            SupplyChainConsignment = _fixture.Create<SupplyChainConsignment>()
        };
        string mockedJson = JsonSerializer.Serialize(mockedObject);

        // Act
        var result = _sut.Deserialise<GeneralCertificateRequest>(mockedJson);

        // Assert
        result.SupplyChainConsignment.ShouldBe(mockedObject.SupplyChainConsignment);
    }
}
