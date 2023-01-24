﻿using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Oak.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Oak.Service.Util;

public static class ServiceExts
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddGrpc(opts =>
        {
            opts.Interceptors.Add<ErrorInterceptor>();
        });
        services.AddScoped<ISessionManager, SessionManager>();
        if (Config.Env == Env.LCL)
        {
            services.AddScoped<IEmailClient, LogEmailClient>();
        }
        else
        {
            services.AddScoped<AmazonSimpleEmailServiceClient>(sp =>
                new AmazonSimpleEmailServiceClient(new BasicAWSCredentials(Config.Email.Key, Config.Email.Secret), Config.Email.GetRegionEndpoint()));
            services.AddScoped<IEmailClient, SesEmailClient>();
        }
        services.AddDbContext<OakDb>(
            dbContextOptions =>
            {
                var cnnStrBldr = new MySqlConnectionStringBuilder(Config.Db.Connection);
                cnnStrBldr.Pooling = true;
                cnnStrBldr.MaximumPoolSize = 100;
                cnnStrBldr.MinimumPoolSize = 1;
                var version = new MariaDbServerVersion(new Version(10, 8));
                dbContextOptions
                    .UseMySql(cnnStrBldr.ToString(), version, opts => { opts.CommandTimeout(1); });
            });
    }
}