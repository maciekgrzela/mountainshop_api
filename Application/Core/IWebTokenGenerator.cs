namespace Application
{
    public interface IWebTokenGenerator
    {
        string CreateToken(Domain.Models.User user, string role);
    }
}