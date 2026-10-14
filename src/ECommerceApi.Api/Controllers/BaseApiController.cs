using ECommerceApi.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value is null)
                return NotFound();

            return Ok(result.Value);
        }

        if (result.Error.Code.EndsWith(".NotFound"))
        {
            return NotFound(new { code = result.Error.Code, description = result.Error.Description });
        }

        return BadRequest(new { code = result.Error.Code, description = result.Error.Description });
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        if (result.Error.Code.EndsWith(".NotFound"))
        {
            return NotFound(new { code = result.Error.Code, description = result.Error.Description });
        }

        return BadRequest(new { code = result.Error.Code, description = result.Error.Description });
    }
}
