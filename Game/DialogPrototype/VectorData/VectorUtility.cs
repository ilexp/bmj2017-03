using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Text;
using System.IO;

namespace DialogPrototype
{
	public static class VectorUtility
	{
		public static void ConvertToBinary(string textPath, string binPath, Action<float> progressCallback = null)
		{
			using (FileStream binStream = File.Open(binPath, FileMode.Create))
			using (FileStream textStream = File.OpenRead(textPath))
			using (StreamReader textReader = new StreamReader(textStream, Encoding.UTF8, true))
			using (BinaryWriter binWriter = new BinaryWriter(binStream, Encoding.UTF8))
			{
				string headerLine = textReader.ReadLine();
				string[] headerLineToken = headerLine.Split(' ');
				int wordCount = int.Parse(headerLineToken[0]);
				int dimensionCount = int.Parse(headerLineToken[1]);

				binWriter.Write((int)1);
				binWriter.Write((int)wordCount);
				binWriter.Write((int)dimensionCount);

				string word = null;
				float[] vector = new float[dimensionCount];
				int wordIndex = 0;
				while (!textReader.EndOfStream)
				{
					string line = textReader.ReadLine();
					if (string.IsNullOrEmpty(line)) continue;

					int startIndex = 0;
					int separatorIndex = 0;
					int tokenIndex = -1;
					while (true)
					{
						separatorIndex = line.IndexOf(' ', startIndex);
						if (separatorIndex == -1) break;

						string token = line.Substring(startIndex, separatorIndex - startIndex);
						if (tokenIndex == -1)
							word = token;
						else
							vector[tokenIndex] = float.Parse(token);

						tokenIndex++;
						startIndex = separatorIndex + 1;
					}

					binWriter.Write((string)word);
					for (int i = 0; i < vector.Length; i++)
						binWriter.Write((float)vector[i]);

					wordIndex++;
					if ((wordIndex % 10000) == 0 && progressCallback != null)
						progressCallback((float)wordIndex / (float)wordCount);
				}
			}
		}
		public static void CreateIndex(string binPath, string indexPath, Action<float> progressCallback = null)
		{
			VectorIndexItem[] index = null;

			// Read Index data
			using (FileStream binStream = File.OpenRead(binPath))
			using (BinaryReader binReader = new BinaryReader(binStream, Encoding.UTF8))
			{
				int version = binReader.ReadInt32();
				int wordCount = binReader.ReadInt32();
				int vectorDimensions = binReader.ReadInt32();

				index = new VectorIndexItem[wordCount];
				for (int wordIndex = 0; wordIndex < index.Length; wordIndex++)
				{
					long offset = binStream.Position;
					string word = binReader.ReadString();

					index[wordIndex].Word = word;
					index[wordIndex].Offset = offset;

					binStream.Seek(vectorDimensions * sizeof(float), SeekOrigin.Current);

					if ((wordIndex % 10000) == 0 && progressCallback != null)
						progressCallback(0.4f * (float)wordIndex / (float)index.Length);
				}
			}

			// Sort index by key
			progressCallback(0.4f);
			Array.Sort(index, (a, b) => string.CompareOrdinal(a.Word, b.Word));
			progressCallback(0.6f);

			// Write index to file
			using (FileStream indexStream = File.Open(indexPath, FileMode.Create))
			using (BinaryWriter indexWriter = new BinaryWriter(indexStream, Encoding.UTF8))
			{
				indexWriter.Write((int)1);
				indexWriter.Write((int)index.Length);
				
				for (int i = 0; i < index.Length; i++)
				{
					indexWriter.Write((long)index[i].Offset);

					if ((i % 10000) == 0 && progressCallback != null)
						progressCallback(0.6f + 0.4f * (float)i / (float)index.Length);
				}
			}
		}
		
		public static void ConvertToBinary()
		{
			Console.WriteLine();
			Console.WriteLine("Converting Word Vectors...");
			ConvertToBinary(
				"..\\..\\..\\WordVectors\\wiki.en.vec", 
				"..\\..\\..\\WordVectors\\wiki.en.vec.bin", 
				progress =>
			{
				Console.WriteLine("{0:F}%", progress * 100.0f);
			});
		}
		public static void CreateIndex()
		{
			Console.WriteLine();
			Console.WriteLine("Indexing Word Vectors...");
			CreateIndex(
				"..\\..\\..\\WordVectors\\wiki.en.vec.bin", 
				"..\\..\\..\\WordVectors\\wiki.en.vec.idx", 
				progress =>
			{
				Console.WriteLine("{0:F}%", progress * 100.0f);
			});
		}
		public static void PrepareData()
		{
			ConvertToBinary();
			CreateIndex();
		}

	}
}
