using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

			var entries = new List<PropertyTuple>();
			var props = source.GetType().GetProperties()
				.Select(p => new
				{
					prop = p,
					name = ((JsonPropertyAttribute)p.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault())?.Name ?? p.Name,
					ignore = ((JsonPropertyAttribute)p.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault())?.Ignore ?? false
				}).Where(pn => options.IgnoreAttributes || !pn.ignore);

			foreach (var prop in props)
			{
				if (options.IgnoreNulls && source.GetFieldOrPropertyValue(prop.prop.Name) is null)
					continue;
				else
				{
					var entry = PropertyTuple.Create(options, source, prop.prop.Name, options.IgnoreAttributes ? prop.prop.Name : prop.name);
					if (entry != null)
						entries.Add(entry);
				}
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
			options ??= new JsonSerializerOptions();
			return Deserialize(jsonObject, type, options);
		}


		private static object Deserialize(JsonObject obj, Type type, JsonSerializerOptions options)
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

				var props = type.GetProperties()
					.Select(p => new
					{
						prop = p,
						name = ((JsonPropertyAttribute)p.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault())?.Name ?? p.Name,
						ignore = ((JsonPropertyAttribute)p.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault())?.Ignore ?? false
					}).Where(pn => options.IgnoreAttributes || !pn.ignore);

				foreach (var prop in props)
				{
					if (prop.prop.CanWrite)
					{
						var name = prop.name;
						var jsonValue = jsonObjectComplex.Complex[name];
						var value = Deserialize(jsonValue, prop.prop.PropertyType, options);
						prop.prop.SetValue(result, value);
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
					list.Add(Deserialize(item, type, options));
				}
				return list;
			}
			throw new IndexOutOfRangeException("Could not deserialize");
		}
	}
}