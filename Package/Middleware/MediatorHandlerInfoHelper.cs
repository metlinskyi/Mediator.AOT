namespace Mediator.Middleware;

internal static class MediatorHandlerInfoHelper
{
    public static (HttpMethod, string) CreateKey(this IMediatorHandlerInfo info)
    {
        return (info.Method, info.RequestType.Name.ToUpper());
    }
}