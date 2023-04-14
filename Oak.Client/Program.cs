using Common.Client;
using Common.Shared.Auth;
using Oak.Api;
using Oak.Client;
using Oak.I18n;

await Client.Run<App, Oak.Api.IApi>(args, S.Inst, Oak.Api.IApi.Init());
