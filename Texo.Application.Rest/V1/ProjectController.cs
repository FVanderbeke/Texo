using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Texo.Application.Api.Model.V1.Dto;

namespace Texo.Application.Rest.Admin.V1
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("admin/v1/[controller]/[action]")]
    public class ProjectController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> FindAll()
        {
            return BadRequest();
        }
    }
}