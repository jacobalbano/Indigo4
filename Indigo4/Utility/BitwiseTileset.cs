using Indigo.Components.Graphics;
using Indigo.Utility;
using System;

namespace Indigo.Loaders
{
	/// <summary>
	/// Loader for setting tiles in a Tilemap based on a bitwise sum of the tiles adjacent in a grid.
	/// </summary>
	public class BitwiseTileset
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
		public BitwiseTileset(Tilemap tilemap, IGrid<bool> grid, bool setImmediately = true)
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
            	map.ClearTile(x, y);
            	return;
            }
        	
            int index = 0;
        	if (grid.CheckCellSafely(x, y - 1)) index += 1;
        	if (grid.CheckCellSafely(x + 1, y)) index += 2;
        	if (grid.CheckCellSafely(x, y + 1)) index += 4;
        	if (grid.CheckCellSafely(x - 1, y)) index += 8;
        	
        	map.SetTile(x, y, index);
        }
	}
}
