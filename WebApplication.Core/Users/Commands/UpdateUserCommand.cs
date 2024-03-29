﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public class Validator : AbstractValidator<UpdateUserCommand>
        {
            public Validator(IUserService userService)
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0);
                
                RuleFor(x => x.GivenNames)
                    .NotEmpty();
                
                RuleFor(x => x.LastName)
                    .NotEmpty();
                
                RuleFor(x => x.EmailAddress)
                    .NotEmpty();
                
                RuleFor(x => x.MobileNumber)
                    .NotEmpty();
                
                RuleFor(x => x.Id)
                    .Custom((id, context) =>
                    {
                        User? user = userService.GetAsync(id).Result;
                        if (user is default(User))
                        {
                            context.AddFailure("Not Found", $"The user '{id}' could not be found.");
                        }
                    }).When(x => x.Id > 0);
            }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }
            
            /// <inheritdoc />
            public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                User user = new User
                {
                    Id = request.Id,
                    GivenNames = request.GivenNames,
                    LastName = request.LastName,
                    ContactDetail = new ContactDetail()
                    {
                        EmailAddress = request.EmailAddress,
                        MobileNumber = request.MobileNumber
                    }
                };
                
                User updatedUser = await _userService.UpdateAsync(user, cancellationToken);
                UserDto result = _mapper.Map<UserDto>(updatedUser);

                return result;
            }
        }
    }
}
