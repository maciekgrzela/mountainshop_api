using System.Threading.Tasks;
using Application.User;

namespace Application
{
    public interface IFacebookGraphApiAccessor
    {
        public Task<FacebookUserInfo> FacebookLogin(string accessToken);
    }
}