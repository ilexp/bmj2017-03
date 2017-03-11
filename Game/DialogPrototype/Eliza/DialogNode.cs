using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class DialogNode
	{
		private Statement input;
		private Statement output;

		public Statement Input
		{
			get { return this.input; }
		}
		public Statement Output
		{
			get { return this.output; }
		}

		public DialogNode(Statement input, Statement output)
		{
			this.input = input;
			this.output = output;
		}

		public float Matches(DialogContext context, Message input)
		{
			float inputSimilarity = this.input.GetSimilarity(context, input);
			return inputSimilarity;
		}
	}
}
