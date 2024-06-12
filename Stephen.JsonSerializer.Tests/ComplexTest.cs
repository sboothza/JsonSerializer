using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using NUnit.Framework;
namespace Stephen.JsonSerializer.Tests;

static class Extensions
{
    public const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    public static string ToBaseString(this DateTime timestamp) => timestamp.ToString(TimestampFormat);
}

public enum DateTimeZoneType
{
    UTC,
    Local,
    Unknown,
}



public class TestListObj
{
    [JsonProperty("list")]
    public List<string> List { get; set; }
}

public class Dummy
{
    public string Value { get; set; }
}

public class TestListObjObj
{
    [JsonProperty("list")]
    public List<Dummy> List { get; set; }
}

[TestFixture]
public class ComplexTest
{
    
    

    [Test]
    public void TestListObjectDeserialize()
    {
        var json = "{\n    \"list\":[\n        \"value1\",\n        \"value2\"\n    ]\n}";
        var obj = JsonSerializer.Deserialize<TestListObj>(json, new JsonSerializerOptions());
        Assert.AreEqual(obj.List.Count, 2);
        Assert.AreEqual(obj.List[0], "value1");
        Assert.AreEqual(obj.List[1], "value2");
        Console.WriteLine(obj);
    }

    [Test]
    public void TestListDeserialize()
    {
        var json = "[\n        \"value1\",\n        \"value2\"\n    ]";
        var obj = JsonSerializer.Deserialize<List<string>>(json, new JsonSerializerOptions());
        Assert.AreEqual(obj.Count, 2);
        Assert.AreEqual(obj[0], "value1");
        Assert.AreEqual(obj[1], "value2");
        Console.WriteLine(obj);
    }

    [Test]
    public void TestListObjectObjectDeserialize()
    {
        var json = "{\n    \"list\":[\n        {\"Value\":\"value1\"},\n        {\"Value\":\"value2\"}\n    ]\n}";
        var obj = JsonSerializer.Deserialize<TestListObjObj>(json, new JsonSerializerOptions());
        Assert.AreEqual(obj.List.Count, 2);
        Assert.AreEqual(obj.List[0].Value, "value1");
        Assert.AreEqual(obj.List[1].Value, "value2");
        Console.WriteLine(obj);
    }
}