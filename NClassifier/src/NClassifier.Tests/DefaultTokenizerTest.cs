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
	public class DefaultTokenizerTest
	{
		[Test]
		public void TestConstructors()
		{
			ITokenizer tok = null;

			try
			{
				tok = new DefaultTokenizer(null);
				Assert.Fail("Shouldn't be able to set a tokenizer of null.");
			}
			catch
			{
				Assert.IsTrue(true);
			}

			tok = new DefaultTokenizer(string.Empty);
			tok = new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WHITESPACE);
			tok = new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WORD_BREAKS);

			try
			{
				tok = new DefaultTokenizer(43);
				Assert.Fail("Shouldn't be able to set a tokenizer of type 43.");
			}
			catch
			{
				Assert.IsTrue(true);
			}
		}

		[Test]
		public void TestTokenize()
		{
			ITokenizer tok = null;
			string[] words = null;

			tok = new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WHITESPACE);
			words = tok.Tokenize("My very,new string!");

			Assert.AreEqual(3, words.Length);
			Assert.AreEqual("My", words[0]);
			Assert.AreEqual("very,new", words[1]);
			Assert.AreEqual("string!", words[2]);

			tok = new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WORD_BREAKS);
			words = tok.Tokenize("My very,new-string!and/more(NIO)peter's 1.4");

			Assert.AreEqual(11, words.Length);
			Assert.AreEqual("My", words[0]);
			Assert.AreEqual("very", words[1]);
			Assert.AreEqual("new", words[2]);
			Assert.AreEqual("string", words[3]);
			Assert.AreEqual("and", words[4]);
			Assert.AreEqual("more", words[5]);
			Assert.AreEqual("NIO", words[6]);

			//TODO shouldn't this be "peter's" instead of "peter" and "s"?
			Assert.AreEqual("peter", words[7]);
			Assert.AreEqual("s", words[8]);

			//TODO shouldn't this be "1.4" instead of "1" and "4"?
			Assert.AreEqual("1", words[9]);
			Assert.AreEqual("4", words[10]);
		}
	}
}