using Shinra.Clients.Models;
using Shinra.Services.Models;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public interface IExpansionPointer
    {
        static Dictionary<string, string> PointableInstances { get; }
        PointCategory GetPointsForExpansion(Category expansion);
    }
}
