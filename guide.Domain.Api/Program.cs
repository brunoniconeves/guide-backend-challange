using guide.Domain.Infra.Repositories;
using guide.Domain.Repositories;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("config.json");

// Add services to the container.
builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigin",
  abuilder => abuilder
  .AllowAnyOrigin()
  .AllowAnyMethod()
  .AllowAnyHeader()
  ));

builder.Services.AddControllers();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

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
