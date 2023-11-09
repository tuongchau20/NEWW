using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using NorthWind.Models;
using NorthWind.Models.account;
using NorthWind.Repositories;
using System.Data;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<testContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<testContext>().AddDefaultTokenProviders();
var serviceProvider = builder.Services.BuildServiceProvider();
var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

// Kết nối đến MongoDB Atlas
const string connectionUri = "mongodb+srv://tuong:tuongchau@cluster0.nr5qab5.mongodb.net/?retryWrites=true&w=majority";
var mongoClient = new MongoClient(connectionUri);
var database = mongoClient.GetDatabase("NorthWind");
using (var sqlConnection = new SqlConnection(connectionString))
{
    sqlConnection.Open();

    DataTable tableSchema = sqlConnection.GetSchema("Tables");
    foreach (DataRow row in tableSchema.Rows)
    {
        string tableName = row["TABLE_NAME"].ToString();

        using (var sqlCommand = new SqlCommand($"SELECT * FROM {tableName}", sqlConnection))
        {
            using (var sqlDataReader = sqlCommand.ExecuteReader())
            {
                var collection = database.GetCollection<BsonDocument>(tableName);

                while (sqlDataReader.Read())
                {
                    var bsonDocument = new BsonDocument();
                    for (int i = 0; i < sqlDataReader.FieldCount; i++)
                    {
                        var columnName = sqlDataReader.GetName(i);
                        var columnValue = sqlDataReader.GetValue(i);

                        if (columnValue != DBNull.Value) 
                        {
                            BsonValue bsonValue = BsonValue.Create(columnValue);
                            bsonDocument.Add(columnName, bsonValue);
                        }
                        else
                        {
                            bsonDocument.Add(columnName, BsonNull.Value);
                        }
                    }


                    collection.InsertOne(bsonDocument);
                }
            }
        }
    }
}



//Life cycle DI 
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

var app = builder.Build(); 
var cultureInfo = new CultureInfo("vi-VN"); // Đặt múi giờ ở đây
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = new List<CultureInfo> { cultureInfo },
    SupportedUICultures = new List<CultureInfo> { cultureInfo }
});
// Configure the HTTP request pipeline.
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
