using Habits.Application.Handlers;
using Habits.Infrastructure;
using MediatR;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var client = new MongoClient("mongodb://127.0.0.1:27017");
var db = client.GetDatabase("habits");

var userHabits = db.GetCollection<UserHabits>("userHabits");
var completedHabits = db.GetCollection<HabitsCompleted>("completedHabits");

builder.Services.AddMediatR(typeof(CreateHabitHandler));
builder.Services.AddSingleton<HabitsService>();
builder.Services.AddSingleton(userHabits);
builder.Services.AddSingleton(completedHabits);

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