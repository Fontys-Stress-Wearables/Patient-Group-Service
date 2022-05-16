using Patient_Group_Service.Models;
using Patient_Group_Service.Services;

namespace Patient_Group_Service.Interfaces;

public interface IOrganizationService
{
    void Create(Organization organization);
    void Remove(Organization organization);
    bool Exists(string id);
    IEnumerable<Organization> GetAll();
}