using Indigo.Engine;
using Indigo.Utility;
using Microsoft.Xna.Framework;
using System;

namespace Indigo.Components.Colliders
{
	public class HitCircle : Collider,
		ICollisionCheck<HitCircle>,
		ICollisionCheck<Hitbox>,
		ICollisionCheck<HitGrid>
	{
		public float Radius
		{
			get { return r; }
			set { UpdateBoundsIfChanged(Math.Max(0, value), ref r); }
		}
		
		public override float Left
		{
			get { return X + AssignedParent.X - OriginX; }
		}
		
		public override float Top
		{
			get { return Y + AssignedParent.Y - OriginY; }
		}
		
		public override float Right
		{
			get { return Left + Radius * 2; }
		}
		
		public override float Bottom
		{
			get { return Top + Radius * 2; }
		}
		
		public HitCircle(float radius = 0)
		{
			Radius = radius;
		}

        public void CenterOrigin()
        {
            OriginX = Radius;
            OriginY = Radius;
        }

        public override bool CollidesWith(float x, float y)
        {
            return MathUtility.DistanceSquared(Left + Radius, Top + Radius, x, y) < Radius * Radius;
        }

        public bool CollidesWith(Hitbox other)
		{
			return Radius > MathUtility.DistanceRectPoint(Left + Radius, Top + Radius, other.Left, other.Top, other.Width, other.Height);
		}
		
		public bool CollidesWith(HitCircle other)
		{
			if (!CheckBoundingBoxes(other))
				return false;

            var tp = AssignedParent;
            var op = other.AssignedParent;
			var tc = new Vector2(tp.X + X + Radius - OriginX, tp.Y + Y + Radius - OriginY);
			var oc = new Vector2(op.X + other.X + other.Radius - other.OriginX, op.Y + other.Y + other.Radius - other.OriginY);
			return MathUtility.DistanceSquared(tc.X, tc.Y, oc.X, oc.Y) < (Radius + other.Radius) * (Radius + other.Radius);
		}
		
		public bool CollidesWith(HitGrid other)
		{
			if (!CheckBoundingBoxes(other))
				return false;
			
			if (Radius <= MathUtility.DistanceRectPoint(Left + Radius, Top + Radius, other.Left, other.Top, other.Width, other.Height))
				return false;

            var thisCenter = new Vector2(Left + Radius, Top + Radius);
            var oCenter = new Vector2(other.Left + other.Width / 2f, other.Top + other.Height / 2f);

			int rowStart = 0, rowMax = other.Rows, rowStep = 1;
			int colStart = 0, colMax = other.Columns, colStep = 1;
			
			if (thisCenter.X > oCenter.X)
			{
				colStart = other.Columns - 1;
				colStep = colMax = -1;
			}
			
			if (thisCenter.Y > oCenter.Y)
			{
				rowStart = other.Rows - 1;
				rowStep = rowMax = -1;
			}
			
			for (int row = rowStart; row != rowMax; row += rowStep)
			{
				for (int col = colStart; col != colMax; col += colStep)
				{
					if (!other[col, row])
						continue;
					
					var cellPos = new Vector2(other.Left + col * other.CellWidth, other.Top + row * other.CellWidth);
					if (Radius > MathUtility.DistanceRectPoint(thisCenter.X, thisCenter.Y, cellPos.X, cellPos.Y, other.CellWidth, other.CellHeight))
						return true;
				}
			}
			
			return false;
		}
		
		private float r;
	}
}
