using System;
using System.Collections.Generic;

public class Estate    {
    public string Greeting { get; set; } 
    public string Name { get; set; } 
    public string Plot { get; set; } 
}

public class Focus    {
    public string Icon { get; set; } 
    public string Name { get; set; } 
    public bool Status { get; set; } 
}

public class Ranking    {
    public int Monthly { get; set; } 
    public int Weekly { get; set; } 
}

public class Reputation    {
    public string Name { get; set; } 
    public int Progress { get; set; } 
    public string Rank { get; set; } 
}

public class Seeking    {
    public string Icon { get; set; } 
    public string Name { get; set; } 
    public bool Status { get; set; } 
}

public class FreeCompany    {
    public string Active { get; set; } 
    public int ActiveMemberCount { get; set; } 
    public List<string> Crest { get; set; } 
    public string DC { get; set; } 
    public Estate Estate { get; set; } 
    public List<Focus> Focus { get; set; } 
    public int Formed { get; set; } 
    public string GrandCompany { get; set; } 
    public string ID { get; set; } 
    public string Name { get; set; } 
    public int ParseDate { get; set; } 
    public int Rank { get; set; } 
    public Ranking Ranking { get; set; } 
    public string Recruitment { get; set; } 
    public List<Reputation> Reputation { get; set; } 
    public List<Seeking> Seeking { get; set; } 
    public string Server { get; set; } 
    public string Slogan { get; set; } 
    public string Tag { get; set; } 
}

public class FreeCompanyMember    {
    public string Avatar { get; set; } 
    public int FeastMatches { get; set; } 
    public int ID { get; set; } 
    public object Lang { get; set; } 
    public string Name { get; set; } 
    public string Rank { get; set; } 
    public string RankIcon { get; set; } 
    public string Server { get; set; } 
}

public class FreeCompanyMembersContainer    {
    public FreeCompany FreeCompany { get; set; } 
    public List<FreeCompanyMember> FreeCompanyMembers { get; set; } 
}

