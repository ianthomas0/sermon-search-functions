using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreachingCollective.Models
{
    public class ListFilterData
    {
        public IEnumerable<string> Authors { get; set; }

        public IEnumerable<string> Sources { get; set; }
    }
}
