// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.API.CertificatesStore.V1.ApiClient.Model;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Models;

public class GeneralCertificateEnrichmentPayloadTests
{
    [Fact]
    public void EnrichmentPayload_ShouldBe_AsExpected()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var establishmentId = Guid.NewGuid();

        // Act
        var sut = new GeneralCertificateEnrichmentPayload
        {
            Applicant = new Applicant(new DefraCustomer(orgId, userId)),
            Consignee = new Consignee(new DefraCustomerOrgInfo(orgId)),
            Consignor = new Consignor(new DefraCustomerOrgInfo(orgId)),
            DestinationLocation = new LocationInfo(new Idcoms(establishmentId)),
            DispatchLocation = new LocationInfo(new Idcoms(establishmentId))
        };

        // Assert
        sut.Applicant.DefraCustomer.UserId.ShouldBe(userId);
        sut.Applicant.DefraCustomer.OrgId.ShouldBe(orgId);
        sut.Consignee.DefraCustomer.OrgId.ShouldBe(orgId);
        sut.Consignor.DefraCustomer.OrgId.ShouldBe(orgId);
        sut.DestinationLocation.Idcoms.EstablishmentId.ShouldBe(establishmentId);
        sut.DispatchLocation.Idcoms.EstablishmentId.ShouldBe(establishmentId);
    }
}