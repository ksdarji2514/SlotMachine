using FluentValidation;
using MongoDB.Driver;
using SlotMachine.API.Middleware;
using SlotMachine.API.Validators;
using SlotMachine.Common.Constants;
using SlotMachine.Database.Entities;
using SlotMachine.Database.Mongo;
using SlotMachine.Repository.Interfaces;
using SlotMachine.Repository.Repositories;
using SlotMachine.Service.Interfaces;
using SlotMachine.Service.Services;

var builder = WebApplication.CreateBuilder(args);
// ---- Configuration + Mongo context ----
builder.Services.AddOptions<MongoSettings>()
    .Bind(builder.Configuration.GetSection("MongoDb"))
    .Validate(s => !string.IsNullOrWhiteSpace(s.ConnectionString),
              ErrorMessages.MissingConnectionString)
    .Validate(s => !string.IsNullOrWhiteSpace(s.Database),
              ErrorMessages.MissingDatabase)
    .ValidateOnStart();

builder.Services.AddSingleton<IMongoContext, MongoContext>();

// ---- Repositories ----
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

// ---- Services / utilities ----
builder.Services.AddScoped<IWinCalculatorService, WinCalculatorService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

builder.Services.AddValidatorsFromAssemblies(new[] { typeof(SpinRequestValidator).Assembly }, ServiceLifetime.Singleton);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSwaggerGen();            

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IMongoContext>();
    var configs = context.GetCollection<GameConfiguration>(MongoConstants.ConfigCollection);

    var exists = await configs
        .Find(Builders<GameConfiguration>.Filter.Eq(c => c.Id, MongoConstants.DefaultConfigId))
        .AnyAsync();

    if (!exists)
    {
        await configs.InsertOneAsync(new GameConfiguration
        {
            Id = MongoConstants.DefaultConfigId,
            Width = 5,
            Height = 3
        });
    }
}
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // serves /swagger/v1/swagger.json
    app.UseSwaggerUI();    // serves the Swagger UI page at /swagger
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
