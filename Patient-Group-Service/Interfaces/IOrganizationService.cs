using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces;

public interface IOrganizationService
{
    void Create(Organization organization);
    void Remove(Organization organization);
    bool Exists(string id);
    IEnumerable<Organization> GetAll();
}