using Shinra.Clients.Models;
using Shinra.Services.Models;

namespace Shinra.Messages.Character
{
    public class CharacterUpdated
    {
        public PointContainer Container { get; private set; }
        public CharacterUpdated(PointContainer container) 
        {
            Container = container;
        }
    }
}
