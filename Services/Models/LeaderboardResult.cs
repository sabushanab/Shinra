using System.Collections.Generic;

namespace Shinra.Services.Models
{
    public class LeaderboardResult
    {
        public long TotalCount { get; set; }
        public List<PointContainer> Characters { get; set; }
    }
}
