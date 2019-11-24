using Indigo.Components.Colliders;
using Indigo.Content;
using Indigo.Engine;
using Indigo.Engine.Rendering;
using Indigo.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using XNA = Microsoft.Xna.Framework;

namespace Indigo.Components.Graphics
{
    /// <summary>
    /// Batched tile rendering.
    /// </summary>
    public class Tilemap : Graphic, IGrid<int>
    {
        /// <summary>
        /// If x/y positions should be used instead of columns/rows.
        /// </summary>
        public bool UsePositions;

        public Texture Texture { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tileset">The source tileset texture.</param>
        /// <param name="width">Width of the tilemap graphic, in pixels.</param>
        /// <param name="height">Height of the tilemap graphic, in pixels.</param>
        /// <param name="tileWidth">Tile width.</param>
        /// <param name="tileHeight">Tile height.</param>
        public Tilemap(Texture tileset, int width, int height, int tileWidth, int tileHeight)
        {
            // set some tilemap information
            Width = width;
            Height = height;

            Columns = (int)Math.Ceiling(Width / (float)tileWidth);
            Rows = (int)Math.Ceiling(Height / (float)tileHeight);

            if (Columns == 0 || Rows == 0)
                throw new Exception("Invalid source size.");

            // initialize map
            _tile = new XNA.Rectangle(0, 0, tileWidth, tileHeight);
            data = new GridData<int>(Columns, Rows);

            for (int x = 0; x < Columns; ++x)
            {
                for (int y = 0; y < Rows; ++y)
                {
                    data[x, y] = -1;
                }
            }

            Texture = tileset;
            if (Texture == null)
                throw new Exception("Invalid tileset texture provided.");

            _setColumns = Texture.Width / tileWidth;
            _setRows = Texture.Height / tileHeight;
            _setCount = _setColumns * _setRows;
        }
        /// <summary>
        /// Copy the tiles from this tilemap to another.
        /// </summary>
        /// <param name="other">The tilemap that will have its tiles set to match this.</param>
        public void CopyTo(Tilemap other)
        {
            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    other.SetTile(col, row, GetTile(col, row));
                }
            }
        }

        /// <summary>
        /// Set the tile at the position.
        /// </summary>
        /// <param name="column">Column of the tile.</param>
        /// <param name="row">Column of the tile.</param>
        /// <param name="index">The tile index.</param>
        public virtual void SetTile(int column, int row, int index = 0)
        {
            if (UsePositions)
            {
                column /= TileWidth;
                row /= TileHeight;
            }

            index %= _setCount;
            column %= Columns;
            row %= Rows;
            data[column, row] = index;
        }

        /// <summary>
        /// Clears the tile at the position.
        /// </summary>
        /// <param name="column">Column of the tile.</param>
        /// <param name="row">Column of the tile.</param>
        public void ClearTile(int column, int row)
        {
            if (UsePositions)
            {
                column /= TileWidth;
                row /= TileHeight;
            }

            column %= Columns;
            row %= Rows;
            data[column, row] = -1;
        }

        /// <summary>
        /// Check that the tile is valid.
        /// </summary>
        /// <param name="column">Tile column.</param>
        /// <param name="row">Tile row.</param>
        /// <returns>If the tile is valid.</returns>
        public bool CheckTile(int column, int row)
        {
            return !(column < 0 || column > Columns - 1 || row < 0 || row > Rows - 1);
        }

        /// <summary>
        /// Get the index of a tile at the position.
        /// </summary>
        /// <param name="column">Column of the tile.</param>
        /// <param name="row">Column of the tile.</param>
        public int GetTile(int column, int row)
        {
            if (UsePositions)
            {
                column = column / TileWidth;
                row = row / TileHeight;
            }

            return data[column % Columns, row % Rows];
        }

        /// <summary>
        /// Fills the rectangular region with tiles.
        /// </summary>
        /// <param name="column">First tile column.</param>
        /// <param name="row">First tile row.</param>
        /// <param name="width">Width in tiles.</param>
        /// <param name="height">Height in tiles.</param>
        /// <param name="index">Index of the tile.</param>
        public void SetRect(int column, int row, int width, int height, int index)
        {
            if (UsePositions)
            {
                column = column / TileWidth;
                row = row / TileHeight;
                width = width / TileWidth;
                height = height / TileHeight;
            }

            column %= Columns;
            row %= Rows;

            int c = column,
                r = column + width,
                b = row + height;
            bool u = UsePositions;
            UsePositions = false;

            while (row < b)
            {
                while (column < r)
                {
                    SetTile(column, row, index);
                    column++;
                }

                column = c;
                row++;
            }

            UsePositions = u;
        }

        /// <summary>
        /// Clears the rectangular region of tiles.
        /// </summary>
        /// <param name="column">First tile column.</param>
        /// <param name="row">First tile row.</param>
        /// <param name="width">Width in tiles.</param>
        /// <param name="height">Height in tiles.</param>
        public void ClearRect(int column, int row, int width, int height)
        {
            SetRect(column, row, width, height, -1);
        }

