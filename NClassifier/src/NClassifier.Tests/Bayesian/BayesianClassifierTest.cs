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
	[TestFixture]
	public class BayesianClassifierTest
	{
		[Test]
		public void TestClassify()
		{
			SimpleWordsDataSource wds = new SimpleWordsDataSource();
			BayesianClassifier classifier = new BayesianClassifier(wds);

			string[] sentence = new string[] { "This", "is", "a", "sentence", "about", "java" };

			Assert.AreEqual(IClassifierConstants.NEUTRAL_PROBABILITY, classifier.Classify(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence), 0d);

			wds.SetWordProbability(new WordProbability("This", .5d));
			wds.SetWordProbability(new WordProbability("is", .5d));
			wds.SetWordProbability(new WordProbability("a", .5d));
			wds.SetWordProbability(new WordProbability("sentence", .2d));
			wds.SetWordProbability(new WordProbability("about", .5d));
			wds.SetWordProbability(new WordProbability("java", .99d));

			Assert.AreEqual(.96d, classifier.Classify(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence), .009d);
		}

		[Test]
		public void TestGetWordsDataSource()
		{
			SimpleWordsDataSource wds = new SimpleWordsDataSource();
			BayesianClassifier classifier = new BayesianClassifier(wds);
			Assert.AreEqual(wds, classifier.WordsDataSource);
		}

		[Test]
		public void TestGetTokenizer()
		{
			SimpleWordsDataSource wds = new SimpleWordsDataSource();
			ITokenizer tokenizer = new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WORD_BREAKS);
			BayesianClassifier classifier = new BayesianClassifier(wds, tokenizer);
			Assert.AreEqual(tokenizer, classifier.Tokenizer);
		}

		[Test]
		public void TestGetStopWordProvider()
		{
			SimpleWordsDataSource wds = new SimpleWordsDataSource();
			ITokenizer tokenizer = new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WORD_BREAKS);
			IStopWordProvider stopWordProvider = new DefaultStopWordProvider();
			BayesianClassifier classifier = new BayesianClassifier(wds, tokenizer, stopWordProvider);
			Assert.AreEqual(stopWordProvider, classifier.StopWordProvider);
		}

		[Test]
		public void TestCaseSensitive()
		{
			BayesianClassifier classifier = new BayesianClassifier();
			Assert.IsFalse(classifier.IsCaseSensitive);
			classifier.IsCaseSensitive = true;
			Assert.IsTrue(classifier.IsCaseSensitive);
		}

		[Test]
		public void TestTransformWord()
		{
			BayesianClassifier classifier = new BayesianClassifier();
			Assert.IsFalse(classifier.IsCaseSensitive);

			string word = null;
			try
			{
				classifier.TransformWord(word);
				Assert.Fail("No exception thrown when null passed.");
			}
			catch {}

			word = "myWord";
			Assert.AreEqual(word.ToLower(), classifier.TransformWord(word));

			classifier.IsCaseSensitive = true;
			Assert.AreEqual(word, classifier.TransformWord(word));
		}

		[Test]
		public void TestCalculateOverallProbability()
		{
			double prob = 0.3d;
			WordProbability wp1 = new WordProbability("myWord1", prob);
			WordProbability wp2 = new WordProbability("myWord2", prob);
			WordProbability wp3 = new WordProbability("myWord3", prob);
		
			WordProbability[] wps = new WordProbability[] { wp1, wp2, wp3 };
			double errorMargin = 0.0001d;
		
			double xy = (prob * prob * prob);
			double z = (1-prob)*(1-prob)*(1-prob);
		
			double result = xy/(xy + z);
		
			BayesianClassifier classifier = new BayesianClassifier();
		 		
			Assert.AreEqual(result, classifier.CalculateOverallProbability(wps), errorMargin);
		}
	}
}
