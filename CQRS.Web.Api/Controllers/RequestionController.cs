using CQRS.Web.Api.Application.Features.Product.Query;
using CQRS.Web.Api.Application.Features.Requestion.Command;
using CQRS.Web.Api.Application.Features.Requestion.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.Web.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RequestionController : Controller
    {
        private readonly IMediator _mediator;

        public RequestionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create/header")]
        public async Task<IActionResult> Create([FromBody] CreateRequestion.Command command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("create/detail")]
        public async Task<IActionResult> CreateDetail([FromBody] CreateRequestionDetail.Command command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updatestatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateRequestion.Command command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("list")]
        public async Task<IActionResult> GetLists([FromQuery] GetListRequestion.Query request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
