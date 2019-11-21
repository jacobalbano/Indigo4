using Indigo.Components.Graphics;
using Indigo.Utility;
using System;
namespace Indigo.Loaders
{
	/// <summary>
	/// Loader for tilesets created with Javi Cepa's AutoTileGen.
	/// </summary>
	public class AutoTileSet
	{
		/// <summary>
		/// The grid used to set the tiles in the tilemap.
		/// </summary>
		public IGrid<bool> Grid { get; private set; }
		
		/// <summary>
		/// The tilemap.
		/// </summary>
		public Tilemap Tilemap { get; private set; }
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tilemap">The tilemap to set tiles in.</param>
		/// <param name="grid">The grid to get tile data from.</param>
		/// <param name="setImmediately">Whether the tilemap should be filled in immediately.</param>
		public AutoTileSet(Tilemap tilemap, IGrid<bool> grid, bool setImmediately = true)
		{
			Tilemap = tilemap;
			Grid = grid;
			
			if (setImmediately)
			{
				CreateFromGrid(tilemap, grid);
			}
		}
		
		/// <summary>
		/// Rebuild the tilemap with data from the Grid.
		/// </summary>
		public void Refresh()
		{
			CreateFromGrid(Tilemap, Grid);
		}
		
		/// <summary>
		/// Rebuild the given tile and the eight tiles surrounding it.
		/// </summary>
		/// <param name="column">The column of tile to rebuild.</param>
		/// <param name="row">The row of the tile to rebuild.</param>
		public void RefreshTile(int column, int row)
		{
			for (int x = -1; x < 2; x++)
			{
				for (int y = -1; y < 2; y++)
				{
					int c = x + column, r = y + row;
					if (Grid.IsValid(c, r))
						SetTile(Tilemap, Grid, c, r);
				}
			}
		}
		
		#region statics
		
		static AutoTileSet()
		{
			near = new bool[8];
		}
		
		/// <summary>
		/// Set every tile in the tilemap based on a collision grid
		/// </summary>
		/// <param name="map">The tilemap to set tiles in.</param>
		/// <param name="grid">The grid to get tile data from.</param>
		public static void CreateFromGrid(Tilemap map, IGrid<bool> grid)
        {
			for (int x = 0; x < grid.Columns; x++)
			{
				for (int y = 0; y < grid.Rows; y++)
				{
					SetTile(map, grid, x, y);
				}
			}
        }
        
