using System;
using System.Collections.Generic;
using System.Linq;

namespace Anixe.Ion
{
    public class TableReader<T>
    {
        private readonly static char[] separator = new char[] { '|' };
        private readonly string sectionName;
        private readonly IIonReader reader;
        private Func<string[], T> tableRowHandler;

        public TableReader(IIonReader reader, string sectionName, Func<string[], T> tableRowHandler = null)
        {
            this.reader = reader;
            this.sectionName = sectionName;
            this.tableRowHandler = tableRowHandler;
        }

        public void RegisterHandler(Func<string[], T> tableRowHandler)
        {
            this.tableRowHandler = tableRowHandler;
        }

        public virtual List<T> Read()
        {
            var retval = new List<T>{ };
            bool isHeaderPassed = false;
            while(reader.Read())
            {
                if(ShouldBreak(isHeaderPassed))
                {
                    break;
                }
                
                if(reader.IsTableRow && reader.IsTableHeaderSeparatorRow && IsInSection())
                {
                    isHeaderPassed = true;
                }
                if(reader.IsTableRow && !reader.IsTableHeaderSeparatorRow && isHeaderPassed)
                {
                    var columns = reader.CurrentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                    var obj = tableRowHandler(columns);
                    retval.Add(obj);
                }
            }
            return retval;      
        }

        private bool ShouldBreak(bool isHeaderPassed)
        {
            return  isHeaderPassed &&  
            (string.IsNullOrWhiteSpace(this.reader.CurrentLine) || !IsInSection());
        }        

        private bool IsInSection()
        {
            return this.reader.CurrentSection.Equals(this.sectionName, StringComparison.OrdinalIgnoreCase);
        }         
    }
}