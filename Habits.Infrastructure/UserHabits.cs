using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habits.Infrastructure;

#nullable disable

public class UserHabits
{
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public string Username { get; set; }

    public List<Habit> Habits { get; set; }
}