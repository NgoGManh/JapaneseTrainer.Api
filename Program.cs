using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.Repositories;
using JapaneseTrainer.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext + SQL Server với retry
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            // Retry tạm thời nếu gặp lỗi network / DB chưa sẵn sàng
            sqlOptions.EnableRetryOnFailure();
        }));

// Controllers
builder.Services.AddControllers();

// Swagger + định nghĩa Bearer auth (sử dụng Microsoft.OpenApi 2.x)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JapaneseTrainer API",
        Version = "v1",
        Description = "API for Japanese language learning application with vocabulary, grammar, exercises, and SRS (Spaced Repetition System) features.",
        Contact = new OpenApiContact
        {
            Name = "JapaneseTrainer Team"
        }
    });

    // Định nghĩa scheme Bearer để Swagger hiển thị nút Authorize
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer 12345abcdef\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Yêu cầu Bearer token cho tất cả endpoints (trừ những endpoint có [AllowAnonymous])
    // Note: Security requirement được xử lý tự động bởi Swashbuckle khi có [Authorize]
    // Có thể thêm manual nếu cần, nhưng với Microsoft.OpenApi 2.x thì cần dùng cách khác

    // Enable XML comments nếu có
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// JWT Auth
var jwtConfig = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtConfig["SecretKey"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Đăng ký Repositories & Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDictionaryRepository, DictionaryRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDictionaryService, DictionaryService>();
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

var app = builder.Build();

// Đảm bảo database & bảng được tạo tự động khi khởi động
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await initializer.EnsureCreatedAsync();
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();