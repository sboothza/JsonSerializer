using System.Collections.Generic;
using System.Text;

namespace Stephen.JsonSerializer;



public abstract class JsonObject
{
}

public class JsonObjectValue : JsonObject
{
	public object Value { get; set; }
	public override string ToString()
	{
		return Value.ToString();
	}
}

public class JsonObjectList : JsonObject
{
	public List<JsonObject> Array { get; set; } = new List<JsonObject>();
	public override string ToString()
	{
		var sb = new StringBuilder(1024);
		sb.Append("[");
		foreach (var item in Array)
		{
			sb.Append(item.ToString());
		}
		sb.Append("]");
		return sb.ToString();
	}
}

public class JsonObjectComplex : JsonObject
{
	public Dictionary<string, JsonObject> Complex { get; set; } = new Dictionary<string, JsonObject>();

	public override string ToString()
	{
		var sb = new StringBuilder(1024);
		sb.Append("{");
		foreach (var name in Complex.Keys)
		{
			sb.Append(name);
			sb.Append(":");
			sb.Append(Complex[name].ToString());
		}
		sb.Append("}");
		return sb.ToString();
	}

}