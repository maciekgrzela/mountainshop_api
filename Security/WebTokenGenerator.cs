﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Security
{
    public class WebTokenGenerator : IWebTokenGenerator
    {
        private readonly SymmetricSecurityKey _key;

        public WebTokenGenerator()
        {
            _key = SecurityKeyGenerator.Instance.GetKey();
        }
        
        
        public string CreateToken(User user, string role)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, user.UserName),
                new(ClaimTypes.Role, role)
            };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}