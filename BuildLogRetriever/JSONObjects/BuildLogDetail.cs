using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildInfoRetriever.JSONObjects
{
    /// <summary>
    /// Code generated via JSON by http://json2csharp.com/
    /// </summary>
    public class BuildLogDetail
    {
        public int count { get; set; }

        public List<BuildStep> value { get; set; }
    }

    public class BuildStep
    {
        public int lineCount { get; set; }

        public string createdOn { get; set; }

        public string lastChangedOn { get; set; }

        public int id { get; set; }

        public string type { get; set; }

        public string url { get; set; }
    }
}
