using System;
using System.Text.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Stephen.JsonSerializer.Tests
{
    [TestFixture]
    public class Tests
    {
        private LoggingEvent _event;
        private string _json;

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
                         ExceptionObject =
                             new ArgumentOutOfRangeException(paramName: "item", message: "item not in range"),
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
                             { "key1", "value1" },
                             { "key2", "value2" },
                             { "key3", "value3" }
                         }
                     };

            _json =
                "{\"Level\":{\"Name\":\"ERROR\",\"Value\":1000},\"TimeStamp\":\"2021-10-11T19:24:59.9788744+02:00\",\"LoggerName\":\"Test\",\"LocationInformation\":{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"12\",\"MethodName\":\"Test\",\"StackFrames\":[{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"12\",\"Method\":{\"Name\":\"Test\",\"Parameters\":[\"param1\",\"param2\"]}},{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"13\",\"Method\":{\"Name\":\"Test2\",\"Parameters\":[\"param3\",\"param4\"]}}]},\"MessageObject\":{\"Message\":\"this is a message\",\"Id\":1},\"ExceptionObject\":{\"Message\":\"item not in range (Parameter 'item')\",\"ActualValue\":null,\"ParamName\":\"item\",\"TargetSite\":null,\"StackTrace\":null,\"Data\":{},\"InnerException\":null,\"HelpLink\":null,\"Source\":null,\"HResult\":-2146233086},\"Properties\":{\"key1\":\"value1\",\"key2\":\"value2\",\"key3\":\"value3\"}}";
        }

        [Test]
        public void TestCustom()
        {
            var serializer = new Stephen.JsonSerializer.JsonSerializer();
            var json = serializer.Serialize(_event);
            Console.WriteLine(json);
        }

        //52ms
        [Test]
        public void MeasureCustomSerialize()
        {
            var start = DateTime.Now;
            var serializer = new Stephen.JsonSerializer.JsonSerializer();
            for (var i = 0; i < 1000; i++)
            {
                var json = serializer.Serialize(_event);
                if (json.Length < 0)
                    throw new InvalidOperationException("broke");
            }

            var spent = DateTime.Now - start;
            Console.WriteLine(spent.TotalMilliseconds);
            Console.WriteLine(serializer.Serialize(_event));
        }

        //130ms
        [Test]
        public void MeasureNewtonsoftSerialize()
        {
            var start = DateTime.Now;
            for (var i = 0; i < 1000; i++)
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
                              IgnoreNullValues = true
                          };

            for (var i = 0; i < 1000; i++)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(_event, options);
                if (json.Length < 0)
                    throw new InvalidOperationException("broke");
            }

            var spent = DateTime.Now - start;
            Console.WriteLine(spent.TotalMilliseconds);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(_event));
        }
    }
}