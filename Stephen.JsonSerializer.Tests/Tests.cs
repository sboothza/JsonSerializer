using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using NUnit.Framework;

namespace Stephen.JsonSerializer.Tests;

public class ModelUpdate<T>
{
    public T OldItem { get; set; }

    public T NewItem { get; set; }

    public ModelUpdate(T oldItem, T newItem)
    {
        this.OldItem = oldItem;
        this.NewItem = newItem;
    }

    public string Key => throw new NotImplementedException();
}

public enum MyEnum
{
    Value1,
    Value2
}

public class Sample
{
    public string Name { get; set; }
    public long Id { get; set; }
    public List<SampleChild> Children { get; set; } = new List<SampleChild>();
}

public class SampleChild
{
    public string Name { get; set; }
    public long Id { get; set; }
    public MyEnum EnumTest { get; set; }
}

[TestFixture]
public class Tests
{
    private LoggingEvent _event;
    private string _json;
    private int loops = 100;

    [SetUp]
    public void Setup()
    {
        _event = new LoggingEvent
        {
            Level = new Level
            {
                Name = "ERROR",
                Value = 1000
            },
            LoggerName = "Test",
            TimeStamp = DateTime.Now,
            //MessageObject = new
            //                {
            //                    Message = "this is a message",
            //                    Id = 1
            //                },
            MessageObject = new MessageObjectTest
            {
                Message = "this is a message",
                Id = 1
            },
            ExceptionObject = new ArgumentOutOfRangeException(paramName: "item", message: "item not in range"),
            LocationInformation = new LocationInfo
            {
                ClassName = "JsonTests.NewtonsoftTests",
                FileName = "c:\\temp\\file.cs",
                LineNumber = "12",
                MethodName = "Test",
                StackFrames =
                {
                    new StackFrameItem
                    {
                        ClassName = "JsonTests.NewtonsoftTests",
                        FileName = "c:\\temp\\file.cs",
                        LineNumber = "12",
                        Method = new MethodItem
                        {
                            Name = "Test",
                            Parameters =
                            {
                                "param1",
                                "param2"
                            }
                        }
                    },
                    new StackFrameItem
                    {
                        ClassName = "JsonTests.NewtonsoftTests",
                        FileName = "c:\\temp\\file.cs",
                        LineNumber = "13",
                        Method = new MethodItem
                        {
                            Name = "Test2",
                            Parameters =
                            {
                                "param3",
                                "param4"
                            }
                        }
                    }
                }
            },
            Properties =
            {
                {
                    "key1", "value1"
                },
                {
                    "key2", "value2"
                },
                {
                    "key3", "value3"
                }
            }
        };

        _json =
            "{\"Level\":{\"Name\":\"ERROR\",\"Value\":1000},\"TimeStamp\":\"2021-10-11T19:24:59.9788744+02:00\",\"LoggerName\":\"Test\",\"LocationInformation\":{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"12\",\"MethodName\":\"Test\",\"StackFrames\":[{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"12\",\"Method\":{\"Name\":\"Test\",\"Parameters\":[\"param1\",\"param2\"]}},{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"13\",\"Method\":{\"Name\":\"Test2\",\"Parameters\":[\"param3\",\"param4\"]}}]},\"MessageObject\":{\"Message\":\"this is a message\",\"Id\":1},\"ExceptionObject\":{\"Message\":\"item not in range (Parameter 'item')\",\"ActualValue\":null,\"ParamName\":\"item\",\"TargetSite\":null,\"StackTrace\":null,\"Data\":{},\"InnerException\":null,\"HelpLink\":null,\"Source\":null,\"HResult\":-2146233086},\"Properties\":{\"key1\":\"value1\",\"key2\":\"value2\",\"key3\":\"value3\"}}";
    }

    [Test]
    public void TestCustom()
    {
        var json = JsonSerializer.Serialize(_event, new JsonSerializerOptions{Pretty = true});
        Console.WriteLine(json);
    }

