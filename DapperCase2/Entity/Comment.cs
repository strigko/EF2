using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFP48.DapperCase.Entity
{
    internal class Comment
    {
        Guid Id { get; set; } = Guid.NewGuid();
        Guid PostId { get; set; }
        Guid UserId {  get; set; }
        public string Content {  get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
