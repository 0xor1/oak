using Common.Client;
using Common.Shared;
using Oak.I18n;
using Oak.Proto;

namespace Oak.Client.Lib;

public record Session(string Id, bool IsAuthed, string Lang, string DateFmt, string TimeFmt)
    : ISession
{
    public Session()
        : this(string.Empty, false, S.DefaultLang, S.DefaultDateFmt, S.DefaultTimeFmt) { }

    public Session(Auth_Session ses)
        : this(ses.Id, ses.IsAuthed, ses.Lang, ses.DateFmt, ses.TimeFmt) { }
}

public class AuthService : IAuthService
{
    private L L;
    private Session? _s;
    private Session? Session
    {
        get => _s;
        set
        {
            _s = value;
            L.Config(_s.Lang, _s.DateFmt, _s.TimeFmt);
            _refreshUI?.Invoke(_s);
        }
    }
    private readonly Api.ApiClient _api;
    private Action<ISession>? _refreshUI;

    public AuthService(Api.ApiClient api, L l)
    {
        _api = api;
        L = l;
    }

    public void RegisterRefreshUi(Action<ISession> a)
    {
        _refreshUI = a;
    }

    public async Task<ISession> GetSession()
    {
        if (Session == null)
        {
            var ses = await _api.Auth_GetSessionAsync(new Nothing());
            Session = new Session(ses);
        }
        return Session;
    }

    public async Task Register(string email, string pwd)
    {
        var ses = await GetSession();
        Throw.OpIf(ses.IsAuthed, "already in authenticated session");
        await _api.Auth_RegisterAsync(new Auth_RegisterReq() { Email = email, Pwd = pwd });
    }

    public async Task<ISession> SignIn(string email, string pwd, bool rememberMe)
    {
        var ses = await GetSession();
        Throw.OpIf(ses.IsAuthed, "already in authenticated session");
        var newSes = await _api.Auth_SignInAsync(
            new Auth_SignInReq()
            {
                Email = email,
                Pwd = pwd,
                RememberMe = rememberMe
            }
        );
        Session = new Session(newSes);
        return Session;
    }

    public async Task<ISession> SignOut()
    {
        var ses = await GetSession();
        if (!ses.IsAuthed)
        {
            return ses;
        }
        var newSes = await _api.Auth_SignOutAsync(new Nothing());
        Session = new Session(newSes);
        return Session;
    }

    public async Task<ISession> SetL10n(string lang, string dateFmt, string timeFmt)
    {
        var newSes = await _api.Auth_SetL10nAsync(
            new Auth_SetL10nReq()
            {
                Lang = lang,
                DateFmt = dateFmt,
                TimeFmt = timeFmt
            }
        );
        Session = new Session(newSes);
        return Session;
    }
}
