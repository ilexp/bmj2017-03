using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogPrototype
{
	public class ElizaDatabase
	{
		private	ElizaStatement stateWelcome = new ElizaStatement { 
		"Hello. Please name what kind of problem you have.",
		"Hey, I'm Bob. What can I do for you?",
		"Hi, how can I help you?",
		"This is Bob - what's the matter?",
		"Hey. I'm Bob, the technical support. Please describe the problem you're experiencing."};
		private	ElizaStatement stateUnknown = new ElizaStatement { 
		"Can you please rephrase that?",
		"I don't get it.",
		"I'm sorry - I didn't get that?",
		"Ahh.. yeah, I don't quite get that?",
		"Please explain.",
		"Hmm.. care to elaborate on that?",
		"Could you clarify that?",
		"What makes you think that?",
		"Did anything unusual happen lately?",
		"Mhm, interesting. Tell me more.",
		"Can you clarify that?",
		"What else can you tell me?",
		"What else?",
		"Go on.",
		"That seems a bit off.",
		"Yes, of course. Go on.",
		"You must be kidding.",
		"Are you serious?",
		"Really?",
		"Are you sure?",
		"Wait, what?",
		"Aha.. well, I'll need some more data on that!",
		"You'll need to give me something I can work with.",
		"Have you tried shutting it down and setting it up again?",
		"Have you tried unplugging and replugging it?",
		"What system are you using?",
		"Hm."};
		private Dictionary<ElizaInputPattern,ElizaStatement> stateMatched = new Dictionary<ElizaInputPattern,ElizaStatement>
		{
			{ 
				new ElizaInputPattern { 
				@"{TOKEN} (?:doesn't|does not|won't|will not|wouldn't|would not|can't|can not|cannot|isn't|is not) (function|work|do|power on|power off|start|run|boot|load|save)(?:ing)*", 
				@"problem with {TOKEN}",
				@"{TOKEN} (?:is|has) broken",
				@"{TOKEN} (?:is|has) crashed",
				@"{TOKEN} crashes", }, 
				new ElizaStatement { 
				"What makes you think that {0} doesn't {1}?",
				"How old is {0}?",
				"Please give me some specifications on {0}.",
				"Are you certain, that it has something to do with {0}?",
				"Okay, so what's the point of {0} anyway?",
				"Are you sure that the problem source is really {0}?",
				"What was the last thing you did with {0}?",
				"Tell me more about {0} please.",
				"So, how exactly can I imagine {0} to be?",
				"How often do you use {0}?",
				"When was the last time {0} worked correctly?",
				 }
			},
			{ 
			    new ElizaInputPattern { 
			    @"nothing happens", 
			    @"(?:doesn't|does not|won't|will not|wouldn't|would not|can't|can not|cannot) do anything", 
			    @"problem", 
			    @"{TOKEN} broken", 
			    @"{TOKEN} ruined", 
			    @"{TOKEN} bust\w+", 
			    @"{TOKEN} shatter\w+", 
			    @"{TOKEN} crashe\w+" }, 
			    new ElizaStatement { 
			    "Oh well. That is unfortunate.",
			    "Damn. That sounds bad.",
				"Huh. That's strange.",
			    "Oh no.",
			    "That doesn't sound good.",
			    "Has this happened before?",
			    "How often has this happened?",
			    "Do you know others who experience the same problem?",
			     }
			},
			{ 
			    new ElizaInputPattern { 
			    @"[\s\w']+(?=\s*[?]+)" }, 
			    new ElizaStatement { 
			    "Well, you've got me there.",
			    "I have no idea.",
			    "You'll need to tell me more.",
				"Umm.. that's.. well. I don't know.",
			    "Hmm.. I think you're not asking the right questions. Let's start over.",
			    "You're on the wrong track here.",
			    "Why are you asking?",
			    "Is this even a question?",
			    "Are you really asking this?",
			    "This shouldn't be relevant.",
			    "Hm.. not sure.",
			     }
			},
			{ 
				new ElizaInputPattern { 
				@"you're {TOKEN}",
				@"you are {TOKEN}",
				@"don't be {TOKEN}" }, 
				new ElizaStatement { 
				"What makes you think that I am {0}?",
				"How can you tell that I'm {0}?",
				"I'm certainly not {0}",
				"I'm {0}? Why would you think that?",
				 }
			},
			{ 
				new ElizaInputPattern { 
				@"you aren't {TOKEN}",
				@"you are not {TOKEN}" }, 
				new ElizaStatement { 
				"Why would you think I'm not {0}?",
				"How can you tell that I'm not {0}?",
				"I'm probably not.",
				 }
			},
			{ 
				new ElizaInputPattern { 
				@"I (?:think|guess),* {TOKEN}" }, 
				new ElizaStatement { 
				"What makes you think that {0}?",
				"Why do you think {0}?",
				"Are you sure?",
				"I'm not quite convinced.",
				"It could be something else.",
				 }
			},
			{ 
				new ElizaInputPattern { 
				@"I(?:'m|'ll)* (?:have to|need to|am going to|will|willing to) {TOKEN}" }, 
				new ElizaStatement { 
				"Why do you have to {0}?",
				"Is there no other way?",
				"You need to {0}... really?",
				"This can't be true.",
				"Yeah. Maybe that's a good idea.",
				"You don't mean that, do you?",
				"Oh, come on. That's just random.",
				 }
			},
			{ 
				new ElizaInputPattern { 
				@"Hey",
				@"Hi",
				@"Hello",
				@"to meet you",
				@"Greetings",
				@"Good day",
				@"Good evening",
				@"Good morning",
				@"Good night"}, 
				new ElizaStatement { 
				"So.. what's the problem?",
				"Yeah, so.. what's the matter?",
				"Why are you calling?",
				"Let's get straight to business. What problem are you experiencing?",
				 }
			},
		};

		public ElizaStatement StateWelcome
		{
			get { return this.stateWelcome; }
		}
		public ElizaStatement StateUnknown
		{
			get { return this.stateUnknown; }
		}

		public ElizaMatchResult MatchInput(string input, Random random)
		{
			List<ElizaMatchResult> matches = new List<ElizaMatchResult>();
			foreach (var pair in stateMatched)
			{
				List<string> match = pair.Key.Matches(input);
				if (match != null) matches.Add(new ElizaMatchResult(pair.Value, match));
			}

			if (matches.Count == 0)
				return ElizaMatchResult.Empty;
			else
				return random.OneOfWeighted(matches, m => m.Statement.Sum(s => s.UsageScore));
		}
	}

	public class ElizaMatchResult
	{
		public static readonly ElizaMatchResult Empty = new ElizaMatchResult(null, null);

		private ElizaStatement statement;
		private List<string> magicTokens;

		public ElizaStatement Statement
		{
			get { return this.statement; }
		}
		public List<string> MagicTokens
		{
			get { return this.magicTokens; }
		}

		public ElizaMatchResult(ElizaStatement state, List<string> token)
		{
			this.statement = state;
			this.magicTokens = token;
		}
	}
}
