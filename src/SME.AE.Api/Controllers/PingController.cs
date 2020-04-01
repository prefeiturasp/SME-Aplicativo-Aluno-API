using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SME.AE.Api.Controllers
{
    public class PingController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<string>> Ping()
        {
            return Ok("pong!");
        }
    }
}