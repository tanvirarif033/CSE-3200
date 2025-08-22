using MediatR;
using System;

namespace CSE3200.Application.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}