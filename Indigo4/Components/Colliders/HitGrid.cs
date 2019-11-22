using Indigo.Engine;
using Indigo.Utility;
using System;

namespace Indigo.Components.Colliders
{
	public class HitGrid : Collider, IGrid<bool>,
        ICollisionCheck<HitCircle>,
        ICollisionCheck<Hitbox>,
        ICollisionCheck<HitGrid>
	{
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        public HitGrid(int width, int height, int cellWidth, int cellHeight)
		{
			Width = width;
			Height = height;
			CellWidth = cellWidth;
			CellHeight = cellHeight;
			Columns = Width / CellWidth;
			Rows = Height / CellHeight;
			data = new GridData<bool>(Columns, Rows);
		}
		
		public bool this[int col, int row]
		{
			get { return data[col, row]; }
			set { data[col, row] = value; }
		}

        /// <summary>
        /// Creates a new HitGrid of the same size and sets its contents to the opposite of this one.
        /// Does not set type or anything else!
        /// </summary>
        /// <returns>The inverted grid</returns>
        public HitGrid Invert()
        {
            var result = new HitGrid(Width, Height, CellWidth, CellHeight);
            for (int c = 0; c < Columns; c++)
            {
                for (int r = 0; r < Rows; r++)
                {
                    result[c, r] = !this[c, r];
                }
            }

            return result;
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

        public override bool CollidesWith(float x, float y)
        {
            onepx.X = x;
            onepx.Y = y;

            return onepx.CollidesWith(this);
        }

        public bool CollidesWith(HitCircle other)
		{
			return other.CollidesWith(this);
		}

        public bool CollidesWith(Hitbox other)
        {
            return other.CollidesWith(this);
        }
        
        public bool CollidesWith(HitGrid other)
        {
            // Find the X edges
            var ax1 = Left;
            var ax2 = Right;
            var bx1 = other.Left;
			var bx2 = other.Right;
			if (ax2 < bx1 || ax1 > bx2) return false;

            // Find the Y edges
            var ay1 = Top;
            var ay2 = Bottom;
            var by1 = other.Top;
            var by2 = other.Bottom;
			if (ay2 < by1 || ay1 > by2) return false;

            // Find the overlapping area
            var ox1 = ax1 > bx1 ? ax1 : bx1;
			var oy1 = ay1 > by1 ? ay1 : by1;
			var ox2 = ax2 < bx2 ? ax2 : bx2;
			var oy2 = ay2 < by2 ? ay2 : by2;

            // Find the smallest tile size, and snap the top and left overlapping
            // edges to that tile size. This ensures that corner checking works
            // properly.
            float tw, th;
            if (CellWidth < other.CellWidth)
            {
                tw = CellWidth;
                ox1 -= Left;
                ox1 = (int)(ox1 / tw) * tw;
                ox1 += Left;
            }
            else
            {
                tw = other.CellWidth;
                ox1 -= other.Left;
                ox1 = (int)(ox1 / tw) * tw;
                ox1 += other.Left;
            }
            if (CellHeight < other.CellHeight)
            {
                th = CellHeight;
                oy1 -= Top;
                oy1 = (int)(oy1 / th) * th;
                oy1 += Top;
            }
            else
            {
                th = other.CellHeight;
                oy1 -= other.Top;
                oy1 = (int)(oy1 / th) * th;
                oy1 += other.Top;
            }

            // Step through the overlapping rectangle
            for (float y = oy1; y < oy2; y += th)
			{
				// Get the row indices for the top and bottom edges of the tile
				int ar1 = (int) (y - Top) / CellHeight;
				int br1 = (int) (y - other.Top) / other.CellHeight;
				int ar2 = (int) ((y - Top) + (th - 1)) / CellHeight;
				int br2 = (int) ((y - other.Top) + (th - 1)) / other.CellHeight;
				for (float x = ox1; x < ox2; x += tw)
				{
					// Get the column indices for the left and right edges of the tile
					int ac1 = (int) (x - Left) / CellWidth;
					int bc1 = (int) (x - other.Left) / other.CellWidth;
					int ac2 = (int) ((x - Left) + (tw - 1)) / CellWidth;
					int bc2 = (int) ((x - other.Left) + (tw - 1)) / other.CellWidth;
					
					// Check all the corners for collisions
					if ((this.CheckCellSafely(ac1, ar1) && other.CheckCellSafely(bc1, br1))
					 || (this.CheckCellSafely(ac2, ar1) && other.CheckCellSafely(bc2, br1))
					 || (this.CheckCellSafely(ac1, ar2) && other.CheckCellSafely(bc1, br2))
					 || (this.CheckCellSafely(ac2, ar2) && other.CheckCellSafely(bc2, br2)))
					{
						return true;
					}
				}
			}
			
			return false;
        }
        
        private GridData<bool> data;
        private Hitbox onepx = new Hitbox(1, 1);
    }
}
