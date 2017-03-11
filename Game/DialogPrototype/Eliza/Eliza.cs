using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class Eliza
	{
		private	ElizaDatabase	database		= new ElizaDatabase();
		private	List<string>	newInput		= new List<string>();
		private	List<string>	processedInput	= new List<string>();
		private	DateTime		lastInput		= DateTime.Now;
		private	DateTime		lastOutput		= DateTime.MinValue;
		private	TimeSpan		waitOffset		= TimeSpan.Zero;
		private	Random			random			= new Random();

		public void Update(bool userTyping)
		{
			// Initial greeting
			if (this.lastOutput == DateTime.MinValue)
			{
				this.Say(this.database.StateWelcome.Phrase(this.random));
			}

			// Don't answer if the user hasn't said anything new - or is still typing.
			if (this.newInput.Count > 0 && !userTyping)
			{
				// Wait a little while until answering.
				// Approximately as long as the user took writing the text.
				TimeSpan userReactionTime = this.lastInput - this.lastOutput;
				TimeSpan timeSinceInput = DateTime.Now - this.lastInput;
				TimeSpan waitTime = TimeSpan.FromMilliseconds(Math.Min((userReactionTime + this.waitOffset).TotalMilliseconds * 0.5f, 5000));
				if (timeSinceInput > waitTime)
				{
					// Pick a new random answer time offset
					this.waitOffset = TimeSpan.FromMilliseconds(0.35f * this.random.Next(-(int)userReactionTime.TotalMilliseconds, (int)userReactionTime.TotalMilliseconds));

					// Join all the available inputs to one text
					string joinedInput = "";
					foreach (string input in newInput)
					{
						string joinStr = input.Trim();
						if (!char.IsPunctuation(joinStr[joinStr.Length - 1]))
							joinStr += ".";
						joinedInput += joinStr + " ";
					}

					// Say something
					if (!string.IsNullOrWhiteSpace(joinedInput))
					{
						this.Say(this.ThinkAbout(joinedInput));
					}

					// Flag input as processed
					this.processedInput.AddRange(this.newInput);
					this.newInput.Clear();
				}
			}

			return;
		}
		public void Listen(string input)
		{
			if (string.IsNullOrEmpty(input)) return;
			this.newInput.Add(input);
			this.lastInput = DateTime.Now;
		}
		public void Say(string output)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("Bob: ");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(output);
			this.lastOutput = DateTime.Now;
		}
		public string ThinkAbout(string input)
		{
			ElizaMatchResult match = this.database.MatchInput(input, this.random);
			string result;
			if (match == ElizaMatchResult.Empty)
			{
				result = this.database.StateUnknown.Phrase(this.random);
			}
			else
			{
				string[] transformedTokens = this.TransformTokens(match.MagicTokens).ToArray();
				result = match.Statement.Phrase(this.random, this.database.StateUnknown, transformedTokens);
			}
			if (result.Length > 0) result = char.ToUpper(result[0]) + result.Remove(0, 1);
			return result;
		}
		public IEnumerable<string> TransformTokens(IEnumerable<string> token)
		{
			return token.Select(delegate (string t)
			{
				t = t.Trim();
				if (t.Length > 0) t = char.ToLower(t[0]) + t.Remove(0, 1);

				{
					IDictionary<string,string> map = new Dictionary<string,string>()
					{
						{ "you are", "I am" },
						{ "you're", "I am" },
						{ "i am", "you are" },
						{ "i'm", "you are" },
						{ "my", "your" },
						{ "me", "you" },
						{ "your", "my" },
						{ "you", "me" },
						{ "i", "you" },
					};
					var regex = new Regex(@"\b(?:" + string.Join("|", map.Keys) + @")\b", RegexOptions.IgnoreCase);
					t = regex.Replace(t, m => map[m.Value.ToLower()]);
				}

				// A little bit hacky, to do that here. Anyway: Remove "glue words" from the beginning.
				t = Regex.Replace(t, @"^(but|so|hence|though|because|whether|or|and)\b\s*", "", RegexOptions.IgnoreCase);

				return t;
			});
		}
	}
}
