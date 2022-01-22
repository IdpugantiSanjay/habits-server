using Habits.Core;
using Habits.Core.Periods;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Habits.Infrastructure;

public sealed record Habit
{
    [BsonConstructor]
    internal Habit(string name, ObjectId id, DateTime startDate, BsonDocument periodDocument)
    {
        Id = id;
        Name = name;
        StartDate = startDate;
        PeriodDocument = periodDocument;

        var type = PeriodDocument["type"].AsString;

        var clonedPeriodDocument = PeriodDocument.Clone().AsBsonDocument;
        clonedPeriodDocument.Remove("type");


        switch (type)
        {
            case nameof(EveryDay):
                Period = BsonSerializer.Deserialize<EveryDay>(clonedPeriodDocument);
                break;
            case nameof(EveryWeek):
                Period = BsonSerializer.Deserialize<EveryWeek>(clonedPeriodDocument);
                break;
            case nameof(EveryMonth):
                Period = BsonSerializer.Deserialize<EveryMonth>(clonedPeriodDocument);
                break;
            case nameof(EveryYear):
                Period = BsonSerializer.Deserialize<EveryYear>(clonedPeriodDocument);
                break;
            case nameof(EverySelectWeekDays):
                Period = BsonSerializer.Deserialize<EverySelectWeekDays>(clonedPeriodDocument);
                break;
            case nameof(EveryXDays):
                Period = BsonSerializer.Deserialize<EveryXDays>(clonedPeriodDocument);
                break;
            case nameof(EveryXTimesPerY):
                Period = BsonSerializer.Deserialize<EveryXTimesPerY>(clonedPeriodDocument);
                break;
            default:
                throw new InvalidOperationException($"Invalid type {type}");
        }
    }


    public Habit(string name, Period period, DateTime startDate)
    {
        Id = ObjectId.GenerateNewId();
        Name = name;
        Period = period;
        StartDate = startDate.Date;


        PeriodDocument = new BsonDocument();
        switch (Period)
        {
            case EveryDay:
                PeriodDocument.AddRange(new BsonDocument("type", nameof(EveryDay)));
                break;
            case EveryWeek:
                PeriodDocument.AddRange(new BsonDocument("type", nameof(EveryWeek)));
                break;
            case EveryMonth:
                PeriodDocument.AddRange(new BsonDocument("type", nameof(EveryMonth)));
                break;
            case EveryYear:
                PeriodDocument.AddRange(new BsonDocument("type", nameof(EveryYear)));
                break;
            case EverySelectWeekDays selectWeekDays:
                PeriodDocument.AddRange(
                    new BsonDocument("type", nameof(EverySelectWeekDays)).Add("SelectDays",
                        new BsonArray(selectWeekDays.SelectDays)));
                break;
            case EveryXDays xDays:
                PeriodDocument.AddRange(new BsonDocument("type", nameof(EveryXDays)).Add("X", new BsonInt32(xDays.X)));
                break;
            case EveryXTimesPerY xTimesPerY:
                PeriodDocument.AddRange(new BsonDocument("type", nameof(EveryXTimesPerY))
                    .Add("X", new BsonInt32(xTimesPerY.X))
                    .Add("Y", new BsonInt32((int) xTimesPerY.Y))
                );
                break;
        }
    }

    [BsonElement("PeriodDocument")] private BsonDocument? PeriodDocument { get; }

    public ObjectId Id { get; }

    [BsonElement(nameof(Name))] public string Name { get; }

    [BsonIgnore] public Period Period { get; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime StartDate { get; init; }

    public bool Equals(Habit? otherHabit) => otherHabit is not null && Id.Equals(otherHabit.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public void Deconstruct(out ObjectId Id, out string Name, out Period Period, out DateTime StartDate)
    {
        Id = this.Id;
        Name = this.Name;
        Period = this.Period;
        StartDate = this.StartDate;
    }
}