using GrpcNotebookService.Services;
using GrpcNotebookService.XML;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<XMLService>(); // Register the XMLService as a singleton service

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<NoteOperations>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
