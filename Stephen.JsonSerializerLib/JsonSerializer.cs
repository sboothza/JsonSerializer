using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace Stephen.JsonSerializer
{
	public static class JsonSerializer
	{
		public static string Serialize(object source, JsonSerializerOptions options = null)
		{
			var writer = new StringWriter();
			Serialize(source, writer, options);
			return writer.ToString();
		}

		private static void Serialize(object source, TextWriter writer, JsonSerializerOptions options)
		{
			if (source is null)
			{
				writer.Write("null");
				return;
			}

			options ??= new JsonSerializerOptions();

			if (source.Flatten(out var result))
			{
				writer.Write(result);
				return;
			}

			//complex
			if (source is IEnumerable sourceEnumerable)
			{
				if (sourceEnumerable is IDictionary dictionarySource)
				{
					//handle dictionary
					writer.Write("{");

					var dictionaryEntries = dictionarySource.GetEnumerator()
						.Cast<DictionaryEntry>();

					dictionaryEntries.ProcessList(entry =>
					{
						writer.Write($"\"{entry.Key}\" : ");
						Serialize(entry.Value, writer, options);

						writer.Write(",");
					}, entry =>
					{
						writer.Write($"\"{entry.Key}\" : ");
						Serialize(entry.Value, writer, options);
					});

					writer.Write("}");
					return;
				}

				//handle list
				writer.Write("[");

				var listEntries = sourceEnumerable.Cast<object>();

				listEntries.ProcessList(o =>
				{
					Serialize(o, writer, options);
					writer.Write(",");
				}, o => Serialize(o, writer, options));

				writer.Write("]");
				return;
			}

			//single object
			writer.Write("{");

			var properties = source.GetType()
				.GetProperties();

			var entries = new List<PropertyTuple>();
			foreach (var prop in properties)
			{
				PropertyTuple tuple = null;
				var attrib = (JsonPropertyAttribute)prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
					.FirstOrDefault();
				if (attrib is not null && !attrib.Ignore && !options.IgnoreAttributes)
					tuple = PropertyTuple.Create(options, source, prop.Name, attrib.Name);
				else if (attrib is null || options.IgnoreAttributes)
					tuple = PropertyTuple.Create(options, source, prop.Name);

				if (tuple is not null && !(options.IgnoreNulls && tuple.Value is null))
					entries.Add(tuple);
			}

			entries.ProcessList(tuple =>
			{
				writer.Write($"\"{tuple.OutputName}\" : ");
				Serialize(tuple.Value, writer, options);
				writer.Write(",");
			}, tuple =>
			{
				writer.Write($"\"{tuple.OutputName}\" : ");
				Serialize(tuple.Value, writer, options);
			});

			writer.Write("}");
		}

		public static T Deserialize<T>(string json, JsonSerializerOptions options)
		{
			return (T)Deserialize(json, typeof(T), options);
		}

		public static object Deserialize(string json, Type type, JsonSerializerOptions options)
		{
			var parser = new JsonParser();
			var tokens = parser.Parse(json);
			var jsonObject = parser.ProcessTokens(tokens);

			return Deserialize(jsonObject, type);
		}


		private static object Deserialize(JsonObject obj, Type type)
		{
			if (obj == null)
				return null;

			if (obj is JsonObjectValue jsonPrimitive)
			{
				if (jsonPrimitive.Value is string strValue)
				{
					if (strValue == "null")
						return null;
					if (strValue.StartsWith("/Date("))
					{
						long unix = long.Parse(strValue.Replace("/Date(", "").Replace(")/", ""));
						var value = DateTimeOffset.FromUnixTimeMilliseconds(unix);
						return value.DateTime;
					}
				}

				return jsonPrimitive.Value;
			}
			if (obj is JsonObjectComplex jsonObjectComplex)
			{
				object result = Activator.CreateInstance(type);
				foreach (var prop in type.GetProperties())
				{
					if (prop.CanWrite)
					{
						var name = prop.Name;
						var custom = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault();
						if (custom is not null)
						{
							name = custom.GetFieldOrPropertyValue("Name").ToString();
						}
						var jsonValue = jsonObjectComplex.Complex[name];
						var value = Deserialize(jsonValue, prop.PropertyType);
						prop.SetValue(result, value);
					}
				}

				return result;
			}
			if (obj is JsonObjectList jsonList)
			{
				var listType = typeof(List<>).MakeGenericType(type);
				IList list = (IList)Activator.CreateInstance(listType);
				foreach (var item in jsonList.Array)
				{
					list.Add(Deserialize(item, type));
				}
				return list;
			}
			throw new IndexOutOfRangeException("Could not deserialize");
		}
	}
}