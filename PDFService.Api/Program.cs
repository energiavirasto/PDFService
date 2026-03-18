using PDFService.Api.Services;
using PDFService.Services.Models;
using PDFService.Services.Services;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;

namespace PDFService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // The following line enables Application Insights telemetry collection.
            builder.Services.AddApplicationInsightsTelemetry();

            // Add the PdfOptions configuration, expose them in the PdfOptions class
            builder.Services.Configure<PdfOptions>(builder.Configuration.GetSection("PdfOptions"));

            // Add services to the container.
            builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<PdfOptions>>().Value);
            builder.Services.AddTransient<IPdfService, PdfService>();
            builder.Services.AddTransient<IApiKeyValidation, ApiKeyValidation>();
            builder.Services.AddTransient<HtmlService>();

            // Http client
            builder.Services.AddHttpClient();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void GetRetryPolicy(ResiliencePipelineBuilder<HttpResponseMessage> builder, ResilienceHandlerContext context)
        {
            //// Enable reloads whenever the named options change
            //context.EnableReloads<HttpRetryStrategyOptions>("RetryOptions");

            //// Retrieve the named options
            //var retryOptions = context.GetOptions<HttpRetryStrategyOptions>("RetryOptions");

            builder.AddRetry(new HttpRetryStrategyOptions()
            {
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(3),
                BackoffType = DelayBackoffType.Constant
            });
        }
    }
}
