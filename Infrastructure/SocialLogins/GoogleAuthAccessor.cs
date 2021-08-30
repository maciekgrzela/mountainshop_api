using System;
using System.Threading.Tasks;
using Application;
using Application.User;
using Google.Apis.Auth;
using Google.Apis.Util;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.SocialLogins
{
    public class GoogleAuthAccessor : IGoogleAuthAccessor
    {
        private readonly IOptions<GoogleAppSettings> _options;

        public GoogleAuthAccessor(IOptions<GoogleAppSettings> options)
        {
            _options = options;
        }
        
        public async Task<GoogleUserInfo> GoogleLogin(string accessToken)
        {
            var success = true;
            var user = new GoogleJsonWebSignature.Payload();
            try
            {
                user = await GoogleJsonWebSignature.ValidateAsync(accessToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new [] {_options.Value.ClientId}
                    });
            }
            catch (Exception e)
            {
                success = false;
            }


            if (!success)
            {
                return null;
            }

            return new GoogleUserInfo
            {
                Id = user.Subject,
                Email = user.Email,
                FirstName = user.GivenName,
                LastName = user.FamilyName,
                Image = user.Picture
            };
        }
    }
}