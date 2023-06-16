using Common.Client;
using Oak.Client;
using Oak.Client.Lib;
using Oak.I18n;

await Client.Run<App, Oak.Api.IApi>(
    args,
    S.Inst,
    (client) => new Oak.Api.Api(client),
    (sc) =>
    {
        sc.AddSingleton<UICtx>();
        sc.AddSingleton<IUserService, UserService>();
    }
);
