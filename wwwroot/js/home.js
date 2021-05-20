$(document).ready(function () {
    $(document).on('click', '.member-item', function (i, e) {
        var $target = $(i.target);
        if ($target.is("span")) {
            $target = $(i.target).closest("li");
        }
        urlParameters = getUrlVars();
        window.history.pushState(null, "Finale Fantasia", "?p=character&id=" + $($target).data("id"));
        $('.content-section').hide();
        displayCharacterPage(($target).data("id"));
    });
});