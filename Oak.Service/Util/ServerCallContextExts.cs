using System.Security;
using System.Security.Cryptography;
using Common;
using Oak.I18n;
using Oak.Proto;
using Google.Protobuf.Collections;
using Grpc.Core;
using MessagePack;
using Microsoft.AspNetCore.Http;

namespace Oak.Service.Util;

[MessagePackObject]
public record Session
{
    [Key(0)]
    public string Id { get; init; }

    [Key(1)]
    public DateTime? StartedOn { get; init; }

    [Key(2)]
    public bool IsAuthed { get; init; }

    [Key(3)]
    public bool RememberMe { get; init; }

    [IgnoreMember]
    public bool IsAnon => !IsAuthed;

    [Key(4)]
    public string Lang { get; init; } = S.DefaultLang;

    [Key(5)]
    public string DateFmt { get; init; } = S.DefaultDateFmt;

    [Key(6)]
    public string TimeFmt { get; init; } = S.DefaultTimeFmt;

    public Auth_Session ToAuth() => new() { Id = Id, IsAuthed = IsAuthed, };
}

public static class ServerCallContextExts
{
    private const string SessionName = "oak";
    private static readonly byte[][] SignatureKeys;

    static ServerCallContextExts()
    {
        SignatureKeys = Config.Session.SignatureKeys.Select(x => Base64.UrlDecode(x)).ToArray();
        if (SignatureKeys.Count(x => x.Length != 64) > 0)
        {
            throw new InvalidDataException(
                "config: all session signature keys must be 64 bytes long"
            );
        }

        if (SignatureKeys.Length == 0)
        {
            throw new InvalidDataException(
                "config: there must be at least 1 session signature key"
            );
        }
    }

    public static Session GetSession(this ServerCallContext stx)
    {
        Session ses;
        if (!stx.UserState.ContainsKey(SessionName))
        {
            ses = GetCookie(stx);
            stx.UserState[SessionName] = ses;
        }
        else
        {
            ses = (Session)stx.UserState[SessionName];
        }
        return ses;
    }

    public static Session CreateSession(
        this ServerCallContext stx,
        string userId,
        bool isAuthed,
        bool rememberMe,
        string lang = S.DefaultLang,
        string dateFmt = S.DefaultDateFmt,
        string timeFmt = S.DefaultTimeFmt
    )
    {
        var ses = new Session()
        {
            Id = userId,
            StartedOn = DateTime.UtcNow,
            IsAuthed = isAuthed,
            RememberMe = rememberMe,
            Lang = lang,
            DateFmt = dateFmt,
            TimeFmt = timeFmt
        };
        stx.UserState[SessionName] = ses;
        SetCookie(stx, ses);
        return ses;
    }

    public static Session ClearSession(this ServerCallContext stx)
    {
        var ses = _ClearSession(stx);
        stx.UserState[SessionName] = ses;
        return ses;
    }

    private static Session _ClearSession(ServerCallContext stx)
    {
        return stx.CreateSession(
            Id.New(),
            false,
            false,
            S.BestLang(
                stx.GetHttpContext().Request.Headers.AcceptLanguage.ToArray().FirstOrDefault() ?? ""
            )
        );
    }

    private static Session GetCookie(ServerCallContext stx)
    {
        var htx = stx.GetHttpContext();
        var c = htx.Request.Cookies[SessionName];
        if (c.IsNull())
        {
            // there is no session set so use sign out to create a
            // new anon session
            return _ClearSession(stx);
        }

        // there is a session so lets get it from the cookie
        var signedSessionBytes = Base64.UrlDecode(c);
        var signedSes = MessagePackSerializer.Deserialize<SignedSession>(signedSessionBytes);
        var i = 0;
        foreach (var signatureKey in SignatureKeys)
        {
            using (var hmac = new HMACSHA256(signatureKey))
            {
                var sesSig = hmac.ComputeHash(signedSes.Session);
                if (sesSig.SequenceEqual(signedSes.Signature))
                {
                    var ses = MessagePackSerializer.Deserialize<Session>(signedSes.Session);
                    if (i > 0)
                    {
                        // if it wasnt signed using the latest key, resign the cookie using the latest key
                        SetCookie(stx, ses);
                    }
                    return ses;
                }
                i++;
            }
        }
        throw new SecurityException("Session signature verification failed");
    }

    private static void SetCookie(ServerCallContext stx, Session ses)
    {
        // turn session into bytes
        var sesBytes = MessagePackSerializer.Serialize(ses);
        // sign the session
        byte[] sesSig;
        using (var hmac = new HMACSHA256(SignatureKeys.First()))
        {
            sesSig = hmac.ComputeHash(sesBytes);
        }
        // create the cookie value with the session and signature
        var signedSes = new SignedSession() { Session = sesBytes, Signature = sesSig };
        // get final cookie bytes
        var cookieBytes = MessagePackSerializer.Serialize(signedSes);
        // create cookie
        stx.GetHttpContext()
            .Response.Cookies.Append(
                SessionName,
                Base64.UrlEncode(cookieBytes),
                new CookieOptions()
                {
                    Secure = true,
                    HttpOnly = true,
                    IsEssential = true,
                    Expires = ses.RememberMe ? DateTime.UtcNow.AddDays(7) : null,
                    SameSite = SameSiteMode.Strict
                }
            );
    }

    [MessagePackObject]
    public record SignedSession
    {
        [Key(0)]
        public byte[] Session { get; init; }

        [Key(1)]
        public byte[] Signature { get; init; }
    }

    // error throwing
    public static void ErrorIf(
        this ServerCallContext stx,
        bool condition,
        string key,
        object? model = null,
        StatusCode code = StatusCode.Internal
    ) => Throw.If(condition, () => new ApiException(stx.String(key, model), code));

    public static void ErrorFromValidationResult(
        this ServerCallContext stx,
        ValidationResult res,
        StatusCode code = StatusCode.Internal
    ) =>
        Throw.If(
            !res.Valid,
            () =>
                new ApiException(
                    $"{stx.String(res.Message.Key, res.Message.Model)}{(res.SubMessages.Any() ? $":\n{string.Join("\n", res.SubMessages.Select(x => stx.String(x.Key, x.Model)))}" : "")}",
                    code
                )
        );

    // i18n string handling

    public static string String(this ServerCallContext stx, string key, object? model = null) =>
        S.Get(stx.GetSession().Lang, key, model);
}
