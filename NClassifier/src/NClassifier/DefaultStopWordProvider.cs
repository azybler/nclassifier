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

namespace NClassifier
{
	public class DefaultStopWordProvider : IStopWordProvider
	{
		#region Fields
		string[] _stopWords = 
		{
			"a", "and", "the", "me", "i", "of", "if", "it",  
			"is", "they", "there", "but", "or", "to", "this", "you", 
			"in", "your", "on", "for", "as", "are", "that", "with",
			"have", "be", "at", "or", "was", "so", "out", "not", "an"
		};

		string[] _sortedStopWords = null;
		#endregion

		public string[] StopWords { get { return _stopWords; } }

		public DefaultStopWordProvider()
		{
			_sortedStopWords = _stopWords;
			Array.Sort(_sortedStopWords);
		}

		public bool IsStopWord(string word)
		{
			if (word == null || word == string.Empty)
				return false;
			else
				return Array.BinarySearch(_sortedStopWords, word.ToLower()) >= 0;
		}
	}
}