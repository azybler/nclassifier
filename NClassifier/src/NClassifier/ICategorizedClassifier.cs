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
	/// <summary>
	/// Defines an interface for a categorized classifier.
	/// </summary>
	public interface ICategorizedClassifier : IClassifier
	{
		/// <summary>
		/// Determines the probability that the specified string matches
		/// a criteria for a given category.
		/// </summary>
		/// <param name="category">The category to check against.</param>
		/// <param name="input">The string to classify.</param>
		/// <returns>The likelihood that this string is a match for this classifier.  1 means 100% likely.</returns>
		double Classify(string category, string input);

		/// <summary>
		/// Determines if a string matches a criteria for a given category.
		/// </summary>
		/// <param name="category">The category to check against.</param>
		/// <param name="input">The string to classify.</param>
		/// <returns>True if the input string has a probability >= the cutoff probability of matching.</returns>
		bool IsMatch(string category, string input);
	}
}
