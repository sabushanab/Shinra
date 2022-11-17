var leaderboard = document.getElementById("leaderboard"),
    addCharacter = document.getElementById("add-character"),
    realm = document.getElementById("realm"),
    characterName = document.getElementById("character-name"),
    dataDictionary = {},
    addCharacterModalElement = document.getElementById('add-character-modal'),
    addCharacterModal = bootstrap.Modal.getOrCreateInstance(addCharacterModalElement),
    addCharacterCanvasElement = document.getElementById('add-character-canvas'),
    addCharacterCanvas = new bootstrap.Offcanvas(addCharacterCanvasElement);

function getAllCharacters() {
    fetch('/blizzard/GetAllCharacters')
        .then((response) => response.json())
        .then((data) => buildCharacterLeaderboards(data));
}

function buildCharacterLeaderboards(data) {
    dataDictionary = {};
    var leaderboardContent = `
        ${data.map(characterTemplate).join("")}
    `;
    leaderboard.innerHTML = `${leaderboardContent}`;
    bindAccordionEvents();
}

function bindAccordionEvents() {
    var elements = document.getElementsByClassName("accordion-collapse");
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener("show.bs.collapse", function (event) {
            var characterData = dataDictionary[event.target.dataset.id],
                accordionBody = event.target.querySelector(".accordion-body");

            accordionBody.innerHTML = pointOverviewTemplate(characterData);
        })
    }
}

function pointOverviewTemplate(data) {
    return `
        <ul class="list-group">
            <li class="list-group-item">
                Level ${data.level}
                <span class="badge bg-primary rounded-pill float-end">${data.level}</span>
            </li>
            ${data.categories.map(categoryTemplate).join("")}
        </div>
    `;
}

function categoryTemplate(data) {
    return `
        <li class="list-group-item">
            ${data.name}
            <span class="badge bg-primary rounded-pill float-end">${data.totalPoints}</span>
            <ul>
            ${Object.keys(data.subCategories).map(key => subCategoryTemplate(key, data)).join("")}
            </ul>
        </li>
    `;
}

function subCategoryTemplate(key, data) {
    return `
        <li>${key}: ${data.subCategories[key]} Point${data.subCategories[key] > 1 ? "s" : ""}</li>
    `;
}

// Images from twitter @keyboardturn
function characterTemplate(data) {
    dataDictionary[data._id] = data;
    return `
        <div class="accordion-item">
            <h2 class="accordion-header" id="heading-${data._id}">
                <button class="accordion-button collapsed px-1 px-md-3${data.hasDied ? ' character-died' : ''}" type="button" data-bs-toggle="collapse" data-bs-target="#collapse-${data._id}" aria-controls="collapse-${data._id}">
                    <img src="/img/${data.characterClass.replace(" ", "").toLowerCase()}_warcraftflat.png" width="25" height="25" class="me-2" />
                    ${data.hasDied ? '<img src="/img/skull.png" width="25" height="25" class="me-2" />' : ''}
                    ${data.characterName}<span class="d-none d-md-inline-block">-${data.realm}</span>
                    <div class="ms-2 level-badge">
                        <span class="badge bg-success">Level ${data.level}</span>
                    </div>
                    <div class="ms-2 point-badge">
                        <span class="badge bg-info">${data.totalPoints} Points</span>
                    </div>
                </button>
            </h2>
            <div id="collapse-${data._id}" class="accordion-collapse collapse" aria-labelledby="heading-${data._id}" data-bs-parent="#leaderboard" data-id="${data._id}">
                <div class="accordion-body">
                    <span class="loader"></span>
                </div>
            </div>
        </div>
    `;
}

addCharacter.addEventListener("click", function (event) {
    if (!realm.value) {
        realm.classList.add("is-invalid");
    }
    if (!characterName.value) {
        characterName.classList.add("is-invalid");
    }
    if (realm.value && characterName.value) {
        addCharacter.setAttribute("disabled", "");
        addCharacter.innerHTML = `
            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Adding Character...
        `;
        fetch(`/blizzard/getcharacterpoints?realm=${realm.value.toLowerCase()}&characterName=${characterName.value.toLowerCase()}`)
            .then((response) => response.json())
            .then((data) => {
                realm.value = "";
                characterName.value = "";
                addCharacter.removeAttribute("disabled");
                addCharacter.innerHTML = `Add Character`;
                var isMobile = ["xs", "sm"].indexOf(getViewport()) > -1;
                if (data.level == 0) {
                    if (isMobile) {
                        addCharacterCanvasElement.querySelector("#offcanvasTopLabel").innerHTML = "Character Not Found";
                        addCharacterCanvasElement.querySelector(".offcanvas-body").innerHTML = `
                            <p>Could not find character: ${data.characterName}-${data.realm}</p>
                        `;
                        addCharacterCanvas.show();
                    } else {
                        addCharacterModalElement.querySelector(".modal-title").innerHTML = "Character Not Found";
                        addCharacterModalElement.querySelector(".modal-body").innerHTML = `
                            <p>Could not find character: ${data.characterName}-${data.realm}</p>
                        `;
                        addCharacterModal.show();
                    }
                } else if (data._notAdded) {
                    if (isMobile) {
                        addCharacterCanvasElement.querySelector("#offcanvasTopLabel").innerHTML = "Character Not Hardcore";
                        addCharacterCanvasElement.querySelector(".offcanvas-body").innerHTML = `
                            <p>Character has deaths already and is not hardcore: ${data.characterName}-${data.realm}</p>
                        `;
                        addCharacterCanvas.show();
                    } else {
                        addCharacterModalElement.querySelector(".modal-title").innerHTML = "Character Not Hardcore";
                        addCharacterModalElement.querySelector(".modal-body").innerHTML = `
                            <p>Character has deaths already and is not hardcore: ${data.characterName}-${data.realm}</p>
                        `;
                        addCharacterModal.show();
                    }
                } else {
                    if (isMobile) {
                        addCharacterCanvasElement.querySelector("#offcanvasTopLabel").innerHTML = "Character Added";
                        addCharacterCanvasElement.querySelector(".offcanvas-body").innerHTML = `
                            ${pointOverviewTemplate(data)}
                        `;
                        addCharacterCanvas.show();
                    } else {
                        addCharacterModalElement.querySelector(".modal-title").innerHTML = "Character Added";
                        addCharacterModalElement.querySelector(".modal-body").innerHTML = `
                            ${pointOverviewTemplate(data)}
                        `;
                        addCharacterModal.show();
                    }
                    getAllCharacters();
                }
            });
    }
});

function getViewport() {
    const width = Math.max(document.documentElement.clientWidth, window.innerWidth || 0)
    if (width <= 576) return 'xs'
    if (width <= 768) return 'sm'
    if (width <= 992) return 'md'
    if (width <= 1200) return 'lg'
    return 'xl'
}

getAllCharacters();