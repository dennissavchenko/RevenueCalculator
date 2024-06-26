using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;

namespace Revenue.Repositories;

public class SoftwareRepository : ISoftwareRepository
{
    private readonly SystemContext _systemContext;

    public SoftwareRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }
    
    public async Task<Software> GetSoftwareAsync(int softwareId)
    {
        return await _systemContext.Softwares.Where(x => x.IdSoftware == softwareId).SingleAsync();
    }

    public async Task<bool> SoftwareExistsAsync(int softwareId)
    {
        return await _systemContext.Softwares.AnyAsync(x => x.IdSoftware == softwareId);
    }
    
}