using KhachSan.Models;
using KhachSan.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services before building the app
builder.Services.Configure<MongoDBSetting>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<MongoDBService>();

builder.Services.AddSingleton<IRoomServiceService, RoomServiceService>();
builder.Services.AddSingleton<IDiscountService, DiscountService>();
builder.Services.AddSingleton<IRoomTypeService, RoomTypeService>();
builder.Services.AddSingleton<IRoomService, RoomService>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IAccountService, AccountService>();
builder.Services.AddSingleton<IRoleService, RoleService>();
builder.Services.AddSingleton<ICustomDiscountService, CustomerDiscountService>();
builder.Services.AddSingleton<IReviewRoomService, ReviewRoomService>();
builder.Services.AddSingleton<IReviewServiceService, ReviewServiceService>();
builder.Services.AddSingleton<ICloudinaryService, CloudinaryService>();

builder.Services.AddSingleton<DropboxService>();

builder.Services.AddControllers();

// Configure CORS before app build
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add Swagger support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.MapControllers();

app.Run();
