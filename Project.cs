using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Issue> Issues { get; set; }
    }
}
