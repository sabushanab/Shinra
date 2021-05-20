﻿var navTemplate,
    memberTemplate,
    characterJobsTemplate,
    characterCollectibleTemplate,
    characterInfoTemplate,
    characterDataContainerTemplate,
    noCharacterDataTemplate,
    urlParameters,
    alertTemplate

$(document).ready(function () {
    var navTemplateSource = document.getElementById("nav-template").innerHTML;
    var memberTemplateSource = document.getElementById("member-template").innerHTML;
    var characterJobsTemplateSource = document.getElementById("character-jobs-template").innerHTML;
    var characterCollectibleTemplateSource = document.getElementById("character-collectible-template").innerHTML;
    var characterInfoTemplateSource = document.getElementById("character-info-template").innerHTML;
    var characterDataContainerTemplateSource = document.getElementById("character-data-container-template").innerHTML;
    var noCharacterDataTemplateSource = document.getElementById("no-character-data-template").innerHTML;
    var alertTemplateSource = document.getElementById("alert-template").innerHTML;

    navTemplate = Handlebars.compile(navTemplateSource);
    memberTemplate = Handlebars.compile(memberTemplateSource);
    characterJobsTemplate = Handlebars.compile(characterJobsTemplateSource);
    characterCollectibleTemplate = Handlebars.compile(characterCollectibleTemplateSource);
    characterInfoTemplate = Handlebars.compile(characterInfoTemplateSource);
    characterDataContainerTemplate = Handlebars.compile(characterDataContainerTemplateSource);
    noCharacterDataTemplate = Handlebars.compile(noCharacterDataTemplateSource);
    alertTemplate = Handlebars.compile(alertTemplateSource);
    urlParameters = getUrlVars();

    if (sessionStorage.getItem("ffxiv_data") != null) {
        ffxivData = JSON.parse(sessionStorage.getItem("ffxiv_data"));
        loadPage();
    } else {
        var loadMounts = new Promise((resolve, reject) => {
            loadCollection("mounts", resolve, reject);
        });
        var loadMinions = new Promise((resolve, reject) => {
            loadCollection("minions", resolve, reject);
        });
        var loadRaces = new Promise((resolve, reject) => {
            loadEnumeration("Race", 1, resolve, reject);
        });
        Promise.all([
            loadMounts,
            loadMinions,
            loadRaces
        ]).then(values => {
            sessionStorage.setItem("ffxiv_data", JSON.stringify(ffxivData));
            loadPage();
        })
    }

    tippy.delegate('#collectible-section', {
        target: ".collectible-item",
        appendTo: document.body,
        theme: "light",
        content(reference) {
            if ($(reference).data("tooltip")) {
                var hover = $(reference).data("tooltip").split("|");
                if (hover.length > 1) {
                    var hoverHtml = "<ul>"
                    $.each(hover, function (i, key) {
                        hoverHtml += "<li>" + key + "</li>";
                    });
                    hoverHtml += "</ul>";
                    return hoverHtml;
                }
                return $(reference).data("tooltip");
            }
        },
        allowHTML: true
    });
})

$(document).on('click', '#my-character-btn', function (i, e) {
    Cookies.set('main_character', $(i.target).data("id"));
    $(i.target).hide();
});

$(document).on('keyup', '#member-search', function () {
    var $noMatches = $('#member-section').find('li').filter(function (i, key) {
        return $(this).data('name').toLowerCase().indexOf($('#member-search').val().toLowerCase()) == -1;
    });
    $noMatches.hide();
    $('#member-section').find('li').not($noMatches).show();
});

$(document).on('keyup', '#mount-search', function () {
    var $noMatches = $('.mount-section').find('.collectible-item').filter(function (i, key) {
        return $(this).data('name').toLowerCase().indexOf($('#mount-search').val().toLowerCase()) == -1;
    });
    $noMatches.hide();
    $('.mount-section').find('.collectible-item').not($noMatches).show();
});

$(document).on('keyup', '#minion-search', function () {
    var $noMatches = $('.minion-section').find('.collectible-item').filter(function (i, key) {
        return $(this).data('name').toLowerCase().indexOf($('#minion-search').val().toLowerCase()) == -1;
    });
    $noMatches.hide();
    $('.minion-section').find('.collectible-item').not($noMatches).show();
});



window.onpopstate = function (event) {
    urlParameters = getUrlVars();
    getEntryPoint();
};

