using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialogPrototype
{
	public class ElizaStatement : IEnumerable<ElizaSentence>
	{
		private	List<ElizaSentence>	sentences	= new List<ElizaSentence>();

		public ElizaStatement() {}
		public ElizaStatement(IEnumerable<ElizaSentence> sentences)
		{
			this.sentences = sentences.ToList();
		}
		public ElizaStatement(IEnumerable<string> sentences) : this(sentences.Select(s => new ElizaSentence(s))) {}

		public string Phrase(Random random, ElizaStatement fallback = null, IEnumerable<string> tokens = null)
		{
			// Pick a sentence via weighted random
			ElizaSentence pick = random.OneOfWeighted(this.sentences, s => s.UsageScore);

			// Overused sentence? Use a fallback
			if (pick.UsageScore < 0.25f && fallback != null && this != fallback)
				return fallback.Phrase(random, fallback, tokens);

			// mark sentence as used and format it.
			pick.TickUsed();
			if (tokens == null)
				return pick.RawText;
			else
				return string.Format(pick.RawText, tokens.Cast<object>().ToArray());
		}

		public void Add(ElizaSentence s)
		{
			this.sentences.Add(s);
		}
		public void Add(string s)
		{
			this.sentences.Add(new ElizaSentence(s));
		}

		IEnumerator<ElizaSentence> IEnumerable<ElizaSentence>.GetEnumerator()
		{
			return this.sentences.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.sentences.GetEnumerator();
		}
	}
}
