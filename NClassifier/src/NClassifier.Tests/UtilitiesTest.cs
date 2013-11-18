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
using System.Collections;
using NUnit.Framework;
using NClassifier.Bayesian;

namespace NClassifier.Tests
{
	[TestFixture]
	public class UtilitiesTest
	{
		string sentence = "Hello there hello again and hello again.";

		[Test]
		public void TestGetWordFrequency()
		{
			// standard test
			Hashtable result = Utilities.GetWordFrequency(sentence);
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.IsNotNull(result["hello"]);
			Assert.AreEqual(3, (int)result["hello"]);
			Assert.AreEqual(2, (int)result["again"]);

			// test case sensitivity
			result = Utilities.GetWordFrequency(sentence, true);
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
			Assert.IsNotNull(result["hello"]);
			Assert.AreEqual(2, (int)result["hello"]);
			Assert.AreEqual(1, (int)result["Hello"]);
			Assert.AreEqual(2, (int)result["again"]);

			// test without a stop word provider
			result = Utilities.GetWordFrequency(sentence, false, new DefaultTokenizer(), null);
			Assert.IsNotNull(result);
			Assert.AreEqual(5, result.Count);
			Assert.IsNotNull(result["hello"]);
			Assert.AreEqual(3, (int)result["hello"]);
			Assert.AreEqual(1, (int)result["there"]);
			Assert.AreEqual(1, (int)result["and"]);
			Assert.AreEqual(2, (int)result["again"]);
		}

		[Test]
		public void TestGetUniqueWords()
		{
			string[] result = Utilities.GetUniqueWords(null);
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Length);

			string[] input = new string[] { "one", "one", "one", "two", "three" };
			string[] expectedResult = new string[] { "one", "three", "two" };

			result = Utilities.GetUniqueWords(input);

			Assert.IsNotNull(result);
			Assert.AreEqual(expectedResult.Length, result.Length);

			Array.Sort(expectedResult);
			Array.Sort(result);

			for (int i = 0; i < expectedResult.Length; i++)
				Assert.AreEqual(expectedResult[i], result[i]);

			string[] words = new DefaultTokenizer().Tokenize(sentence.ToLower());
			result = Utilities.GetUniqueWords(words);
			Assert.AreEqual(5, result.Length);
		}

		[Test]
		public void TestCountWords()
		{
			string[] words = { "word", "word", "word", "notword", "z", "a" };
			Array.Sort(words);
			Assert.AreEqual(3, Utilities.CountWords("word", words));

			string[] words2 = { "word", "word", "word" };
			Array.Sort(words2);
			Assert.AreEqual(3, Utilities.CountWords("word", words2));

			string[] words3 = {};
			Array.Sort(words3);
			Assert.AreEqual(0, Utilities.CountWords("word", words3));

			string[] words4 = { "notword", "z", "a" };
			Array.Sort(words4);
			Assert.AreEqual(0, Utilities.CountWords("word", words4));
		}

		[Test]
		public void TestGetSentences()
		{
			string[] result = Utilities.GetSentences(null);
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Length);

			string sentence1 = "This is sentence one";
			string sentence2 = "This is sentence two";
			string someSentences = sentence1 + "... " + sentence2 + "..";
			result = Utilities.GetSentences(someSentences);
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(sentence1, result[0].Trim());
			Assert.AreEqual(sentence2, result[1].Trim());

			someSentences = sentence1 + "! " + sentence2 + ".";
			result = Utilities.GetSentences(someSentences);
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(sentence1, result[0].Trim());
			Assert.AreEqual(sentence2, result[1].Trim());

			someSentences = sentence1 + "? " + sentence2 + ".";
			result = Utilities.GetSentences(someSentences);
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(sentence1, result[0].Trim());
			Assert.AreEqual(sentence2, result[1].Trim());
		}

