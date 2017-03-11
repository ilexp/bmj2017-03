using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class Eliza
	{
		private	List<string>	newInput		= new List<string>();
		private	List<string>	processedInput	= new List<string>();
		private	DateTime		lastInput		= DateTime.Now;
		private	DateTime		lastOutput		= DateTime.Now;
		private	TimeSpan		waitOffset		= TimeSpan.Zero;
		private	Random			random			= new Random();

		public void Update(bool userTyping)
		{
			// Don't answer if the user hasn't said anything new - or is still typing.
			if (this.newInput.Count > 0 && !userTyping)
			{
				// Wait a little while until answering.
				// Approximately as long as the user took writing the text.
				TimeSpan userReactionTime = this.lastInput - this.lastOutput;
				TimeSpan timeSinceInput = DateTime.Now - this.lastInput;
				TimeSpan waitTime = TimeSpan.FromSeconds(Math.Min((userReactionTime + this.waitOffset).TotalSeconds * 0.5f, 5.0f));
				if (timeSinceInput > waitTime && userReactionTime.TotalSeconds > 0.0f)
				{
					// Pick a new random answer time offset
					this.waitOffset = TimeSpan.FromMilliseconds(0.35f * this.random.Next(
						-(int)userReactionTime.TotalMilliseconds, 
						(int)userReactionTime.TotalMilliseconds));

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
			return "What are you talking about?";
		}
	}
}