    //52ms
    [Test]
    public void MeasureCustomSerialize()
    {
        var start = DateTime.Now;
        for (var i = 0; i < loops; i++)
        {
            var json = JsonSerializer.Serialize(_event);
            if (json.Length < 0)
                throw new InvalidOperationException("broke");
        }

        var spent = DateTime.Now - start;
        Console.WriteLine(spent.TotalMilliseconds);
        Console.WriteLine(JsonSerializer.Serialize(_event));
    }

    //130ms
    [Test]
    public void MeasureNewtonsoftSerialize()
    {
        var start = DateTime.Now;
        for (var i = 0; i < loops; i++)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(_event);
            if (json.Length < 0)
                throw new InvalidOperationException("broke");
        }

        var spent = DateTime.Now - start;
        Console.WriteLine(spent.TotalMilliseconds);
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(_event));
    }

    //50ms
    [Test]
    public void MeasureMicrosoftSerialize()
    {
        var start = DateTime.Now;
        var options = new System.Text.Json.JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        for (var i = 0; i < loops; i++)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_event, options);
            if (json.Length < 0)
                throw new InvalidOperationException("broke");
        }

        var spent = DateTime.Now - start;
        Console.WriteLine(spent.TotalMilliseconds);
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(_event));
    }

    [Test]
    public void TestError()
    {
        var item = new Sample
        {
            Name = "Sample",
            Id = 12,
            Children =
            {
                new SampleChild
                {
                    Id = 34,
                    Name = "Child Name",
                    EnumTest = MyEnum.Value2
                }
            }
        };

        var update = new ModelUpdate<Sample>(null, item);

        try
        {
            var json = JsonSerializer.Serialize(update, new JsonSerializerOptions
            {
                IgnoreErrors = false
            });
            Assert.IsTrue(false, "should not pass");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(true, "should fail here");
        }

        try
        {
            var json = JsonSerializer.Serialize(update, new JsonSerializerOptions
            {
                IgnoreErrors = true
            });
            Console.WriteLine(json);
            Assert.IsTrue(true, "should pass");
            Assert.IsNotEmpty(json);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "should not fail here");
        }
    }

    [Test]
    [Explicit]
    public void TestErrorMS()
    {
        var item = new Sample
        {
            Name = "Sample",
            Id = 12,
            Children =
            {
                new SampleChild
                {
                    Id = 34,
                    Name = "Child Name"
                }
            }
        };

        var update = new ModelUpdate<Sample>(null, item);
        var options = new System.Text.Json.JsonSerializerOptions
        {
        };
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(update, options);
            Assert.IsTrue(false, "should not pass");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(true, "should fail here");
        }

        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(update, options);
            Console.WriteLine(json);
            Assert.IsTrue(true, "should pass");
            Assert.IsNotEmpty(json);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "should not fail here");
        }
    }

    [Test]
    public void DeserializeSingleValues()
    {
        var json = "\"bob\"";
        var strObj = JsonSerializer.Deserialize<string>(json);
        Assert.AreEqual(strObj, "bob");

        json = "null";
        var objObj = JsonSerializer.Deserialize<object>(json);
        Assert.AreEqual(objObj, null);

        json = "true";
        var boolObj = JsonSerializer.Deserialize<bool>(json);
        Assert.AreEqual(boolObj, true);

        json = "false";
        boolObj = JsonSerializer.Deserialize<bool>(json);
        Assert.AreEqual(boolObj, false);

        json = "123";
        var intObj = JsonSerializer.Deserialize<int>(json);
        Assert.AreEqual(intObj, 123);

        json = "123.6";
        var numObj = JsonSerializer.Deserialize<double>(json);
        Assert.AreEqual(numObj, 123.6);
    }

    [Test]
    public void TestDeserializeSimpleList()
    {
        var json = "[123,456,789]";
        var listObj = JsonSerializer.Deserialize<List<int>>(json);
        Assert.AreEqual(listObj.Count, 3);
        Assert.Contains(123, listObj);
        Assert.Contains(456, listObj);
        Assert.Contains(789, listObj);
    }
    
    [Test]
    public void TestSimpleListPretty()
    {
        var json = "[123,456,789]";
        var listObj = JsonSerializer.Deserialize<List<int>>(json);
        var newJson = JsonSerializer.Serialize(listObj, new JsonSerializerOptions{Pretty = true});
        Console.WriteLine(newJson);
    }
    
    [Test]
    public void TestDeserializeSimpleListWithWhitespace()
    {
        var json = " [ 123 , 456 , 789 ] ";
        var listObj = JsonSerializer.Deserialize<List<int>>(json);
        Assert.AreEqual(listObj.Count, 3);
        Assert.Contains(123, listObj);
        Assert.Contains(456, listObj);
        Assert.Contains(789, listObj);
    }

    [Test]
    public void DeserializeComplexObject()
    {
        var json = "{\"Id\":123,\"Name\":\"Bob\",\"EnumTest\":\"Value2\"}";
        // var sc = new SampleChild { Id = 123, Name = "Bob", EnumTest = MyEnum.Value2 };
        var obj = JsonSerializer.Deserialize<SampleChild>(json);
        Assert.AreEqual(obj.Id,123);
        Assert.AreEqual(obj.Name,"Bob");
        Assert.AreEqual(obj.EnumTest,MyEnum.Value2);
    }
    
    [Test]
    public void TestComplexObjectPretty()
    {
        var json = "{\"Id\":123,\"Name\":\"Bob\",\"EnumTest\":\"Value2\"}";
        // var sc = new SampleChild { Id = 123, Name = "Bob", EnumTest = MyEnum.Value2 };
        var obj = JsonSerializer.Deserialize<SampleChild>(json);
        var newJson = JsonSerializer.Serialize(obj, new JsonSerializerOptions { Pretty = true });
        Console.WriteLine(newJson);
    }
    
    [Test]
    public void DeserializeMoreComplexObject()
    {
        var json = "{\"Id\":123,\"Name\":\"Bob\",\"Children\":[{\"Id\":123,\"Name\":\"Bob\",\"EnumTest\":\"Value2\"},{\"Id\":456,\"Name\":\"Bill\",\"EnumTest\":\"Value1\"}]}";
        // var sc = new SampleChild { Id = 123, Name = "Bob", EnumTest = MyEnum.Value2 };
        var obj = JsonSerializer.Deserialize<Sample>(json);
        Assert.AreEqual(obj.Id,123);
        Assert.AreEqual(obj.Name,"Bob");
        Assert.AreEqual(obj.Children.Count, 2);
        Assert.AreEqual(obj.Children[0].Name, "Bob");
        Assert.AreEqual(obj.Children[1].Name, "Bill");
    }

    [Test]
    public void TestMoreComplexObjectPretty()
    {
        var json =
            "{\"Id\":123,\"Name\":\"Bob\",\"Children\":[{\"Id\":123,\"Name\":\"Bob\",\"EnumTest\":\"Value2\"},{\"Id\":456,\"Name\":\"Bill\",\"EnumTest\":\"Value1\"}]}";
        // var sc = new SampleChild { Id = 123, Name = "Bob", EnumTest = MyEnum.Value2 };
        var obj = JsonSerializer.Deserialize<Sample>(json);
        var newJson = JsonSerializer.Serialize(obj, new JsonSerializerOptions { Pretty = true });
        Console.WriteLine(newJson);
    }
    
    [Test]
    public void DeserializeMoreComplexObjectWithWhitespace()
    {
        var json = " { \"Id\" : 123 , \"Name\" : \"Bob\",\"Children\" : [ { \"Id\":123,\"Name\":\"Bob\" , \"EnumTest\" : \"Value2\" } , {\"Id\":456,\"Name\":\"Bill\",\"EnumTest\":\"Value1\" } ] } ";
        // var sc = new SampleChild { Id = 123, Name = "Bob", EnumTest = MyEnum.Value2 };
        var obj = JsonSerializer.Deserialize<Sample>(json);
        Assert.AreEqual(obj.Id,123);
        Assert.AreEqual(obj.Name,"Bob");
        Assert.AreEqual(obj.Children.Count, 2);
        Assert.AreEqual(obj.Children[0].Name, "Bob");
        Assert.AreEqual(obj.Children[1].Name, "Bill");
    }
}