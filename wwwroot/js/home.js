$(document).ready(function () {
    $(document).on('click', '.member-item', function (i, e) {
        urlParameters = getUrlVars();
        window.history.pushState(null, "Finale Fantasia", "?p=character&id=" + $(i.target).data("id"));
        $('.content-section').hide();
        displayCharacterPage($(i.target).data("id"));
    });
});