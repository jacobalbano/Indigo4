
using System;
using System.Runtime.CompilerServices;

namespace Indigo.Utility
{
	public static class MathUtility
	{
		/// <summary>
		/// Multiply a radian angle value by this to convert it to degrees.
		/// </summary>
		public const float DEG = 180f / (float) Math.PI;
		
		/// <summary>
		/// Multiply a degree angle by this to convert it to radians.
		/// </summary>
		public const float RAD = (float) Math.PI / 180f;
		
		/// <summary>
		/// Finds the angle (in degrees) from point 1 to point 2.
		/// </summary>
		/// <param name="x1">The first x-position.</param>
		/// <param name="y1">The first y-position.</param>
		/// <param name="x2">The second x-position.</param>
		/// <param name="y2">The second y-position.</param>
		/// <returns>The angle from (x1, y1) to (x2, y2).</returns>
		public static float Angle(float x1, float y1, float x2, float y2)
		{
			double a = Math.Atan2(y2 - y1, x2 - x1) * DEG;
			return (float) (a < 0 ? a + 360 : a);
		}
		
		/// <summary>
		/// Sets the x/y values of an object to a vector of the specified angle and length.
		/// </summary>
		/// <param name="X">X coordinate of the object to set.</param>
		/// <param name="Y">Y coordinate of the object to set.</param>
		/// <param name="angle">The angle of the vector, in degrees.</param>
		/// <param name="length">The distance to the vector from (0, 0).</param>
		/// <param name="xOffset">X offset.</param>
		/// <param name="yOffset">Y offset.</param>
		public static void AngleXY(ref float X, ref float Y, float angle, float length, float xOffset = 0, float yOffset = 0)
		{
			angle *= RAD;
			X = (float) Math.Cos(angle) * length + xOffset;
			Y = (float) Math.Sin(angle) * length + yOffset;
		}
		
		/// <summary>
		/// Gets the difference of two angles, wrapped around to the range -180 to 180.
		/// </summary>
		/// <param name="a">First angle in degrees.</param>
		/// <param name="b">Second angle in degrees.</param>
		/// <returns>Difference in angles, wrapped around to the range -180 to 180.</returns>
		public static float AngleDiff(float a, float b)
		{
			float diff = a - b;
			
			while (diff > 180) diff -= 360;
			while (diff <= -180) diff += 360;

			return diff;
		}
		
		/// <summary>
		/// Find the distance between two points.
		/// </summary>
		/// <param name="x1">The first x-position.</param>
		/// <param name="y1">The first y-position.</param>
		/// <param name="x2">The second x-position.</param>
		/// <param name="y2">The second y-position.</param>
		/// <returns>The distance.</returns>
		public static float Distance(float x1, float y1, float x2, float y2)
		{
			return (float) Math.Sqrt(DistanceSquared(x1, y1, x2, y2));
		}
		
