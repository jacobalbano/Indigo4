
using System;
using Indigo;
using Microsoft.Xna.Framework;

namespace Indigo.Utility
{
	/// <summary>
	/// A collection of stateless easing functions.
	/// </summary>
	public static class Approach
	{
		/// <summary>
		/// Approaches the value towards the target, by the specified amount, without overshooting the target.
		/// </summary>
		/// <param name="value">The starting value.</param>
		/// <param name="target">The target that you want value to approach.</param>
		/// <param name="amount">How much you want the value to approach target by.</param>
		/// <returns>The new value.</returns>
		public static float Towards(float value, float target, float amount)
		{
			if (value < target - amount)
			{
				return value + amount;
			}
			else if (value > target + amount)
			{
				return value - amount;
			}
			else
			{
				return target;
			}
		}
		
		/// <summary>
		/// Steps X and Y coordinates towards a point.
		/// </summary>
		/// <param name="x">The X value to move</param>
		/// <param name="y">The Y value to move</param>
		/// <param name="toX">X position to step towards.</param>
		/// <param name="toY">Y position to step towards.</param>
		/// <param name="distance">The distance to step (will not overshoot target).</param>
		public static void Towards(ref float x, ref float y, float toX, float toY, float distance)
		{
			var point = new Vector2(toX - x, toY - y);
			
			if (point.Length() <= distance)
			{
				x = toX;
				y = toY;
				return;
			}

            point.Normalize();
			
			x += (float) point.X * distance;
			y += (float) point.Y * distance;
		}
		
		/// <summary>
		/// Approach one value to another by a constant distance scalar.
		/// </summary>
		/// <param name="target">The current value.</param>
		/// <param name="to">The desired value.</param>
		/// <param name="amount">The amount to move each time. Default to 0.1 (1/10 of the distance)</param>
		/// <returns>The new value;</returns>
		public static void TowardsWithDecay(ref float target, float to, float amount = 0.1f)
		{
			target += (to - target) * amount;
		}
		
		/// <summary>
		/// Smoothly change an angle to point at a position.
		/// </summary>
		/// <param name="x">The x position of the object to be rotated.</param>
		/// <param name="y">The y position of the object to be rotated.</param>
		/// <param name="targetX">The x position of the object to aim towards.</param>
		/// <param name="targetY">The y position of the object to aim towards.</param>
		/// <param name="currentAngle">The angle of the object to be rotated.</param>
		/// <param name="lookEase">How drastic the approach should be. Default to 0.3 (3/10 of the distance).</param>
		/// <returns>The resulting angle, in degrees.</returns>
		public static float Angle(float x, float y, float targetX, float targetY, float currentAngle, float lookEase = 0.3f)
		{
			currentAngle = MathUtility.RAD * currentAngle;
			float cx = (float) Math.Cos(currentAngle), cy = (float) Math.Sin(currentAngle);
			float tnormX = targetX - x, tnormY = targetY - y;
			Normalize(ref tnormX, ref tnormY);
			
			cx += (tnormX - cx) * lookEase;
			cy += (tnormY - cy) * lookEase;
			
			var angle = MathUtility.RAD * (float) Math.Atan2(cy, cx);
			angle %= 360;
			if (angle < 0)
				angle += 360;
			
			return angle;
		}
		
		public static void Angle(ref float angle, float targetAngle, float lookEase)
		{
			var point = new Vector2((float) Math.Cos(targetAngle), (float) Math.Sin(targetAngle)) * 50;
			angle = Angle(0, 0, point.X, point.Y, angle, lookEase);
		}
		
		static void Normalize(ref float x, ref float y)
		{
			if (x == 0 && y == 0)
				return;
			
			var length = (float) Math.Sqrt(x * x + y * y);
			var scale = 1f / length;
			x = x * scale;
			y = y * scale;
		}
	}
}
