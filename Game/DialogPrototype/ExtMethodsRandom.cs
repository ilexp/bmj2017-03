using System;
using System.Collections.Generic;
using System.Linq;

namespace DialogPrototype
{
	/// <summary>
	/// Provides extension methods for <see cref="System.Random">random number generators</see>.
	/// </summary>
	public static class ExtMethodsRandom
	{
		/// <summary>
		/// Returns a random byte.
		/// </summary>
		/// <param name="r">A random number generator.</param>
		/// <returns></returns>
		public static byte NextByte(this Random r)
		{
			return (byte)(r.Next() % 256);
		}
		/// <summary>
		/// Returns a random byte.
		/// </summary>
		/// <param name="r">A random number generator.</param>
		/// <param name="max">Exclusive maximum value.</param>
		/// <returns></returns>
		public static byte NextByte(this Random r, byte max)
		{
			return (byte)(r.Next() % ((int)max + 1));
		}
		/// <summary>
		/// Returns a random byte.
		/// </summary>
		/// <param name="r">A random number generator.</param>
		/// <param name="min">Inclusive minimum value.</param>
		/// <param name="max">Exclusive maximum value.</param>
		/// <returns></returns>
		public static byte NextByte(this Random r, byte min, byte max)
		{
			return (byte)(min + (r.Next() % ((int)max - min + 1)));
		}

		/// <summary>
		/// Returns a random float.
		/// </summary>
		/// <param name="r">A random number generator.</param>
		/// <returns></returns>
		public static float NextFloat(this Random r)
		{
			return (float)r.NextDouble();
		}
		/// <summary>
		/// Returns a random float.
		/// </summary>
		/// <param name="r">A random number generator.</param>
		/// <param name="max">Exclusive maximum value.</param>
		/// <returns></returns>
		public static float NextFloat(this Random r, float max)
		{
			return max * (float)r.NextDouble();
		}
		/// <summary>
		/// Returns a random float.
		/// </summary>
		/// <param name="r">A random number generator.</param>
		/// <param name="min">Inclusive minimum value.</param>
		/// <param name="max">Exclusive maximum value.</param>
		/// <returns></returns>
		public static float NextFloat(this Random r, float min, float max)
		{
			return min + (max - min) * (float)r.NextDouble();
		}

		/// <summary>
		/// Returns a random bool.
		/// </summary>
		/// <param name="r">A random number generator.</param>
		/// <returns></returns>
		public static bool NextBool(this Random r)
		{
			return r.NextDouble() > 0.5d;
		}

		/// <summary>
		/// Returns a random value from a weighted value pool.
		/// </summary>
		/// <typeparam name="T">Type of the random values.</typeparam>
		/// <param name="r">A random number generator.</param>
		/// <param name="values">A pool of values.</param>
		/// <param name="weights">One weight for each value in the pool.</param>
		/// <returns></returns>
		public static T OneOfWeighted<T>(this Random r, IEnumerable<T> values, IEnumerable<float> weights)
		{
			float totalWeight = weights.Sum();
			float pickedWeight = r.NextFloat(totalWeight);
			
			IEnumerator<T> valEnum = values.GetEnumerator();
			if (!valEnum.MoveNext()) return default(T);

			foreach (float w in weights)
			{
				pickedWeight -= w;
				if (pickedWeight < 0.0f) return valEnum.Current;
				valEnum.MoveNext();
			}

			return default(T);
		}
		/// <summary>
		/// Returns a random value from a weighted value pool.
		/// </summary>
		/// <typeparam name="T">Type of the random values.</typeparam>
		/// <param name="r">A random number generator.</param>
		/// <param name="values">A pool of values.</param>
		/// <param name="weights">One weight for each value in the pool.</param>
		/// <returns></returns>
		public static T OneOfWeighted<T>(this Random r, IEnumerable<T> values, params float[] weights)
		{
			return OneOfWeighted<T>(r, values, weights as IEnumerable<float>);
		}
		/// <summary>
		/// Returns a random value from a weighted value pool.
		/// </summary>
		/// <typeparam name="T">Type of the random values.</typeparam>
		/// <param name="r">A random number generator.</param>
		/// <param name="weightedValues">A weighted value pool.</param>
		/// <returns></returns>
		public static T OneOfWeighted<T>(this Random r, IEnumerable<KeyValuePair<T,float>> weightedValues)
		{
			float totalWeight = weightedValues.Sum(v => v.Value);
			float pickedWeight = r.NextFloat(totalWeight);
			
			foreach (KeyValuePair<T,float> pair in weightedValues)
			{
				pickedWeight -= pair.Value;
				if (pickedWeight < 0.0f) return pair.Key;
			}

			return default(T);
		}
		/// <summary>
		/// Returns a random value from a weighted value pool.
		/// </summary>
		/// <typeparam name="T">Type of the random values.</typeparam>
		/// <param name="r">A random number generator.</param>
		/// <param name="weightedValues">A weighted value pool.</param>
		/// <returns></returns>
		public static T OneOfWeighted<T>(this Random r, params KeyValuePair<T,float>[] weightedValues)
		{
			return OneOfWeighted<T>(r, weightedValues as IEnumerable<KeyValuePair<T,float>>);
		}
		/// <summary>
		/// Returns a random value from a weighted value pool.
		/// </summary>
		/// <typeparam name="T">Type of the random values.</typeparam>
		/// <param name="r">A random number generator.</param>
		/// <param name="values">A pool of values.</param>
		/// <param name="weightFunc">A weight function that provides a weight for each value from the pool.</param>
		/// <returns></returns>
		public static T OneOfWeighted<T>(this Random r, IEnumerable<T> values, Func<T,float> weightFunc)
		{
			return OneOfWeighted<T>(r, values, values.Select(v => weightFunc(v)));
		}

		/// <summary>
		/// Returns one randomly selected element.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="r"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T OneOf<T>(this Random r, IEnumerable<T> values)
		{
			return values.ElementAt(r.Next(values.Count()));
		}
	}
}
