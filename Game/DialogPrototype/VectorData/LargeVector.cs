using System;
using System.Collections.Generic;

namespace DialogPrototype
{
	public struct LargeVector
	{
		public static LargeVector Empty = new LargeVector(new float[0]);


		private float[] data;


		public bool IsEmpty
		{
			get { return this.data == null || this.data.Length == 0; }
		}
		public int Dimensions
		{
			get { return this.data != null ? this.data.Length : 0; }
		}
		public float[] Data
		{
			get { return this.data ?? Empty.data; }
		}


		public LargeVector(int dimensions)
		{
			this.data = new float[dimensions];
		}
		public LargeVector(float[] data)
		{
			this.data = data;
		}

		public float GetLength()
		{
			float lengthSquared = Dot(this, this);
			return (float)Math.Sqrt(lengthSquared);
		}
		public void Normalize()
		{
			float length = GetLength();
			float invLength = 1.0f / length;
			for (int i = 0; i < this.data.Length; i++)
			{
				this.data[i] *= invLength;
			}
		}


		public static float Dot(LargeVector left, LargeVector right)
		{
			float result = 0.0f;
			for (int i = 0; i < left.data.Length; i++)
			{
				result += left.data[i] * right.data[i];
			}
			return result;
		}
		public static LargeVector Lerp(LargeVector left, LargeVector right, float factor)
		{
			LargeVector result = new LargeVector(Math.Max(left.Dimensions, right.Dimensions));
			float[] leftData = left.IsEmpty ? result.data : left.data;
			float[] rightData = right.IsEmpty ? result.data : right.data;
			for (int i = 0; i < result.data.Length; i++)
			{
				result.data[i] = leftData[i] * (1.0f - factor) + rightData[i] * factor;
			}
			return result;
		}
	}
}