		/// <summary>
		/// Find the squared distance between two points.
		/// </summary>
		/// <param name="x1">The first x-position.</param>
		/// <param name="y1">The first y-position.</param>
		/// <param name="x2">The second x-position.</param>
		/// <param name="y2">The second y-position.</param>
		/// <returns>The squared distance.</returns>
		public static float DistanceSquared(float x1, float y1, float x2, float y2)
		{
			return (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
		}
		
		/// <summary>
		/// Find the distance between two rectangles. Will return 0 if the rectangles overlap.
		/// </summary>
		/// <param name="x1">The x-position of the first rect.</param>
		/// <param name="y1">The y-position of the first rect.</param>
		/// <param name="w1">The width of the first rect.</param>
		/// <param name="h1">The height of the first rect.</param>
		/// <param name="x2">The x-position of the second rect.</param>
		/// <param name="y2">The y-position of the second rect.</param>
		/// <param name="w2">The width of the second rect.</param>
		/// <param name="h2">The height of the second rect.</param>
		/// <returns>The distance.</returns>
		public static float DistanceRects(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
		{
			if (x1 < x2 + w2 && x2 < x1 + w1)
			{
				if (y1 < y2 + h2 && y2 < y1 + h1) return 0;
				if (y1 > y2) return y1 - (y2 + h2);
				return y2 - (y1 + h1);
			}
			if (y1 < y2 + h2 && y2 < y1 + h1)
			{
				if (x1 > x2) return x1 - (x2 + w2);
				return x2 - (x1 + w1);
			}
			if (x1 > x2)
			{
				if (y1 > y2) return Distance(x1, y1, (x2 + w2), (y2 + h2));
				return Distance(x1, y1 + h1, x2 + w2, y2);
			}
			if (y1 > y2) return Distance(x1 + w1, y1, x2, y2 + h2);
			return Distance(x1 + w1, y1 + h1, x2, y2);
		}
		
		/// <summary>
		/// Find the distance between a point and a rectangle. Returns 0 if the point is within the rectangle.
		/// </summary>
		/// <param name="px">The x-position of the point.</param>
		/// <param name="py">The y-position of the point.</param>
		/// <param name="rx">The x-position of the rect.</param>
		/// <param name="ry">The y-position of the rect.</param>
		/// <param name="rw">The width of the rect.</param>
		/// <param name="rh">The height of the rect.</param>
		/// <returns>The distance.</returns>
		public static float DistanceRectPoint(float px, float py, float rx, float ry, float rw, float rh)
		{
			if (px >= rx && px <= rx + rw)
			{
				if (py >= ry && py <= ry + rh) return 0;
				if (py > ry) return py - (ry + rh);
				return ry - py;
			}
			if (py >= ry && py <= ry + rh)
			{
				if (px > rx) return px - (rx + rw);
				return rx - px;
			}
			if (px > rx)
			{
				if (py > ry) return Distance(px, py, rx + rw, ry + rh);
				return Distance(px, py, rx + rw, ry);
			}
			if (py > ry) return Distance(px, py, rx, ry + rh);
			return Distance(px, py, rx, ry);
		}
		
		public static bool Between<T>(T value, T min, T max) where T : IComparable<T>
		{
			return value.CompareTo(min) > 0 && value.CompareTo(max) < 0;
		}
		
		/// <summary>
		/// Clamps the value within the minimum and maximum values.
		/// </summary>
		/// <param name="value">The value to evaluate.</param>
		/// <param name="min">The minimum range.</param>
		/// <param name="max">The maximum range.</param>
		/// <returns>The clamped value.</returns>
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (max.CompareTo(min) > 0)
			{
				if (value.CompareTo(min) < 0) return min;
				else if (value.CompareTo(max) > 0) return max;
				else return value;
			}
			else
			{
				// Min/max swapped
				if (value.CompareTo(max) < 0) return max;
				else if (value.CompareTo(min) > 0) return min;
				else return value;
			}
		}

        /// <summary>
        /// Clamps the value within the minimum and maximum values.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="min">The minimum range.</param>
        /// <param name="max">The maximum range.</param>
        /// <returns>The clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
		{
			if (max > min)
			{
				if (value < min) return min;
				else if (value > max) return max;
				else return value;
			}
			else
			{
				// Min/max swapped
				if (value < max) return max;
				else if (value > min) return min;
				else return value;
			}
		}

        /// <summary>
        /// Clamps the value within the minimum and maximum values.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="min">The minimum range.</param>
        /// <param name="max">The maximum range.</param>
        /// <returns>The clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
		{
			if (max > min)
			{
				if (value < min) return min;
				else if (value > max) return max;
				else return value;
			}
			else
			{
				// Min/max swapped
				if (value < max) return max;
				else if (value > min) return min;
				else return value;
			}
		}
		
		public static void Clamp<T>(ref T value, T min, T max) where T : IComparable<T>
		{
			value = Clamp(value, min, max);
		}

        /// <summary>
        /// Clamps the object inside the rectangle.
        /// </summary>
        /// <param name="objX">The X property of the object to clamp.</param>
        /// <param name="objY">The Y property of the object to clamp.</param>
        /// <param name="x">Rectangle's x.</param>
        /// <param name="y">Rectangle's y.</param>
        /// <param name="width">Rectangle's width.</param>
        /// <param name="height">Rectangle's height.</param>
        /// <param name="padding">Optional padding around the edges.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClampInRect(ref float objX, ref float objY, float x, float y, float width, float height, float padding = 0)
		{
			objX = Clamp(objX, x + padding, x + width - padding);
			objY = Clamp(objY, y + padding, y + height - padding);
		}
		
		/// <summary>
		/// Transfers a value from one scale to another scale. For example, scale(.5, 0, 1, 10, 20) == 15, and scale(3, 0, 5, 100, 0) == 40.
		/// </summary>
		/// <param name="value">The value on the first scale.</param>
		/// <param name="min">The minimum range of the first scale.</param>
		/// <param name="max">The maximum range of the first scale.</param>
		/// <param name="min2">The minimum range of the second scale.</param>
		/// <param name="max2">The maximum range of the second scale.</param>
		/// <returns>The scaled value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Scale(float value, float min, float max, float min2, float max2)
		{
			return min2 + ((value - min) / (max - min)) * (max2 - min2);
		}

        /// <summary>
        /// Transfers a value from one scale to another scale, but clamps the return value within the second scale.
        /// </summary>
        /// <param name="value">The value on the first scale.</param>
        /// <param name="min">The minimum range of the first scale.</param>
        /// <param name="max">The maximum range of the first scale.</param>
        /// <param name="min2">The minimum range of the second scale.</param>
        /// <param name="max2">The maximum range of the second scale.</param>
        /// <returns>The scaled and clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ScaleClamped(float value, float min, float max, float min2, float max2)
		{
			value = min2 + ((value - min) / (max - min)) * (max2 - min2);
			if (max2 > min2)
			{
				value = value < max2 ? value : max2;
				return value > min2 ? value : min2;
			}
			value = value < min2 ? value : min2;
			return value > max2 ? value : max2;
		}
	}
}
