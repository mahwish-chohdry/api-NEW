using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Model
{
    public enum OpSet
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        Contains
    }


    public class LinqFilter
    {
        public string ColumnName { get; set; }
        public OpSet Op { get; set; }
        public bool isNumeric { get; set; }

        public string Value { get; set; }


        public static Dictionary<OpSet, String> QueryExpressionEquivilant = new Dictionary<OpSet, String>()
        {
            { OpSet.Equals, " [ColumnName] = [Value] " },
            { OpSet.GreaterThan, " [ColumnName] > [Value] " },
            { OpSet.LessThan, " [ColumnName] < [Value] " },
            { OpSet.GreaterThanOrEqualTo, " [ColumnName] >= [Value] " },
            { OpSet.LessThanOrEqualTo, " [ColumnName] <= [Value] " },
            //{ OpSet.Contains, " [ColumnName] Like '%[Value]%' " }                        
            { OpSet.Contains, " (c.[ColumnName] , '[Value]') " }
        };
    }
}
