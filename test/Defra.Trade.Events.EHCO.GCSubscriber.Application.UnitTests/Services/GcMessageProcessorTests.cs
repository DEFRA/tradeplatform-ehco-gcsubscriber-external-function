// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using Defra.Trade.API.CertificatesStore.V1.ApiClient.Api;
using Defra.Trade.API.CertificatesStore.V1.ApiClient.Model;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Services;

public class GcMessageProcessorTests
{
    private readonly Mock<ILogger<GcMessageProcessor>> _logger;
    private readonly Mock<IEhcoGeneralCertificateApplicationApi> _ehcoGeneralCertificateApplicationApi;
    private readonly IFixture _fixture;
    private readonly GcMessageProcessor _sut;

    public GcMessageProcessorTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _logger = new Mock<ILogger<GcMessageProcessor>>();
        _ehcoGeneralCertificateApplicationApi = new Mock<IEhcoGeneralCertificateApplicationApi>();

        _sut = new GcMessageProcessor(_logger.Object, _ehcoGeneralCertificateApplicationApi.Object);
    }

    [Fact]
    public void Ctors_EnsureNotNullAndCorrectExceptionParameterName()
    {
        var assertion = new GuardClauseAssertion(_fixture);
        assertion.Verify(typeof(SbMessageProcessor).GetConstructors());
    }

    [Fact]
    public async Task ProcessMessage_WhenCalled_ShouldProcessToStore()
    {
        // Arrange
        var requestMessage = _fixture.Create<GeneralCertificateRequest>();
        var gcMessage = new EhcoGeneralCertificateApplication(
            requestMessage.ExchangedDocument,
            requestMessage.SupplyChainConsignment);

        _ehcoGeneralCertificateApplicationApi
            .Setup(
                x =>
                    x.SaveEHCOGeneralCertificateApplicationAsync(
                        "v1",
                        gcMessage,
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await _sut.ProcessMessage(requestMessage);

        // Assert
        _ehcoGeneralCertificateApplicationApi.Verify(
            x => x.SaveEHCOGeneralCertificateApplicationAsync(
                "v1",
                gcMessage,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task ProcessMessage_WhenCalledV1_ShouldNotCallDependency()
    {
        // Arrange
        var exchangedDocumentMock = _fixture.Build<ExchangedDocument>()
            .With(x => x.CertificatePDFLocation, "https;//www.domain.org/gcpdf.pdf")
            .Create();

        var requestMessage = _fixture.Build<GeneralCertificateRequest>()
            .With(x => x.ExchangedDocument, exchangedDocumentMock)
            .Create();

        var gcMessage = new EhcoGeneralCertificateApplication(
            requestMessage.ExchangedDocument,
            requestMessage.SupplyChainConsignment);

        _ehcoGeneralCertificateApplicationApi
            .Setup(
                x =>
                    x.SaveEHCOGeneralCertificateApplicationAsync(
                        "v1",
                        gcMessage,
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await _sut.ProcessMessage(requestMessage);

        // Assert
        _ehcoGeneralCertificateApplicationApi.Verify(
            x => x.SaveEHCOGeneralCertificateApplicationAsync(
                "v1",
                It.Is<EhcoGeneralCertificateApplication>(c =>
                    c.ExchangedDocument.CertificatePDFLocation == string.Empty),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
