using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Commands;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Core.Users.Queries;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        // GET /Users - Return a single user
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(
            [FromQuery] GetUserQuery query,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            
            return Ok(result);
        }
        
        // GET /Find - Return a list of matching users
        [HttpGet("Find")] 
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindUsersAsync(
            [FromQuery] FindUsersQuery query,
            CancellationToken cancellationToken)
        {
            IEnumerable<UserDto> result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        
        // GET /List - Return a paginated list of users
        [HttpGet("List")]
        [ProducesResponseType(typeof(PaginatedDto<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsersAsync(
            [FromQuery] ListUsersQuery query,
            CancellationToken cancellationToken)
        {
            PaginatedDto<IEnumerable<UserDto>> result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        
        // POST /Users - create a user
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUserAsync(
            [FromBody] CreateUserCommand body,
            CancellationToken cancellationToken
            )
        {
            UserDto result  = await _mediator.Send(body, cancellationToken);
            string location =  "/" + ControllerContext.ActionDescriptor.ControllerName + "?id=" + result.UserId;
            
            return Created(location, result);
        }
        
        // PUT /Users - update an existing user
        [HttpPut]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync(
            [FromBody] UpdateUserCommand body,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(body, cancellationToken);
            return Ok(result);
        }
        
        // DELETE /Users - delete an existing user
        [HttpDelete]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync(
            [FromQuery] DeleteUserCommand query,
            CancellationToken cancellationToken
            )
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
