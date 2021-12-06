using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Clients
{
    public interface ILodestoneClient
    {
        Task<HtmlDocument> GetCharacter(long characterID);
        Task<HtmlDocument> GetCharacterClassJob(long characterID);
        Task<HtmlDocument> GetCharacterMounts(long characterID);
        Task<HtmlDocument> GetCharacterMinions(long characterID);
    }
}
