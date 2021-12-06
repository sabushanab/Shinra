using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public class LodestoneParserService
    {
        public CharacterContainer ParseCharacter(HtmlDocument doc)
        {
            CharacterContainer container = new CharacterContainer();
            container = ParseProfileName(container, doc);

            IList<HtmlNode> nodes = doc.QuerySelectorAll(".character__profile__data__detail .character-block");
            foreach(HtmlNode node in nodes)
            {
                var blockTitle = node.QuerySelector(".character-block__title")?.InnerHtml;
                switch(blockTitle)
                {
                    case "Race/Clan/Gender":
                        container = ParseProfileRaceTribeGender(container, node);
                        break;
                    case "Grand Company":
                        container = ParseGrandCompany(container, node);
                        break;
                }
            }
            return container;
        }

        public CharacterContainer ParseCharacterClassJob(CharacterContainer container, HtmlDocument doc)
        {
            IList<HtmlNode> nodes = doc.QuerySelectorAll(".character__content li");
            foreach(HtmlNode node in nodes)
            {
                string level = node.QuerySelector(".character__job__level").InnerHtml;
                if (level == "-") { continue; }

                string name = node.QuerySelector(".character__job__name").InnerHtml;
                ClassJob job = new ClassJob();
                job.Name = name;
                int.TryParse(level, out var intLevel);
                job.Level = intLevel;

                string jobExp = node.QuerySelector(".character__job__exp").InnerHtml;
                var currentMax = jobExp.Split(" / ");
                int.TryParse(currentMax[0], NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var expLevel);
                int.TryParse(currentMax[1], NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var expLevelMax);
                job.ExpLevel = expLevel;
                job.ExpLevelMax = expLevelMax;
                job.ExpLevelTogo = expLevelMax - expLevel;
                container.Character.ClassJobs.Add(job);
            }
            return container;
        }

        public CharacterContainer ParseCharacterMounts(CharacterContainer container, HtmlDocument doc)
        {
            IList<HtmlNode> nodes = doc.QuerySelectorAll(".mount__list__item");
            foreach(HtmlNode node in nodes)
            {
                Mount mount = new Mount();
                mount.Name = WebUtility.HtmlDecode(node.QuerySelector(".mount__name").InnerHtml);
                mount.Icon = node.QuerySelector(".mount__list__icon__inner img").Attributes["data-original"]?.Value;
                container.Mounts.Add(mount);
            }

            return container;
        }

        public CharacterContainer ParseCharacterMinions(CharacterContainer container, HtmlDocument doc)
        {
            IList<HtmlNode> nodes = doc.QuerySelectorAll(".minion__list__item");
            foreach (HtmlNode node in nodes)
            {
                Minion minion = new Minion();
                minion.Name = WebUtility.HtmlDecode(node.QuerySelector(".minion__name").InnerHtml);
                minion.Icon = node.QuerySelector(".minion__list__icon__inner img").Attributes["data-original"]?.Value;
                container.Minions.Add(minion);
            }

            return container;
        }

        private CharacterContainer ParseProfileName(CharacterContainer container, HtmlDocument doc)
        {
            string name = doc.QuerySelector(".frame__chara__name").InnerHtml;
            name = name.Replace("&#39;", "'");
            container.Character.Name = name;
            return container;
        }

        private CharacterContainer ParseProfileRaceTribeGender(CharacterContainer container, HtmlNode node)
        {
            string html = node.QuerySelector(".character-block__name").InnerHtml;
            html = html.Replace("&#39;", "'").Replace("<br>", " / ");
            var raceTribeGender = html.Split(" / ");
            container.Character.Race = raceTribeGender[0];
            container.Character.Tribe = raceTribeGender[1];
            container.Character.Gender = raceTribeGender[2] == "♀" ? "female" : "male";
            return container;
        }

        private CharacterContainer ParseGrandCompany(CharacterContainer container, HtmlNode node)
        {
            string html = node.QuerySelector(".character-block__name").InnerHtml;
            var grandCompany = html.Split(" / ");
            container.Character.GrandCompany.Name = grandCompany[0];
            container.Character.GrandCompany.Rank = grandCompany[1];
            return container;
        }
    }
}
