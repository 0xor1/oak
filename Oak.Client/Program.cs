using Common.Client;
using Oak.Client;
using Oak.Client.Lib;
using Oak.I18n;
using Oak.Proto;

await Client.Run<App, Api.ApiClient, AuthService>(args, S.Inst, ci => new Api.ApiClient(ci));
