
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Indigo.Utility
{
	public interface IGrid<T>
	{
		T this[int column, int row] { get; set; }
		int Columns { get; }
		int Rows { get; }
	}
	
	public class GridData<T> : IGrid<T>
	{
		public int Columns { get; private set; }
		public int Rows { get; private set; }
		
		public GridData(int columns, int rows)
		{
			Columns = columns;
			Rows = rows;
			data = new T[rows, columns];
		}
		
		public GridData<T> Clone()
		{
            return new GridData<T>(Columns, Rows) { data = (T[,])data.Clone() };
		}
		
		public T this[int col, int row]
		{
			get { return data[row, col]; }
			set { data[row, col] = value; }
		}
		
		public T this[GridIndex index]
		{
			get { return data[index.Row, index.Column]; }
			set { data[index.Row, index.Column] = value; }
		}
		
		private T[,] data;
	}
	
	public struct GridIndex
	{
		public GridIndex(int column, int row)
		{
			Column = column;
			Row = row;
		}
		
		public int Column, Row;
	}
	
	public struct GridRect
	{
		public GridRect(int column, int row, int width, int height)
		{
			Column = column;
			Row = row;
			Width = width;
			Height = height;
		}
		
		public int Column, Row, Width, Height;
	}
}
