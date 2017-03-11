using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class DialogContext
	{
		private string id;

		public string Id
		{
			get { return this.id; }
		}

		public DialogContext(string id)
		{
			this.id = id;
		}
	}
}
