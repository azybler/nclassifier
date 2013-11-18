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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NClassifier
{
	public class Utilities
	{
		public static Hashtable GetWordFrequency(string input)
		{
			return GetWordFrequency(input, false);
		}

		public static Hashtable GetWordFrequency(string input, bool caseSensitive)
		{
			return GetWordFrequency(input, caseSensitive, new DefaultTokenizer(), new DefaultStopWordProvider());
		}

		/// <summary>
		/// Gets a Hashtable of words and integers representing the number of each word.
		/// </summary>
		/// <param name="input">The string to get the word frequency of.</param>
		/// <param name="caseSensitive">True if words should be treated as separate if they have different casing.</param>
		/// <param name="tokenizer">A instance of ITokenizer.</param>
		/// <param name="stopWordProvider">An instance of IStopWordProvider.</param>
		/// <returns></returns>
		public static Hashtable GetWordFrequency(string input, bool caseSensitive, ITokenizer tokenizer, IStopWordProvider stopWordProvider)
		{
			string convertedInput = input;
			if (!caseSensitive)
				convertedInput = input.ToLower();

			string[] words = tokenizer.Tokenize(convertedInput);
			Array.Sort(words);

			string[] uniqueWords = GetUniqueWords(words);

			Hashtable result = new Hashtable();
			for (int i = 0; i < uniqueWords.Length; i++)
			{
				if (stopWordProvider == null || (IsWord(uniqueWords[i]) && !stopWordProvider.IsStopWord(uniqueWords[i])))
				{
					if (result.ContainsKey(uniqueWords[i]))
						result[uniqueWords[i]] = (int)result[uniqueWords[i]] + CountWords(uniqueWords[i], words);
					else
						result.Add(uniqueWords[i], CountWords(uniqueWords[i], words));
				}
			}

			return result;
		}

		public static bool IsWord(string word)
		{
			if (word != null && word.Trim() != string.Empty)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Find all unique words in an array of words.
		/// </summary>
		/// <param name="input">An array of strings.</param>
		/// <returns>An array of all unique strings.  Order is not guaranteed.</returns>
		public static string[] GetUniqueWords(string[] input)
		{
			if (input == null)
				return new string[0];
			else
			{
				ArrayList result = new ArrayList();
				for (int i = 0; i < input.Length; i++)
					if (!result.Contains(input[i]))
						result.Add(input[i]);
				return (string[])result.ToArray("".GetType());
			}
		}

		/// <summary>
		/// Count how many times a word appears in an array of words.
		/// </summary>
		/// <param name="word">The word to count.</param>
		/// <param name="words">A non-null array of words.</param>
		public static int CountWords(string word, string[] words)
		{
			// find the index of one of the items in the array
			int itemIndex = Array.BinarySearch(words, word);

			// iterate backwards until we find the first match
			if (itemIndex > 0)
				while (itemIndex > 0 && words[itemIndex] == word)
					itemIndex--;

			// now itemIndex is one item before the start of the words
			int count = 0;
			while (itemIndex < words.Length && itemIndex >= 0)
			{
				if (words[itemIndex] == word)
					count++;

				itemIndex++;

				if (itemIndex < words.Length)
					if (words[itemIndex] != word)
						break;
			}

			return count;
		}

		/// <summary>
		/// Gets an array of sentences.
		/// </summary>
		/// <param name="input">A string that contains sentences.</param>
		/// <returns>An array of strings, each element containing a sentence.</returns>
		public static string[] GetSentences(string input)
		{
			if (input == null)
				return new string[0];
			else
			{
				// split on a ".", a "!", a "?" followed by a space or EOL
				// the original Java regex was (\.|!|\?)+(\s|\z)
				string[] result = Regex.Split(input, @"(?:\.|!|\?)+(?:\s+|\z)");

				// hacky... doing this to pass the unit tests
				ArrayList list = new ArrayList();
				foreach (string s in result)
					if (s.Length > 0)
						list.Add(s);
				return (string[])list.ToArray(typeof(string));
			}
		}

      //*********************************************************************************************************
      // © 2013 jakemdrew.com. All rights reserved. 
      // This source code is licensed under The GNU General Public License (GPLv3):  
      // http://opensource.org/licenses/gpl-3.0.html
      //*********************************************************************************************************

      //*********************************************************************************************************
      //makeNgrams - Example n-gram creator.
      //Created By - Jake Drew 
      //Version -    1.0, 04/22/2013
      //*********************************************************************************************************
      public static IEnumerable<string> makeNgrams(string text, int nGramSize)
      {
         if (nGramSize == 0) throw new Exception("nGram size was not set");

         StringBuilder nGram = new StringBuilder();
         Queue<int> wordLengths = new Queue<int>();

         int wordCount = 0;
         int lastWordLen = 0;

         //append the first character, if valid.
         //avoids if statement for each for loop to check i==0 for before and after vars.
         if (text != "" && char.IsLetterOrDigit(text[0]))
         {
            nGram.Append(text[0]);
            lastWordLen++;
         }

         //generate ngrams
         for (int i = 1; i < text.Length - 1; i++)
         {
            char before = text[i - 1];
            char after = text[i + 1];

            if (char.IsLetterOrDigit(text[i])
                    ||
               //keep all punctuation that is surrounded by letters or numbers on both sides.
                    (text[i] != ' '
                    && (char.IsSeparator(text[i]) || char.IsPunctuation(text[i]))
                    && (char.IsLetterOrDigit(before) && char.IsLetterOrDigit(after))
                    )
                )
            {
               nGram.Append(text[i]);
               lastWordLen++;
            }
            else
            {
               if (lastWordLen > 0)
               {
                  wordLengths.Enqueue(lastWordLen);
                  lastWordLen = 0;
                  wordCount++;

                  if (wordCount >= nGramSize)
                  {
                     yield return nGram.ToString();
                     nGram.Remove(0, wordLengths.Dequeue() + 1);
                     wordCount -= 1;
                  }

                  nGram.Append(" ");
               }
            }
         }
         //nGram.Append(text.Last());
         nGram.Append(text);
         yield return nGram.ToString();
      }
	}
}