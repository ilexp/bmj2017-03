using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class Message
	{
		private static readonly Regex RegexWordBounds = new Regex(@"\s+", RegexOptions.Compiled);

		private string rawText;
		private string[] words;
		private LargeVector[] vectors;

		public Message(string text, VectorDataStore vectorData)
		{
			this.rawText = text;

			// Make sure punctuation counts as individual words
			text = this.WordifyPunctuation(text);

			// Split the input text into words
			this.words = RegexWordBounds.Split(text)
				.Where(token => !string.IsNullOrWhiteSpace(token))
				.ToArray();

			// Retrieve a vector for each word
			this.vectors = new LargeVector[this.words.Length];
			for (int i = 0; i < this.words.Length; i++)
			{
				this.vectors[i] = vectorData.Get(this.words[i].ToLower());
			}
		}

		private string WordifyPunctuation(string text)
		{
			text = text.Replace(".", " . ");
			text = text.Replace("?", " ?");
			text = text.Replace("!", " ! ");
			return text;
		}
	}
}
