using Library.API.Domain.Repositories;
using Library.API.Domain.Services;
using Library.API.Infrastructure.Contexts;
using Library.API.Infrastructure.Repositories;
using Library.API.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddDbContext<ApiDbContext>(options => options.UseInMemoryDatabase("library-api-in-memory-database"));

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

WebApplication app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
