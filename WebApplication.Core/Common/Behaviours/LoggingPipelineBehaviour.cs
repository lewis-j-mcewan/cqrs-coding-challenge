using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace WebApplication.Core.Common.Behaviours
{
    public class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger;

        public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation("Starting request {@RequestName}, {@DateTimeUtc}", 
                typeof(TRequest).Name,
                DateTime.UtcNow);
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var result = await next();
            
            /*
             * Note: this code would not be committed irl
             * Tried to implement the Exception logging. This code fails a unit test but does log
             * the thrown exception along with time to execute.
             * Hoping InfoTrack can let me know what I could've changed to get this working
             * (thinking some sort of Result class which holds exceptions and a success/failure bool,
             * but not sure how to implement)
             */
            // TResponse result = default;
            // try
            // { 
            //     result = await next();
            // }
            // catch(Exception ex)
            // {
            //     _logger.LogError("Request failure {@RequestName}, {@DateTimeUtc}, {@Error}",
            //         typeof(TRequest).Name,
            //         DateTime.UtcNow,
            //         ex.Message);
            // }
            
            stopwatch.Stop();
            
            _logger.LogInformation("Completed request {@RequestName}, {@DateTimeUtc}, Time to complete: {@ExecutionTime}ms", 
                typeof(TRequest).Name,
                DateTime.UtcNow,
                stopwatch.ElapsedMilliseconds);
            
            return result;
        }
    }
}