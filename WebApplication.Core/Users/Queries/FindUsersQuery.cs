using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
    public class FindUsersQuery : IRequest<IEnumerable<UserDto>>
    {
        public string? GivenNames { get; set; }
        public string? LastName { get; set; }

        public class Validator : AbstractValidator<FindUsersQuery>
        {
            public Validator()
            {
                RuleFor(x => x.GivenNames)
                    .NotEmpty()
                    .When(x => string.IsNullOrWhiteSpace(x.LastName));

                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .When(x => string.IsNullOrWhiteSpace(x.GivenNames));
            }
        }

        public class Handler : IRequestHandler<FindUsersQuery, IEnumerable<UserDto>>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<IEnumerable<UserDto>> Handle(FindUsersQuery request, CancellationToken cancellationToken)
            {
                IEnumerable<User> users = await _userService.FindAsync(request.GivenNames, request.LastName, cancellationToken);

                List<User> usersList = users.ToList();

                if (usersList.Any()) 
                    return usersList.Select(user => _mapper.Map<UserDto>(user));
                
                string missingUsers = (request.GivenNames != null && request.LastName == null)
                                            ? request.GivenNames
                                            : (request.GivenNames == null && request.LastName != null) 
                                            ? request.LastName 
                                            : $"'{request.GivenNames} {request.LastName}'";
                
                throw new NotFoundException($"The user '{missingUsers}' could not be found");

            }
        }
    }
}
