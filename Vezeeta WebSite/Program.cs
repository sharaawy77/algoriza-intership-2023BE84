using CoreLayer.Interface;
using CoreLayer.Models;
using EFLayer.Data;
using EFLayer.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServicesLayer.Services;
using System.Text;
using Vezeeta_WebSite.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))


    };
}).AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleAuth:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
});



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<JWT, JWT>();
builder.Services.AddTransient(typeof(IBaseRepo<>), typeof(BaseRepo<>));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRankingService, RankingService>();

//Cors 

builder.Services.AddCors(options => options.AddPolicy("MyPolicy",
    corsPolicybuilder=>corsPolicybuilder.AllowAnyMethod().AllowAnyHeader()
    .AllowAnyOrigin()
    ));



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
app.UseCors("MyPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
