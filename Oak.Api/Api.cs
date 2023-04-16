using Common.Shared.Auth;
using Oak.Api.Org;

namespace Oak.Api;

public interface IApi : Common.Shared.Auth.IApi
{
    private static IApi? _inst;
    public static IApi Init() => _inst ??= new Api();
    
    public IOrgApi Org { get; }
}

internal class Api: IApi
{
    public IAuthApi Auth { get; } = IAuthApi.Init();
    public IOrgApi Org { get; } = IOrgApi.Init();
}