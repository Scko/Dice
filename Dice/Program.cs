using Dice.BusinessLogic;
using Dice.BusinessLogic.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProbabilityCalculator, DiceProbabilityCalculator>();
builder.Services.AddSingleton<IMathHelper, MathHelper>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Dice Probability API", Version = "v1" });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dice Probability API V1");
    });
}

app.Run();