function getEntryPoint() {
    urlParameters = getUrlVars();
    $('.content-section').hide();
    if (urlParameters["id"]) {
        displayCharacterPage(urlParameters["id"]);
    } else if (Cookies.get("main_character")) {
        displayCharacterPage(Cookies.get("main_character"));
    } else {
        $('#intro-section').html(alertTemplate({ Text: "Select a member on the left menu to begin!" })).show();
    }
}

function loadPage() {
    getEntryPoint();
    $.get("/XIVAPI/GetFreeCompanyMembers", function (data) {
        $('#nav-section').html(navTemplate(data.freeCompany));
        $.each(data.freeCompanyMembers, function (i, key) {
            var nameArr = key.name.split(" ");
            key.firstName = nameArr[0];
            key.lastName = nameArr[1];
            key.lastNameAbr = nameArr[1].charAt(0);
        });
        var memberTemplateData = {
            "Members": data.freeCompanyMembers
        };
        $('#member-section').html(memberTemplate(memberTemplateData));
        console.log(data);
    });
}

function displayCharacterPage(id) {
    $('#jobs-section').html(characterDataContainerTemplate({ DataType: "Jobs" })).show();
    $('#collectible-section').html(characterDataContainerTemplate({ DataType: "Collectibles" })).show();
    $.get("/XIVAPI/GetCharacter?id=" + id, function (data) {
        if (data.character == null) {
            $("#jobs-section .dynamic-data-section").html(noCharacterDataTemplate({ DataType: "job" }));
            $('#collectible-section .dynamic-data-section').html(noCharacterDataTemplate({ DataType: "collectible" }));
        } else {
            displayCharacterInfo(data.character, id);
            displayJobs(data.character);
            displayMounts(data);
        }
        window.scroll({ top: 0, left: 0, behavior: 'smooth' });
    });
}

function displayCharacterInfo(character, id) {
    var mainCharacterID = Cookies.get("main_character");
    var characterInfoTemplateData = {
        ID: id,
        Name: character.name,
        Gender: gender[character.gender],
        Race: ffxivData["Race"][character.race].Name,
        GrandCompany: grandCompanies[character.grandCompany.nameID],
        ShowButton: mainCharacterID && mainCharacterID == id.toString() ? false : true
    };
    $('#info-section').html(characterInfoTemplate(characterInfoTemplateData)).show();
}

function displayJobs(character) {
    var jobs = { "Damage": [], "Hand": [], "Tank": [], "Healer": [] };
    for (var classJob in character.classJobs) {
        var key = character.classJobs[classJob];
        var currentClass = classes[key.jobID];
        if (key.level == 80) {
            jobs[currentClass.Type].push({ "Icon": currentClass.Icon, "ExpLevel": "", "ExpLevelMax": 100, "Level": key.level, "Name": currentClass.Name, "Width": 100 });
        } else if (key.level != 0) {
            jobs[currentClass.Type].push({ "Icon": currentClass.Icon, "ExpLevel": key.expLevel, "ExpLevelMax": key.expLevelMax, "Level": key.level, "Name": currentClass.Name, "Width": key.expLevel / key.expLevelMax * 100 });
        }
    };
    characterJobsTemplateData = {
        ClassTypes: [
            { Name: "Tank", Classes: jobs.Tank },
            { Name: "Damage", Classes: jobs.Damage },
            { Name: "Healer", Classes: jobs.Healer },
            { Name: "Hand & Land", "Classes": jobs.Hand }]
    }
    $('#jobs-section .dynamic-data-section').html(characterJobsTemplate(characterJobsTemplateData));
}

function displayMounts(data) {
    characterCollectibleTemplateData = {
        OwnedMountCount: data.mounts.length,
        MountCount: Object.keys(ffxivData["mounts"]).length,
        Mounts: [],
        OwnedMinionCount: data.minions.length,
        MinionCount: Object.keys(ffxivData["minions"]).length,
        Minions: []
    }
    var ownedMounts = {};
    $.each(data.mounts.sort(compare), function (i, key) {
        var mount = ffxivData["mounts"][key.name.toLowerCase()];
        ownedMounts[mount.id] = {};
        mount.Owned = true;
        characterCollectibleTemplateData.Mounts.push(ffxivData["mounts"][key.name.toLowerCase()])
    });
    $.each(ffxivData["mounts"], function (i, key) {
        key = setTooltip(key);
        if (!(key.id in ownedMounts)) {
            key.Owned = false
            characterCollectibleTemplateData.Mounts.push(key)
        }
    });
    var ownedMinions = {};
    $.each(data.minions.sort(compare), function (i, key) {
        var minion = ffxivData["minions"][key.name.toLowerCase()];
        ownedMinions[minion.id] = {};
        minion.Owned = true;
        characterCollectibleTemplateData.Minions.push(ffxivData["minions"][key.name.toLowerCase()])
    });
    $.each(ffxivData["minions"], function (i, key) {
        key = setTooltip(key);
        if (!(key.id in ownedMinions)) {
            key.Owned = false;
            characterCollectibleTemplateData.Minions.push(key)
        }
    });
    $('#collectible-section .dynamic-data-section').html(characterCollectibleTemplate(characterCollectibleTemplateData));
}