//		[Test]
//		public void TestGetString()
//		{
//			Assert.AreEqual(sentence, Utilities.GetString(new ByteArra

		[Test]
		public void TestTeaching()
		{
			BayesianClassifier classifier = new BayesianClassifier();

			string[] sentence1 = new string[] {"The", "menu", "tag", "library", "manages", "the", 
								"complex", "process", "of", "creating", "menus", "in",
								"JavaScript", "The", "menu", "tag", "itself", "is", 
								"an", "abstract", "class", "that", "extends", "the", 
								"TagSupport", "class", "and", "overrides", "the", 
								"doStartTag", "and", "doEndTag", "methods.", "The", 
								"getMenu", "method,", "which", "is", "a", "template", 
								"method", "and", "should", "be", "overridden", "in", 
								"the", "subclasses,", "provides", "JavaScript", "to", 
								"add", "menu", "items", "in", "the", "menu", 
								"structure", "created", "in", "the", "doStartTag", 
								"method", "Subclasses", "of", "the", "menu", "tag", 
								"override", "the", "getMenu", "method,", "which", 
								"uses", "menu", "builders", "to", "render", "menu", 
								"data", "from", "the", "data", "source"};
								  						
			string[] sentence2 = new string[] {"I", "witness", "a", "more", "subtle", 
								"demonstration", "of", "real", "time", "physics", 
								"simulation", "at", "the", "tiny", "Palo", "Alto", 
								"office", "of", "Havok", "a", "competing", "physics", 
								"engine", "shop", "On", "the", "screen", "a", 
								"computer", "generated", "sailboat", "floats", "in", 
								"a", "stone", "lined", "pool", "of", "water", "The", 
								"company's", "genial", "Irish", "born", "cofounder", 
								"Hugh", "Reynolds", "shows", "me", "how", "to", 
								"push", "the", "boat", "with", "a", "mouse", "When", 
								"I", "nudge", "it", "air", "fills", "the", "sail", 
								"causing", "the", "ship", "to", "tilt", "leeward", 
								"Ripples", "in", "the", "water", "deflect", "off", 
								"the", "stones", "intersecting", "with", "one", 
								"another", "I", "urge", "the", "boat", "onward", 
								"and", "it", "glides", "effortlessly", "into", "the", 
								"wall", "Reynolds", "tosses", "in", "a", "handful", 
								"of", "virtual", "coins", "they", "spin", "through", 
								"the", "air,", "splash", "into", "the", "water,", 
								"and", "sink"};
								  
			string[] sentence3 = new string[] {"The", "New", "Input", "Output", "NIO", "libraries", 
								"introduced", "in", "Java", "2", "Platform", 
								"Standard", "Edition", "J2SE", "1.4", "address", 
								"this", "problem", "NIO", "uses", "a", "buffer", 
								"oriented", "model", "That", "is", "NIO", "deals", 
								"with", "data", "primarily", "in", "large", "blocks", 
								"This", "eliminates", "the", "overhead", "caused", 
								"by", "the", "stream", "model", "and", "even", "makes",
								"use", "of", "OS", "level", "facilities", "where", 
								"possible", "to", "maximize", "throughput"};
								 
			string[] sentence4 = new string[] {"As", "governments", "scramble", "to", "contain", 
								"SARS", "the", "World", "Health", "Organisation", 
								"said", "it", "was", "extending", "the", "scope", "of",
								"its", "April", "2", "travel", "alert", "to", 
								"include", "Beijing", "and", "the", "northern", 
								"Chinese", "province", "of", "Shanxi", "together", 
								"with", "Toronto", "the", "epicentre", "of", "the", 
								"SARS", "outbreak", "in", "Canada"};
								 
			string[] sentence5 = new string[] {"That", "was", "our", "worst", "problem", "I", 
								"tried", "to", "see", "it", "the", "XP", "way", "Well",
								"what", "we", "can", "do", "is", "implement", 
								"something", "I", "can't", "give", "any", "guarantees",
								"as", "to", "how", "much", "of", "it", "will", "be", 
								"implemented", "in", "a", "month", "I", "won't", 
								"even", "hazard", "a", "guess", "as", "to", "how", 
								"long", "it", "would", "take", "to", "implement", "as",
								"a", "whole", "I", "can't", "draw", "UML", "diagrams", 
								"for", "it", "or", "write", "technical", "specs", 
								"that", "would", "take", "time", "from", "coding", 
								"it", "which", "we", "can't", "afford", "Oh", "and", 
								"I", "have", "two", "kids", "I", "can't", "do", "much",
								"OverTime", "But", "I", "should", "be", "able", "to", 
								"do", "something", "simple", "that", "will", "have", 
								"very", "few", "bugs", "and", "show", "a", "working", 
								"program", "early", "and", "often"}; 		
	    

			classifier.TeachMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence1);
			classifier.TeachNonMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence2);
			classifier.TeachMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence3);
			classifier.TeachNonMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence4);
			classifier.TeachMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence5);

			Assert.IsTrue(classifier.IsMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence1));
			Assert.IsTrue(!classifier.IsMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence2));
			Assert.IsTrue(classifier.IsMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence3));
			Assert.IsTrue(!classifier.IsMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence4));
			Assert.IsTrue(classifier.IsMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, sentence5));
		}
	}
}
