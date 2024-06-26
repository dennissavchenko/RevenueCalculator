using Revenue.Entities;

namespace Revenue.Repositories;

public interface ISoftwareRepository
{
    public Task<Software> GetSoftwareAsync(int softwareId);
    public Task<bool> SoftwareExistsAsync(int softwareId);
}