using Serilog;
using System.Diagnostics;
using System.Text;
using ILogger = Serilog.ILogger;

namespace PackageSyncWebAPI.Middleware
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Log.ForContext<LoggerMiddleware>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var uri = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            if(request.Method == "POST" || request.Method == "PUT")
            {
                var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                _logger.Information("Request started: {Protocol} {Method} {URI} with request body {RequestBody}.", request.Protocol, request.Method, uri, requestBody);
                request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            }
            else
            {
                _logger.Information("Request started: {Protocol} {Method} {URI}.", request.Protocol, request.Method, uri);
            }



            var response = httpContext.Response;
            Stream originalBody = response.Body;
            try
            {
                using var memStream = new MemoryStream();
                response.Body = memStream;

                var stopwatch = Stopwatch.StartNew();
                await _next(httpContext);
                stopwatch.Stop();

                memStream.Position = 0;
                string responseBody = new StreamReader(memStream).ReadToEnd();

                if (response.StatusCode == 200 || response.StatusCode == 201)
                {
                    _logger.Information("Request finished successfully: {Protocol} {Method} {URI} with status {StatusCode} and the following response {ResponseBody} in {EclipsedMiliseconds}ms.",
                        request.Protocol,
                        request.Method,
                        uri,
                        response.StatusCode,
                        responseBody,
                        stopwatch.ElapsedMilliseconds);
                }
                else if (response.StatusCode == 400 || response.StatusCode == 401 || response.StatusCode == 404)
                {
                    _logger.Warning("Client-side error occured: {Protocol} {Method} {URI} with status {StatusCode} and the following response {ResponseBody} in {EclipsedMiliseconds}ms.",
                        request.Protocol,
                        request.Method,
                        uri,
                        response.StatusCode,
                        responseBody,
                        stopwatch.ElapsedMilliseconds);
                }
                else if (response.StatusCode == 500)
                {
                    _logger.Error("Server-side error occurred: {Protocol} {Method} {URI} with status {StatusCode} and the following response {ResponseBody} in {ElapsedMilliseconds} ms.",
                        request.Protocol,
                        request.Method,
                        uri,
                        response.StatusCode,
                        responseBody,
                        stopwatch.ElapsedMilliseconds);
                }

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);
            }
            catch (Exception)
            {
                throw new Exception("Something went wrong!");
            }
            finally
            {
                response.Body = originalBody;
            }
        }
    }
}
