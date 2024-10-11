// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoFixture;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Validators;
using Shouldly;

namespace Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.Validators;

public class GeneralCertificateInboundValidatorTests
{
    private readonly Fixture _fixture;
    private readonly GeneralCertificateInboundValidator _validator;

    public GeneralCertificateInboundValidatorTests()
    {
        _fixture = new Fixture();
        _validator = new GeneralCertificateInboundValidator();
    }

    [Fact]
    public void Validate_With_ValidRequest_ReturnsTrue()
    {
        var createProductionAppCredentials = _fixture.Build<GeneralCertificateInbound>()

            .Create();
        var result = _validator.Validate(createProductionAppCredentials);

        result.IsValid.ShouldBeTrue();
    }
}
