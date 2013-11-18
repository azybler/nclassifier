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
	public class SimpleClassifierTest
	{
		SimpleClassifier _classifier = null;

		[TestFixtureSetUp]
		protected void Setup()
		{
			_classifier = new SimpleClassifier();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_classifier = null;
		}

		[Test]
		public void TestGetSearchWord()
		{
			string word = "dotnet";
			_classifier.SearchWord = word;
			Assert.AreEqual(word, _classifier.SearchWord);
		}

		[Test]
		public void TestClassify()
		{
			string word = "dotnet";
			_classifier.SearchWord = word;

			string sentence = "This is a sentence about dotnet";
			Assert.AreEqual(1d, _classifier.Classify(sentence), 0d);
			
			sentence = "This is not";
			Assert.AreEqual(0d, _classifier.Classify(sentence), 0d);
		}

		[Test]
		public void TestMatch()
		{
			string word = "dotnet";
			_classifier.SearchWord = word;

			string sentence = "This is a sentence about dotnet";
			Assert.IsTrue(_classifier.IsMatch(sentence));
		}

		[Test]
		public void TestIsMatchDouble()
		{
			Assert.IsTrue(_classifier.IsMatch(IClassifierConstants.DEFAULT_CUTOFF));
			Assert.IsTrue(_classifier.IsMatch(IClassifierConstants.DEFAULT_CUTOFF + 0.01d));
			Assert.IsFalse(_classifier.IsMatch(IClassifierConstants.DEFAULT_CUTOFF - 0.01d));
		}
	}
}