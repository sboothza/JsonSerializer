using System;
using NUnit.Framework;
namespace Stephen.JsonSerializer.Tests;

[TestFixture]
public class LayoutWriterTests
{
    [Test]
    public void TestProcessList()
    {
        var items = new[]
        {
            "item1",
            "item2",
            "item3"
        };
        var numNormalIterations = 0;
        var numLastIterations = 0;
        items.ProcessList(s => numNormalIterations++, s => numLastIterations++);
        
        Assert.AreEqual(2, numNormalIterations);
        Assert.AreEqual(1, numLastIterations);
    }
    
    [Test]
    public void TestBasicLayout()
    {
        using (var writer = new LayoutStreamWriter())
        {
            writer.WriteLine("FIRST");
            writer.Indent();
            writer.WriteLine("SECOND");
            writer.UnIndent();
            writer.WriteLine("FIRST AGAIN");
            writer.Indent();
            writer.WriteLine("SECOND AGAIN");
            writer.Indent();
            writer.WriteLine("THIRD");
            writer.UnIndent();
            writer.UnIndent();
            writer.WriteLine("FIRST END");
            var result = writer.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(result, "FIRST\r\n\tSECOND\r\nFIRST AGAIN\r\n\tSECOND AGAIN\r\n\t\tTHIRD\r\nFIRST END\r\n");
        }
    }
    
    [Test]
    public void TestComplexLayout()
    {
        using (var writer = new LayoutStreamWriter())
        {
            writer.WriteLine("FIRST");
            writer.Indent();
            writer.Write("SECOND");
            writer.WriteLine(" LINE");
            writer.UnIndent();
            writer.WriteLine("FIRST AGAIN");
            writer.Indent();
            writer.WriteLine("SECOND AGAIN");
            writer.Indent();
            writer.Write("THIRD");
            writer.Write(" LINE");
            writer.WriteLine(" TOGETHER");
            writer.UnIndent();
            writer.UnIndent();
            writer.WriteLine("FIRST END");
            var result = writer.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(result, "FIRST\r\n\tSECOND LINE\r\nFIRST AGAIN\r\n\tSECOND AGAIN\r\n\t\tTHIRD LINE TOGETHER\r\nFIRST END\r\n");
        }
    }

    [Test]
    public void TestJson()
    {
        using (var writer = new LayoutStreamWriter())
        {
            writer.WriteLine("{");
            writer.Indent();
            for (var i = 0; i < 10; i++)
            {
                writer.WriteLine($"\"name{i}\" : {i},");
            }
            writer.WriteLine("\"array\" : [");
            writer.Indent();
            for (var i = 0; i < 10; i++)
            {
                writer.WriteLine("{");
                writer.Indent();
                writer.WriteLine($"\"name\" : \"name_{i}\",");
                writer.WriteLine($"\"value\" : \"value_{i}\"");
                writer.UnIndent();
                writer.WriteLine("},");
            }
            writer.UnIndent();
            writer.WriteLine("],");
            writer.WriteLine("\"lastname\" : \"lastvalue\"");
            writer.UnIndent();
            writer.WriteLine("}");

            var result = writer.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(result,
                "{\r\n\t\"name0\" : 0,\r\n\t\"name1\" : 1,\r\n\t\"name2\" : 2,\r\n\t\"name3\" : 3,\r\n\t\"name4\" : 4,\r\n\t\"name5\" : 5,\r\n\t\"name6\" : 6,\r\n\t\"name7\" : 7,\r\n\t\"name8\" : 8,\r\n\t\"name9\" : 9,\r\n\t\"array\" : [\r\n\t\t{\r\n\t\t\t\"name\" : \"name_0\",\r\n\t\t\t\"value\" : \"value_0\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_1\",\r\n\t\t\t\"value\" : \"value_1\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_2\",\r\n\t\t\t\"value\" : \"value_2\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_3\",\r\n\t\t\t\"value\" : \"value_3\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_4\",\r\n\t\t\t\"value\" : \"value_4\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_5\",\r\n\t\t\t\"value\" : \"value_5\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_6\",\r\n\t\t\t\"value\" : \"value_6\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_7\",\r\n\t\t\t\"value\" : \"value_7\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_8\",\r\n\t\t\t\"value\" : \"value_8\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_9\",\r\n\t\t\t\"value\" : \"value_9\"\r\n\t\t},\r\n\t],\r\n\t\"lastname\" : \"lastvalue\"\r\n}\r\n");
        }
    }
    
    [Test]
    public void TestJsonLayoutBlock()
    {
        using (var writer = new LayoutStreamWriter())
        {
            writer.WriteLine("{");
            using (writer.StartBlock(true))
            {
                for (var i = 0; i < 10; i++)
                    writer.WriteLine($"\"name{i}\" : {i},");
                
                writer.WriteLine("\"array\" : [");
                using (writer.StartBlock(true))
                {
                    for (var i = 0; i < 10; i++)
                    {
                        writer.WriteLine("{");
                        using (writer.StartBlock(true))
                        {
                            writer.WriteLine($"\"name\" : \"name_{i}\",");
                            writer.WriteLine($"\"value\" : \"value_{i}\"");
                        }
                        writer.WriteLine("},");
                    }
                }
                writer.WriteLine("],");
                writer.WriteLine("\"lastname\" : \"lastvalue\"");
            }
            writer.WriteLine("}");

            var result = writer.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(result,
                "{\r\n\t\"name0\" : 0,\r\n\t\"name1\" : 1,\r\n\t\"name2\" : 2,\r\n\t\"name3\" : 3,\r\n\t\"name4\" : 4,\r\n\t\"name5\" : 5,\r\n\t\"name6\" : 6,\r\n\t\"name7\" : 7,\r\n\t\"name8\" : 8,\r\n\t\"name9\" : 9,\r\n\t\"array\" : [\r\n\t\t{\r\n\t\t\t\"name\" : \"name_0\",\r\n\t\t\t\"value\" : \"value_0\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_1\",\r\n\t\t\t\"value\" : \"value_1\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_2\",\r\n\t\t\t\"value\" : \"value_2\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_3\",\r\n\t\t\t\"value\" : \"value_3\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_4\",\r\n\t\t\t\"value\" : \"value_4\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_5\",\r\n\t\t\t\"value\" : \"value_5\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_6\",\r\n\t\t\t\"value\" : \"value_6\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_7\",\r\n\t\t\t\"value\" : \"value_7\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_8\",\r\n\t\t\t\"value\" : \"value_8\"\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\" : \"name_9\",\r\n\t\t\t\"value\" : \"value_9\"\r\n\t\t},\r\n\t],\r\n\t\"lastname\" : \"lastvalue\"\r\n}\r\n");
        }
    }

}