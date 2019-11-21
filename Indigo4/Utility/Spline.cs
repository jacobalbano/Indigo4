using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Indigo.Utility
{
	public class Spline : IEnumerable<float>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="points">The points that comprise the spline.</param>
		public Spline(IEnumerable<float> points)
		{
			path = new List<float>(points);
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="points">The points that comprise the spline.</param>
		public Spline(params float[] points)
		{
			path = new List<float>(points);
		}
		
		/// <summary>
		/// Get the value of the spline at a specified time.
		/// </summary>
		/// <param name="t">The completion of the spline, in range [0..1].</param>
		/// <returns>The value of the spline at the given time.</returns>
		public float Point(float t)
		{
			// Shoutouts to Chevy Ray
			int b = (int)((path.Count - 1) * t);
			int a = b - 1;
			int c = b + 1;
			int d = c + 1;
			
			a = (int) MathUtility.Clamp(a, 0, path.Count - 1);
			b = (int) MathUtility.Clamp(b, 0, path.Count - 1);
			c = (int) MathUtility.Clamp(c, 0, path.Count - 1);
			d = (int) MathUtility.Clamp(d, 0, path.Count - 1);
			
			float scale = 1f / (path.Count - 1);
			t = (t - b * scale) / scale;
			return CatmullRom(path[a], path[b], path[c], path[d], t);
		}
		
		/// <summary>
		/// Get each point on the curve at a specified interval.
		/// </summary>
		/// <param name="interval">How far the calculation should step on the curve.</param>
		/// <para>Interval should lie in the range [0..1] where 0 represents</para>
		/// <para>the very beginning of the spline and 1 represents the end.</para>
		/// <returns>Each point along the spline, at the specified interval.</returns>
		public IEnumerable<float> Step(float interval)
		{
			for (float t = 0; t < 1; t += interval)
				yield return Point(t);
			
			yield return Point(1);
		}
		
		/// <summary>
		/// Reverse the order of the points in the spline.
		/// </summary>
		public void Reverse()
		{
			path.Reverse();
		}
				
		private static float CatmullRom(float a, float b, float c, float d, float t)
        {
            double t2 = t * t;
			double t3 = t2 * t;
			return (float) (0.5 * (((2.0 * b + (c - a) * t) + ((2.0 * a - 5.0 * b + 4.0 * c - d) * t2) + (3.0 * b - a - 3.0 * c + d) * t3) ));
        }
		
		private List<float> path;
		
		public IEnumerator<float> GetEnumerator()
		{
			return path.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
