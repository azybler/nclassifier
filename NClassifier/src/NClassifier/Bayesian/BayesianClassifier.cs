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

namespace NClassifier.Bayesian
{
	/// <summary>
	/// An implementation of IClassifer based on Bayes' algorithm.
	/// </summary>
	public class BayesianClassifier : AbstractClassifier, ITrainableClassifier
	{
		#region Fields
		IWordsDataSource _wordsData;
		ITokenizer _tokenizer;
		IStopWordProvider _stopWordProvider;
		bool _isCaseSensitive = false;
		#endregion

		#region Properties
		public bool IsCaseSensitive { get { return _isCaseSensitive; } set { _isCaseSensitive = value; } }
		public IWordsDataSource WordsDataSource { get { return _wordsData; } }
		public ITokenizer Tokenizer { get { return _tokenizer; } }
		public IStopWordProvider StopWordProvider { get { return _stopWordProvider; } }
		#endregion

		#region Constructors
		public BayesianClassifier() : this(new SimpleWordsDataSource(), new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WORD_BREAKS)) {}

		public BayesianClassifier(IWordsDataSource wd) : this(wd, new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WORD_BREAKS)) {}

		public BayesianClassifier(IWordsDataSource wd, ITokenizer tokenizer) : this(wd, tokenizer, new DefaultStopWordProvider()) {}

		public BayesianClassifier(IWordsDataSource wd, ITokenizer tokenizer, IStopWordProvider swp)
		{
			if (wd == null)
				throw new ArgumentNullException("IWordsDataSource cannot be null.");
			_wordsData = wd;

			if (tokenizer == null)
				throw new ArgumentNullException("ITokenizer cannot be null.");
			_tokenizer = tokenizer;

			if (swp == null)
				throw new ArgumentNullException("IStopWordProvider cannot be null.");
			_stopWordProvider = swp;
		}
		#endregion

		public bool IsMatch(string category, string input)
		{
			return IsMatch(category, _tokenizer.Tokenize(input));
		}

		public override double Classify(string input)
		{
			return Classify(ICategorizedClassifierConstants.DEFAULT_CATEGORY, input);
		}

		public double Classify(string category, string input)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			if (input == null)
				throw new ArgumentNullException("Input cannot be null.");

			CheckCategoriesSupported(category);

			return Classify(category, _tokenizer.Tokenize(input));
		}

		public double Classify(string category, string[] words)
		{
			WordProbability[] wps = CalcWordsProbability(category, words);
			return NormalizeSignificance(CalculateOverallProbability(wps));
		}

		public void TeachMatch(string input)
		{
			TeachMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, input);
		}

		public void TeachNonMatch(string input)
		{
			TeachNonMatch(ICategorizedClassifierConstants.DEFAULT_CATEGORY, input);
		}

		public void TeachMatch(string category, string input)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			if (input == null)
				throw new ArgumentNullException("Input cannot be null.");

			CheckCategoriesSupported(category);

			TeachMatch(category, _tokenizer.Tokenize(input));
		}

		public void TeachNonMatch(string category, string input)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			if (input == null)
				throw new ArgumentNullException("Input cannot be null.");

			CheckCategoriesSupported(category);

			TeachNonMatch(category, _tokenizer.Tokenize(input));
		}

		public bool IsMatch(string category, string[] input)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			if (input == null)
				throw new ArgumentNullException("Input cannot be null.");

			CheckCategoriesSupported(category);

			double matchProbability = Classify(category, input);

			return (matchProbability >= cutoff);
		}

		public void TeachMatch(string category, string[] words)
		{
			bool categorize = false;
			if (_wordsData is ICategorizedWordsDataSource)
				categorize = true;
			for (int i = 0; i <= words.Length - 1; i++)
			{
				if (IsClassifiableWord(words[i]))
				{
					if (categorize)
						((ICategorizedWordsDataSource)_wordsData).AddMatch(category, TransformWord(words[i]));
					else
						_wordsData.AddMatch(TransformWord(words[i]));
				}
			}
		}

		public void TeachNonMatch(string category, string[] words)
		{
			bool categorize = false;
			if (_wordsData is ICategorizedWordsDataSource)
				categorize = true;
			for (int i = 0; i <= words.Length - 1; i++)
			{
				if (IsClassifiableWord(words[i]))
				{
					if (categorize)
						((ICategorizedWordsDataSource)_wordsData).AddNonMatch(category, TransformWord(words[i]));
					else
						_wordsData.AddNonMatch(TransformWord(words[i]));
				}
			}
		}

		/// <summary>
		/// Allows transformations to be done to the given word.
		/// </summary>
		/// <param name="word">The word to transform.</param>
		/// <returns>The transformed word.</returns>
		public string TransformWord(string word)
		{
			if (word != null)
			{
				if (!_isCaseSensitive)
					return word.ToLower();
				else
					return word;
			}
			else
				throw new ArgumentNullException("Word cannot be null.");
		}

		public double CalculateOverallProbability(WordProbability[] wps)
		{
			if (wps == null || wps.Length == 0)
				return IClassifierConstants.NEUTRAL_PROBABILITY;

			// we need to calculate xy/(xy + z) where z = (1 - x)(1 - y)
			
			// first calculate z and xy
			double z = 0d;
			double xy = 0d;
			for (int i = 0; i < wps.Length; i++)
			{
				if (z == 0)
					z = (1 - wps[i].Probability);
				else
					z = z * (1 - wps[i].Probability);

				if (xy == 0)
					xy = wps[i].Probability;
				else
					xy = xy * wps[i].Probability;                    
			}

			double numerator = xy;
			double denominator = xy + z;

			return numerator / denominator;
		}

		private WordProbability[] CalcWordsProbability(string category, string[] words)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");

			bool categorize = false;
			if (_wordsData is ICategorizedWordsDataSource)
				categorize = true;

			CheckCategoriesSupported(category);
			
			if (words == null)
				return new WordProbability[0];
			else
			{
				ArrayList wps = new ArrayList();
				for (int i = 0; i < words.Length; i++)
				{
					if (IsClassifiableWord(words[i]))
					{
						WordProbability wp = null;
						if (categorize)
							wp = ((ICategorizedWordsDataSource)_wordsData).GetWordProbability(category, TransformWord(words[i]));
						else
							wp = _wordsData.GetWordProbability(TransformWord(words[i]));

						if (wp != null)
							wps.Add(wp);
					}
				}
				return (WordProbability[])wps.ToArray(typeof(WordProbability));
			}
		}

		private void CheckCategoriesSupported(string category)
		{
			// if the category is not the default
			if (ICategorizedClassifierConstants.DEFAULT_CATEGORY != category)
				if (!(_wordsData is ICategorizedWordsDataSource))
					throw new ArgumentException("Word Data Source does not support non-default categories.");
		}

		private bool IsClassifiableWord(string word)
		{
			if (word == null || word == string.Empty || _stopWordProvider.IsStopWord(word))
				return false;
			else
				return true;
		}

		public static double NormalizeSignificance(double sig)
		{
			if (IClassifierConstants.UPPER_BOUND < sig)
				return IClassifierConstants.UPPER_BOUND;
			else if (IClassifierConstants.LOWER_BOUND > sig)
				return IClassifierConstants.LOWER_BOUND;
			else
				return sig;
		}
	}
}