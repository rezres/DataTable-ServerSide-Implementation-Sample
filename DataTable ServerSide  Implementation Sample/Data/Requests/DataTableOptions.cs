using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DataTable_ServerSide__Implementation_Sample.Data.Requests
{
    public class DataTableOptions
    {
        [JsonProperty("draw")]
        public string Draw { get; set; }
        [JsonProperty("start")]
        public int Start { get; set; }
        [JsonProperty("length")]
        public int Length { get; set; }
        [JsonProperty("order")]
        public List<DataTableColumnOrder> Order { get; set; }
        [JsonProperty("columns")]
        public List<DataTableColumn> Columns { get; set; }
        [JsonProperty("search")]
        public DataTableColumnSearch Search { get; set; }
        [JsonProperty("params")]
        public List<string> Params { get; set; }

        public DataTableOptions() { }

        public DataTableOptions(SerializationInfo info, StreamingContext context)
        {
            Search = new DataTableColumnSearch();
            Params = new List<string>();

            Draw = info.GetString("draw");
            Start = info.GetInt32("start");
            Length = info.GetInt32("length");
            Order = (List<DataTableColumnOrder>)info.GetValue("order", typeof(List<DataTableColumnOrder>));
            Columns = (List<DataTableColumn>)info.GetValue("columns", typeof(List<DataTableColumn>));
            Search = (DataTableColumnSearch)info.GetValue("search", typeof(DataTableColumnSearch));
            Params = (List<string>)info.GetValue("params", typeof(List<string>));

        }

    }
    
    public class DataTableColumn
    {
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("searchable")]
        public bool Searchable { get; set; }
        [JsonProperty("orderable")]
        public bool Orderable { get; set; }
        [JsonProperty("search")]
        public DataTableColumnSearch Search { get; set; }

        public DataTableColumn() { }

        protected DataTableColumn(SerializationInfo info, StreamingContext context)
        {
            Data = info.GetString("data");
            Name = info.GetString("name");
            Searchable = info.GetBoolean("searchable");
            Orderable = info.GetBoolean("orderable");
            Search = (DataTableColumnSearch)info.GetValue("search", typeof(DataTableColumnSearch));
        }
    }
    public class DataTableColumnSearch
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("regex")]
        public bool Regex { get; set; }

        public DataTableColumnSearch() { }

        protected DataTableColumnSearch(SerializationInfo info, StreamingContext context)
        {
            Value = info.GetString("value");
            Regex = info.GetBoolean("regex");
        }
    }
    public class DataTableColumnOrder
    {
        [JsonProperty("column")]
        public int Column { get; set; }
        [JsonProperty("dir")]
        public string Dir { get; set; }

        public DataTableColumnOrder() { }

        protected DataTableColumnOrder(SerializationInfo info, StreamingContext context)
        {
            Column = info.GetInt16("column");
            Dir = info.GetString("dir");
        }
    }


}
