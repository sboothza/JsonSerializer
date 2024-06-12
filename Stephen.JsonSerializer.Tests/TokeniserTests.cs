﻿using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Stephen.JsonSerializer.Tests
{
	[TestFixture]
	public class TokeniserTests
	{
		[Test]
		public void TestString()
		{
			var json = "\"this is a string\"";
			var parser = new JsonParser();
			var result = parser.Parse(json);
			Assert.AreEqual(typeof(StringToken), result[0].GetType());
			Assert.AreEqual("this is a string", (result[0] as StringToken).Value);
		}

		[Test]
		public void TestStringWithWhitespace()
		{
			var json = "  \t\"this is a string\"   \t\r\n";
			var parser = new JsonParser();
			var result = parser.Parse(json);
			Assert.AreEqual(typeof(StringToken), result[0].GetType());
			Assert.AreEqual("this is a string", (result[0] as StringToken).Value);
		}

		[Test]
		public void TestNumber()
		{
			var json = "  -123.5 ";
			var parser = new JsonParser();
			var result = parser.Parse(json);
			Assert.AreEqual(typeof(NumberToken), result[0].GetType());
			Assert.AreEqual("-123.5", (result[0] as NumberToken).Value);
		}

		[Test]
		public void TestBoolean()
		{
			var json = "  false  TruE ";
			var parser = new JsonParser();
			var result = parser.Parse(json);
			Assert.AreEqual(typeof(UnquotedConstantToken), result[0].GetType());
			Assert.AreEqual(typeof(UnquotedConstantToken), result[1].GetType());
			Assert.AreEqual("false", (result[0] as UnquotedConstantToken).Value);
			Assert.AreEqual("true", (result[1] as UnquotedConstantToken).Value);
		}

		[Test]
		public void TestCombined()
		{
			var json = "  false -123.5  TruE \"this is a string\" ";
			var parser = new JsonParser();
			var result = parser.Parse(json);

			Assert.AreEqual(typeof(UnquotedConstantToken), result[0].GetType());
			Assert.AreEqual("false", (result[0] as UnquotedConstantToken).Value);

			Assert.AreEqual(typeof(NumberToken), result[1].GetType());
			Assert.AreEqual("-123.5", (result[1] as NumberToken).Value);

			Assert.AreEqual(typeof(UnquotedConstantToken), result[2].GetType());
			Assert.AreEqual("true", (result[2] as UnquotedConstantToken).Value);

			Assert.AreEqual(typeof(StringToken), result[3].GetType());
			Assert.AreEqual("this is a string", (result[3] as StringToken).Value);
		}

		[Test]
		public void TestTokeniseBasic()
		{
			var json = "{\"str1\":null,\"str2\":{\"Id\":\"123\",\"Source\":\"src1\",\"Tags\":[{\"Name\":\"name1\",\"Value\":\"value1\",\"Category\":0},{\"Name\":\"name2\",\"Value\":\"\",\"Category\":0}]}}";
			var parser = new JsonParser();
			var result = parser.Parse(json);
			var expectedResult = new List<JsonToken>
			{
				new ObjectStartToken(),
				new StringToken("str1"),
				new MemberSeparatorToken(),
				new UnquotedConstantToken("null"),
				new CommaToken(),
				new StringToken("str2"),
				new MemberSeparatorToken(),
				new ObjectStartToken(),
				new StringToken("Id"),
				new MemberSeparatorToken(),
				new StringToken("123"),
				new CommaToken(),
				new StringToken("Source"),
				new MemberSeparatorToken(),
				new StringToken("src1"),
				new CommaToken(),
				new StringToken("Tags"),
				new MemberSeparatorToken(),
				new ListStartToken(),
				new ObjectStartToken(),
				new StringToken("Name"),
				new MemberSeparatorToken(),
				new StringToken("name1"),
				new CommaToken (),
				new StringToken("Value"),
				new MemberSeparatorToken (),
				new StringToken("value1"),
				new CommaToken  (),
				new StringToken("Category"),
				new MemberSeparatorToken (),
				new NumberToken("0"),
				new ObjectEndToken(),
				new CommaToken (),
				new ObjectStartToken(),
				new StringToken("Name"),
				new MemberSeparatorToken(),
				new StringToken("name2"),
				new CommaToken (),
				new StringToken("Value"),
				new MemberSeparatorToken (),
				new StringToken(""),
				new CommaToken(),
				new StringToken("Category"),
				new MemberSeparatorToken (),
				new NumberToken("0"),
				new ObjectEndToken(),
				new ListEndToken(),
				new ObjectEndToken(),
				new ObjectEndToken()
			};

			for (int i = 0; i < expectedResult.Count; i++)
			{
				var expected = expectedResult[i];
				var actual = result[i];
				Assert.AreEqual(expected.GetType(), actual.GetType());
				if (expected is StringToken stringToken)
				{
					Assert.AreEqual(stringToken.Value, (actual as StringToken).Value);
				}
				else if (expected is NumberToken numberToken)
				{
					Assert.AreEqual(numberToken.Value, (actual as NumberToken).Value);
				}
				else if (expected is UnquotedConstantToken unquotedConstantToken)
				{
					Assert.AreEqual(unquotedConstantToken.Value, (actual as UnquotedConstantToken).Value);
				}
			}

		}


		[Test]
		public void TestTokeniseList()
		{
			var json = "[\"value1\",\"value2\",0,\"\",false]";
			var parser = new JsonParser();
			var result = parser.Parse(json);

			Assert.AreEqual(11, result.Count);

			Assert.AreEqual(typeof(ListStartToken), result[0].GetType());

			Assert.AreEqual(typeof(StringToken), result[1].GetType());
			Assert.AreEqual("value1", (result[1] as StringToken).Value);

			Assert.AreEqual(typeof(CommaToken), result[2].GetType());

			Assert.AreEqual(typeof(StringToken), result[3].GetType());
			Assert.AreEqual("value2", (result[3] as StringToken).Value);

			Assert.AreEqual(typeof(CommaToken), result[4].GetType());

			Assert.AreEqual(typeof(NumberToken), result[5].GetType());
			Assert.AreEqual("0", (result[5] as NumberToken).Value);

			Assert.AreEqual(typeof(CommaToken), result[6].GetType());

			Assert.AreEqual(typeof(StringToken), result[7].GetType());
			Assert.AreEqual("", (result[7] as StringToken).Value);

			Assert.AreEqual(typeof(CommaToken), result[8].GetType());

			Assert.AreEqual(typeof(UnquotedConstantToken), result[9].GetType());
			Assert.AreEqual("false", (result[9] as UnquotedConstantToken).Value);

			Assert.AreEqual(typeof(ListEndToken), result[10].GetType());
		}

		[Test]
		public void TestObjectList()
		{
			var json = "{\n    \"list\":[\n        \"value1\",\n        \"value2\"\n    ]\n}";
			var parser = new JsonParser();
			var result = parser.Parse(json);
			foreach (var item in result)
				Console.WriteLine(item);
		}
		
		[Test]
		public void TestObjectListObject()
		{
			var json = "{\n    \"list\":[\n        {\"Value\":\"value1\"},\n        {\"Value\":\"value2\"}\n    ]\n}";
			var parser = new JsonParser();
			var result = parser.Parse(json);
			foreach (var item in result)
				Console.WriteLine(item);
		}


		[Test]
		public void TestTokeniseJson()
		{
			var json = "{\"<OldItem>k__BackingField\":null,\"<NewItem>k__BackingField\":{\"Id\":\"1656264681828\",\"Source\":\"Betradar\",\"SportId\":\"Tennis\",\"FixtureId\":\"34312309\",\"Timestamp\":\"/Date(1701328228680)/\",\"Type\":\"MatchStatusUpdate\",\"Tags\":[{\"Name\":\"Server\",\"Value\":\"\",\"Category\":0},{\"Name\":\"MatchTime\",\"Value\":\"00:00:00\",\"Category\":0},{\"Name\":\"BetRadarUuid\",\"Value\":\"fed4c17c-8dff-41a4-a4d9-eacd42806495\",\"Category\":0},{\"Name\":\"CourtNumber\",\"Value\":\"221019\",\"Category\":0},{\"Name\":\"FirstServer\",\"Value\":null,\"Category\":0},{\"Name\":\"MatchState\",\"Value\":null,\"Category\":0},{\"Name\":\"NumberOfSets\",\"Value\":\"3\",\"Category\":0},{\"Name\":\"ScoringType\",\"Value\":\"No advantage rule, super tiebreak to 10 points\",\"Category\":0},{\"Name\":\"Team\",\"Value\":\"NONE\",\"Category\":0},{\"Name\":\"PlayerId\",\"Value\":\"NONE\",\"Category\":0},{\"Name\":\"TeamAPlayer1\",\"Value\":\"CHAVEZ VILLALPANDO, LUIS DIEGO\",\"Category\":0},{\"Name\":\"TeamAPlayer1Id\",\"Value\":\"6608164\",\"Category\":0},{\"Name\":\"TeamAPlayer1Country\",\"Value\":null,\"Category\":0},{\"Name\":\"TeamAPlayer2\",\"Value\":\"LOPEZ HERNAEZ, MIKEL\",\"Category\":0},{\"Name\":\"TeamAPlayer2Id\",\"Value\":\"8838316\",\"Category\":0},{\"Name\":\"TeamAPlayer2Country\",\"Value\":null,\"Category\":0},{\"Name\":\"TeamBPlayer1\",\"Value\":\"TADDIA, LEONARDO\",\"Category\":0},{\"Name\":\"TeamBPlayer1Id\",\"Value\":\"9910999\",\"Category\":0},{\"Name\":\"TeamBPlayer1Country\",\"Value\":null,\"Category\":0},{\"Name\":\"TeamBPlayer2\",\"Value\":\"GARCIA, MIGUEL\",\"Category\":0},{\"Name\":\"TeamBPlayer2Id\",\"Value\":\"113995\",\"Category\":0},{\"Name\":\"TeamBPlayer2Country\",\"Value\":null,\"Category\":0},{\"Name\":\"TieBreakType\",\"Value\":\"\",\"Category\":0},{\"Name\":\"TossWinnerChoice\",\"Value\":null,\"Category\":0},{\"Name\":\"TossWinner\",\"Value\":\"\",\"Category\":0},{\"Name\":\"UmpireCode\",\"Value\":\"\",\"Category\":0},{\"Name\":\"UmpireCountry\",\"Value\":\"\",\"Category\":0},{\"Name\":\"UmpireName\",\"Value\":\"\",\"Category\":0},{\"Name\":\"Won\",\"Value\":\"0:0\",\"Category\":0}],\"KeyValues\":[\"Betradar\",\"34312309\",\"1656264681828\"]}}";
			var parser = new JsonParser();
			var result = parser.Parse(json);
		}

		[Test]
		public void TestParseValues()
		{
			var json = "  \t\"this is a string\"   \t\r\n";
			var parser = new JsonParser();
			var tokens = parser.Parse(json);
			var result = parser.ProcessTokens(tokens);
			Assert.AreEqual(typeof(JsonObjectValue), result.GetType());
			Assert.AreEqual("this is a string", (result as JsonObjectValue).Value);

			json = "  -123.5 ";
			tokens = parser.Parse(json);
			result = parser.ProcessTokens(tokens);
			Assert.AreEqual(typeof(JsonObjectValue), result.GetType());
			Assert.AreEqual("-123.5", (result as JsonObjectValue).Value);
		}

		[Test]
		public void TestParseSimpleObjectTree()
		{
			var json = "{\"<OldItem>k__BackingField\":null,\"<NewItem>k__BackingField\":{\"Id\":\"1656264681828\",\"Source\":\"Betradar\",\"SportId\":\"Tennis\",\"FixtureId\":\"34312309\",\"Timestamp\":\"/Date(1701328228680)/\",\"Type\":\"MatchStatusUpdate\"}}";
			var parser = new JsonParser();
			var tokens = parser.Parse(json);
			var result = parser.ProcessTokens(tokens);
			Console.WriteLine(result);
		}
	}
}
