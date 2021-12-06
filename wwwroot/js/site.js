var navTemplate,
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
    $.get("/Character/GetFreeCompanyMembers", function (data) {
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
    });
}

function displayCharacterPage(id) {
    $('#jobs-section').html(characterDataContainerTemplate({ DataType: "Jobs" })).show();
    $('#collectible-section').html(characterDataContainerTemplate({ DataType: "Collectibles" })).show();
    $.get("/Character/GetCharacter?id=" + id, function (data) {
        console.log(data);
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
        Gender: character.gender,
        Race: character.race,
        GrandCompany: character.grandCompany.name,
        ShowButton: mainCharacterID && mainCharacterID == id.toString() ? false : true
    };
    $('#info-section').html(characterInfoTemplate(characterInfoTemplateData)).show();
}

function displayJobs(character) {
    var jobs = { "Damage": [], "Hand": [], "Tank": [], "Healer": [] };
    for (var classJob in character.classJobs) {
        var key = character.classJobs[classJob];
        var currentClass = classes[key.name.toLowerCase()];
        console.log(key, currentClass);
        if (key.level == 90) {
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
        console.log(key.name.toLowerCase(), mount);
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
        console.log(key)
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
classes["gladiator"] = { "ID": 1, "Icon": "https://xivapi.com/cj/1/gladiator.png", "Name": "gladiator", "Type": "Damage" }
classes["pugilist"] = { "ID": 2, "Icon": "https://xivapi.com/cj/1/pugilist.png", "Name": "pugilist", "Type": "Damage" }
classes["marauder"] = { "ID": 3, "Icon": "https://xivapi.com/cj/1/marauder.png", "Name": "marauder", "Type": "Damage" }
classes["lancer"] = { "ID": 4, "Icon": "https://xivapi.com/cj/1/lancer.png", "Name": "lancer", "Type": "Damage" }
classes["archer"] = { "ID": 5, "Icon": "https://xivapi.com/cj/1/archer.png", "Name": "archer", "Type": "Damage" }
classes["conjurer"] = { "ID": 6, "Icon": "https://xivapi.com/cj/1/conjurer.png", "Name": "conjurer", "Type": "Damage" }
classes["thaumaturge"] = { "ID": 7, "Icon": "https://xivapi.com/cj/1/thaumaturge.png", "Name": "thaumaturge", "Type": "Damage" }
classes["carpenter"] = { "ID": 8, "Icon": "https://xivapi.com/cj/1/carpenter.png", "Name": "carpenter", "Type": "Hand" }
classes["blacksmith"] = { "ID": 9, "Icon": "https://xivapi.com/cj/1/blacksmith.png", "Name": "blacksmith", "Type": "Hand" }
classes["armorer"] = { "ID": 10, "Icon": "https://xivapi.com/cj/1/armorer.png", "Name": "armorer", "Type": "Hand" }
classes["goldsmith"] = { "ID": 11, "Icon": "https://xivapi.com/cj/1/goldsmith.png", "Name": "goldsmith", "Type": "Hand" }
classes["leatherworker"] = { "ID": 12, "Icon": "https://xivapi.com/cj/1/leatherworker.png", "Name": "leatherworker", "Type": "Hand" }
classes["weaver"] = { "ID": 13, "Icon": "https://xivapi.com/cj/1/weaver.png", "Name": "weaver", "Type": "Hand" }
classes["alchemist"] = { "ID": 14, "Icon": "https://xivapi.com/cj/1/alchemist.png", "Name": "alchemist", "Type": "Hand" }
classes["culinarian"] = { "ID": 15, "Icon": "https://xivapi.com/cj/1/culinarian.png", "Name": "culinarian", "Type": "Hand" }
classes["miner"] = { "ID": 16, "Icon": "https://xivapi.com/cj/1/miner.png", "Name": "miner", "Type": "Hand" }
classes["botanist"] = { "ID": 17, "Icon": "https://xivapi.com/cj/1/botanist.png", "Name": "botanist", "Type": "Hand" }
classes["fisher"] = { "ID": 18, "Icon": "https://xivapi.com/cj/1/fisher.png", "Name": "fisher", "Type": "Hand" }
classes["paladin"] = { "ID": 19, "Icon": "https://xivapi.com/cj/1/paladin.png", "Name": "paladin", "Type": "Tank" }
classes["monk"] = { "ID": 20, "Icon": "https://xivapi.com/cj/1/monk.png", "Name": "monk", "Type": "Damage" }
classes["warrior"] = { "ID": 21, "Icon": "https://xivapi.com/cj/1/warrior.png", "Name": "warrior", "Type": "Tank" }
classes["dragoon"] = { "ID": 22, "Icon": "https://xivapi.com/cj/1/dragoon.png", "Name": "dragoon", "Type": "Damage" }
classes["bard"] = { "ID": 23, "Icon": "https://xivapi.com/cj/1/bard.png", "Name": "bard", "Type": "Damage" }
classes["white mage"] = { "ID": 24, "Icon": "https://xivapi.com/cj/1/whitemage.png", "Name": "white mage", "Type": "Healer" }
classes["black mage"] = { "ID": 25, "Icon": "https://xivapi.com/cj/1/blackmage.png", "Name": "black mage", "Type": "Damage" }
classes["arcanist"] = { "ID": 26, "Icon": "https://xivapi.com/cj/1/arcanist.png", "Name": "arcanist", "Type": "Damage" }
classes["summoner"] = { "ID": 27, "Icon": "https://xivapi.com/cj/1/summoner.png", "Name": "summoner", "Type": "Damage" }
classes["scholar"] = { "ID": 28, "Icon": "https://xivapi.com/cj/1/scholar.png", "Name": "scholar", "Type": "Healer" }
classes["rogue"] = { "ID": 29, "Icon": "https://xivapi.com/cj/1/rogue.png", "Name": "rogue", "Type": "Damage" }
classes["ninja"] = { "ID": 30, "Icon": "https://xivapi.com/cj/1/ninja.png", "Name": "ninja", "Type": "Damage" }
classes["machinist"] = { "ID": 31, "Icon": "https://xivapi.com/cj/1/machinist.png", "Name": "machinist", "Type": "Damage" }
classes["dark knight"] = { "ID": 32, "Icon": "https://xivapi.com/cj/1/darkknight.png", "Name": "dark knight", "Type": "Tank" }
classes["astrologian"] = { "ID": 33, "Icon": "https://xivapi.com/cj/1/astrologian.png", "Name": "astrologian", "Type": "Healer" }
classes["samurai"] = { "ID": 34, "Icon": "https://xivapi.com/cj/1/samurai.png", "Name": "samurai", "Type": "Damage" }
classes["red mage"] = { "ID": 35, "Icon": "https://xivapi.com/cj/1/redmage.png", "Name": "red mage", "Type": "Damage" }
classes["blue mage"] = { "ID": 36, "Icon": "https://xivapi.com/cj/1/bluemage.png", "Name": "blue mage", "Type": "Damage" }
classes["gunbreaker"] = { "ID": 37, "Icon": "https://xivapi.com/cj/1/gunbreaker.png", "Name": "gunbreaker", "Type": "Tank" }
classes["dancer"] = { "ID": 38, "Icon": "https://xivapi.com/cj/1/dancer.png", "Name": "dancer", "Type": "Damage" }
classes["sage"] = { "ID": 39, "Icon": "https://xivapi.com/cj/1/sage.png", "Name": "sage", "Type": "Healer" }
classes["reaper"] = { "ID": 40, "Icon": "https://xivapi.com/cj/1/reaper.png", "Name": "reaper", "Type": "Damage" }

function compare(a, b) {
    if (a.name < b.name) {
        return -1;
    }
    if (a.name > b.name) {
        return 1;
    }
    return 0;
}