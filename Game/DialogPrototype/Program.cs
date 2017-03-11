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
		public static void Main(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			using (VectorDataStore vectorData = new VectorDataStore())
			{
				vectorData.Open(
					"..\\..\\..\\WordVectors\\wiki.en.vec.bin", 
					"..\\..\\..\\WordVectors\\wiki.en.vec.idx");

				Console.WriteLine("{0} vectors available", vectorData.VectorCount);
				Console.WriteLine("{0} dimensions per vector", vectorData.VectorDimensions);
			
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
		public static void Main2(string[] args)
		{
			AsyncConsole.UserPrefix = "You: ";
			AsyncConsole.UserPrefixColor = ConsoleColor.Red;

			Eliza eliza = new Eliza();

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
			}
		}
	}
}
