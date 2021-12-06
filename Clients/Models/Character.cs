
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

public class UnlockedState    {
    public int ID { get; set; } 
    public string Name { get; set; } 
}

public class UnlockedState2    {
    public int? ID { get; set; } 
    public string Name { get; set; } 
}

public class ClassJob    {
    public int ClassID { get; set; } 
    public int ExpLevel { get; set; } 
    public int ExpLevelMax { get; set; } 
    public int ExpLevelTogo { get; set; } 
    public bool IsSpecialised { get; set; } 
    public int JobID { get; set; } 
    public int Level { get; set; } 
    public string Name { get; set; } 
    public UnlockedState2 UnlockedState { get; set; } 
}

public class ClassJobsElemental    {
    public int ExpLevel { get; set; } 
    public int ExpLevelMax { get; set; } 
    public int ExpLevelTogo { get; set; } 
    public int Level { get; set; } 
    public string Name { get; set; } 
}

public class GrandCompany    {
    public string Name { get; set; } 
    public string Rank { get; set; } 
}

public class Character    {
    public string Avatar { get; set; } 
    public string Bio { get; set; }
    public List<ClassJob> ClassJobs { get; set; } = new List<ClassJob>();
    public ClassJobsElemental ClassJobsElemental { get; set; } 
    public string DC { get; set; } 
    public string FreeCompanyId { get; set; } 
    public string Gender { get; set; }
    public GrandCompany GrandCompany { get; set; } = new GrandCompany();
    public int GuardianDeity { get; set; } 
    public int ID { get; set; } 
    public object Lang { get; set; } 
    public string Name { get; set; } 
    public string Nameday { get; set; } 
    public int ParseDate { get; set; } 
    public string Portrait { get; set; } 
    public object PvPTeamId { get; set; } 
    public string Race { get; set; } 
    public string Server { get; set; } 
    public int Title { get; set; } 
    public bool TitleTop { get; set; } 
    public int Town { get; set; } 
    public string Tribe { get; set; } 
}

public class Minion    {
    public string Icon { get; set; } 
    public string Name { get; set; } 
}

public class Mount    {
    public string Icon { get; set; } 
    public string Name { get; set; } 
}

public class CharacterContainer    {
    public object Achievements { get; set; } 
    public object AchievementsPublic { get; set; }
    public Character Character { get; set; } = new Character();
    public object FreeCompany { get; set; } 
    public object FreeCompanyMembers { get; set; } 
    public object Friends { get; set; } 
    public object FriendsPublic { get; set; }
    public List<Minion> Minions { get; set; } = new List<Minion>();
    public List<Mount> Mounts { get; set; } = new List<Mount>();
    public object PvPTeam { get; set; }
}

