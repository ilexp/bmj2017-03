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
			AsyncConsole.UserPrefix = "You: ";
			AsyncConsole.UserPrefixColor = ConsoleColor.Red;

			using (VectorDataStore vectorData = new VectorDataStore())
			{
				vectorData.Open(
					"..\\..\\..\\WordVectors\\wiki.en.vec.bin",
					"..\\..\\..\\WordVectors\\wiki.en.vec.idx");

				Console.WriteLine("{0} vectors available", vectorData.VectorCount);
				Console.WriteLine("{0} dimensions per vector", vectorData.VectorDimensions);

				Eliza eliza = new Eliza(vectorData);

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
}
