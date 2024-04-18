using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NUnit.Framework;
namespace Stephen.JsonSerializer.Tests;

static class Extensions
{
    internal static bool TagsEqual(this List<Tag> tags, List<Tag> otherTags)
    {
        if (tags == otherTags)
            return true;
        int num1 = tags?.Count ?? 0;
        int num2 = otherTags?.Count ?? 0;
        if (num1 == 0 && num2 == 0)
            return true;
        if (!num1.Equals(num2))
            return false;
        if (tags == null)
            return true;
        foreach (var tag in tags)
        {
            string str;
            if (!otherTags.TryGetValue(tag.Name, out str, tag.Category) || !tag.Value.Equals(str))
                return false;
        }
        return true;
    }

    private static bool TryGetValue(this List<Tag> tags, string name, out string value, TagCategory category = TagCategory.Default)
    {
        if (tags == null)
        {
            value = (string)null;
            return false;
        }
        var tag = tags.Find((Predicate<Tag>)(t => t.Name.ToLower() == name.ToLower() && t.Category == category));
        if (tag != null)
        {
            value = tag.Value;
            return true;
        }
        value = null;
        return false;
    }

    internal static bool ParticipantsEqual(this List<Participant> thisParticipants, List<Participant> otherParticipants)
    {
        if (!thisParticipants.Select((Func<Participant, string>)(p => p.Id))
                             .SequenceEqual(otherParticipants.Select((Func<Participant, string>)(r => r.Id))))
            return false;
        foreach (var thisParticipant in thisParticipants)
        {
            var p1 = thisParticipant;
            var participant = otherParticipants.First((Func<Participant, bool>)(p => p.Id == p1.Id));
            if (!p1.Equals(participant))
                return false;
        }
        return true;
    }
}

public enum DateTimeZoneType
{
    UTC,
    Local,
    Unknown,
}

public enum TagCategory
{
    Default,
    BaseVariable,
    CompTag,
    CompMetaTag,
    SmmTag,
    SpinSport,
    SpinCategory,
    Competition,
}

public enum ParticipantDescription
{
    Individual,
    Team,
    Home,
    Away,
    None,
    Unknown,
}

public class Tag : IEquatable<Tag>
{
    public string Name { get; set; }
    public string Value { get; set; }
    public TagCategory Category { get; set; }

    public Tag()
    {
    }

    public Tag(string name, string value, TagCategory tagCategory = TagCategory.Default)
    {
        Name = name;
        Value = value;
        Category = tagCategory;
    }

    public Tag(Enum name, string value, TagCategory tagCategory = TagCategory.Default) : this(name.ToString(), value, tagCategory)
    {
    }

    public Tag(Enum name, Enum value, TagCategory tagCategory = TagCategory.Default) : this(name.ToString(), value.ToString(),
        tagCategory)
    {
    }

    public Tag(Enum name, object value, TagCategory tagCategory = TagCategory.Default) : this(name.ToString(), value.ToString(),
        tagCategory)
    {
    }

    public bool Equals(Tag other)
    {
        if (this == other)
            return true;
        return Value == other.Value && Category == other.Category && Name == other.Name;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() => Value;
}

public class SubParticipant
{
    public SubParticipant() => Tags = new List<Tag>();
    public string Id { get; set; }
    public string Source { get; set; }
    public string SportId { get; set; }
    public string Name { get; set; }
    public List<Tag> Tags { get; set; }
}

public class Participant
{
    public string Id { get; set; }
    public string Source { get; set; }
    public string Name { get; set; }
    public DateTime Timestamp { get; set; }
    public string HomeOrAway { get; set; }
    public string IndividualOrTeam { get; set; }
    public List<Tag> Tags { get; set; }
    public List<SubParticipant> SubParticipants { get; set; }

    public Participant()
    {
        Tags = new List<Tag>();
        SubParticipants = new List<SubParticipant>();
    }

    public string Key => $"{Source}|{Id}";

    public override bool Equals(object obj)
    {
        if (!(obj is Participant participant))
            return false;
        bool flag = participant.Name == Name && participant.Id == Id && participant.IndividualOrTeam == IndividualOrTeam &&
                    participant.HomeOrAway == HomeOrAway;
        return Tags.TagsEqual(participant.Tags) & flag;
    }

    public override int GetHashCode() => Key.GetHashCode();
}

public class Fixture
{
    public Fixture()
    {
        DateTimeZone = DateTimeZoneType.Unknown.ToString();
        Participants = new List<Participant>();
        Tags = new List<Tag>();
    }

    public Fixture(string feedSource, string id) : this()
    {
        Source = feedSource;
        Id = id;
    }

    public string Id { get; set; }
    public string Description { get; set; }
    public string SportId { get; set; }
    public string CompetitionId { get; set; }
    public DateTime Date { get; set; }
    public string DateTimeZone { get; set; }
    public List<Participant> Participants { get; set; }
    public string Source { get; set; }
    public DateTime Timestamp { get; set; }

    public List<string> ParticipantIds
    {
        get => Participants.Select((Func<Participant, string>)(p => p.Id))
                           .ToList();
        set { }
    }

    public string Key
    {
        get => $"{Source}|{Id}";
        set { }
    }

