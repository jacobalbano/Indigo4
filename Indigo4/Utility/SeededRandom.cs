using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Indigo.Utility
{
	public class SeededRandom
	{
        public static SeededRandom Default { get; } = new SeededRandom();

		/// <summary>
		/// Constructor.
		/// </summary>
		public SeededRandom() : this(Guid.NewGuid().GetHashCode()) { }
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SeededRandom(int seed)
		{
			_seed = seed;
		}
		
		/// <summary>
		/// Randomizes the seed.
		/// </summary>
		/// <returns>The seed's value prior to randomizing it.</returns>
		public int Randomize()
		{
			var result = _seed;
			_seed = Guid.NewGuid().GetHashCode();
			return _seed;
		}
		
		public int Seed
		{
			get { return _seed; }
			set { _seed = Math.Abs(value); }
		}
		
		/// <summary>
		/// Determines the chance of a condition.
		/// </summary>
		/// <param name="percentage">A value between 0 and 100.</param>
		public bool Chance(float percentage)
		{
			return Float() < percentage / 100f;
		}

        /// <summary>
        /// point within radiusmax from (0, 0).
        /// </summary>
        public Vector2 InCircle(float radiusMax)
        {
            return InCircle(0, radiusMax);
        }

        /// <summary>
        /// point within radiusmax from (0, 0), and no closer than radiusmin.
        /// </summary>
        public Vector2 InCircle(float radiusMin, float radiusMax)
        {
            var angle = Angle();
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Float(radiusMin, radiusMax);
        }

        /// <summary>
        /// Either true or false.
        /// </summary>
        public bool Bool()
		{
			return Math.Round(Float()) == 0;
		}
		
		/// <summary>
		/// Value from 0 to 360. Lower inclusive, upper exclusive.
		/// </summary>
		public float Angle()
		{
			return Float(360);
		}
		
		/// <summary>
		/// Either -1 or 1.
		/// </summary>
		public int Sign()
		{
			return Bool() ? -1 : 1;
		}
		
		/// <summary>
		/// Value from 0 to int.MaxValue.
		/// </summary>
		public int Int()
		{
			return Int(int.MaxValue);
		}
		
		/// <summary>
		/// Value from 0 to max. Lower inclusive, upper exclusive.
		/// </summary>
		public int Int(int max)
		{
			return (int) Float(max);
		}
		
		/// <summary>
		/// Value from min to max. Lower inclusive, upper exclusive.
		/// </summary>
		public int Int(int min, int max)
		{
			return (int) Float(min, max);
		}
		
		/// <summary>
		/// Value from 0 to 1.
		/// </summary>
		/// <returns></returns>
		public float Float()
		{
			//	what is this though
			return ((_seed = (int) ((_seed * 48271.0) % 2147483647.0)) & 0x3FFFFFFF) / 1073741823f;
		}
		
		/// <summary>
		/// Value from 0 to max. Lower inclusive, upper exclusive.
		/// </summary>
		public float Float(float max)
		{
			return max * Float();
		}
		
		/// <summary>
		/// Value from min to max. Lower inclusive, upper exclusive.
		/// </summary>
		public float Float(float min, float max)
		{
			return (max - min) * Float() + min;
		}

        /// <summary>
        /// Color with random components.
        /// </summary>
        public Color Color()
        {
            return new Color(Int(255), Int(255), Int(255));
        }

        /// <summary>
        /// Shuffles the order of the elements in a list.
        /// </summary>
        public void Shuffle<T>(IList<T> list)
		{
			int j = 0, i = list.Count;
			
			while (--i > 0)
			{
				var t = list[i];
				list[i] = list[j = Int(i + 1)];
				list[j] = t;
			}
		}
		
		private int _seed;
	}
}