        /// <summary>
        /// Loads the Tilemap tile index data from a string.
        /// The implicit array should not be bigger than the Tilemap.
        /// </summary>
        /// <param name="str">The string data, which is a set of tile values separated by the columnSep and rowSep strings.</param>
        /// <param name="columnSep">The string that separates each tile value on a row, default is ",".</param>
        /// <param name="rowSep">The string that separates each row of tiles, default is "\n".</param>
        public void LoadFromString(string str, string columnSep = ",", string rowSep = "\n")
        {
            data.LoadFromString(str, columnSep, rowSep);
        }

        /// <summary>
        /// Saves the Tilemap tile index data to a string.
        /// </summary>
        /// <param name="columnSep">The string that separates each tile value on a row, default is ",".</param>
        /// <param name="rowSep">The string that separates each row of tiles, default is "\n".</param>
        /// <returns>The string version of the array.</returns>
        public string SaveToString(string columnSep = ",", string rowSep = "\n")
        {
            return data.SaveToString(columnSep, rowSep);
        }

        /// <summary>
        /// Create a grid based on this tilemap.
        /// </summary>
        /// <param name="solidTiles">The tile indeces to be treated as solid.</param>
        /// <returns></returns>
        public HitGrid CreateGrid(params int[] solidTiles)
        {
            var tiles = solidTiles.ToList();
            var grid = new HitGrid(Width, Height, TileWidth, TileHeight);
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    grid[col, row] = tiles.Contains(data[col, row]);
                }
            }

            return grid;
        }

        /// <summary>
        /// Gets the index of a tile, based on its column and row in the tileset.
        /// </summary>
        /// <param name="tilesColumn">Tileset column.</param>
        /// <param name="tilesRow">Tileset row.</param>
        /// <returns>Index of the tile.</returns>
        public int GetIndex(int tilesColumn, int tilesRow)
        {
            return (tilesRow % _setRows) * _setColumns + (tilesColumn % _setColumns);
        }

        public int this[int column, int row]
        {
            get { return GetTile(column, row); }
            set { SetTile(column, row, value); }
        }

        protected override void OnRender(RenderContext ctx)
        {
            var inverse = Matrix.Invert(ctx.RenderTransform);
            
            //  transform corners to get camera bounds
            float maxWidth = ctx.ViewportWidth, maxHeight = ctx.ViewportHeight;
            Vector2
                tl = Vector2.Transform(new Vector2(0,         0),           inverse),
                tr = Vector2.Transform(new Vector2(maxWidth,  0),           inverse),
                br = Vector2.Transform(new Vector2(maxWidth,  maxHeight),   inverse),
                bl = Vector2.Transform(new Vector2(0,         maxHeight),   inverse);
            
            //  left, top, right, bottom
            int l = (int) Math.Max(0, Math.Min(tl.X, Math.Min(tr.X, Math.Min(br.X, bl.X))) / TileWidth),
                t = (int) Math.Max(0, Math.Min(tl.Y, Math.Min(tr.Y, Math.Min(br.Y, bl.Y))) / TileHeight),
                r = (int) Math.Min(Columns, Math.Ceiling(Math.Max(tl.X, Math.Max(tr.X, Math.Max(br.X, bl.X))) / TileWidth)),
                b = (int) Math.Min(Rows, Math.Ceiling(Math.Max(tl.Y, Math.Max(tr.Y, Math.Max(br.Y, bl.Y))) / TileHeight));
            
            for (int tx = l; tx < r; tx++)
            {
                for (int ty = t; ty < b; ty++)
                {
                    int tileIndex = GetTile(
                        MathUtility.Clamp(tx, 0, Columns),
                        MathUtility.Clamp(ty, 0, Rows)
                    );

                    if (tileIndex >= 0)
                    {
                        CalcTile(tx, ty, tileIndex, out var tilePos, out var tileRect);
                        ctx.SpriteBatch.Draw(Texture.XnaTexture, tilePos, tileRect, Color.White);
                    }
                }
            }
        }

        private void CalcTile(int col, int row, int index, out XNA.Rectangle spriteRect, out XNA.Rectangle textureRect)
        {
            var textureX = (index % Columns) * TileWidth;
            var textureY = (index / Columns) * TileHeight;
            var spriteX = col * TileWidth;
            var spriteY = row * TileHeight;

            spriteRect = new XNA.Rectangle(
                spriteX,
                spriteY,
                MathUtility.Clamp(spriteX + TileWidth, 0, Width) - spriteX,
                MathUtility.Clamp(spriteY + TileHeight, 0, Height) - spriteY
            );

            textureRect = new XNA.Rectangle(
                textureX,
                textureY,
                spriteRect.Width,
                spriteRect.Height
            );
        }
        
        /// <summary>
        /// Width of the graphic.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the graphic.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The tile width.
        /// </summary>
        public int TileWidth { get { return _tile.Width; } }

        /// <summary>
        /// The tile height.
        /// </summary>
        public int TileHeight { get { return _tile.Height; } }

        /// <summary>
        /// How many tiles the tilemap has.
        /// </summary>
        public int TileCount
        {
            get { return _setCount; }
        }

        /// <summary>
        /// How many columns the tilemap has.
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// How many rows the tilemap has.
        /// </summary>
        public int Rows { get; private set; }

        // Tilemap information.
        private GridData<int> data;

        // Tileset information.
        private int _setColumns;
        private int _setRows;
        private int _setCount;
        private XNA.Rectangle _tile;
    }
}
