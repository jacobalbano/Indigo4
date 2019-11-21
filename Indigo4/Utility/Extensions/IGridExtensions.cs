using Indigo.Utility;
using System.Text;
using System.Text.RegularExpressions;

public static class IGridExtensions
{
    public static bool IsValid<T>(this IGrid<T> grid, int col, int row)
    {
        return col >= 0 && row >= 0 && col < grid.Columns && row < grid.Rows;
    }
        
    public static void SetRect<T>(this IGrid<T> grid, int col, int row, int colsWide, int rowsTall, T value)
    {
        for (int i = 0, x = col; i < colsWide; ++i, ++x)
        {
            for (int j = 0, y = row; j < rowsTall; ++j, ++y)
            {
                if (grid.IsValid(x, y))
                    grid[x, y] = value;
            }
        }
    }

    public static bool CheckCellSafely(this IGrid<bool> grid, int col, int row)
    {
        if (col < 0 || row < 0)
            return false;

        if (col >= grid.Columns || row >= grid.Rows)
            return false;

        return grid[col, row];
    }
    /// <summary>
    /// Loads the tile index data from a string.
    /// The implicit array should not be bigger than the grid.
    /// </summary>
    /// <param name="str">The string data, which is a set of tile values separated by the columnSep and rowSep strings.</param>
    /// <param name="columnSeperator">The string that separates each tile value on a row, default is ",".</param>
    /// <param name="rowSeperator">The string that separates each row of tiles; default is "\n".</param>
    public static void LoadFromString(this IGrid<int> data, string str, string columnSeperator = ",", string rowSeperator = "\n")
    {
        DoLoad(data, ParseInt, str, columnSeperator, rowSeperator);
    }

    /// <summary>
    /// Loads the tile index data from a string.
    /// The implicit array should not be bigger than the grid.
    /// </summary>
    /// <param name="str">The string data, which is a set of tile values separated by the columnSep and rowSep strings.</param>
    /// <param name="columnSeperator">The string that separates each tile value on a row, default is ",".</param>
    /// <param name="rowSeperator">The string that separates each row of tiles; default is "\n".</param>
    public static void LoadFromString(this IGrid<bool> data, string str, string columnSeperator = ",", string rowSeperator = "\n")
    {
        DoLoad(data, ParseBool, str, columnSeperator, rowSeperator);
    }

    /// <summary>
    /// Loads the tile index data from a string.
    /// The implicit array should not be bigger than the grid.
    /// </summary>
    /// <param name="str">The string data, which is a set of tile values separated by the columnSep and rowSep strings.</param>
    /// <param name="columnSeperator">The string that separates each tile value on a row, default is ",".</param>
    /// <param name="rowSeperator">The string that separates each row of tiles; default is "\n".</param>
    public static void LoadFromString(this IGrid<string> data, string str, string columnSeperator = ",", string rowSeperator = "\n")
    {
        DoLoad(data, s => s, str, columnSeperator, rowSeperator);
    }

    public static string SaveToString<T>(this IGrid<T> data, string columnSep, string rowSep)
    {
        var builder = new StringBuilder();
        for (int y = 0; y < data.Rows; ++y)
        {
            for (int x = 0; x < data.Columns; ++x)
            {
                builder.Append(data[x, y]);
                if (x != data.Columns - 1)
                    builder.Append(columnSep);
            }

            if (y != data.Rows - 1)
                builder.Append(rowSep);
        }

        return builder.ToString();
    }

    private static void DoLoad<T>(IGrid<T> data, Parser<T> parse, string str, string columnSeperator = ",", string rowSeperator = "\n")
    {
        var csep = Regex.Escape(columnSeperator);
        var rsep = Regex.Escape(rowSeperator);

        var rows = Regex.Split(str, rsep);
        var startX = csep.Length == 0 ? 1 : 0;

        for (int i = 0, r = 0; i < rows.Length && r < data.Rows; i++, r++)
        {
            var columns = Regex.Split(rows[i], csep);
            for (int j = startX, c = 0; j < columns.Length && c < data.Columns; j++, c++)
                data[c, r] = parse(columns[j]);
        }
    }

    private static int ParseInt(string value)
    {
        int.TryParse(value, out int val);
        return val;
    }

    private static bool ParseBool(string value)
    {
        int.TryParse(value, out int val);
        return val > 0;
    }

    private delegate T Parser<T>(string value);
}