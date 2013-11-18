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
using NClassifier.Summarizer;
using NUnit.Framework;

namespace NClassifier.Tests.Summarizer
{
	[TestFixture]
	public class SimpleSummarizerTest
	{
		SimpleSummarizer summarizer = null;

		[TestFixtureSetUp]
		public void Setup()
		{
			summarizer = new SimpleSummarizer();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			summarizer = null;
		}

		[Test]
		public void TestSummarize()
		{
			string input = "NClassifier is a dotnet assembly for working with text.  NClassifier includes a summarizer.";
			string expectedResult = "NClassifier is a dotnet assembly for working with text.";
			string result = summarizer.Summarize(input, 1);
			Assert.AreEqual(expectedResult, result);

			input = "NClassifier is a dotnet assembly for working with text. NClassifier includes a summarizer. A Summarizer allows the summary of text. A Summarizer is really cool. I don't think there are any other dotnet summarizers.";
			expectedResult = "NClassifier is a dotnet assembly for working with text. NClassifier includes a summarizer.";
			result = summarizer.Summarize(input, 2);
			Assert.AreEqual(expectedResult, result);
		}
	}
}