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
using NClassifier.Bayesian;

namespace NClassifier.Tests.Bayesian
{
	public class AbstractWordsDataSourceSupport
	{
		protected IWordsDataSource wordsDataSource = null;

		public void TestEmptySource()
		{
			wordsDataSource = new SimpleWordsDataSource();
			WordProbability wp = wordsDataSource.GetWordProbability("myWord");
			Assert.IsNull(wp);
		}

		public void TestAddMatch()
		{
			wordsDataSource = new SimpleWordsDataSource();
			wordsDataSource.AddMatch("myWord");
			WordProbability wp = wordsDataSource.GetWordProbability("myWord");
			Assert.IsNotNull(wp);
			Assert.AreEqual(1, wp.MatchingCount);
			Assert.AreEqual(0, wp.NonMatchingCount);

			wordsDataSource.AddMatch("myWord");

			Assert.AreEqual(2, wp.MatchingCount);
			Assert.AreEqual(0, wp.NonMatchingCount);
		}

		public void TestAddNonMatch()
		{
			wordsDataSource = new SimpleWordsDataSource();
			wordsDataSource.AddNonMatch("myWord");
			WordProbability wp = wordsDataSource.GetWordProbability("myWord");
			Assert.IsNotNull(wp);
			Assert.AreEqual(0, wp.MatchingCount);
			Assert.AreEqual(1, wp.NonMatchingCount);

			wordsDataSource.AddNonMatch("myWord");

			wp = wordsDataSource.GetWordProbability("myWord");
			Assert.IsNotNull(wp);
			Assert.AreEqual(0, wp.MatchingCount);
			Assert.AreEqual(2, wp.NonMatchingCount);
		}

		public void TestAddMultipleMatches()
		{
			wordsDataSource = new SimpleWordsDataSource();
			string word = "myWord";
			int count = 10;
			for (int i = 0; i < count; i++)
				wordsDataSource.AddMatch(word);
			WordProbability wp = wordsDataSource.GetWordProbability(word);
			Assert.IsNotNull(wp);
			Assert.AreEqual(count, wp.MatchingCount);
		}

		public void TestAddMultipleNonMatches()
		{
			wordsDataSource = new SimpleWordsDataSource();
			string word = "myWord";
			int count = 10;
			for (int i = 0; i < count; i++)
				wordsDataSource.AddNonMatch(word);
			WordProbability wp = wordsDataSource.GetWordProbability(word);
			Assert.IsNotNull(wp);
			Assert.AreEqual(count, wp.NonMatchingCount);
		}

		public void TestMultipleWrites()
		{
			wordsDataSource = new SimpleWordsDataSource();
			string word = "myWord";
			int count = 500;
			for (int i =0; i < count; i++)
				wordsDataSource.AddNonMatch(word + count);
		}
	}
}