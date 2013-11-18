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
	/// Defines an interface for the classification of strings.
	/// </summary>
	public interface IClassifier
	{
		/// <summary>
		/// Sets the cutoff below which the input is not considered a match.
		/// </summary>
		double MatchCutoff { get; set; }

		/// <summary>
		/// Determines the probability of a string matching the criteria.
		/// </summary>
		/// <param name="input">The string to classify.</param>
		/// <returns>The likelyhood that this string is a match.  1 means 100% likely.</returns>
		double Classify(string input);

		/// <summary>
		/// Determines if a string matches a criteria.
		/// </summary>
		/// <param name="input">The string to classify.</param>
		/// <returns>Returns true if the input string has a probability >= the cutoff probability of matching.</returns>
		bool IsMatch(string input);

		/// <summary>
		/// Convenience method which takes a match probability and checks if it would be classified as a match.
		/// </summary>
		/// <param name="matchProbability">The match probability to check.</param>
		/// <returns>True if match, false otherwise.</returns>
		bool IsMatch(double matchProbability);
	}
}
