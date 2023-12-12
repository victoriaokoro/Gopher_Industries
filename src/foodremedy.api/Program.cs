using foodremedy.api.Extensions;
using foodremedy.database.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFiles();
builder.Services.ConfigureCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(t => t.FullName));
builder.Services.UseLowercaseUrls();
builder.Services.AddDatabase();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddInternalServices();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();    TODO: Re-enable when we have HTTPS capability
app.UseCors();
app.MapControllers().RequireAuthorization();

app.Run();
