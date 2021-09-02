namespace Application.Core
{
    public enum HandlerResponse : int
    {
        ResourceNotFound = 1,
        ClientIsNotAuthorized = 2,
        InvalidRequest = 3,
        ClientHasNoAccess = 4
    }
}