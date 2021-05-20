$(document).ready(function () {
    var partyMountTemplateSource = document.getElementById("party-mount-template").innerHTML;
    var partyMinionTemplateSource = document.getElementById("party-minion-template").innerHTML;

    partyMountTemplate = Handlebars.compile(partyMountTemplateSource);
    partyMinionTemplate = Handlebars.compile(partyMinionTemplateSource);

    var partyList = {};
    $(document).on('click', '.member-item', function (i, e) {
        var $target = $(i.target);
        if ($target.is("span")) {
            $target = $(i.target).closest("li");
        }

        var memberId = $target.data("id"),
            memberName = $target.data("name"),
            $emptyElement = $('.party-member:not(.member-selected):first');

        if (memberId in partyList) { return; }
        if ($emptyElement.length == 0) { return; }
        partyList[memberId] = memberName;
        $emptyElement.html(memberName);
        $emptyElement.addClass("member-selected");
        $emptyElement.data("memberId", memberId);
        $emptyElement.data("memberName", memberName);

        displayCollectibleSection(partyList);
        buildPartyCollectibles(partyList);
    });

    $(document).on('click', '.member-selected', function (i, e) {
        var memberId = $(i.target).data("memberId"),
            $partyElement = $(i.target);

        delete partyList[memberId];
        $partyElement.html("Select Member");
        $partyElement.removeClass("member-selected");
        $partyElement.removeData("memberId");
        $partyElement.removeData("memberName");

        displayCollectibleSection(partyList);
        buildPartyCollectibles(partyList);
    });

    tippy.delegate('.party-container', {
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
});

function buildPartyCollectibles(partyList) {
    var partyMounts = {},
        partyMinions = {},
        characterPromises = [];

    $.each(partyList, function (key, value) {
        characterPromises.push(new Promise((resolve, reject) => {
            loadCharacterData(key, partyMounts, partyMinions, resolve, reject);
        }));
    });

    if (Object.keys(partyList).length) {
        Promise.all(characterPromises).then(values => {
            displayPartyMounts(partyMounts);
            displayPartyMinions(partyMinions);
        });
    }
}

function displayPartyMounts(partyMounts) {
    var partyMountTemplateData = {
        OwnedMountCount: Object.keys(partyMounts).length,
        MountCount: Object.keys(ffxivData["mounts"]).length,
        Mounts: []
    }
    var ownedMounts = {};
    $.each(partyMounts, function (key, value) {
        var mount = ffxivData["mounts"][value.toLowerCase()];
        ownedMounts[key] = {};
        mount.Owned = true;
        partyMountTemplateData.Mounts.push(ffxivData["mounts"][value.toLowerCase()])
    });
    $.each(ffxivData["mounts"], function (i, key) {
        key = setTooltip(key);
        if (!(key.id in ownedMounts)) {
            key.Owned = false
            partyMountTemplateData.Mounts.push(key)
        }
    });
    $('#mount-section .dynamic-data-section').html(partyMountTemplate(partyMountTemplateData));
}

function displayPartyMinions(partyMinions) {
    var partyMinionTemplateData = {
        OwnedMinionCount: Object.keys(partyMinions).length,
        MinionCount: Object.keys(ffxivData["minions"]).length,
        Minions: []
    }
    var ownedMinions = {};
    $.each(partyMinions, function (key, value) {
        var minion = ffxivData["minions"][value.toLowerCase()];
        ownedMinions[key] = {};
        minion.Owned = true;
        partyMinionTemplateData.Minions.push(ffxivData["minions"][value.toLowerCase()])
    });
    $.each(ffxivData["minions"], function (i, key) {
        key = setTooltip(key);
        if (!(key.id in ownedMinions)) {
            key.Owned = false;
            partyMinionTemplateData.Minions.push(key)
        }
    });
    $('#minion-section .dynamic-data-section').html(partyMinionTemplate(partyMinionTemplateData));
}

function displayCollectibleSection(partyList) {
    if (Object.keys(partyList).length) {
        $('#mount-section').html(characterDataContainerTemplate({ DataType: "Mounts" })).show();
        $('#minion-section').html(characterDataContainerTemplate({ DataType: "Minions" })).show();
    } else {
        $('#mount-section').html("");
        $('#minion-section').html("");
    }
}

function loadCharacterData(characterId, partyMounts, partyMinions, resolve) {
    $.get("/XIVAPI/GetCharacter?id=" + characterId, function (data) {
        if (data.character == null) {
            $("#mount-section .dynamic-data-section").html(noCharacterDataTemplate({ DataType: "job" }));
            $('#minion-section .dynamic-data-section').html(noCharacterDataTemplate({ DataType: "collectible" }));
            resolve();
        } else {
            $.each(data.mounts, function (i, mount) {
                var mountData = ffxivData["mounts"][mount.name.toLowerCase()];
                partyMounts[mountData.id] = mountData.name;
            });
            $.each(data.minions, function (i, minion) {
                var minionData = ffxivData["minions"][minion.name.toLowerCase()];
                partyMinions[minionData.id] = minionData.name;
            });
            resolve();
        }
    });
}