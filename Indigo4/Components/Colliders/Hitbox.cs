using Indigo.Engine;
using Indigo.Utility;
using Microsoft.Xna.Framework;
using System;

namespace Indigo.Components.Colliders
{
	public class Hitbox : Collider,
		ICollisionCheck<HitCircle>,
		ICollisionCheck<Hitbox>,
        ICollisionCheck<HitGrid>
	{
		public float Width
		{
			get { return w; }
			set { UpdateBoundsIfChanged(Math.Max(0, value), ref w); }
		}
		
		public float Height
		{
			get { return h; }
			set { UpdateBoundsIfChanged(Math.Max(0, value), ref h); }
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
			get { return Left + Width; }
		}
		
		public override float Bottom
		{
			get { return Top + Height; }
		}
		
		public Hitbox(float width = 0, float height = 0)
		{
			Width = width;
			Height = height;
		}

        public override bool CollidesWith(float x, float y)
        {
            return x > Left && x < Right && y > Top && y < Bottom;
        }

        public bool CollidesWith(HitCircle other)
        {
            return other.CollidesWith(this);
		}
		
		public bool CollidesWith(Hitbox other)
		{
			return CheckBoundingBoxes(other);
		}
        
        public bool CollidesWith(HitGrid other)
        {
            if (!CheckBoundingBoxes(other))
                return false;
            
            var thisCenter = new Vector2(Left + Width / 2f, Top + Height / 2f);
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
                    if (MathUtility.DistanceRects(Left, Top, Width, Height, cellPos.X, cellPos.Y, other.CellWidth, other.CellHeight) == 0)
                        return true;
                }
            }

            return false;
        }

        public void CenterOrigin()
        {
            OriginX = Width / 2f;
            OriginY = Height / 2f;
        }

  //      public override void RenderDebug(Renderer.IRenderContext ctx)
		//{
  //          throw new NotImplementedException();
		//	//console.Lines.Rect(Left, Top, (int) Width, (int) Height, Color.Orange);
		//}

        private float w, h;
	}
}
