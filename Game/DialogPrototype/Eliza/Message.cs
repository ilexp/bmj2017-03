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

		public string Text
		{
			get { return this.rawText; }
		}

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

		public float GetSimilarity(Message other)
		{
			float similarity = 0.0f;

			const int StageCount = 6;
			const int StepCount = 6;
			for (int stage = 0; stage < StageCount; stage++)
			{
				float length = 1.0f / (1.0f + (2.0f * (float)stage / (float)StageCount));
				for (int step = 0; step < StepCount; step++)
				{
					float pos = (1.0f - length) * ((1.0f / (float)StepCount) * (float)step);
					similarity = Math.Max(similarity, this.GetSimilarity(other, pos, length));
					if (stage == 0) break;
				}
			}

			return similarity;
		}

		private float GetSimilarity(Message other, float begin, float length)
		{
			double totalSimilarity = 1.0d;

			const int SampleCount = 20;
			for (int i = 0; i < SampleCount; i++)
			{
				float pos = begin + length * ((float)i / (float)(SampleCount - 1));

				LargeVector localSample = this.SampleVector(pos);
				if (localSample.IsEmpty) continue;

				LargeVector otherSample = other.SampleVector(pos);
				if (otherSample.IsEmpty) continue;

				float lengthA = localSample.GetLength();
				float lengthB = otherSample.GetLength();

				float similarity = LargeVector.Dot(localSample, otherSample) / (lengthA * lengthB);
				totalSimilarity *= (double)similarity;
			}

			return (float)Math.Pow(totalSimilarity, 5.0d / (double)SampleCount);
		}
		private LargeVector SampleVector(float position)
		{
			float targetIndex = (this.vectors.Length - 1) * position;
			int firstIndex = (int)Math.Floor(targetIndex);
			int secondIndex = (int)Math.Ceiling(targetIndex);
			float alpha = targetIndex - firstIndex;
			return LargeVector.Lerp(this.vectors[firstIndex], this.vectors[secondIndex], alpha);
		}
		private string WordifyPunctuation(string text)
		{
			text = text.Replace(".", " . ");
			text = text.Replace("?", " ? ");
			text = text.Replace("!", " ! ");

			text = text.Replace("'", " ");

			return text;
		}
	}
}
