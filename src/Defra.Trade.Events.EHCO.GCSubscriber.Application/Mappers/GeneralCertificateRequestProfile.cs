// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Mappers;

public class GeneralCertificateRequestProfile : Profile
{
    public GeneralCertificateRequestProfile()
    {
        CreateMap<GeneralCertificateInbound, GeneralCertificateRequest>();
    }
}