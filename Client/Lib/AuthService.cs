using Common;
using Oak.Proto;

namespace Oak.Client.Lib;

public record Session(string Id, bool IsAuthed)
{
    public bool IsAnon => !IsAuthed;
}

public interface IAuthService
{
    Task<Session> GetSession();
    Task Register(string email, string pwd);
    Task<Session> SignIn(string email, string pwd, bool rememberMe);
    Task<Session> SignOut();
}

public class AuthService: IAuthService
{
    private Session? _session;
    private readonly Api.ApiClient _api;
    
    public AuthService(Api.ApiClient api)
    {
        _api = api;
    }

    public async Task<Session> GetSession()
    {
        if (_session == null)
        {
            var ses = await _api.Auth_GetSessionAsync(new Nothing());
            _session = new Session(ses.Id, ses.IsAuthed);
        }
        return _session;
    }

    public async Task Register(string email, string pwd)
    {
        var ses = await GetSession();
        Throw.OpIf(ses.IsAuthed, "already in authenticated session");
        await _api.Auth_RegisterAsync(new Auth_RegisterReq()
        {
            Email = email,
            Pwd = pwd
        });
    }

    public async Task<Session> SignIn(string email, string pwd, bool rememberMe)
    {
        var ses = await GetSession();
        Throw.OpIf(ses.IsAuthed, "already in authenticated session");
        var newSes = await _api.Auth_SignInAsync(new Auth_SignInReq()
        {
            Email = email,
            Pwd = pwd,
            RememberMe = rememberMe
        });
        _session = new Session(newSes.Id, newSes.IsAuthed);
        return _session;
    }

    public async Task<Session> SignOut()
    {
        var ses = await GetSession();
        if (!ses.IsAuthed)
        {
            return ses;
        }
        var newSes = await _api.Auth_SignOutAsync(new Nothing());
        _session = new Session(newSes.Id, newSes.IsAuthed);
        return _session;
    }
}