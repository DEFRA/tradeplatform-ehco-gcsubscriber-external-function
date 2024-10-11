// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Common.Functions.Models.Enum;
using Defra.Trade.Common.Functions.Validation;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using FluentValidation;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Validators;

public class GCMessageHeaderValidator : AbstractValidator<TradeEventMessageHeader>
{
    public GCMessageHeaderValidator() : base()
    {
        RuleFor(x => x.MessageId).Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .NotEmpty().WithMessage(ValidationMessages.EmptyField);

        RuleFor(x => x.CorrelationId).Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .NotEmpty().WithMessage(ValidationMessages.EmptyField);

        RuleFor(x => x.ContentType)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .NotEmpty().WithMessage(ValidationMessages.EmptyField)
            .Must(c => c.Equals(GcMessageConstants.MessageContentType, StringComparison.OrdinalIgnoreCase)).WithMessage(GcMessageConstants.ContentErrorMessage);

        RuleFor(x => x.EntityKey)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .NotEmpty().WithMessage(ValidationMessages.EmptyField);

        RuleFor(x => x.Label)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .NotEmpty().WithMessage(ValidationMessages.EmptyField)
            .Must(lbl => lbl.Equals(GcMessageConstants.BrokerLabel, StringComparison.OrdinalIgnoreCase)).WithMessage(GcMessageConstants.LabelErrorMessage);

        RuleFor(x => x.PublisherId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .NotEmpty().WithMessage(ValidationMessages.EmptyField)
            .Must(type => type.Equals(GcSubscriberSettings.PublisherId, StringComparison.OrdinalIgnoreCase)).WithMessage(GcMessageConstants.PublisherIdErrorMessage);

        RuleFor(x => x.Status).Cascade(CascadeMode.Stop).NotNull()
            .WithMessage(ValidationMessages.NullField).NotEmpty()
            .WithMessage(ValidationMessages.EmptyField)
            .Must(type => type.Equals(GcMessageConstants.Status, StringComparison.OrdinalIgnoreCase)).WithMessage(GcMessageConstants.StatusErrorMessage);

        RuleFor(x => x.Type)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .IsInEnum().WithMessage(GcMessageConstants.TypeErrorMessage)
            .Must(type => type.Equals(EventType.Internal)).WithMessage(GcMessageConstants.TypeErrorMessage);

        RuleFor(x => x.SchemaVersion).Cascade(CascadeMode.Stop).NotNull().WithMessage(ValidationMessages.NullField)
            .NotEmpty().WithMessage(ValidationMessages.EmptyField)
            .Must(sv => sv.Equals(GcMessageConstants.SchemaVersionV2))
            .WithMessage(GcMessageConstants.SchemaVersionMessage);

        RuleFor(x => x.TimestampUtc)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .NotEmpty().WithMessage(ValidationMessages.EmptyField);
    }
}