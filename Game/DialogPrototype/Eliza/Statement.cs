using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class Statement
	{
		private List<Message> messages = new List<Message>();
		
		public IEnumerable<Message> Messages
		{
			get { return this.messages; }
		}

		public Statement(IEnumerable<Message> messages)
		{
			this.messages.AddRange(messages);
		}
		public float GetSimilarity(Message other)
		{
			float maxSimilarity = 0.0f;
			foreach (Message message in this.messages)
			{
				float similarity = message.GetSimilarity(other);
				maxSimilarity = (float)Math.Max(similarity, maxSimilarity);
			}
			return maxSimilarity;
		}
	}
}
