using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.GIAS2.Query.Service.Core.Establishments;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.Model;
using DfE.GIAS2.Query.Service.Persistence.Establishments;
using DfE.GIAS2.Query.Service.Presentation.API.Establishments.Mappers;
using Microsoft.AspNetCore.ResponseCompression;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Domain registrations
builder.Services.AddEstablishmentsUseCaseDependencies();
builder.Services.AddEstablishmentsInfrastructureDependencies();
builder.Services.AddSingleton<IMapper<Establishment, object?>, EstablishmentModelToViewModelMapper>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.WriteIndented = false;
    options.SerializerOptions.MaxDepth = 0;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();