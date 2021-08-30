using System.Threading.Tasks;
using Application.User;
using MediatR;

namespace Application
{
    public interface IGoogleAuthAccessor
    {
        public Task<GoogleUserInfo> GoogleLogin(string accessToken);
    }
}