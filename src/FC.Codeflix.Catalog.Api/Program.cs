using FC.Codeflix.Catalog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppConnections(builder.Configuration)
    .AddUseCases()
    .AddRabbitMq(builder.Configuration)
    .AddMessageProducer()
    .AddMessageConsumer(builder.Configuration)
    .AddStorage(builder.Configuration)
    .AddAndConfigureControllers();

var app = builder.Build();

app.UseDocumentation();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

public partial class Program
{
}