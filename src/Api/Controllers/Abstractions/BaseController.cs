using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Abstractions;

[ApiController]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    
}