using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DialogPrototype
{
	public class VectorDataStore : IDisposable
	{
		private int vectorCount = 0;
		private int vectorDimensions = 0;
		private Dictionary<string, LargeVector> cache = new Dictionary<string, LargeVector>();
		private BinaryReader dataReader = null;
		private BinaryReader indexReader = null;


		public int VectorCount
		{
			get { return this.vectorCount; }
		}
		public int VectorDimensions
		{
			get { return this.vectorDimensions; }
		}


		public void Dispose()
		{
			this.Close();
			this.cache = null;
		}
		public void Close()
		{
			if (this.dataReader != null)
			{
				this.dataReader.Dispose();
				this.dataReader = null;
			}
			if (this.indexReader != null)
			{
				this.indexReader.Dispose();
				this.indexReader = null;
			}
			this.ClearCache();
		}
		public void Open(string binPath, string indexPath)
		{
			this.Close();

			this.dataReader = new BinaryReader(File.OpenRead(binPath), Encoding.UTF8, false);
			this.indexReader = new BinaryReader(File.OpenRead(indexPath), Encoding.UTF8, false);
			
			int version = this.dataReader.ReadInt32();
			this.vectorCount = this.dataReader.ReadInt32();
			this.vectorDimensions = this.dataReader.ReadInt32();
		}
		public LargeVector Get(string word)
		{
			LargeVector vec;
			if (!this.cache.TryGetValue(word, out vec))
			{
				vec = this.ReadVector(word);
				this.cache.Add(word, vec);
			}
			return vec;
		}
		public float GetSimilarity(string wordA, string wordB)
		{
			LargeVector vecA = this.Get(wordA);
			if (vecA.IsEmpty) return 0.0f;

			LargeVector vecB = this.Get(wordB);
			if (vecB.IsEmpty) return 0.0f;

			float lengthA = vecA.GetLength();
			float lengthB = vecB.GetLength();
			return LargeVector.Dot(vecA, vecB) / (lengthA * lengthB);
		}

		private void ClearCache()
		{
			this.vectorDimensions = 0;
			this.cache.Clear();
		}
		private long SearchIndex(string word)
		{
			int min = 0;
			int max = this.vectorCount - 1;
			while (min <= max)
			{
				int mid = (min + max) / 2;
				VectorIndexItem item = this.ReadIndex(mid);
				if (word == item.Word)
					return item.Offset;
				else if (string.CompareOrdinal(word, item.Word) < 0)
					max = mid - 1;
				else
					min = mid + 1;
		   }
		   return -1;
		}
		private VectorIndexItem ReadIndex(int index)
		{
			this.indexReader.BaseStream.Seek(sizeof(int) + sizeof(int) + index * sizeof(long), SeekOrigin.Begin);
			long offset = this.indexReader.ReadInt64();

			this.dataReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			string word = this.dataReader.ReadString();

			return new VectorIndexItem
			{
				Word = word,
				Offset = offset
			};
		}
		private LargeVector ReadVector(string word)
		{
			long offset = this.SearchIndex(word);
			if (offset == -1) return LargeVector.Empty;

			this.dataReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			this.dataReader.ReadString();
			float[] vectorData = new float[this.vectorDimensions];
			for (int i = 0; i < vectorData.Length; i++)
				vectorData[i] = this.dataReader.ReadSingle();

			LargeVector vector = new LargeVector(vectorData);
			return vector;
		}
	}
}
