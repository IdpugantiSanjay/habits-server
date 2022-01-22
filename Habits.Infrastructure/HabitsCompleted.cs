using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habits.Infrastructure;

public class HabitsCompleted
{
    public ObjectId Id { get; set; }

    public ObjectId HabitId { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CompletedDate { get; set; }
}