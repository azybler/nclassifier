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
	/// Stemmer, implementing the Porter Stemming algorithm
	/// </summary>
	public class PorterStemmer
	{
		char[] _wordToStem;
		int _i, // offset into _wordToStem
			_i_end, // offset to end of stemmed word
			_j, _k;

		const int INC = 50;

		public int ResultLength { get { return _i_end; } }
		public char[] ResultBuffer { get { return _wordToStem; } }

		public PorterStemmer()
		{
			_wordToStem = new char[INC];
			_i = 0;
			_i_end = 0;
		}

		/// <summary>
		/// Add a character to the word being stemmed.  When you are finished adding
		/// characters, you can call stem() to stem the word.
		/// </summary>
		public void Add(char ch)
		{
			if (_i == _wordToStem.Length)
			{
				char[] new_b = new char[_i + INC];
				for (int c = 0; c < _i; c++)
					new_b[c] = _wordToStem[c];
				_wordToStem = new_b;
			}
			_wordToStem[_i++] = ch;
		}

		/// <summary>
		/// After a word has been stemmed, it can be retrieved by ToString(), or a
		/// reference to the internal buffer can be retrieved by the ResultBuffer
		/// and ResultLength properties (which is generally more efficient).
		/// </summary>
		public override string ToString()
		{
			return new string(_wordToStem, 0, _i_end);
		}

		private bool Cons(int i)
		{
			switch (_wordToStem[_i])
			{
				case 'a' :
				case 'e' :
				case 'i' :
				case 'o' :
				case 'u' : 
					return false;
				case 'y' :
					return (_i == 0) ? true : !Cons(_i - 1);
				default :
					return true;
			}
		}

		/// <summary>
		/// Measures the number of consonant sequences between 0 and j.  If c is a consonant
		/// sequence and v a vowel sequence, and <..> indicates arbitrary presence,
		/// 
		///		<c><v>			gives 0
		///		<c>vc<v>		gives 1
		///		<c>vCvc<v>		gives 2
		///		<c>vCvcvc<v>	gives 3
		///		....
		///		
		/// </summary>
		private int M()
		{
			int n = 0;
			int i = 0;
			while (true)
			{
				if (i > _j)
					return n;
				if (!Cons(i))
					break;
				i++;
			}
			i++;
			while (true) 
			{
				while (true) 
				{
					if (i > _j) 
					{                
						return n;
					}
					if (Cons(i)) 
					{
						break;
					}                    
					i++;
				}
				i++;
				n++;
				while (true) 
				{
					if (i > _j) 
					{                
						return n;
					}
					if (Cons(i)) 
					{
						break;
					}                    
					i++;
				}
				i++;
			}
		}

		/* VowelInStem() is true <=> 0,...j contains a vowel */
		private bool VowelInStem() 
		{
			int i;
			for (i = 0; i <= _j; i++) 
			{
				if (!Cons(i)) 
				{
					return true;
				}
			}

			return false;
		}

		/* DoubleC(j) is true <=> j,(j-1) contain a double consonant. */
		private bool DoubleC(int j) 
		{
			if (j < 1) 
			{
				return false;
			} 
			else if (_wordToStem[j] != _wordToStem[j - 1]) 
			{
				return false;
			} 
			else 
			{
				return Cons(j);
			}        
		}

		/* Cvc(i) is true <=> i-2,i-1,i has the form consonant - vowel - consonant
		   and also if the second c is not w,x or y. this is used when trying to
		   restore an e at the end of a short word. e.g.
    
			  cav(e), lov(e), hop(e), crim(e), but
			  snow, box, tray.
    
		*/
		private bool Cvc(int i) 
		{
			if (i < 2 || !Cons(i) || Cons(i - 1) || !Cons(i - 2)) 
			{
				return false;
			} 
			else 
			{
				int ch = _wordToStem[i];
				if (ch == 'w' || ch == 'x' || ch == 'y') 
				{
					return false;
				}

			}
			return true;
		}

		private bool End(string s) 
		{
			int l = s.Length;
			int o = _k - l + 1;
			if (o < 0) 
			{
				return false;
			}

			for (int i = 0; i < l; i++) 
			{
				if (_wordToStem[o + i] != s[i]) 
				{
					return false;            
				}                
			} 
			_j = _k - l;
			return true;
		}

		/* setto(s) sets (j+1),...k to the characters in the string s, readjusting
		   k. */
		private void setto(string s) 
					  {
						  int l = s.Length;
						  int o = _j + 1;
						  for (int i = 0; i < l; i++)
							  _wordToStem[o + i] = s[i];
						  _k = _j + l;
					  }

		/* r(s) is used further down. */
		private void r(string s) 
					  {
						  if (M() > 0)
							  setto(s);
					  }

		/* Step1() gets rid of plurals and -ed or -ing. e.g.
    
			   caresses  ->  caress
			   ponies    ->  poni
			   ties      ->  ti
			   caress    ->  caress
			   cats      ->  cat
    
			   feed      ->  feed
			   agreed    ->  agree
			   disabled  ->  disable
    
			   matting   ->  mat
			   mating    ->  mate
			   meeting   ->  meet
			   milling   ->  mill
			   messing   ->  mess
    
			   meetings  ->  meet
    
		*/
		private void Step1() 
					  {
						  if (_wordToStem[_k] == 's') 
						  {
							  if (End("sses"))
								  _k -= 2;
							  else if (End("ies"))
								  setto("i");
							  else if (_wordToStem[_k - 1] != 's')
								  _k--;
						  }
						  if (End("eed")) 
						  {
							  if (M() > 0)
								  _k--;
						  } 
						  else if ((End("ed") || End("ing")) && VowelInStem()) 
						  {
							  _k = _j;
							  if (End("at"))
								  setto("ate");
							  else if (End("bl"))
								  setto("ble");
							  else if (End("iz"))
								  setto("ize");
							  else if (DoubleC(_k)) 
							  {
								  _k--;
							  {
								  int ch = _wordToStem[_k];
								  if (ch == 'l' || ch == 's' || ch == 'z')
									  _k++;
							  }
							  } 
							  else if (M() == 1 && Cvc(_k))
								  setto("e");
						  }
					  }

		/* Step2() turns terminal y to i when there is another vowel in the stem. */
		private void Step2() 
					  {
						  if (End("y") && VowelInStem())
							  _wordToStem[_k] = 'i';
					  }

		/* Step3() maps double suffices to single ones. so -ization ( = -ize plus
		   -ation) maps to -ize etc. note that the string before the suffix must give
		   m() > 0. */

		private void Step3() 
		{
			if (_k == 0)
				return; /* For Bug 1 */
			switch (_wordToStem[_k - 1]) 
			{
				case 'a' :
					if (End("ational")) 
					{
						r("ate");
						break;
					}
					if (End("tional")) 
					{
						r("tion");
						break;
					}
					break;
				case 'c' :
					if (End("enci")) 
					{
						r("ence");
						break;
					}
					if (End("anci")) 
					{
						r("ance");
						break;
					}
					break;
				case 'e' :
					if (End("izer")) 
					{
						r("ize");
						break;
					}
					break;
				case 'l' :
					if (End("bli")) 
					{
						r("ble");
						break;
					}
					if (End("alli")) 
					{
						r("al");
						break;
					}
					if (End("entli")) 
					{
						r("ent");
						break;
					}
					if (End("eli")) 
					{
						r("e");
						break;
					}
					if (End("ousli")) 
					{
						r("ous");
						break;
					}
					break;
				case 'o' :
					if (End("ization")) 
					{
						r("ize");
						break;
					}
					if (End("ation")) 
					{
						r("ate");
						break;
					}
					if (End("ator")) 
					{
						r("ate");
						break;
					}
					break;
				case 's' :
					if (End("alism")) 
					{
						r("al");
						break;
					}
					if (End("iveness")) 
					{
						r("ive");
						break;
					}
					if (End("fulness")) 
					{
						r("ful");
						break;
					}
					if (End("ousness")) 
					{
						r("ous");
						break;
					}
					break;
				case 't' :
					if (End("aliti")) 
					{
						r("al");
						break;
					}
					if (End("iviti")) 
					{
						r("ive");
						break;
					}
					if (End("biliti")) 
					{
						r("ble");
						break;
					}
					break;
				case 'g' :
					if (End("logi")) 
					{
						r("log");
						break;
					}
					break;
			}
		}

		/* Step4() deals with -ic-, -full, -ness etc. similar strategy to Step3. */

		private void Step4() 
		{
			switch (_wordToStem[_k]) 
			{
				case 'e' :
					if (End("icate")) 
					{
						r("ic");
						break;
					}
					if (End("ative")) 
					{
						r("");
						break;
					}
					if (End("alize")) 
					{
						r("al");
						break;
					}
					break;
				case 'i' :
					if (End("iciti")) 
					{
						r("ic");
						break;
					}
					break;
				case 'l' :
					if (End("ical")) 
					{
						r("ic");
						break;
					}
					if (End("ful")) 
					{
						r("");
						break;
					}
					break;
				case 's' :
					if (End("ness")) 
					{
						r("");
						break;
					}
					break;
			}
		}

		/* Step5() takes off -ant, -ence etc., in context <c>vCvc<v>. */

		private void Step5() 
		{
			if (_k == 0)
				return; /* for Bug 1 */
			switch (_wordToStem[_k - 1]) 
			{
				case 'a' :
					if (End("al"))
						break;
					return;
				case 'c' :
					if (End("ance"))
						break;
					if (End("ence"))
						break;
					return;
				case 'e' :
					if (End("er"))
						break;
					return;
				case 'i' :
					if (End("ic"))
						break;
					return;
				case 'l' :
					if (End("able"))
						break;
					if (End("ible"))
						break;
					return;
				case 'n' :
					if (End("ant"))
						break;
					if (End("ement"))
						break;
					if (End("ment"))
						break;
					/* element etc. not stripped before the m */
					if (End("ent"))
						break;
					return;
				case 'o' :
					if (End("ion") && _j >= 0 && (_wordToStem[_j] == 's' || _wordToStem[_j] == 't'))
						break;
					/* j >= 0 fixes Bug 2 */
					if (End("ou"))
						break;
					return;
					/* takes care of -ous */
				case 's' :
					if (End("ism"))
						break;
					return;
				case 't' :
					if (End("ate"))
						break;
					if (End("iti"))
						break;
					return;
				case 'u' :
					if (End("ous"))
						break;
					return;
				case 'v' :
					if (End("ive"))
						break;
					return;
				case 'z' :
					if (End("ize"))
						break;
					return;
				default :
					return;
			}
			if (M() > 1)
				_k = _j;
		}

		/* Step6() removes a -e if m() > 1. */

		private void Step6() 
		{
			_j = _k;
			if (_wordToStem[_k] == 'e') 
			{
				int a = M();
				if (a > 1 || a == 1 && !Cvc(_k - 1))
					_k--;
			}
			if (_wordToStem[_k] == 'l' && DoubleC(_k) && M() > 1)
				_k--;
		}

		/** Stem the word placed into the Stemmer buffer through calls to add().
		 * Returns true if the stemming process resulted in a word different
		 * from the input.  You can retrieve the result with
		 * getResultLength()/getResultBuffer() or tostring().
		 */
		public void stem() 
		{
			_k = _i - 1;
			if (_k > 1) 
			{
				Step1();
				Step2();
				Step3();
				Step4();
				Step5();
				Step6();
			}
			_i_end = _k + 1;
			_i = 0;
		}
	}
}