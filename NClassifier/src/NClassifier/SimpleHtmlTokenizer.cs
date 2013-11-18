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
using System.Text.RegularExpressions;

namespace NClassifier
{
	/// <summary>
	/// Simple HTML tokenizer.  Its goal is to tokenize words that would be displayed in a normal
	/// web browser.
	/// </summary>
	/// <remarks>
	/// It does not handle meta tags, alt or text attributes, but it does remove CSS style
	/// definitions and javascript code.
	/// 
	/// It handles entity reference by replacing them with a space. This can be overridden.
	/// </remarks>
	public class SimpleHtmlTokenizer : DefaultTokenizer
	{
		/// <summary>
		/// Constructor uses BREAK_ON_WORD_BREAKS tokenizer config by default.
		/// </summary>
		public SimpleHtmlTokenizer() : base() {}

		public SimpleHtmlTokenizer(int tokenizerConfig) : base(tokenizerConfig) {}

		public SimpleHtmlTokenizer(string regularExpression) : base(regularExpression) {}

		/// <summary>
		/// Replaces entity references with spaces.
		/// </summary>
		/// <param name="contentsWithUnresolvedEntityReferences">The contents with the entity references.</param>
		/// <returns>The contents with the entities replaced with spaces.</returns>
		public string ResolveEntities(string contentsWithUnresolvedEntityReferences)
		{
			if (contentsWithUnresolvedEntityReferences == null)
				throw new ArgumentException("Cannot pass null.", "contentsWithUnresolvedEntityReferences");
			return Regex.Replace(contentsWithUnresolvedEntityReferences, "&.{2,8};", " ");
		}

		public override string[] Tokenize(string input)
		{
			Stack stack = new Stack();
			Stack tagStack = new Stack();

			// iterate over the input string and parse find text that would be displayed
			char[] chars = input.ToCharArray();

            StringBuilder result = new StringBuilder();
			StringBuilder currentTagName = new StringBuilder();
			for (int i = 0; i < chars.Length; i++)
			{
				switch (chars[i])
				{
					case '<' : 
						stack.Push(true);
						currentTagName = new StringBuilder();
						break;
					case '>' :
						stack.Pop();
						if (currentTagName != null)
						{
							string currentTag = currentTagName.ToString();
							if (currentTag.StartsWith("/"))
								tagStack.Pop();
							else
								tagStack.Push(currentTag.ToLower());
						}
						break;
					default :
						if (stack.Count == 0)
						{
							string currentTag = (string)tagStack.Peek();
							// ignore everything inside <script></script> or <style></style>
							if (currentTag != null)
							{
								if (!(currentTag.StartsWith("script") || currentTag.StartsWith("style")))
									result.Append(chars[i]);
							}
							else
								result.Append(chars[i]);
						}
						else
							currentTagName.Append(chars[i]);
						break;
				}
			}
			return base.Tokenize(ResolveEntities(result.ToString()).Trim());
		}
	}
}