class HomeIndexPage {
    init() {
        $(".glyphicon-trash").click(() => confirm("Sure?"));

        var showIgnoredCheckbox = $("#showIgnored");
        showIgnoredCheckbox.click(() => {
            if (showIgnoredCheckbox.is(":checked")) {
                $(".ignored").show();
                window.localStorage["showIgnored"] = "true";
            } else {
                $(".ignored").hide();
                window.localStorage["showIgnored"] = "false";
            }
        });

        $(document).ready(() => {
            showIgnoredCheckbox.attr("checked", null);
            if (window.localStorage["showIgnored"] === "true") {
                showIgnoredCheckbox.click();
            }
        });

        var messageTitle = $(".message-title");
        messageTitle.click((evnt) => {
            $(evnt.target).siblings("div").toggle();
        });
    }
}