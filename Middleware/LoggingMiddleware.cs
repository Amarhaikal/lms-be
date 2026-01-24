using System.Diagnostics;
using System.Text.Json;
using QUANTM.Models;

namespace QUANTM.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _logFilePath = "logs.json";
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Capture response body
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            string? error = null;
            string? response = null;

            try
            {
                await _next(context);

                // Read response body
                responseBody.Seek(0, SeekOrigin.Begin);
                response = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                // Copy response back to original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw;
            }
            finally
            {
                stopwatch.Stop();

                // Combine error and response into single errorMessage
                var errorMessage = error ?? (context.Response.StatusCode >= 400 ? response : null);

                var logEntry = new LogEntry
                {
                    Timestamp = DateTime.UtcNow,
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    StatusCode = context.Response.StatusCode,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    ErrorMessage = errorMessage
                };

                await WriteLogToFileAsync(logEntry);
            }
        }

        private async Task WriteLogToFileAsync(LogEntry logEntry)
        {
            await _semaphore.WaitAsync();
            try
            {
                List<LogEntry> logs = new List<LogEntry>();

                if (File.Exists(_logFilePath))
                {
                    var existingContent = await File.ReadAllTextAsync(_logFilePath);
                    if (!string.IsNullOrWhiteSpace(existingContent))
                    {
                        logs = JsonSerializer.Deserialize<List<LogEntry>>(existingContent) ?? new List<LogEntry>();
                    }
                }

                logs.Add(logEntry);

                var json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_logFilePath, json);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}