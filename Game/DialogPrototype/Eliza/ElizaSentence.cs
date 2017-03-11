using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialogPrototype
{
	public class ElizaSentence
	{
		private	string rawText;
		private DateTime lastUsed;

		public string RawText
		{
			get { return this.rawText; }
		}
		public DateTime LastUsed
		{
			get { return this.lastUsed; }
		}
		public float UsageScore
		{
			get { return this.lastUsed > DateTime.MinValue ? Math.Min(1.0f, ((float)(DateTime.Now - this.lastUsed).TotalSeconds / 300.0f)) : 1.0f; }
		}

		public ElizaSentence(string text)
		{
			this.rawText = text;
			this.lastUsed = DateTime.MinValue;
		}
		public void TickUsed()
		{
			this.lastUsed = DateTime.Now;
		}
	}
}
