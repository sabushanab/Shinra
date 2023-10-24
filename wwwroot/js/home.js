var leaderboard = document.getElementById("leaderboard"),
    addCharacter = document.getElementById("add-character"),
    usRegion = document.getElementById("us-region"),
    euRegion = document.getElementById("eu-region"),
    realm = document.getElementById("realm"),
    characterName = document.getElementById("character-name"),
    dataDictionary = {},
    addCharacterModalElement = document.getElementById('add-character-modal'),
    addCharacterModal = bootstrap.Modal.getOrCreateInstance(addCharacterModalElement),
    addCharacterCanvasElement = document.getElementById('add-character-canvas'),
    addCharacterCanvas = new bootstrap.Offcanvas(addCharacterCanvasElement),
    pagination = document.getElementById("pagination"),
    pageSize = 50;

handleRegionSelect(usRegion.value);
function getAllCharacters(page) {
    fetch(`/blizzard/GetAllCharacters?page=${page}`)
        .then((response) => response.json())
        .then((data) => {
            buildCharacterLeaderboards(data);
            buildPagination(data, page);
        });
}

function buildCharacterLeaderboards(data) {
    dataDictionary = {};
    var leaderboardContent = `
        ${data.characters.map(characterTemplate).join("")}
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

function buildPagination(data, page) {
    maxPage = Math.floor(data.totalCount / pageSize);
    maxPage = maxPage == 0 ? 1 : maxPage;
    if (maxPage == 1) { return; }
    var paginationContent = `
        ${previousPagesTemplate(page)}
        ${pageTemplate(page, true)}
        ${nextPagesTemplate(page, maxPage)}
    `;
    pagination.innerHTML = paginationContent;
    bindPaginationEvents();
}

function bindPaginationEvents() {
    var elements = document.getElementsByClassName("page-link");
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener("click", function (event) {
            event.preventDefault();
            if (event.target.classList.contains("active")) {
                return;
            }
            getAllCharacters(parseInt(event.target.dataset.page));
        })
    }
}

function previousPagesTemplate(page) {
    var previousPages = [];
    for (var i = page - 1; i >= 1; i--) {
        if (previousPages.length >= 3) {
            break;
        }
        previousPages.push(pageTemplate(i, false));
    }
    return previousPages.join("");
}

function nextPagesTemplate(page, maxPage) {
    var nextPages = [];
    for (var i = page + 1; i <= maxPage; i++) {
        if (nextPages.length >= 3) {
            break;
        }
        nextPages.push(pageTemplate(i, false));
    }
    return nextPages.join("");
}

function pageTemplate(page, currentPage) {
    return `<li class="page-item ${currentPage ? "active" : ""}"><a class="page-link" href="#" data-page="${page}">${page}</a></li>`;
}

function pointOverviewTemplate(data) {
    var lastUpdatedDate = new Date(data.lastUpdated);
    return `
        <ul class="list-group">
            <li class="list-group-item">
                Level ${data.level}
                <span class="badge bg-primary rounded-pill float-end">${data.level}</span>
            </li>
            ${data.mythicPlusScore > 0 ? mythicPlusTemplate(data) : ''}
            ${data.categories.map(categoryTemplate).join("")}
            <li class="list-group-item">
                Last Updated - ${lastUpdatedDate.toLocaleString()}
            </li>
        </ul>
    `;
}

function mythicPlusTemplate(data) {
    return `
        <li class="list-group-item">
            Mythic Plus Rating
            <span class="badge bg-primary rounded-pill float-end">${Math.trunc(data.mythicPlusScore)}</span>
        </li>
    `
}

function categoryTemplate(data) {
    return `
        <li class="list-group-item">
            ${data.name}
            <span class="badge bg-primary rounded-pill float-end">${Math.trunc(data.totalPoints)}</span>
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


function characterTemplate(data) {
    dataDictionary[data._id] = data;
    return `
        <div class="accordion-item">
            <h2 class="accordion-header" id="heading-${data._id}">
                <button class="accordion-button collapsed px-1 px-md-3${data.hasDied ? ' character-died' : ''}" type="button" data-bs-toggle="collapse" data-bs-target="#collapse-${data._id}" aria-controls="collapse-${data._id}">
                    <img src="/img/${data.characterClass.replace(" ", "").toLowerCase()}_warcraftflat.png" width="25" height="25" class="me-2" />
                    ${data.hasDied ? '<img src="/img/skull.png" width="25" height="25" class="me-2" />' : ''}
                    ${data.characterName}<span class="d-none d-md-inline-block">-${data.realm}</span>
                    <div class="ms-2 point-badge">
                        <span class="badge bg-success">Level ${data.level}</span>
                        <span class="badge bg-info">${Math.trunc(data.totalPoints)} Points</span>
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
        let region = document.querySelector(".region-btn.active").value;
        addCharacter.setAttribute("disabled", "");
        addCharacter.innerHTML = `
            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Adding Character...
        `;
        fetch(`/blizzard/getcharacterpoints?realm=${realm.value.toLowerCase()}&characterName=${characterName.value.toLowerCase()}&region=${region.toLowerCase()}`)
            .then((response) => response.json())
            .then((data) => {
                characterName.value = "";
                addCharacter.removeAttribute("disabled");
                addCharacter.innerHTML = `Add Character`;
                var isMobile = ["xs", "sm"].indexOf(getViewport()) > -1;
                var title = "";
                var body = "";
                if (data.level == -1) {
                    title = "Character Not Found";
                    body = `<p>Could not find character: ${data.characterName}-${data.realm}</p>`;
                } else if (data._notAdded && data.hasDied) {
                    title = "Character Not Hardcore";
                    body = `<p>Character has deaths already and is not hardcore: ${data.characterName}-${data.realm}</p>`;
                } else if (data._notAdded && data.boosted) {
                    title = "Character was boosted";
                    body = `<p>Character has been flagged for boosting: ${data.characterName}-${data.realm}</p>`;
                } else if (data._notAdded) {
                    title = "Character already added";
                    body = `<p>Character has already been added: ${data.characterName}-${data.realm}</p>`;
                } else {
                    title = "Character Added";
                    body = `${pointOverviewTemplate(data)}`;
                    getAllCharacters();
                }
                if (isMobile) {
                    addCharacterCanvasElement.querySelector("#offcanvasTopLabel").innerHTML = title;
                    addCharacterCanvasElement.querySelector(".offcanvas-body").innerHTML = body
                    addCharacterCanvas.show();
                } else {
                    addCharacterModalElement.querySelector(".modal-title").innerHTML = title;
                    addCharacterModalElement.querySelector(".modal-body").innerHTML = body;
                    addCharacterModal.show();
                }
            });
    }
});

usRegion.addEventListener("click", function (event) {
    if (event.target.classList.contains("active")) {
        return;
    } else {
        usRegion.classList.add("active");
        euRegion.classList.remove("active");
        handleRegionSelect(event.target.value);
    }
});

euRegion.addEventListener("click", function (event) {
    if (event.target.classList.contains("active")) {
        return;
    } else {
        euRegion.classList.add("active");
        usRegion.classList.remove("active");
        handleRegionSelect(event.target.value);
    }
});

function handleRegionSelect(value) {
    fetch(`/blizzard/GetRealms?region=${value}`)
        .then((response) => response.json())
        .then((data) => buildRegionSelect(data));
}

function buildRegionSelect(regions) {
    var optionHtml = `
        <option value="" disabled selected>--Select Realm--</option>
        ${regions.map(region => optionTemplate(region)).join("")}
    `;
    realm.innerHTML = optionHtml;
    const config = {
        search: true,
        creatable: true,
        maxHeight: '250px',
        size: '',
    }
    dselect(realm, config)
}

function optionTemplate(region) {
    return `<option value="${region}">${region}</option>`;
}

function getViewport() {
    const width = Math.max(document.documentElement.clientWidth, window.innerWidth || 0)
    if (width <= 576) return 'xs'
    if (width <= 768) return 'sm'
    if (width <= 992) return 'md'
    if (width <= 1200) return 'lg'
    return 'xl'
}

getAllCharacters(1);