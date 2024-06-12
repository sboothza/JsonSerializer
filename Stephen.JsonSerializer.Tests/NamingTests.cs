using NUnit.Framework;
namespace Stephen.JsonSerializer.Tests;

[TestFixture]
public class NamingTests
{
    [Test]
    public void TestPascalNormal()
    {
        var test = "ThisIsATest";
        var match = test.ToPascalCase();
        Assert.AreEqual(test, match);

        test = "thisIsATest";
        match = test.ToPascalCase();
        Assert.AreEqual("ThisIsATest", match);
        
        test = "this_is_a_test";
        match = test.ToPascalCase();
        Assert.AreEqual("ThisIsATest", match);
    }
    
    [Test]
    public void TestCamelNormal()
    {
        var test = "ThisIsATest";
        var match = test.ToCamelCase();
        Assert.AreEqual("thisIsATest", match);

        test = "thisIsATest";
        match = test.ToCamelCase();
        Assert.AreEqual("thisIsATest", match);
        
        test = "this_is_a_test";
        match = test.ToCamelCase();
        Assert.AreEqual("thisIsATest", match);
    }
    
    [Test]
    public void TestSnakeNormal()
    {
        var test = "ThisIsATest";
        var match = test.ToSnakeCase();
        Assert.AreEqual("this_is_a_test", match);

        test = "thisIsATest";
        match = test.ToSnakeCase();
        Assert.AreEqual("this_is_a_test", match);
        
        test = "this_is_a_test";
        match = test.ToSnakeCase();
        Assert.AreEqual("this_is_a_test", match);
    }

    [Test]
    public void TestCombinedNormal()
    {
        var test = "ThisIsATest";
        var match = test.ConvertName(NamingOptions.PascalCase);
        Assert.AreEqual("ThisIsATest", match);

        match = test.ConvertName(NamingOptions.CamelCase);
        Assert.AreEqual("thisIsATest", match);
        
        match = test.ConvertName(NamingOptions.SnakeCase);
        Assert.AreEqual("this_is_a_test", match);
    }

    [Test]
    public void TestNull()
    {
        string test = null;
        var match = test.ConvertName(NamingOptions.PascalCase);
        Assert.AreEqual("", match);
        
        test = "";
        match = test.ConvertName(NamingOptions.PascalCase);
        Assert.AreEqual("", match);
    }
}