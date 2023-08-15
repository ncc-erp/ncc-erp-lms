using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Scorms.Dto
{
    public class ScormNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public int Level { get; set; }
        public bool IsDone { get; set; }
        public List<ScormNode> Children { get; set; }
    }
}
