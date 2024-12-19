using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class IssueTag
    {
        public int IssueId { get; set; }
        public Issue Issue { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

}
