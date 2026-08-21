using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Client;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;

public class GetBeneficiariesQueryHandler : IRequestHandler<GetBeneficiariesQuery, List<BeneficiaryDto>>
{
    private readonly IGenericRepository<Beneficiary> _beneficiaryRepository;

    public GetBeneficiariesQueryHandler(IGenericRepository<Beneficiary> beneficiaryRepository)
    {
        _beneficiaryRepository = beneficiaryRepository;
    }

    public async Task<List<BeneficiaryDto>> Handle(GetBeneficiariesQuery request, CancellationToken cancellationToken)
    {
        var allBeneficiaries = await _beneficiaryRepository.GetAllAsync();

        return allBeneficiaries
            .Where(b => b.OwnerUserId == request.OwnerUserId)
            .Select(b => new BeneficiaryDto
            {
                Id = b.Id,
                Alias = b.Alias,
                BeneficiaryAccountNumber = b.BeneficiaryAccountNumber
            })
            .ToList();
    }
}