using Common.Server;
using Common.Shared;
using Oak.I18n;
using Grpc.Core;

namespace Oak.Service.Util;

public static class ServerCallContextExts
{
    public static Session GetSession(this ServerCallContext stx) => stx.GetSession(S.Inst);

    public static Session ClearSession(this ServerCallContext stx) => stx.ClearSession(S.Inst);

    public static void ErrorIf(
        this ServerCallContext stx,
        bool condition,
        string key,
        object? model = null,
        StatusCode code = StatusCode.Internal
    ) => stx.ErrorIf(condition, S.Inst, key, model, code);

    public static void ErrorFromValidationResult(
        this ServerCallContext stx,
        ValidationResult res,
        StatusCode code = StatusCode.Internal
    ) => stx.ErrorFromValidationResult(S.Inst, res, code);

    // i18n string handling
    public static string String(this ServerCallContext stx, string key, object? model = null) =>
        S.Get(stx.GetSession().Lang, key, model);
}
