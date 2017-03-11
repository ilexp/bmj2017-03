using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DialogPrototype
{
	public static class AsyncConsole
	{
		private static Thread        readThread      = null;
		private static Queue<string> finishedLines   = new Queue<string>();
		private static string        currentLine     = "";
		private static string        userPrefix      = "";
		private static ConsoleColor  userPrefixColor = ConsoleColor.Gray;
		private static object        accessLock      = new object();

		public static bool HasLine
		{
			get 
			{
				lock (accessLock)
				{
					return finishedLines.Count > 0;
				}
			}
		}
		public static bool IsReceivingLine
		{
			get 
			{
				lock (accessLock)
				{
					return !string.IsNullOrEmpty(currentLine);
				}
			}
		}
		public static string FinishedLine
		{
			get 
			{
				lock (accessLock)
				{
					return finishedLines.Peek();
				}
			}
		}
		public static string ReceivingLine
		{
			get 
			{
				lock (accessLock)
				{
					return currentLine;
				}
			}
		}
		public static string UserPrefix
		{
			get { return userPrefix; }
			set
			{
				lock (accessLock)
				{
					userPrefix = value;
				}
			}
		}
		public static ConsoleColor UserPrefixColor
		{
			get { return userPrefixColor; }
			set
			{
				lock (accessLock)
				{
					userPrefixColor = value;
				}
			}
		}

		public static string ReadLine()
		{
			lock (accessLock)
			{
				if (finishedLines.Count > 0)
					return finishedLines.Dequeue();
				else
					return null;
			}
		}

		static AsyncConsole()
		{
			finishedLines = new Queue<string>();
			readThread = new Thread(ReadThreadMain);
			readThread.CurrentCulture = CultureInfo.InvariantCulture;
			readThread.CurrentUICulture = CultureInfo.InvariantCulture;
			readThread.IsBackground = true;
			readThread.Start();
		}
		private static void ReadThreadMain()
		{
			while (true)
			{
				if (Console.KeyAvailable)
				{
					ConsoleKeyInfo key = Console.ReadKey(true);
					lock (accessLock)
					{
						if (key.Key == ConsoleKey.Enter && currentLine.Length > 0)
						{
							finishedLines.Enqueue(currentLine);
							currentLine = "";
							Console.WriteLine();
						}
						else if (key.Key == ConsoleKey.Escape && currentLine.Length > 0)
						{
							EraseChars(currentLine.Length + userPrefix.Length);
							currentLine = "";
						}
						else if (key.Key == ConsoleKey.Backspace && currentLine.Length > 0)
						{
							EraseChars(1);
							currentLine = currentLine.Remove(currentLine.Length - 1, 1);
							if (currentLine.Length == 0)
							{
								EraseChars(userPrefix.Length);
							}
						}
						else if (!char.IsControl(key.KeyChar))
						{
							if (string.IsNullOrEmpty(currentLine))
							{
								Console.ForegroundColor = userPrefixColor;
								Console.Write(userPrefix);
								Console.ForegroundColor = ConsoleColor.Gray;
							}
							currentLine += key.KeyChar;
							Console.Write(key.KeyChar);
						}
					}
				}
				Thread.Sleep(50);
			}
		}
		private static void EraseChars(int count)
		{
			int x = Console.CursorLeft;
			int y = Console.CursorTop;
			int i = y * Console.WindowWidth + x;
			i -= count;
			int eraseX = i % Console.WindowWidth;
			int eraseY = i / Console.WindowWidth;
			Console.SetCursorPosition(eraseX, eraseY);
			Console.Write(new string(' ', count));
			Console.SetCursorPosition(eraseX, eraseY);
		}
	}
}
