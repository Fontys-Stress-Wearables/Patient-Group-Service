using Microsoft.AspNetCore.Mvc;

namespace Patient_Group_Service.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientGroupController : ControllerBase
{
    private readonly ILogger<PatientGroupController> _logger;

    public PatientGroupController(ILogger<PatientGroupController> logger)
    {
        _logger = logger;
    }

}
