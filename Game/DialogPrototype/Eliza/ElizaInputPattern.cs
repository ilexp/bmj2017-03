using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class ElizaInputPattern : IEnumerable<Regex>
	{
		private	List<Regex>	patterns	= new List<Regex>();

		public List<string> Matches(string input)
		{
			foreach (Regex pattern in this.patterns)
			{
				MatchCollection matches = pattern.Matches(input);
				Match lastMatch = matches.OfType<Match>().LastOrDefault();
				if (lastMatch != null)
				{
					List<string> result = new List<string>();
					foreach (Group group in lastMatch.Groups.OfType<Group>().Skip(1))
					{
						result.Add(group.Value);
					}
					return result;
				}
			}
			return null;
		}

		public void Add(Regex s)
		{
			this.patterns.Add(s);
		}
		public void Add(string s)
		{
			this.patterns.Add(new Regex(@"\b" + s.Replace("{TOKEN}", @"([\s\w']+)") + @"\b", RegexOptions.IgnoreCase));
		}

		IEnumerator<Regex> IEnumerable<Regex>.GetEnumerator()
		{
			return this.patterns.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.patterns.GetEnumerator();
		}
	}
}
