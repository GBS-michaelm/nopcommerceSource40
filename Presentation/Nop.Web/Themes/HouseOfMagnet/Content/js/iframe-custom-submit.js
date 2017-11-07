(function ($) {

    $.fn.SubmitItem = function (inputs) {
        this.click(function () {
            var postdata = JSON.stringify({ test: inputs });
            $.ajax({
                cache: false,
                url: "/shoppingcart/submititem?test=" + postdata,
                type: "post",
                data: postdata,
                success: function (data) {
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    _notImplemented("update price failed responce handling not implemented");
                }
            });
            console.log(inputs.test);
        })
    };

    var DataInput = function (inputType, inputValue) {
        this.inputType = inputType;
        this.inputValue = inputValue;
    }
}(jQuery));
