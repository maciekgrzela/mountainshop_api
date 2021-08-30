using System.Threading.Tasks;
using Application.User;

namespace Application
{
    public interface IFacebookGraphAPIAccessor
    {
        public Task<FacebookUserInfo> FacebookLogin(string accessToken);
    }
}