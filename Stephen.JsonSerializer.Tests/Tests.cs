﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using NUnit.Framework;

namespace Stephen.JsonSerializer.Tests
{
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
        public bool BoolValue { get; set; }
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
            var json = JsonSerializer.Serialize(_event);
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
                        EnumTest = MyEnum.Value2,
                        BoolValue = true
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
    }
}