		/// <summary>
		/// Set a single tile in a tilemap based on a collision grid.
		/// </summary>
		/// <param name="map">The tilemap to set tiles in.</param>
		/// <param name="grid">The grid to get tile data from.</param>
		/// <param name="x">The column of the tile to set.</param>
		/// <param name="y">The row of the tile to set.</param>
        public static void SetTile(Tilemap map, IGrid<bool> grid, int x, int y)
        {
            if (!grid[x, y])
            {
                map.SetTile(x, y, map.TileCount - 1);
                return;
            }
            
            Func<int, int, bool> test = (i, j) => grid[x + i, y + j];
            
            near[0] = test(0, -1);
            near[1] = test(1, 0);
            near[2] = test(0, 1);
            near[3] = test(-1, 0);
            near[4] = test(-1, -1);
            near[5] = test(1, -1);
            near[6] = test(1, 1);
            near[7] = test( -1, 1);
            
            //    value for a single block with no neighbors, otherwise it'll be set to (0, 0)
            int sx = 6, sy = 2;
            if (all(near[3])){sx=2;sy=2;}
            if (all(near[0])){sx=3;sy=2;}
            if (all(near[1])){sx=4;sy=2;}
            if (all(near[2])){sx=5;sy=2;}
            if (all(near[1],near[3])){sx=0;sy=2;}
            if (all(near[0],near[2])){sx=1;sy=2;}
            if (all(near[0],near[3])){sx=7;sy=1;}
            if (all(near[1],near[0])){sx=0;sy=0;}
            if (all(near[2],near[1])){sx=1;sy=0;}
            if (all(near[3],near[2])){sx=2;sy=0;}
            if (all(near[2],near[3],near[7])){sx=4;sy=3;}
            if (all(near[3],near[0],near[4])){sx=5;sy=3;}
            if (all(near[0],near[1],near[5])){sx=6;sy=3;}
            if (all(near[1],near[2],near[6])){sx=7;sy=3;}
            if (all(near[0],near[1],near[3])){sx=3;sy=0;}
            if (all(near[1],near[2],near[0])){sx=4;sy=0;}
            if (all(near[2],near[3],near[1])){sx=5;sy=0;}
            if (all(near[3],near[0],near[2])){sx=6;sy=0;}
            if (all(near[0],near[1],near[2],near[3])){sx=7;sy=4;}
            if (all(near[0],near[2],near[3],near[7])){sx=7;sy=2;}
            if (all(near[1],near[3],near[0],near[4])){sx=0;sy=1;}
            if (all(near[2],near[0],near[1],near[5])){sx=1;sy=1;}
            if (all(near[3],near[1],near[2],near[6])){sx=2;sy=1;}
            if (all(near[0],near[1],near[3],near[5])){sx=3;sy=1;}
            if (all(near[1],near[2],near[0],near[6])){sx=4;sy=1;}
            if (all(near[2],near[3],near[1],near[7])){sx=5;sy=1;}
            if (all(near[3],near[0],near[2],near[4])){sx=6;sy=1;}
            if (all(near[1],near[2],near[3],near[6],near[7])){sx=0;sy=3;}
            if (all(near[0],near[2],near[3],near[4],near[7])){sx=1;sy=3;}
            if (all(near[0],near[1],near[3],near[4],near[5])){sx=2;sy=3;}
            if (all(near[0],near[1],near[2],near[5],near[6])){sx=3;sy=3;}
            if (all(near[0],near[1],near[2],near[3],near[7])){sx=3;sy=4;}
            if (all(near[0],near[1],near[2],near[3],near[4])){sx=4;sy=4;}
            if (all(near[0],near[1],near[2],near[3],near[5])){sx=5;sy=4;}
            if (all(near[0],near[1],near[2],near[3],near[6])){sx=6;sy=4;}
            if (all(near[0],near[1],near[2],near[3],near[5],near[7])){sx=1;sy=4;}
            if (all(near[0],near[1],near[2],near[3],near[4],near[6])){sx=2;sy=4;}
            if (all(near[0],near[1],near[2],near[3],near[6],near[7])){sx=5;sy=5;}
            if (all(near[0],near[1],near[2],near[3],near[4],near[7])){sx=6;sy=5;}
            if (all(near[0],near[1],near[2],near[3],near[4],near[5])){sx=7;sy=5;}
            if (all(near[0],near[1],near[2],near[3],near[5],near[6])){sx=0;sy=4;}
            if (all(near[0],near[1],near[2],near[3],near[5],near[6],near[7])){sx=1;sy=5;}
            if (all(near[0],near[1],near[2],near[3],near[4],near[6],near[7])){sx=2;sy=5;}
            if (all(near[0],near[1],near[2],near[3],near[4],near[5],near[7])){sx=3;sy=5;}
            if (all(near[0],near[1],near[2],near[3],near[4],near[5],near[6])){sx=4;sy=5;}
            if (all(near[0],near[1],near[2],near[3],near[4],near[5],near[6],near[7])){sx=0;sy=5;}
            
            //    convert coords for top-left index
            int currentTileIndex = ( 5 - sy ) * 8 + sx;
            
            map.SetTile(x, y, currentTileIndex);
        }
        
        private static bool all(params bool[] bools)
        {
        	for (int i = 0; i < bools.Length; i++) {
        		if (!bools[i])
        			return false;
        	}
        	
        	return true;
        }
        
		private static bool[] near;
		
		#endregion
	}
}
