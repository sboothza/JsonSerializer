using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Stephen.JsonSerializer.Tests
{
	public class ModelUpdate<T>
	{
		[JsonProperty("<OldItem>k__BackingField")]
		[System.Text.Json.Serialization.JsonPropertyName("<OldItem>k__BackingField")]
		[Newtonsoft.Json.JsonProperty("<OldItem>k__BackingField")]
		public T OldItem { get; set; }

		[JsonProperty("<NewItem>k__BackingField")]
		[System.Text.Json.Serialization.JsonPropertyName("<NewItem>k__BackingField")]
		[Newtonsoft.Json.JsonProperty("<NewItem>k__BackingField")]
		public T NewItem { get; set; }

		public ModelUpdate(T oldItem, T newItem)
		{
			this.OldItem = oldItem;
			this.NewItem = newItem;
		}

		public string Key => throw new NotImplementedException();

		public ModelUpdate()
		{

		}
	}

	public class ModelUpdateMicrosoft<T>
	{
		[JsonProperty("<OldItem>k__BackingField")]
		[System.Text.Json.Serialization.JsonPropertyName("<OldItem>k__BackingField")]
		[Newtonsoft.Json.JsonProperty("<OldItem>k__BackingField")]
		public T OldItem { get; set; }

		[JsonProperty("<NewItem>k__BackingField")]
		[System.Text.Json.Serialization.JsonPropertyName("<NewItem>k__BackingField")]
		[Newtonsoft.Json.JsonProperty("<NewItem>k__BackingField")]
		public T NewItem { get; set; }

		public ModelUpdateMicrosoft(T oldItem, T newItem)
		{
			this.OldItem = oldItem;
			this.NewItem = newItem;
		}

		//public string Key => throw new NotImplementedException(); this breaks with ms deserialize

		public ModelUpdateMicrosoft()
		{

		}
	}

	public class BasicItem
	{
		public string Id { get; set; }
		public string Source { get; set; }
		public string SportId { get; set; }
		public string FixtureId { get; set; }
		public DateTime Timestamp { get; set; }
		public string Type { get; set; }

		public BasicItem()
		{

		}
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
		public List<SampleChild> Children { get; set; } = [];
	}

	public class SampleChild
	{
		public string Name { get; set; }
		public long Id { get; set; }
		public MyEnum EnumTest { get; set; }
		public bool BoolValue { get; set; }
		[JsonProperty(Ignore = true)]
		public string ShouldIgnore { get; set; }
	}

	[TestFixture]
	public class Tests
	{
		private LoggingEvent _event;
		private string _json;
		private const int LOOPS = 1000;

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

		//453
		[Test]
		public void TestCustom()
		{
			var json = JsonSerializer.Serialize(_event.LocationInformation);
			Console.WriteLine(json.Length);
		}

		//25.81
		[Test]
		public void MeasureCustomSerialize()
		{
			var start = DateTime.Now;
			for (var i = 0; i < LOOPS; i++)
			{
				var json = JsonSerializer.Serialize(_event, new JsonSerializerOptions());
				if (json.Length < 0)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}

		//35.89
		[Test]
		public void MeasureNewtonsoftSerialize()
		{
			var start = DateTime.Now;
			for (var i = 0; i < LOOPS; i++)
			{
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(_event);
				if (json.Length < 0)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}

		//30.74
		[Test]
		public void MeasureMicrosoftSerialize()
		{
			var start = DateTime.Now;
			var options = new System.Text.Json.JsonSerializerOptions
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};

			for (var i = 0; i < LOOPS; i++)
			{
				var json = System.Text.Json.JsonSerializer.Serialize(_event, options);
				if (json.Length < 0)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
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
			catch
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
			catch
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
		public void TestNotPretty()
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
			var json = JsonSerializer.Serialize(item, new JsonSerializerOptions
			{
				IgnoreErrors = true
			});
			var jsonToMatch =
				"{\"Name\" : \"Sample\",\"Id\" : 12,\"Children\" : [{\"Name\" : \"Child Name\",\"Id\" : 34,\"EnumTest\" : \"Value2\",\"BoolValue\" : \"True\"}]}";

			Console.WriteLine(json);
			Assert.AreEqual(json, jsonToMatch);
		}

		[Test]
		public void TestNulls()
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
						Name = null,
						EnumTest = MyEnum.Value2,
						BoolValue = true
					}
				}
			};
			var json = JsonSerializer.Serialize(item, new JsonSerializerOptions
			{
				IgnoreErrors = true,
				IgnoreNulls = true
			});
			var jsonToMatch =
				"{\"Name\" : \"Sample\",\"Id\" : 12,\"Children\" : [{\"Id\" : 34,\"EnumTest\" : \"Value2\",\"BoolValue\" : \"True\"}]}";

			Console.WriteLine(json);
			Assert.AreEqual(json, jsonToMatch);

			item = new Sample
			{
				Name = "Sample",
				Id = 12,
				Children = null
			};
			json = JsonSerializer.Serialize(item, new JsonSerializerOptions
			{
				IgnoreErrors = true,
				IgnoreNulls = true
			});
			jsonToMatch = "{\"Name\" : \"Sample\",\"Id\" : 12}";

			Console.WriteLine(json);
			Assert.AreEqual(json, jsonToMatch);
		}

		[Test]
		public void TestRename()
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

			var json = JsonSerializer.Serialize(update, new JsonSerializerOptions
			{
				IgnoreErrors = true
			});
			Console.WriteLine(json);
			Assert.IsNotEmpty(json);
			Assert.AreEqual(
				"{\"<OldItem>k__BackingField\" : null,\"<NewItem>k__BackingField\" : {\"Name\" : \"Sample\",\"Id\" : 12,\"Children\" : [{\"Name\" : \"Child Name\",\"Id\" : 34,\"EnumTest\" : \"Value2\",\"BoolValue\" : \"True\"}]}}",
				json);

			json = JsonSerializer.Serialize(update, new JsonSerializerOptions
			{
				IgnoreErrors = true,
				IgnoreAttributes = true
			});
			Console.WriteLine(json);
			Assert.IsNotEmpty(json);
			Assert.AreEqual(
				"{\"OldItem\" : null,\"NewItem\" : {\"Name\" : \"Sample\",\"Id\" : 12,\"Children\" : [{\"Name\" : \"Child Name\",\"Id\" : 34,\"EnumTest\" : \"Value2\",\"BoolValue\" : \"True\",\"ShouldIgnore\" : null}]}}",
				json);
		}

		[Test]
		public void TestDeserialize()
		{
			var json = "{\"<OldItem>k__BackingField\":null,\"<NewItem>k__BackingField\":{\"Id\":\"1656264681828\",\"Source\":\"Betradar\",\"SportId\":\"Tennis\",\"FixtureId\":\"34312309\",\"Timestamp\":\"/Date(1701328228680)/\",\"Type\":\"MatchStatusUpdate\"}}";
			var item = JsonSerializer.Deserialize<ModelUpdate<BasicItem>>(json, new JsonSerializerOptions
			{
				IgnoreErrors = true
			});
			Console.WriteLine(item);
		}

		//20.10
		[Test]
		public void MeasureCustomDeserialize()
		{
			var json = "{\"<OldItem>k__BackingField\":null,\"<NewItem>k__BackingField\":{\"Id\":\"1656264681828\",\"Source\":\"Betradar\",\"SportId\":\"Tennis\",\"FixtureId\":\"34312309\",\"Timestamp\":\"/Date(1701328228680)/\",\"Type\":\"MatchStatusUpdate\"}}";
			var start = DateTime.Now;
			var options = new JsonSerializerOptions
			{
				IgnoreErrors = true
			};
			for (var i = 0; i < LOOPS; i++)
			{
				var obj = JsonSerializer.Deserialize<ModelUpdate<BasicItem>>(json, options);
				if (obj.NewItem is null)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}

		//24.96
		[Test]
		public void MeasureNewtonsoftDeserialize()
		{
			var json = "{\"<OldItem>k__BackingField\":null,\"<NewItem>k__BackingField\":{\"Id\":\"1656264681828\",\"Source\":\"Betradar\",\"SportId\":\"Tennis\",\"FixtureId\":\"34312309\",\"Timestamp\":\"/Date(1701328228680)/\",\"Type\":\"MatchStatusUpdate\"}}";
			var settings = new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; } };
			var start = DateTime.Now;
			for (var i = 0; i < LOOPS; i++)
			{
				var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ModelUpdate<BasicItem>>(json, settings);
				if (obj.NewItem is null)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}

		//22.94
		[Test]
		public void MeasureMicrosoftDeserialize()
		{
			var json = "{\"<OldItem>k__BackingField\":null,\"<NewItem>k__BackingField\":{\"Id\":\"1656264681828\",\"Source\":\"Betradar\",\"SportId\":\"Tennis\",\"FixtureId\":\"34312309\",\"Timestamp\":\"2023-11-30T07:10:28\",\"Type\":\"MatchStatusUpdate\"}}";
			var start = DateTime.Now;
			var options = new System.Text.Json.JsonSerializerOptions
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};

			for (var i = 0; i < LOOPS; i++)
			{
				var obj = System.Text.Json.JsonSerializer.Deserialize<ModelUpdateMicrosoft<BasicItem>>(json, options);
				if (obj.NewItem is null)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}
	}
}