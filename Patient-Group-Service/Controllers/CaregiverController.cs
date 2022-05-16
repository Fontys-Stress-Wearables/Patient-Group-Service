using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Patient_Group_Service.Dtos;
using Patient_Group_Service.Interfaces;

namespace Patient_Group_Service.Controllers;

[ApiController]
[Authorize]
[Route("caregiver")]
public class CaregiverController : ControllerBase
{
    private readonly IPatientGroupService _patientGroupService;
    private readonly IMapper _mapper;
    
    public CaregiverController(IPatientGroupService patientGroupService, IMapper mapper)
    {
        _patientGroupService = patientGroupService;
        _mapper = mapper;
    }
    
    [HttpGet("{id}/patient-groups")]
    public async Task<IEnumerable<PatientGroupDTO>> GetCaregiversGroups(string id)
    {
        var groups = await _patientGroupService.GetForCaregiver(id,HttpContext.User.GetTenantId()!);
        return _mapper.Map<IEnumerable<PatientGroupDTO>>(groups);
    }
}