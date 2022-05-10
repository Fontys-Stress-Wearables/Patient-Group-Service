using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces;

public interface IOrganizationService
{
    bool Exists(string id);
}