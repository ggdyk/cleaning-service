using Api.Controllers.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/test")]
public class TestController : BaseController
{
    [HttpGet("run")]
    public IActionResult Run()
        => Ok();
}