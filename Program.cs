using ProvisionAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

//add dependency 
/**
 * Transient => always different; a new instance is provided to every controller and every service
 * scoped => objects are the same within a request
 * singletion => same for every object and every request 
 * refer to https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences
 */

builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IProjectDbConn>(x =>
	new ProjectDbConn(builder.Configuration.GetConnectionString("ProjectDB")));
builder.Services.AddScoped<ICustomEncryption, CustomEncryption>();


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
