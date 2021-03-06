﻿using System;
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
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine("Opening word vector database...");
				vectorData.Open(
					"..\\..\\..\\WordVectors\\wiki.en.vec.bin",
					"..\\..\\..\\WordVectors\\wiki.en.vec.idx");

				Console.WriteLine("  {0} vectors available", vectorData.VectorCount);
				Console.WriteLine("  {0} dimensions per vector", vectorData.VectorDimensions);
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine("This is an experiment for a dialog system. Imagine you are");
				Console.WriteLine("actually playing some open world RPG game. Right now, you are");
				Console.WriteLine("standing in front of the castle gate and need to get in to");
				Console.WriteLine("fulfill some epic quest. However, castle guard Bob is in your way.");
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("Talk your way in.");
				Console.ResetColor();
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
			return DialogTree.Load("SampleDialog.json", vectorData);
		}
	}
}
