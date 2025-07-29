using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using StudentInfoSys.Business.DataProtection;
using StudentInfoSys.Business.Operations.Role;
using StudentInfoSys.Business.Operations.User;
using StudentInfoSys.Data.Context;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudentInfoSysDbContext>(options => options.UseSqlServer(connectionString));

// Service Registrations
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IRoleService, RoleManager>();

// Data Protection
builder.Services.AddScoped<IDataProtection, DataProtection>();
var keysDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys"));
builder.Services.AddDataProtection()
                .SetApplicationName("StudentInfoSys")
                .PersistKeysToFileSystem(keysDirectory);

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