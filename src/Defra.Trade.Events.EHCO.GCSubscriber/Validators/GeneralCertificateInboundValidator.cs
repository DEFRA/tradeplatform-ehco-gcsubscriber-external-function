// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using FluentValidation;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Validators;

public class GeneralCertificateInboundValidator : AbstractValidator<GeneralCertificateInbound>
{
    public GeneralCertificateInboundValidator()
    {
        RuleFor(x => x.ExchangedDocument)
            .NotEmpty().WithMessage("Should not be null.");
    }
}