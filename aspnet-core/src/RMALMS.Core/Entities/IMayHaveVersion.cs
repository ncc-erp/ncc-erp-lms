using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Entities
{
    public interface IMayHaveVersion
    {
        string Version { get; set; }
    }
}