function setTooltip(enumeration) {
    if (enumeration.sources.length > 0) {
        hover = [];
        $.each(enumeration.sources, function (i, key) {
            hover.push(key.type + ": " + key.text);
        });
        enumeration.hover = hover.join("|");
    } else {
        enumeration.hover = enumeration.name;
    }
    return enumeration;
}

function loadCollection(enumerationType, resolve) {
    if (!(enumerationType in ffxivData)) {
        ffxivData[enumerationType] = {};
    }
    $.get("https://ffxivcollect.com/api/" + enumerationType, function (data) {
        $.each(data.results.sort(compare), function (i, key) {
            ffxivData[enumerationType][key.name.toLowerCase()] = key;
        });
        resolve();
    });
}

function loadEnumeration(enumerationType, page, resolve, reject) {
    if (!(enumerationType in ffxivData)) {
        ffxivData[enumerationType] = {};
    }
    $.get("https://xivapi.com/" + enumerationType + "?page=" + page, function (data) {
        $.each(data.Results, function (i, key) {
            if (key.Name != "") {
                ffxivData[enumerationType][key.Name.toLowerCase()] = key;
                ffxivData[enumerationType][key.ID] = key;
            }
        });
        if (data.Pagination.PageTotal > data.Pagination.Page) {
            loadEnumeration(enumerationType, page + 1, resolve, reject);
        } else {
            resolve();
        }
    });
}

//Helper functions
function getUrlVars() {
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
        vars[key] = value;
    });
    return vars;
}

var ffxivData = {};

