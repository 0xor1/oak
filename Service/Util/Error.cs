using Common;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Oak.Service.Util;

public static class Error
{
    public static void If(bool condition, string message, StatusCode code = StatusCode.Internal, bool @public = false, bool log = true)
        => Throw.If(condition, () => new ApiException(code, message, @public, log));
    public static void FromValidationResult(ValidationResult res, StatusCode code = StatusCode.Internal, bool @public = true, bool log = false)
        => If(!res.Valid, $"{res.Message}{(res.SubMessages.Any() ? $":\n{String.Join("\n",res.SubMessages)}": "")}", code, @public, log);
}

public class ApiException : Exception
{
    public StatusCode Code { get; }
    public bool Public { get; }
    public bool Log { get; }

    public ApiException(StatusCode code, string message, bool @public = false, bool log = false): base(message)
    {
        Code = code;
        Public = @public;
        Log = log;
    }
}

public class ErrorInterceptor : Interceptor
{
    private readonly ILogger<ErrorInterceptor> _log;

    public ErrorInterceptor(ILogger<ErrorInterceptor> log)
    {
        _log = log;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await base.UnaryServerHandler(request, context, continuation);
            // return await continuation(request, context);
        }
        catch (Exception ex)
        {
            HandleException(context, ex);
            throw;
        }
    }

    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return base.ClientStreamingServerHandler(requestStream, context, continuation);
        }
        catch (Exception ex)
        {
            HandleException(context, ex);
            throw;
        }
    }

    public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
        }
        catch (Exception ex)
        {
            HandleException(context, ex);
            throw;
        }
    }

    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
        }
        catch (Exception ex)
        {
            HandleException(context, ex);
            throw;
        }
        
    }
    
    private void HandleException(ServerCallContext context, Exception ex)
    {
        var log = true;
        var code = StatusCode.Internal;
        var msg = "An unexpected error occurred";
            
        if (ex.GetType() == typeof(ApiException))
        {
            var apiEx = (ApiException)ex;
            log = apiEx.Log;
            if (apiEx.Public)
            {
                code = apiEx.Code;
                msg = apiEx.Message;
            }
        }

        if (log)
        {
            _log.LogError(ex, $"Error thrown by {context.Method}.");
        }
            
        throw new RpcException(new Status(code, msg));
    }
}