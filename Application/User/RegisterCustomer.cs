using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Validators;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
    public class RegisterCustomer
    {
        public class Query : IRequest<LoggedUserResource>
        {
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.UserName).NotEmpty();
                RuleFor(p => p.FirstName).NotEmpty().MaximumLength(150);
                RuleFor(p => p.LastName).NotEmpty().MaximumLength(200);
                RuleFor(p => p.PhoneNumber).NotEmpty().Length(11);
                RuleFor(p => p.Email).NotEmpty().EmailAddress();
                RuleFor(p => p.Password).Password();
            }
        }

        
    }
}