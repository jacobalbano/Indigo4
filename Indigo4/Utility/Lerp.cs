using System;

namespace Indigo.Utility
{
	/// <summary>
	/// Description of Lerp.
	/// </summary>
	public static class Lerp
	{
		/// <summary>
		/// Linear interpolation between two values.
		/// </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="t">Interpolation factor</param>
		/// <returns>When t=0, returns a. When t=1, returns b. When t=0.5, will return halfway between a and b. Etc.</returns>
		public static float Float(float a, float b, float t)
		{
			return a + (b - a) * t;
		}
		
		/// <summary>
		/// Linear interpolation between two colors.
		/// </summary>
		/// <param name="from">First color.</param>
		/// <param name="to">Second color.</param>
		/// <param name="t">Interpolation value. Clamped to the range [0, 1].</param>
		/// <returns>RGB component-interpolated color value.</returns>
		public static int Color(int from, int to, float t)
		{
			if (t <= 0) { return from; }
			if (t >= 1) { return to; }
			float a = from >> 24 & 0xFF;
			float r = from >> 16 & 0xFF;
			float g = from >> 8 & 0xFF;
			float b = from & 0xFF;
			
			float da = (to >> 24 & 0xFF) - a;
			float dr = (to >> 16 & 0xFF) - r;
			float db = (to >> 8 & 0xFF) - g;
			float dg = (to & 0xFF) - b;

			a += da * t;
			r += dr * t;
			g += dg * t;
			b += db * t;
		
			return (int) a << 24 | (int) r << 16 | (int) g << 8 | (int) b;
		}
		
		//public static Color Color(Color from, Color to, float t)
		//{
		//	return new Color(Color(Indigo.Graphics.Color.ToHex(from), Indigo.Graphics.Color.ToHex(to), t));
		//}
	}
}
