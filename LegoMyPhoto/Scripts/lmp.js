    $(function () {
        
        var getPage = function () {
            var $a = $(this);

            var options = {
                url: $a.attr("href"),
                type: "get"
            };

            $.ajax(options).done(function (data) {
                var target = $a.parents("div.pagedList").attr("data-lmp-target");
                $(target).replaceWith(data);
            });
        }

        $(".main-content").on("click", ".pagedList a", getPage);
    });

