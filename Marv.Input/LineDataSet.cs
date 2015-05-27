using System;
using System.Collections.Generic;
using System.Data;

namespace Marv.Input
{
    public class LineDataSet : DataSet
    {
        public DataSet MergedDataSet { get; set; }
        public List<double> NewList { get; set; }

        public DataSet GetMergerdDataSet(DataSet unMergedSet)
        {
            this.MergedDataSet = new DataSet();
            this.NewList = new List<double>();

            var tables = unMergedSet.Tables;

            // Generate a list which holds the modified section ranges
            foreach (DataTable table in tables)
            {
                var rows = table.Rows;

                foreach (DataRow row in rows)
                {
                    var from = row["From"];
                    var to = row["To"];

                    if ((!DBNull.Value.Equals(from) && from != null))
                    {
                        this.NewList.Add((double) from);
                    }

                    if ((!DBNull.Value.Equals(to) && to != null))
                    {
                        this.NewList.Add((double) to);
                    }
                }
            }

            this.NewList.Sort(); // sorting the new list

         
            foreach (DataTable table in tables)
            {
                var modifiedTable = GetModifiedTable(table, tables);

                if (modifiedTable != null)
                {
                    this.MergedDataSet.Tables.Add(modifiedTable);
                }
            }

            return this.MergedDataSet;
        }

        private DataTable GetModifiedTable(DataTable table, DataTableCollection tables)
        {
            var newTable = new DataTable();

            foreach (DataColumn col in table.Columns)
            {
                if (col.ColumnName == "ID")
                {
                    newTable.Columns.Add("ID", typeof (string));
                }
                else if (col.ColumnName == "From")
                {
                    newTable.Columns.Add("From", typeof (double));
                }
                else if (col.ColumnName == "To")
                {
                    newTable.Columns.Add("To", typeof (double));
                }
                else
                {
                    newTable.Columns.Add(col.ColumnName, typeof (string));
                }
            }

            if (this.NewList != null)
            {
                var i = 0;
                while (i < NewList.Count - 1)
                {
                    var newRow = newTable.NewRow();
                    newRow["From"] = NewList[i];
                    newRow["To"] = NewList[i + 1];
                   
                    newTable.Rows.Add(newRow);
                    i++;
                }
            }

            Console.WriteLine("no of rows before deletion= {0} ", newTable.Rows.Count);

            // identifying the spurious rows to be deleted
            var deleteRows = new List<DataRow>();
           
            foreach (DataRow newrow in newTable.Rows)
            {
                if (newrow["From"].Equals(newrow["To"]))
                {
                    var pointvalue = newrow["From"];
                    foreach (DataTable oldtable in tables)
                    {
                        foreach (DataRow oldRow in oldtable.Rows)
                        {
                            if (oldRow["From"].Equals(oldRow["To"]))
                            {
                                if (oldRow["From"].Equals(pointvalue) && oldRow["To"].Equals(pointvalue))
                                {
                                    // do nothing
                                }
                                else
                                {
                                    deleteRows.Add(newrow);
                                }
                            }
                           
                        }
                    }
                }
            }

            // deleting the spurious rows 
            foreach (var row in deleteRows)
            {
                if (row != null)
                {
                    Console.WriteLine("delete rows= {0}, {1}", row["From"], row["To"]);
                    newTable.Rows.Remove(row);
                }
            }

            Console.WriteLine("no of rows after deletion= {0} ", newTable.Rows.Count);

            // populate data for newTable
            newTable = PopulateData(newTable, table);

            // printing table with new data
            foreach (DataRow row in newTable.Rows)
            {
                foreach (DataColumn col in newTable.Columns)
                {
                    Console.Write("{0}-", row[col] );
                }

                Console.WriteLine("");
            }


            return newTable;
        }

        private DataTable PopulateData(DataTable newTable, DataTable table)
        {
            if (newTable != null && table != null)
            {
                foreach (DataRow newrow in newTable.Rows)
                {
                    foreach (DataRow oldrow in table.Rows)
                    {
                        if ((double) newrow["From"] >= (double) oldrow["From"] &&
                            (double) newrow["To"] <= (double) oldrow["To"])
                        {
                            var count = 3;

                            while(count < newTable.Columns.Count)
                            {
                                newrow[count] = oldrow[count];
                                count++;
                            }

                        }
                      
                    }
                }
            }
            return newTable;
        }
    }
}