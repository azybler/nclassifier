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
	public sealed class IClassifierConstants
	{
		/// <summary>
		/// Default value to use if the implementation cannot work out how well a string matches.
		/// </summary>
		public static double NEUTRAL_PROBABILITY = .5d;

		/// <summary>
		/// The minimum likelyhood that a string matches.
		/// </summary>
		public static double LOWER_BOUND = .01d;

		/// <summary>
		/// The maximum likelyhood that a string matches.
		/// </summary>
		public static double UPPER_BOUND = .99d;

		/// <summary>
		/// Default cutoff value used by default implementation of IsMatch.
		/// Any match probability greater than or equal to this value
		/// will be classified as a match.
		/// </summary>
		public static double DEFAULT_CUTOFF = .9d;
	}
}