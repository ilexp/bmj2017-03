using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class DialogNode
	{
		private DialogNode context;
		private Statement input;
		private Statement output;

		public DialogNode Context
		{
			get { return this.context; }
		}
		public Statement Input
		{
			get { return this.input; }
		}
		public Statement Output
		{
			get { return this.output; }
		}

		public DialogNode(DialogNode context, Statement input, Statement output)
		{
			this.context = context;
			this.input = input;
			this.output = output;
		}

		public float Matches(DialogNode context, Message input)
		{
			float inputSimilarity = this.input.GetSimilarity(input);
			return inputSimilarity;
		}
	}
}