    public List<Tag> Tags { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == this)
            return true;
        if (!(obj is Fixture fixture))
            return false;
        bool flag1 = fixture.Date == Date && fixture.Description == Description && fixture.Source == Source && fixture.Id == Id &&
                     fixture.SportId == SportId;
        int num1 = Participants.ParticipantsEqual(fixture.Participants) ? 1 : 0;
        bool flag2 = Tags.TagsEqual(fixture.Tags);
        int num2 = flag1 ? 1 : 0;
        return (num1 & num2 & (flag2 ? 1 : 0)) != 0;
    }

    public override int GetHashCode() => Key.GetHashCode();

    public void BuildDescription()
    {
        string str1;
        string str2;
        if (Participants.Count == 2)
        {
            str1 = Participants[0].Name;
            str2 = Participants[1].Name;
            Participants.ForEach((Action<Participant>)(p => p.IndividualOrTeam = ParticipantDescription.Individual.ToString()));
        }
        else
        {
            str1 = Participants[0].Name + ", " + Participants[1].Name;
            str2 = Participants[2].Name + ", " + Participants[3].Name;
            Participants.ForEach((Action<Participant>)(p => p.IndividualOrTeam = ParticipantDescription.Team.ToString()));
        }
        Description = str1 + " v " + str2;
        AddDefaultTag("TeamADescription", str1);
        AddDefaultTag("TeamBDescription", str2);
    }

    public void AddDefaultTag(string name, string value) => Tags.Add(new Tag(name, value));

    public void AddBaseVariableTag(string name, string value) => Tags.Add(new Tag(name, value, TagCategory.BaseVariable));

    public void AddParticipant(Participant participant) => Participants.Add(participant);

    public bool IsLogged => true;
}

[TestFixture]
public class ComplexTest
{
    private string fixture_json =
        "{\n    \"Id\": \"44017420\",\n    \"Description\": \"Faucon, Romain v Dellavedova, Matthew\",\n    \"SportId\": \"Tennis\",\n    \"CompetitionId\": \"138169\",\n    \"Date\": \"2023-09-26T09:55:00.0000000Z\",\n    \"DateTimeZone\": \"Utc\",\n    \"Participants\": [\n        {\n            \"Id\": \"15546439\",\n            \"Source\": \"Betradar\",\n            \"Name\": \"Faucon, Romain\",\n            \"Timestamp\": \"2023-11-14T08:27:35.3167604Z\",\n            \"HomeOrAway\": \"Home\",\n            \"IndividualOrTeam\": \"Individual\",\n            \"Tags\": [\n                {\n                    \"Name\": \"BetradarParticipantName\",\n                    \"Value\": \"Faucon, Romain\",\n                    \"Category\":\"Default\"\n                }\n            ],\n            \"SubParticipants\": [],\n            \"Key\": \"Betradar|15546439\"\n        },\n        {\n            \"Id\": \"7748952\",\n            \"Source\": \"Betradar\",\n            \"Name\": \"Dellavedova, Matthew\",\n            \"Timestamp\": \"2023-11-14T08:27:35.3306814Z\",\n            \"HomeOrAway\": \"Away\",\n            \"IndividualOrTeam\": \"Individual\",\n            \"Tags\": [\n                {\n                    \"Name\": \"BetradarParticipantName\",\n                    \"Value\": \"Dellavedova, Matthew\",\n                    \"Category\":\"Default\"\n                }\n            ],\n            \"SubParticipants\": [],\n            \"Key\": \"Betradar|7748952\"\n        }\n    ],\n    \"Source\": \"Betradar\",\n    \"Timestamp\": \"2023-11-14T08:27:35.2292475Z\",\n    \"ParticipantIds\": [\n        \"15546439\",\n        \"7748952\"\n    ],\n    \"Key\": \"Betradar|44017420\",\n    \"Tags\": [\n        {\n            \"Name\": \"TeamADescription\",\n            \"Value\": \"Faucon, Romain\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"TeamBDescription\",\n            \"Value\": \"Dellavedova, Matthew\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"BetradarId\",\n            \"Value\": \"44017420\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"Qualifying\",\n            \"Value\": \"false\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"Gender\",\n            \"Value\": \"Male\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"Gender\",\n            \"Value\": \"Male\",\n            \"Category\":\"BaseVariable\"\n        },\n        {\n            \"Name\": \"CompetitionType\",\n            \"Value\": \"Singles\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"CompetitionName\",\n            \"Value\": \"Monastir, Singles Main, M-Itf-Tun-43A\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"CompetitionDisplayName\",\n            \"Value\": \"Monastir, Singles Main, M-Itf-Tun-43A\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"Surface\",\n            \"Value\": \"Unknown\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"CourtId\",\n            \"Value\": \"243640\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"CourtName\",\n            \"Value\": \"Court 3\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"Organisation\",\n            \"Value\": \"Itf Men\",\n            \"Category\":\"Default\"\n        },\n        {\n            \"Name\": \"TournamentCompetitionType\",\n            \"Value\": \"Monastir, Singles Main, M-Itf-Tun-43A\",\n            \"Category\":\"Default\"\n        }\n    ],\n    \"IsLogged\":\"True\"\n}";

    [Test]
    public void SampleTest()
    {
        var fixture = System.Text.Json.JsonSerializer.Deserialize<Fixture>(fixture_json, new System.Text.Json.JsonSerializerOptions
        {
            AllowTrailingCommas = true, 
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });
        var newJson = JsonSerializer.Serialize(fixture, new JsonSerializerOptions());
        Console.WriteLine(newJson);
    }

    [Test]
    public void TestDeserialize()
    {
        
    }
}