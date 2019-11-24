using Indigo.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Indigo.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XLCol : Attribute
    {
        public string Column { get; }

        public XLCol(string column)
        {
            Column = column;
        }
    }

    public static class ExcelBinder
    {
        public static T FromRow<T>(DataRow row) where T : new()
        {
            return Implementor<T>.Create(row);
        }

        public static T FromRow<T>(ExcelDocument.Row row) where T : new()
        {
            return Implementor<T>.Create(row);
        }

        private static class Implementor<T> where T : new()
        {
            public static T Create(DataRow row)
            {
                var result = FastFactory<T>.CreateInstance();

                foreach (var p in props)
                {
                    var col = p.Key;
                    var accessor = p.Value.Item2;
                    var pType = Nullable.GetUnderlyingType(p.Value.Item1) ?? p.Value.Item1;
                    accessor.SetValue(result, Convert.ChangeType(row[col], pType));
                }

                return result;
            }

            public static T Create(ExcelDocument.Row row)
            {
                var result = FastFactory<T>.CreateInstance();

                foreach (var p in props)
                {
                    var col = p.Key;
                    var data = row[col]?.GetData();
                    var accessor = p.Value.Item2;
                    var pType = Nullable.GetUnderlyingType(p.Value.Item1) ?? p.Value.Item1;
                    accessor.SetValue(result, Convert.ChangeType(data, pType));
                }

                return result;
            }

            static Implementor()
            {
                props = typeof(T)
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.IsDefined(typeof(XLCol)))
                    .ToDictionary(
                        p => p.GetCustomAttribute<XLCol>().Column,
                        p => Tuple.Create(p.PropertyType, new FastAccessor(p))
                    );
            }

            private static Dictionary<string, Tuple<Type, FastAccessor>> props;
        }
    }

    public sealed class ExcelDocument
    {
        public IEnumerable<Worksheet> Worksheets => sheetList.AsEnumerable();

        public bool TryGetWorksheet(string sheetName, out Worksheet result) => sheetLookup.TryGetValue(sheetName, out result);

        public ExcelDocument(string fullPath)
        {
            sheetLookup = new Dictionary<string, Worksheet>();
            sheetList = new List<Worksheet>();

            using (var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var zip = new ZipArchive(stream);

                sharedStrings = X.Deserialize<X.SharedStringTable>(zip.Entries.First(e => e.FullName == "xl/sharedStrings.xml"));
                var sheetIds = X.Deserialize<X.Workbook>(zip.Entries.First(e => e.FullName == "xl/workbook.xml")).Sheets;

                var worksheetEntries = zip.Entries.Where(e => e.FullName.StartsWith("xl/worksheets/sheet"));
                foreach (var entry in worksheetEntries)
                {
                    //  extract {X} from "sheet{X}.xml"
                    var id = entry.Name.Substring(5, entry.Name.Length - 9);
                    var sheetName = sheetIds.First(i => i.SheetId == id).Name;

                    var sheetData = X.Deserialize<X.Worksheet>(entry);
                    var sheet = worksheetFactory.Create(sheetName, sheetData, sharedStrings);

                    sheetLookup[sheet.Name] = sheet;
                    sheetList.Add(sheet);
                }
            }
        }

        public DataSet ToDataSet()
        {
            var ds = new DataSet();

            foreach (var worksheet in Worksheets)
            {
                var table = new DataTable(worksheet.Name);

                Cell[] maxRow = Array.Empty<Cell>();
                var data = new List<Cell[]>();
                foreach (var row in worksheet.Rows)
                {
                    var cellRow = row.Cells.ToArray();
                    if (cellRow.Length > maxRow.Length)
                        maxRow = cellRow;

                    data.Add(cellRow);
                }

                foreach (var cell in maxRow)
                    table.Columns.Add(cell.Column, GetColumnType(cell));

                foreach (var row in data)
                {
                    var dr = table.NewRow();
                    foreach (var c in row)
                        dr[c.Column] = c.GetData();

                    table.Rows.Add(dr);
                }

                ds.Tables.Add(table);
            }

            return ds;
            
            Type GetColumnType(Cell c)
            {
                switch (c.DataType)
                {
                    case CellDataType.None: return null;
                    case CellDataType.Bool: return typeof(bool);
                    case CellDataType.Date: return typeof(DateTime);
                    case CellDataType.Error: return typeof(string);
                    case CellDataType.Number: return typeof(double);
                    case CellDataType.String: return typeof(string);
                    case CellDataType.Formula: return typeof(string);
                    default: throw new Exception("Invalid data type for cell!");
                }
            }
        }

        public enum CellDataType
        {
            None,
            Bool,
            Date,
            Error,
            Number,
            String,
            Formula
        }

        public class Worksheet
        {
            public string Name { get; }

            public IReadOnlyList<Row> Rows { get; }

            public Cell GetCell(string cellId)
            {
                var idparts = ParseCellId(cellId);
                if (idparts.Item2 - 1 >= Rows.Count)
                    throw new Exception(string.Format("Invalid row {0}!", idparts.Item2));

                if (!Rows[idparts.Item2 - 1].TryGetCell(idparts.Item1, out var c))
                    throw new Exception(string.Format("Invalid cell {0}!", idparts.Item1));

                return c;
            }

            private Worksheet(string name, X.Worksheet worksheet, X.SharedStringTable sst)
            {
                Name = name;

                Rows = worksheet.Rows
                    .Select(r => rowFactory.Create(r, sst))
                    .ToList();
            }

            public class Factory : IWorksheetFactory
            {
                public Worksheet Create(string name, X.Worksheet worksheet, X.SharedStringTable sst)
                    => new Worksheet(name, worksheet, sst);
            }
        }

        public class Row
        {
            public int Columns { get; }

            public IEnumerable<Cell> Cells => cellList.AsEnumerable();

            public Cell this[string column]
            {
                get { if (TryGetCell(column, out var c)) return c; else return null; }
            }

            public Cell this[int index]
            {
                get { if (cellColumnLookup.TryGetValue(index, out var c)) return c; else return null; }
            }

            public bool TryGetCell(string column, out Cell c)
            {
                return cellLookup.TryGetValue(column, out c);
            }

            private Row(X.Row row, X.SharedStringTable sst)
            {
                cellList = row.Cells
                    .Where(c => c.CellType != X.CellDataType.None)
                    .Select(c => cellFactory.Create(c, sst))
                    .ToList();

                if (cellList.Count > 0)
                    Columns = row.Cells.Length;

                cellColumnLookup = cellList
                    .ToDictionary(c => ColumnToIndex(c.Column));

                cellLookup = cellList
                    .ToDictionary(c => c.Column);
            }

            public class Factory : IRowFactory
            {
                public Row Create(X.Row row, X.SharedStringTable sst) => new Row(row, sst);
            }

            private IReadOnlyList<Cell> cellList;
            private IReadOnlyDictionary<string, Cell> cellLookup;
            private IReadOnlyDictionary<int, Cell> cellColumnLookup;

            private static int ColumnToIndex(string column)
            {
                int result = 0;
                for (int i = 0; i < column.Length; i++)
                {
                    result *= 26;
                    result += (column[i] - 'A' + 1);
                }

                return result - 1;
            }
        }

        public class Cell
        {
            public string RawValue { get; }
            public string StringValue { get; }
            public string ErrorText { get; }

            public bool? BoolValue { get; }
            public DateTime? DateTimeValue { get; }
            public double? NumberValue { get; }

            public string CellId { get; }
            public string Column { get; }
            public int Row { get; }

            public CellDataType DataType { get; }

            private Cell(X.Cell cell, X.SharedStringTable sst)
            {
                CellId = cell.CellId;
                RawValue = cell.Value;

                switch (cell.CellType)
                {
                    case X.CellDataType.None:
                        break;

                    case X.CellDataType.Bool:
                        BoolValue = Convert.ToBoolean(RawValue);
                        DataType = CellDataType.Bool;
                        break;

                    case X.CellDataType.ISO8601Date:
                        DateTimeValue = DateTime.FromOADate(Convert.ToDouble(RawValue));
                        break;

                    case X.CellDataType.Error:
                        ErrorText = RawValue;
                        DataType = CellDataType.String;
                        break;

                    case X.CellDataType.FormulaString:
                        StringValue = RawValue;
                        DataType = CellDataType.Formula;
                        break;

                    case X.CellDataType.InlineString:
                        StringValue = RawValue;
                        DataType = CellDataType.String;
                        break;

                    case X.CellDataType.Number:
                        DataType = CellDataType.Number;
                        NumberValue = Convert.ToDouble(RawValue);
                        break;

                    case X.CellDataType.SharedString:
                        StringValue = sst.Instances[int.Parse(RawValue)].Value;
                        DataType = CellDataType.String;
                        break;

                    default:
                        throw new Exception("Invalid value for c.@t (how did this happen?)");
                }

                var idParts = ParseCellId(CellId);
                Column = idParts.Item1;
                Row = idParts.Item2;
            }

            public class Factory : ICellFactory
            {
                public Cell Create(X.Cell cell, X.SharedStringTable sst) => new Cell(cell, sst);
            }

            public object GetData()
            {
                switch (DataType)
                {
                    case CellDataType.None: return null;
                    case CellDataType.Bool: return BoolValue;
                    case CellDataType.Date: return DateTimeValue;
                    case CellDataType.Error: return ErrorText;
                    case CellDataType.Number: return NumberValue;
                    case CellDataType.String: return StringValue;
                    case CellDataType.Formula: return StringValue;
                    default: throw new Exception("Invalid data type for cell!");
                }
            }
        }

        private static readonly Worksheet.Factory worksheetFactory;
        private static readonly Row.Factory rowFactory;
        private static readonly Cell.Factory cellFactory;
        private static readonly Regex idMatcher = new Regex(@"([A-Za-z]+)([0-9]+)", RegexOptions.Compiled);

        private static Tuple<string, int> ParseCellId(string cellId)
        {
            var match = idMatcher.Match(cellId);
            return Tuple.Create(
                match.Groups[1].Value.ToUpper(),
                int.Parse(match.Groups[2].Value)
            );
        }

        private Dictionary<string, Worksheet> sheetLookup;
        private X.SharedStringTable sharedStrings;
        private List<Worksheet> sheetList;

        private interface IWorksheetFactory { Worksheet Create(string worksheetName, X.Worksheet data, X.SharedStringTable sst); }
        private interface IRowFactory { Row Create(X.Row data, X.SharedStringTable sst); }
        private interface ICellFactory { Cell Create(X.Cell data, X.SharedStringTable sst); }

        public class X
        {
            public static T Deserialize<T>(ZipArchiveEntry zipArchiveEntry)
            {
                using (var stream = zipArchiveEntry.Open())
                    return XmlUtility.DeserializeFromStream<T>(stream);
            }

            public enum CellDataType
            {
                [XmlEnum("")] None,
                [XmlEnum("b")] Bool,
                [XmlEnum("d")] ISO8601Date,
                [XmlEnum("e")] Error,
                [XmlEnum("inlineStr")] InlineString,
                [XmlEnum("n")] Number,
                [XmlEnum("s")] SharedString,
                [XmlEnum("str")] FormulaString
            }

            [Serializable]
            [XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
            [XmlRoot("workbook", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
            public class Workbook
            {
                [XmlArray("sheets")]
                [XmlArrayItem("sheet")]
                public Sheet[] Sheets;
            }

            public class Sheet
            {
                [XmlAttribute("name")]
                public string Name;

                [XmlAttribute("sheetId")]
                public string SheetId;
            }

            [Serializable]
            [XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
            [XmlRoot("worksheet", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
            public class Worksheet
            {
                [XmlArray("sheetData")]
                [XmlArrayItem("row")]
                public Row[] Rows;
            }

            [Serializable]
            [XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
            [XmlRoot("sst", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
            public class SharedStringTable
            {
                [XmlAttribute("uniqueCount")] public string UniqueCount;
                [XmlAttribute("count")] public string Count;
                [XmlElement("si")] public SharedString[] Instances
                {
                    get { return instances; }
                    set
                    {
                        instances = value;
                        lookup = instances.ToDictionary(s => s.Value);
                    }
                }

                public class SharedString
                {
                    [XmlElement("t")]
                    public string Value;
                }
                
                private SharedString[] instances;
                private Dictionary<string, SharedString> lookup;
            }

            public class Row
            {
                [XmlElement("c")]
                public Cell[] Cells;
            }

            public class Cell
            {
                [XmlAttribute("r")]
                public string CellId;

                [XmlAttribute("s")]
                public string StyleIndex;

                [XmlAttribute("t")]
                public CellDataType CellType;

                [XmlElement("v")]
                public string Value;
            }
        }

        static ExcelDocument()
        {
            worksheetFactory = new Worksheet.Factory();
            rowFactory = new Row.Factory();
            cellFactory = new Cell.Factory();
        }
    }
}
