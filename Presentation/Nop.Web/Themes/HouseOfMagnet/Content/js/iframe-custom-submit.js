(function ($) {
   
    $.fn.SubmitItem = function (option,  qty) {
        this.click(function () {
            var userInput = GetCSV(option);
            $.ajax({
                cache: false,
                url: "/shoppingcart/submititem?json=" + userInput + "&qty=" + qty,
                type: "post",
                success: function (data) {
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });

        })
    };

    $.fn.CollectProductAttributes = function ( formselector, url) {
        this.click(function () {
            $.ajax({
                cache: false,
                url: "shoppingcart/getproductoptions",
                data: $(formselector).serialize(),
                type: "post",
                success: function (data) {
                    window.location = url
                },
                error: function (xhr, ajaxOptions, thrownError) {
    
                }
            });

        })
    };
   
  

    //var GetCSV = function (input) {
    //    var csvVal = "";
    //    for (var key in input) {

    //        csvVal += key + ":" + input[key] + ","

    //    }
    //    csvVal = csvVal.slice(0, 1);

    //    return csvVal;
    //}
    
}(jQuery));

