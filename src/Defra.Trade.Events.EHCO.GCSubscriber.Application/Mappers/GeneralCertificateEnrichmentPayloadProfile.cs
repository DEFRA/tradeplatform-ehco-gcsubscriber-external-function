// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Mappers;

public class GeneralCertificateEnrichmentPayloadProfile : Profile
{
    public GeneralCertificateEnrichmentPayloadProfile()
    {
        CreateMap<GeneralCertificateRequest, GeneralCertificateEnrichmentPayload>()
            .ForMember(d => d.GcId, opt => opt.MapFrom(s => s.ExchangedDocument.Id))
            .ForMember(d => d.Applicant, opt => opt.MapFrom(s => s.ExchangedDocument.Applicant))
            .ForMember(d => d.Consignor, opt => opt.MapFrom(s => s.SupplyChainConsignment.Consignor))
            .ForMember(d => d.Consignee, opt => opt.MapFrom(s => s.SupplyChainConsignment.Consignee))
            .ForMember(d => d.DispatchLocation, opt => opt.MapFrom(s => s.SupplyChainConsignment.DispatchLocation))
            .ForMember(d => d.DestinationLocation,
                opt => opt.MapFrom(s => s.SupplyChainConsignment.DestinationLocation));
    }
}