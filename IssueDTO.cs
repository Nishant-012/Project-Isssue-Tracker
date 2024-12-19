using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class IssueDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int ProjectId { get; set; }  // Assuming an issue is related to a project
        public int UserId { get; set; }      // Assuming an issue is created by a user
    }
}
