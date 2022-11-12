var leaderboard = document.getElementById("leaderboard"),
    addCharacter = document.getElementById("add-character"),
    realm = document.getElementById("realm"),
    characterName = document.getElementById("character-name");

function getAllCharacters() {
    fetch('/blizzard/GetAllCharacters')
        .then((response) => response.json())
        .then((data) => buildCharacterLeaderboards(data));
}

function buildCharacterLeaderboards(data) {
    var leaderboardContent = `${data.map(characterTemplate).join("")}`;
    leaderboard.innerHTML = `${leaderboardContent}`;
}

function characterTemplate(data) {
    return `
        <tr>
            <td>${data._id}</td>
            <td>${data.characterClass}</td>
            <td>${data.level}</td>
            <td>${data.totalPoints}</td>
        </tr>
    `;
}

addCharacter.addEventListener("click", function (event) {
    if (!realm.value) {
        realm.classList.add("is-invalid");
    }
    if (!characterName.value) {
        characterName.classList.add("is-invalid");
    }
    fetch(`/blizzard/getcharacterpoints?realm=${realm.value.toLowerCase()}&characterName=${characterName.value.toLowerCase()}`)
        .then((response) => response.json())
        .then((data) => {
            realm.value = "";
            characterName.value = "";
            getAllCharacters();
        });
});

getAllCharacters();