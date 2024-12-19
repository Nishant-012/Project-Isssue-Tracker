using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class Issue
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }
        public ICollection<IssueTag> IssueTags { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
