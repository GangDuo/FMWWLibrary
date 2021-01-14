using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMWW.Controls
{
    [DataContract]
    public class Component
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "native")]
        public string Native { get; set; }

        [DataMember(Name = "inputAble")]
        public bool InputAble { get; set; }

        [DataMember(Name = "format")]
        public int Format { get; set; }

        [DataMember(Name = "pointPosition")]
        public int PointPosition { get; set; }

        [DataMember(Name = "align")]
        public string Align { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "depth")]
        public int Depth { get; set; }

        [DataMember(Name = "attribute")]
        public int Attribute { get; set; }

        [DataMember(Name = "margeRight")]
        public object MargeRight { get; set; }

        [DataMember(Name = "margeBottom")]
        public object MargeBottom { get; set; }

        [DataMember(Name = "width")]
        public int Width { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "front")]
        public object Front { get; set; }

        [DataMember(Name = "bottom")]
        public object Bottom { get; set; }
    }
}
