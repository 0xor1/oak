using Common.Server;
using Oak.Service.Services;
using Oak.Db;
using Oak.I18n;

Server.Run<OakDb, ApiService>(args, S.UnexpectedError);
