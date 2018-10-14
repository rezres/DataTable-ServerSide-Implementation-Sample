using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Data.Responses
{
    public class DataTableResponse : ISerializable
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<object> Data { get; set; }
        public bool Status { get; set; }
        public string Msg { get; set; }
        public string ModelClass { get; set; }
        public int Timezone { get; set; }

        public DataTableResponse() { }

        public DataTableResponse(bool Status = false, int Draw = 1, List<object> Data = null, string ErrorMsg = "")
        {
            this.Status = Status;
            this.Draw = Draw;
            this.Data = Data;
            this.Msg = ErrorMsg;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("draw", Draw);
            info.AddValue("recordsTotal", RecordsTotal);
            info.AddValue("recordsFiltered", RecordsFiltered);
            info.AddValue("Status", Status);
            info.AddValue("Msg", Msg);
            info.AddValue("data", Data);
        }
    }

}
