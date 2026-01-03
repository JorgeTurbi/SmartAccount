using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
[Route("api/v{version:apiVersion}/[controller]")]
    public class TestController : ControllerBase
    {
        

        [HttpGet]
         [MapToApiVersion("1.0")] 
        public IActionResult GetPrueba()
        {
            return Ok("Test endpoint working.");
        }
    }
}