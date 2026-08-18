using DiwyLearn.PublicSample;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<CourseCatalogQuery>();
builder.Services.AddDbContext<LearningDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("LearningDb")
        ?? "Data Source=diwylearn-public.db"));

var app = builder.Build();

app.MapOpenApi();
if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await SampleData.InitializeAsync(app.Services);
await app.RunAsync();

public partial class Program;
