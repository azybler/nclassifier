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

namespace NClassifier.Bayesian
{
	public interface ICategorizedWordsDataSource : IWordsDataSource
	{
		/// <summary>
		/// Gets the word probability of the specified word.
		/// </summary>
		/// <param name="category">The category to check against.</param>
		/// <param name="word">The word to calculate the probability of.</param>
		/// <returns>The word probability if the word exists, null otherwise.</returns>
		WordProbability GetWordProbability(string category, string word);

		/// <summary>
		/// Add a matching word to the data source.
		/// </summary>
		/// <param name="category">The category to add the match to.</param>
		/// <param name="word">The word that matches.</param>
		void AddMatch(string category, string word);

		/// <summary>
		/// Adds a non-matching word to the data source.
		/// </summary>
		/// <param name="category">The category to add the non-match to.</param>
		/// <param name="word">The word that does not match.</param>
		void AddNonMatch(string category, string word);
	}
}