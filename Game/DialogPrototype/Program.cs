using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Text;
using System.IO;

using DialogPrototype;

namespace DialogPrototype
{
	public static class Program
	{
		public static void Main2(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			using (VectorDataStore vectorData = new VectorDataStore())
			{
				Console.WriteLine("Opening word vector database...");
				vectorData.Open(
					"..\\..\\..\\WordVectors\\wiki.en.vec.bin", 
					"..\\..\\..\\WordVectors\\wiki.en.vec.idx");

				Console.WriteLine("  {0} vectors available", vectorData.VectorCount);
				Console.WriteLine("  {0} dimensions per vector", vectorData.VectorDimensions);

				Console.WriteLine();
				Console.WriteLine("Enter two words to check semantic similarity");
				Console.WriteLine();
				while (true)
				{
					Console.Write("First: ");
					string first = Console.ReadLine();
					Console.Write("Second: ");
					string second = Console.ReadLine();
					Console.WriteLine("Similarity: {0:F}", vectorData.GetSimilarity(first, second));
					Console.WriteLine();
				}
			}
		}
		public static void Main(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			AsyncConsole.UserPrefix = "You: ";
			AsyncConsole.UserPrefixColor = ConsoleColor.Red;

			using (VectorDataStore vectorData = new VectorDataStore())
			{
				Console.WriteLine("Opening word vector database...");
				vectorData.Open(
					"..\\..\\..\\WordVectors\\wiki.en.vec.bin",
					"..\\..\\..\\WordVectors\\wiki.en.vec.idx");

				Console.WriteLine("  {0} vectors available", vectorData.VectorCount);
				Console.WriteLine("  {0} dimensions per vector", vectorData.VectorDimensions);
				Console.WriteLine();

				DialogTree sampleDialog = CreateSampleTree(vectorData);
				Eliza eliza = new Eliza(vectorData, sampleDialog);

				while (true)
				{
					// Receive user input
					bool userTyping = AsyncConsole.IsReceivingLine;
					string userInput = AsyncConsole.ReadLine();
					if (!string.IsNullOrEmpty(userInput))
					{
						// Quit program
						if (string.Equals(userInput, "bye", StringComparison.InvariantCultureIgnoreCase))
							break;

						// Say something: hand it over to Eliza
						eliza.Listen(userInput);
					}

					// Let Eliza think
					eliza.Update(userTyping);

					// Don't run at CPU max speed
					Thread.Sleep(1);
				}
			}
		}

		private static DialogTree CreateSampleTree(VectorDataStore vectorData)
		{
			DialogTree dialogTree = new DialogTree();

			DialogNode greetings = new DialogNode(
				new Statement(new Message[]
				{
					new Message("hi", vectorData),
					new Message("hello", vectorData),
					new Message("good day", vectorData),
					new Message("have a good day", vectorData)
				}),
				new Statement(new Message[]
				{
					new Message("What do you want?", vectorData),
					new Message("What's up?", vectorData)
				}));
			dialogTree.Add(greetings);

			DialogContext dontLetYouInContext = new DialogContext("DontLetYouInContext");
			DialogNode letMeIn = new DialogNode(
				new Statement(new Message[]
				{
					new Message("let me in", vectorData),
					new Message("open the gate", vectorData)
				}),
				new Statement(dontLetYouInContext, new Message[]
				{
					new Message("No.", vectorData),
					new Message("Definitely not.", vectorData),
					new Message("Forget it.", vectorData),
				}));
			dialogTree.Add(letMeIn);

			DialogContext castleStaffContext = new DialogContext("CastleStaffContext");
			DialogNode letMeInWhyNot = new DialogNode(
				new Statement(new Message[]
				{
					new Message(dontLetYouInContext, "why not?", vectorData),
					new Message(dontLetYouInContext, "what do you mean?", vectorData),
					new Message("why don't you let me in?", vectorData),
					new Message("why can't I get in?", vectorData)
				}),
				new Statement(new Message[]
				{
					new Message("You are not authorized.", vectorData),
					new Message("Nobody can pass this gate.", vectorData),
					new Message(castleStaffContext, "Only the castle staff may enter.", vectorData),
					new Message("I can't just let anyone in.", vectorData),
				}));
			dialogTree.Add(letMeInWhyNot);

			DialogNode castleStaff = new DialogNode(
				new Statement(castleStaffContext, new Message[]
				{
					new Message("i'm the chef", vectorData),
					new Message("i belong to the castle staff", vectorData)
				}),
				new Statement(new Message[]
				{
					new Message("I know the staff, and you don't belong to it.", vectorData),
					new Message("I don't think so.", vectorData)
				}));
			dialogTree.Add(castleStaff);

			DialogNode password = new DialogNode(
				new Statement(new Message[]
				{
					new Message("clubmate", vectorData)
				}),
				new Statement(new Message[]
				{
					new Message("Alright. I'll open the gate.", vectorData),
				}));
			dialogTree.Add(password);

			return dialogTree;
		}
	}
}