//Static Enumerations
var classes = {};
classes[1] = { "ID": 1, "Icon": "https://xivapi.com/cj/1/gladiator.png", "Name": "gladiator", "Type": "Damage" }
classes[2] = { "ID": 2, "Icon": "https://xivapi.com/cj/1/pugilist.png", "Name": "pugilist", "Type": "Damage" }
classes[3] = { "ID": 3, "Icon": "https://xivapi.com/cj/1/marauder.png", "Name": "marauder", "Type": "Damage" }
classes[4] = { "ID": 4, "Icon": "https://xivapi.com/cj/1/lancer.png", "Name": "lancer", "Type": "Damage" }
classes[5] = { "ID": 5, "Icon": "https://xivapi.com/cj/1/archer.png", "Name": "archer", "Type": "Damage" }
classes[6] = { "ID": 6, "Icon": "https://xivapi.com/cj/1/conjurer.png", "Name": "conjurer", "Type": "Damage" }
classes[7] = { "ID": 7, "Icon": "https://xivapi.com/cj/1/thaumaturge.png", "Name": "thaumaturge", "Type": "Damage" }
classes[8] = { "ID": 8, "Icon": "https://xivapi.com/cj/1/carpenter.png", "Name": "carpenter", "Type": "Hand" }
classes[9] = { "ID": 9, "Icon": "https://xivapi.com/cj/1/blacksmith.png", "Name": "blacksmith", "Type": "Hand" }
classes[10] = { "ID": 10, "Icon": "https://xivapi.com/cj/1/armorer.png", "Name": "armorer", "Type": "Hand" }
classes[11] = { "ID": 11, "Icon": "https://xivapi.com/cj/1/goldsmith.png", "Name": "goldsmith", "Type": "Hand" }
classes[12] = { "ID": 12, "Icon": "https://xivapi.com/cj/1/leatherworker.png", "Name": "leatherworker", "Type": "Hand" }
classes[13] = { "ID": 13, "Icon": "https://xivapi.com/cj/1/weaver.png", "Name": "weaver", "Type": "Hand" }
classes[14] = { "ID": 14, "Icon": "https://xivapi.com/cj/1/alchemist.png", "Name": "alchemist", "Type": "Hand" }
classes[15] = { "ID": 15, "Icon": "https://xivapi.com/cj/1/culinarian.png", "Name": "culinarian", "Type": "Hand" }
classes[16] = { "ID": 16, "Icon": "https://xivapi.com/cj/1/miner.png", "Name": "miner", "Type": "Hand" }
classes[17] = { "ID": 17, "Icon": "https://xivapi.com/cj/1/botanist.png", "Name": "botanist", "Type": "Hand" }
classes[18] = { "ID": 18, "Icon": "https://xivapi.com/cj/1/fisher.png", "Name": "fisher", "Type": "Hand" }
classes[19] = { "ID": 19, "Icon": "https://xivapi.com/cj/1/paladin.png", "Name": "paladin", "Type": "Tank" }
classes[20] = { "ID": 20, "Icon": "https://xivapi.com/cj/1/monk.png", "Name": "monk", "Type": "Damage" }
classes[21] = { "ID": 21, "Icon": "https://xivapi.com/cj/1/warrior.png", "Name": "warrior", "Type": "Tank" }
classes[22] = { "ID": 22, "Icon": "https://xivapi.com/cj/1/dragoon.png", "Name": "dragoon", "Type": "Damage" }
classes[23] = { "ID": 23, "Icon": "https://xivapi.com/cj/1/bard.png", "Name": "bard", "Type": "Damage" }
classes[24] = { "ID": 24, "Icon": "https://xivapi.com/cj/1/whitemage.png", "Name": "white mage", "Type": "Healer" }
classes[25] = { "ID": 25, "Icon": "https://xivapi.com/cj/1/blackmage.png", "Name": "black mage", "Type": "Damage" }
classes[26] = { "ID": 26, "Icon": "https://xivapi.com/cj/1/arcanist.png", "Name": "arcanist", "Type": "Damage" }
classes[27] = { "ID": 27, "Icon": "https://xivapi.com/cj/1/summoner.png", "Name": "summoner", "Type": "Damage" }
classes[28] = { "ID": 28, "Icon": "https://xivapi.com/cj/1/scholar.png", "Name": "scholar", "Type": "Healer" }
classes[29] = { "ID": 29, "Icon": "https://xivapi.com/cj/1/rogue.png", "Name": "rogue", "Type": "Damage" }
classes[30] = { "ID": 30, "Icon": "https://xivapi.com/cj/1/ninja.png", "Name": "ninja", "Type": "Damage" }
classes[31] = { "ID": 31, "Icon": "https://xivapi.com/cj/1/machinist.png", "Name": "machinist", "Type": "Damage" }
classes[32] = { "ID": 32, "Icon": "https://xivapi.com/cj/1/darkknight.png", "Name": "dark knight", "Type": "Tank" }
classes[33] = { "ID": 33, "Icon": "https://xivapi.com/cj/1/astrologian.png", "Name": "astrologian", "Type": "Healer" }
classes[34] = { "ID": 34, "Icon": "https://xivapi.com/cj/1/samurai.png", "Name": "samurai", "Type": "Damage" }
classes[35] = { "ID": 35, "Icon": "https://xivapi.com/cj/1/redmage.png", "Name": "red mage", "Type": "Damage" }
classes[36] = { "ID": 36, "Icon": "https://xivapi.com/cj/1/bluemage.png", "Name": "blue mage", "Type": "Damage" }
classes[37] = { "ID": 37, "Icon": "https://xivapi.com/cj/1/gunbreaker.png", "Name": "gun breaker", "Type": "Tank" }
classes[38] = { "ID": 38, "Icon": "https://xivapi.com/cj/1/dancer.png", "Name": "dancer", "Type": "Damage" }

var grandCompanies = {};
grandCompanies[1] = "Maelstrom";
grandCompanies[2] = "Order of the Twin Adder";
grandCompanies[3] = "Immortal Flames";

var gender = {};
gender[1] = "Male";
gender[2] = "Female";

var tribes = {};
tribes[1] = "Midlander";
tribes[2] = "Highlander";
tribes[3] = "Wildwood";
tribes[4] = "Duskwight";
tribes[5] = "Plainsfolk";
tribes[6] = "Dunesfolk";
tribes[7] = "Seeker of the Sun";
tribes[8] = "Keeper of the Moon";
tribes[9] = "Sea Wolf";
tribes[10] = "Hellsguard";
tribes[11] = "Raen";
tribes[12] = "Xaela";

function compare(a, b) {
    if (a.name < b.name) {
        return -1;
    }
    if (a.name > b.name) {
        return 1;
    }
    return 0;
}