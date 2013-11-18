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
using System.Text;

namespace NClassifier.Summarizer
{
	public class SimpleSummarizer : ISummarizer
	{
		private int FindMaxValue(ArrayList input)
		{
			input.Sort();
			return (int)input[0];
		}

		private string[] FindWordsWithFrequency(Hashtable wordFrequencies, int frequency)
		{
			if (wordFrequencies == null || frequency == 0)
				return new string[0];
			else
			{
				ArrayList results = new ArrayList();
				foreach (string word in wordFrequencies.Keys)
				{
					if (frequency == (int)wordFrequencies[word])
						results.Add(word);
				}
				return (string[])results.ToArray(typeof(string));
			}
		}

		protected ArrayList GetMostFrequentWords(int count, Hashtable wordFrequencies)
		{
			ArrayList result = new ArrayList();
			int freq = 0;
			foreach (DictionaryEntry entry in wordFrequencies)
				if ((int)entry.Value > freq)
					freq = (int)entry.Value;
			while (result.Count < count && freq > 0)
			{
				string[] words = FindWordsWithFrequency(wordFrequencies, freq);
                foreach (string word in words)
					result.Add(word);
				freq--;
			}
			return result;
		}

		public string Summarize(string input, int numberOfSentences)
		{
			// get the frequency of each word in the input
			Hashtable wordFrequencies = Utilities.GetWordFrequency(input);

			// now create a set of the X most frequent words
			ArrayList mostFrequentWords = GetMostFrequentWords(100, wordFrequencies);

			// break the input up into sentences
			string[] workingSentences = Utilities.GetSentences(input.ToLower());
			string[] actualSentences = Utilities.GetSentences(input);

			// iterate over the most frequent words, and add the first sentence
			// that includes each word to the result.
			ArrayList outputSentences = new ArrayList();
			foreach (string word in mostFrequentWords)
			{
				for (int i = 0; i < workingSentences.Length; i++)
				{
					if (workingSentences[i].IndexOf(word) >= 0)
					{
						outputSentences.Add(actualSentences[i]);
						break;
					}
					if (outputSentences.Count >= numberOfSentences)
						break;
				}
				if (outputSentences.Count >= numberOfSentences)
					break;
			}

			ArrayList reorderedOutputSentences = ReorderSentences(outputSentences, input);

			StringBuilder result = new StringBuilder();
			foreach (string sentence in reorderedOutputSentences)
			{
				if (result.Length > 0)
					result.Append(" ");
				result.Append(sentence);
				result.Append("."); // this isn't correct - it should be whatever symbol the sentence finished with
			}

			return result.ToString();
		}

		private ArrayList ReorderSentences(ArrayList outputSentences, string input)
		{
			ArrayList result = new ArrayList(outputSentences);
			result.Sort(new SimpleSummarizerComparer(input));
			return result;
		}
	}

	public class SimpleSummarizerComparer : IComparer
	{
		string _input = string.Empty;

		public SimpleSummarizerComparer(string input)
		{
			_input = input;
		}

		#region IComparer Members
		public int Compare(object x, object y)
		{
			string sentence1 = (string)x;
			string sentence2 = (string)y;

			int indexOfSentence1 = _input.IndexOf(sentence1.Trim());
			int indexOfSentence2 = _input.IndexOf(sentence2.Trim());
			int result = indexOfSentence1 - indexOfSentence2;

			return result;
		}
		#endregion

	}
}