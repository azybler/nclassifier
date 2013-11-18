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
using System.Diagnostics;
using System.Data;
using System.Data.Odbc;

namespace NClassifier.Bayesian
{
	public class OdbcWordsDataSource : ICategorizedWordsDataSource
	{
		protected IDbConnectionManager connectionManager;
		protected string tableName = "WordProbability";
		protected string wordColumn = "Word";
		protected string categoryColumn = "Category";
		protected string matchCountColumn = "Matches";
		protected string nonMatchCountColumn = "NonMatches";

		/// <summary>
		/// Create a SqlWordsDataSource using the DEFAULT_CATEGORY ("DEFAULT")
		/// </summary>
		/// <param name="conMgr">The connection manager to use.</param>
		/// <param name="aTableName">The name of the table storing word probabilities.</param>
		/// <param name="aWordColumn">The word column.</param>
		/// <param name="aCategoryColumn">The category column.</param>
		/// <param name="aMatchCountColumn">The match count column.</param>
		/// <param name="aNonMatchCountColumn">The non-match count column.</param>
		public OdbcWordsDataSource(IDbConnectionManager conMgr, string aTableName, string aWordColumn, string aCategoryColumn, 
			string aMatchCountColumn, string aNonMatchCountColumn) : this(conMgr)
		{
			connectionManager = conMgr;
			tableName = aTableName;
			wordColumn = aWordColumn;
			categoryColumn = aCategoryColumn;
			matchCountColumn = aMatchCountColumn;
			nonMatchCountColumn = aNonMatchCountColumn;
		}

		public OdbcWordsDataSource(IDbConnectionManager conMgr)
		{
			connectionManager = conMgr;
			CreateTable();
		}

		public WordProbability GetWordProbability(string category, string word)
		{
			WordProbability wp = null;
			int matchingCount = 0;
			int nonMatchingCount = 0;

			OdbcConnection connection = null;
			try
			{
				connection = (OdbcConnection)connectionManager.GetConnection();
				IDbCommand command = new OdbcCommand("SELECT " + matchCountColumn + ", " + nonMatchCountColumn + " FROM " + tableName + " WHERE " + wordColumn + " = ? AND " + categoryColumn + " = ?", connection);
				command.Parameters.Add(new OdbcParameter("@Word", OdbcType.VarChar, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Proposed, word));
				command.Parameters.Add(new OdbcParameter("@Category", OdbcType.VarChar, 20, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Proposed, category));
				IDataReader reader = command.ExecuteReader();
				if (reader.Read())
				{
					matchingCount = (int)reader[matchCountColumn];
					nonMatchingCount = (int)reader[nonMatchCountColumn];
				}
				reader.Close();
				wp = new WordProbability(word, matchingCount, nonMatchingCount);
			}
			catch (Exception ex)
			{
				throw new WordsDataSourceException("Problem obtaining WordProbability from database.", ex);
			}
			finally
			{
				if (connection != null)
				{
					try
					{
						connectionManager.ReturnConnection(connection);
					}
					catch {}
				}
			}

			Debug.WriteLine("GetWordProbability() WordProbability loaded [" + wp + "]");

			return wp;
		}

		public WordProbability GetWordProbability(string word)
		{
			return GetWordProbability(ICategorizedClassifierConstants.DEFAULT_CATEGORY, word);
		}

		private void UpdateWordProbability(string category, string word, bool isMatch)
		{
			string fieldName = isMatch ? matchCountColumn : nonMatchCountColumn;

			// truncate word at 255 characters
			if (word.Length > 255)
				word = word.Substring(0, 255);

			OdbcConnection connection = null;
			try
			{
				connection = (OdbcConnection)connectionManager.GetConnection();
				IDbCommand command = null;
				IDataReader reader = null;

				// see if the word exists in the table
				command = new OdbcCommand("SELECT * FROM " + tableName + " WHERE " + wordColumn + " = ? AND " + categoryColumn + " = ?", connection);
				command.Parameters.Add(new OdbcParameter("@Word", OdbcType.VarChar, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Proposed, word));
				command.Parameters.Add(new OdbcParameter("@Category", OdbcType.VarChar, 20, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Proposed, category));
				reader = command.ExecuteReader(CommandBehavior.SingleResult);

				if (!reader.Read()) // word is not in table, so insert the word
				{
					reader.Close();
					command = new OdbcCommand("INSERT " + tableName + " (" + wordColumn + ", " + categoryColumn + ") VALUES (?, ?)", connection);
					command.Parameters.Add(new OdbcParameter("@Word", OdbcType.VarChar, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Proposed, word));
					command.Parameters.Add(new OdbcParameter("@Category", OdbcType.VarChar, 20, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Proposed, category));
					command.ExecuteNonQuery();
				}
				else
					reader.Close();

				// update the word count
				command = new OdbcCommand("UPDATE " + tableName + " SET " + fieldName + " = " + fieldName + " + 1 WHERE " + wordColumn + " = ? AND " + categoryColumn + " = ?", connection);
				command.Parameters.Add(new OdbcParameter("@Word", OdbcType.VarChar, 255, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Proposed, word));
				command.Parameters.Add(new OdbcParameter("@Category", OdbcType.VarChar, 20, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Proposed, category));
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw new WordsDataSourceException("Problem updating WordProbability.", ex);
			}
			finally
			{
				if (connection != null)
				{
					try
					{
						connectionManager.ReturnConnection(connection);
					}
					catch {}
				}
			}
		}

        public void AddMatch(string category, string word)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			UpdateWordProbability(category, word, true);
		}

		public void AddMatch(string word)
		{
			UpdateWordProbability(ICategorizedClassifierConstants.DEFAULT_CATEGORY, word, true);
		}

		public void AddNonMatch(string category, string word)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			UpdateWordProbability(category, word, false);
		}

		public void AddNonMatch(string word)
		{
			UpdateWordProbability(ICategorizedClassifierConstants.DEFAULT_CATEGORY, word, false);
		}

		/// <summary>
		/// Create the table if it does not already exist.
		/// </summary>
		private void CreateTable()
		{
			OdbcConnection connection = null;
			try
			{
				connection = (OdbcConnection)connectionManager.GetConnection();
				// TODO figure out better way to see if table exists
				OdbcCommand command = new OdbcCommand("SELECT TOP 1 * FROM " + tableName, connection);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					command = new OdbcCommand("CREATE TABLE [" + tableName + "] "
						+ "([" + wordColumn + "] VARCHAR(255) NOT NULL, "
						+ "[" + categoryColumn + "] VARCHAR(20) NOT NULL, "
						+ "[" + matchCountColumn + "] INT DEFAULT 0 NOT NULL, "
						+ "[" + nonMatchCountColumn + "] INT DEFAULT 0 NOT NULL, "
						+ "PRIMARY KEY([" + wordColumn + "], [" + categoryColumn + "]))", connection);
					command.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				throw new WordsDataSourceException("Problem creating table.", ex);
			}
			finally
			{
				if (connection != null)
				{
					try
					{
						connectionManager.ReturnConnection(connection);
					}
					catch (Exception ex) {}
				}
			}
		}
	}
}