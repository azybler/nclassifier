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
using System.Diagnostics;
using NUnit.Framework;
using NClassifier.Bayesian;

namespace NClassifier.Tests.Bayesian
{
	[TestFixture]
	public class WordProbabilityTest
	{
		[Test]
		public void TestAccessors()
		{
			WordProbability wp = null;

			wp = new WordProbability(string.Empty, 0.96d);
			Assert.AreEqual(string.Empty, wp.Word);
			try
			{
				Assert.AreEqual(0, wp.MatchingCount);
				Assert.Fail("Shouldn't be able to obtain matching count when we haven't set them.");
			}
			catch {}

			try
			{
				Assert.AreEqual(0, wp.NonMatchingCount);
				Assert.Fail("Shouldn't be able to obtain non-matchin count when we haven't set them.");
			}
			catch {}
			Assert.AreEqual(0.96d, wp.Probability, 0);

			wp = new WordProbability("aWord", 10, 30);
			Assert.AreEqual("aWord", wp.Word);
			Assert.AreEqual(10, wp.MatchingCount);
			Assert.AreEqual(30, wp.NonMatchingCount);
			Assert.AreEqual(0.25d, wp.Probability, 0d);

			try
			{
				wp.MatchingCount = -10;
				Assert.Fail("Shouldn't be able to set negative MatchingCount.");
			}
			catch {}

			try
			{
				wp.NonMatchingCount = -10;
				Assert.Fail("Shouldn't be able to set negative NonMatchingCount.");
			}
			catch {}
		}

		[Test]
		public void TestCalculateProbability()
		{
			WordProbability wp = null;
			
			wp = new WordProbability(string.Empty, 10, 10);
			Assert.AreEqual(IClassifierConstants.NEUTRAL_PROBABILITY, wp.Probability, 0);

			wp = new WordProbability(string.Empty, 20, 10);
			Assert.AreEqual(0.66, wp.Probability, 0.01);

			wp = new WordProbability(string.Empty, 30, 10);
			Assert.AreEqual(0.75, wp.Probability, 0);

			wp = new WordProbability(string.Empty, 10, 20);
			Assert.AreEqual(0.33, wp.Probability, 0.01);

			wp = new WordProbability(string.Empty, 10, 30);
			Assert.AreEqual(0.25, wp.Probability, 0);

			wp = new WordProbability(string.Empty, 10, 0);
			Assert.AreEqual(IClassifierConstants.UPPER_BOUND, wp.Probability, 0);

			wp = new WordProbability(string.Empty, 100, 1);
			Assert.AreEqual(IClassifierConstants.UPPER_BOUND, wp.Probability, 0);

			wp = new WordProbability(string.Empty, 1000, 1);
			Assert.AreEqual(IClassifierConstants.UPPER_BOUND, wp.Probability, 0);

			wp = new WordProbability(string.Empty, 0, 10);
			Assert.AreEqual(IClassifierConstants.LOWER_BOUND, wp.Probability, 0);

			wp = new WordProbability(string.Empty, 1, 100);
			Assert.AreEqual(IClassifierConstants.LOWER_BOUND, wp.Probability, 0);

			wp = new WordProbability(string.Empty, 1, 1000);
			Assert.AreEqual(IClassifierConstants.LOWER_BOUND, wp.Probability, 0);
		}

		[Test]
		public void TestComparer()
		{
			string method = "TestComparer() ";

			WordProbability wp = null;
			WordProbability wp2 = null;

			wp = new WordProbability("a", 0, 0);
			wp2 = new WordProbability("b", 0, 0);

			try
			{
				wp.CompareTo(new object());
				Assert.Fail("Shouldn't be able to compare to objects other than WordProbability.");
			}
			catch {}

			Debug.WriteLine(method + "wp.Probability " + wp.Probability);
			Debug.WriteLine(method + "wp2.Probability " + wp2.Probability);

			Assert.IsTrue(wp.CompareTo(wp2) < 0);
			Assert.IsTrue(wp2.CompareTo(wp) > 0);
		}

		[Test]
		public void TestMatchingAndNonMatchingCountRollover()
		{
			WordProbability wp = new WordProbability("aWord", long.MaxValue, long.MaxValue);
			try
			{
				wp.RegisterMatch();
				Assert.Fail("Should detect rollover.");
			}
			catch {}

			try
			{
				wp.RegisterNonMatch();
				Assert.Fail("Should detect rollover.");
			}
			catch {}
		}
	}
}