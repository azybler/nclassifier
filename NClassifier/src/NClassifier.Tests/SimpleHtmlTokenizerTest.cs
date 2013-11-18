#region Copyright (c) 2004, Ryan Whitaker
/*********************************************************************************
'
' Copyright (c) 2004 Ryan Whitaker
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the authors be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that 
' you wrote the original software. If you use this software in a product, an 
' acknowledgment (see the following) in the product documentation is required.
'
' This product uses software written by the developers of NClassifier
' (http://nclassifier.sourceforge.net).  NClassifier is a .NET port of the Nick
' Lothian's Java text classification engine, Classifier4J 
' (http://classifier4j.sourceforge.net).
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'********************************************************************************/
#endregion

using System;
using NUnit.Framework;

namespace NClassifier.Tests
{
	[TestFixture]
	public class SimpleHtmlTokenizerTest
	{
		SimpleHtmlTokenizer tokenizer;

		[TestFixtureSetUp]
		protected void Setup()
		{
			tokenizer = new SimpleHtmlTokenizer();
		}

		[TestFixtureTearDown]
		protected void TearDown()
		{
			tokenizer = null;
		}

		[Test]
		public void TestTokenize()
		{
			string input = "<h1>This is in an html tag &gt;</h1>";
			string[] expected = { "This", "is", "in", "an", "html", "tag" };

			string[] output = tokenizer.Tokenize(input);
			Assert.IsNotNull(output);
			Assert.AreEqual(expected.Length, output.Length);

			for (int i = 0; i < output.Length; i++)
				Assert.AreEqual(expected[i], output[i]);
		}

		[Test]
		public void TestResolveEntities()
		{
			string normalString = "this is a normal string";
			string resolvedString = tokenizer.ResolveEntities(normalString);
			Assert.AreEqual(normalString, resolvedString);

			string withEnt = "this includes a non-breaking space ";
			resolvedString = tokenizer.ResolveEntities(withEnt + "&nbsp;");
			Assert.AreEqual(withEnt + " ", resolvedString);
		}
	}
}