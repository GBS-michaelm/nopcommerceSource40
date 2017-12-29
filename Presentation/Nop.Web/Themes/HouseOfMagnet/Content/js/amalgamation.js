
    $(document).ready(function () {
             
        $(".increase-quantity.amal-group").click(function () {
            AmalIncreaseTextBoxQuantityGallery($("#addtocart_" + $(this).data('productid') + "_EnteredQuantity"));
        });

        $(".decrease-quantity.amal-group").click(function () {
            AmalDecreaseTextBoxQuantityGallery($("#addtocart_" + $(this).data('textboxid') + "_EnteredQuantity"));
        })


        //$(".amalgamation-textbox").unbind().change(function () {
        $(".amalgamation-textbox").unbind().on('input', function () {

            console.log("drop down changed");

            var productId = $(this).data('productid');
            var cartType = $("#" + productId).data('cart-type');
            var qty = $(this).val();
            
            console.log("qty: " + qty);

            var obj = jQuery.parseJSON('{ "productId": "' + productId + '","cartType": "' + cartType + '" , "quantity": "' + qty + '" }');

            var textbox = $(this);
            var addToCartButton = $("#" + productId);
            var buttonStyling = $('#amalgamationButton' + productId);

            console.log("textbox: " + textbox);
            console.log("addToCartButton: " + addToCartButton);
            console.log("buttonStyling: " + buttonStyling);

            //change button text to update unless they dont have an in cart class tag then leave as add to cart
            if (textbox.hasClass('in-cart')) {

                console.log("in-cart change entered");

                textbox.removeClass('in-cart');

                addToCartButton.removeClass('in-cart');
                addToCartButton.addClass('readyforcart');
                addToCartButton.val('Update'); //button text

                buttonStyling.addClass('amalgamation-button');
                buttonStyling.removeClass('amalgamation-button-in-cart');
                buttonStyling.removeClass('amalgamation-btn-incart');
                buttonStyling.css('opacity', 1);
                buttonStyling.find('i').removeClass('fa fa-check');

            }

            if (!textbox.hasClass('in-cart')) {

                console.log("NOT in-cart change entered");

                addToCartButton.addClass('readyforcart');

                buttonStyling.css('opacity', 1);

            }                      

            CreateCartLink(obj);

            //if (textbox.hasClass('group-amalgamation-textbox')) {
            //    console.log("is group page textbox");
            //    //AmalgamationIncreaseQuantity(dropdown);
                GetCurCartCountForThisCategory(textbox, false);
            //}
            
        });

        $(".amalgamation-product-box-add-to-cart-button").unbind().click(function () {
            console.log("Add to cart clicked");

            var addToCartButton = $(this);
            //ignore button clicks on items already in cart that havent had their quantities changed
            if (!addToCartButton.hasClass('in-cart') && addToCartButton.hasClass('readyforcart')) {
                AmalgamationAddToCart(addToCartButton);
            }

        });
        
        $('.amalgamation-textbox').each(function (index, value) {

            var textbox = $(this);
            if (textbox.hasClass('in-cart')) {
                var curInCart = textbox.data('initqty');
                
                textbox.val(curInCart);//.prop('selected', true);

                //AmalgamationIncreaseQuantity(dropdown);
                GetCurCartCountForThisCategory(textbox, true);
            }

        });
        




        LoadBar();
                
    });

    function UpdateBar() {
        console.log("bar update entered");
            var currentCategoryId = $("#AmalgamationBarWrap").data('category-id');
            var currentFeaturedProductId = $("#AmalgamationBarWrap").data('featured-id');

            //console.log("catid: " + currentCategoryId);
            //console.log("featuredId: " + currentFeaturedProductId);

            UpdateAmalgamationBar(currentCategoryId, currentFeaturedProductId);
               
    }

    function LoadBar() {
        //check on init load if page should display amalgamation bar(items for amalgamtion already exist in cart)
        $('.amalgamation-textbox').each(function (index, value) {
        
            var curInCart = $(this).val();

            console.log("cur in cart " + curInCart);

            if (curInCart > 0) {
                //$(".amalgamation-bar-handler").click();
                UpdateBar();
                return false; //break out of loop
            }

        });
    }


    //call to change cart link when a quantity is changed
    function CreateCartLink(obj) {
        var link = "addproducttocart/amalgamation/" + obj.productId + "/" + obj.cartType + "/" + obj.quantity + "";
        //var finalLink = "AjaxCart.addproducttocart_catalog('" + link + "'); return false";

        //var finalLink = link;



        $("#" + obj.productId).attr('data-cartlink', link);

        $("#" + obj.productId).data('cartlink', link);

        console.log("finallink: " + link);
        //console.log("link custom attr" + $("#" + obj.productId).data('cartlink'));


    }
    
    function AmalgamationAddToCart(addToCartButton) {

        var textbox = $("#addtocart_" + addToCartButton.attr('id') + "_EnteredQuantity");
        var buttonStyling = $('#amalgamationButton' + addToCartButton.attr('id'));
        var link = addToCartButton.data('cartlink');

        console.log("addtocartlink : " + link);


        $.ajax({
            type: "POST",
            url: link,
            cache: false,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                console.log("success");
                console.log(msg); //entire object
                console.log(msg.qty);

                if (msg.qty == 0) {
                    console.log("removed from cart");

                    //set text To Add To Cart change styling to green style and opacity
                    textbox.removeClass('in-cart');

                    addToCartButton.val("Add To Cart");
                    addToCartButton.removeClass('readyforcart');
                    addToCartButton.removeClass('amalgamation-btn-incart');

                    buttonStyling.addClass('amalgamation-button');
                    buttonStyling.removeClass('amalgamation-button-in-cart');
                    buttonStyling.removeClass('amalgamation-btn-incart');
                    buttonStyling.css('opacity', 0.3);
                    //buttonStyling.find('i').removeClass('fa fa-check');

                } else {
                    console.log("added to cart");

                    //Set Text to In Cart and set to blue stlying                       
                    textbox.addClass('in-cart');

                    addToCartButton.val("In Cart");
                    addToCartButton.removeClass('readyforcart');
                    addToCartButton.addClass('amalgamation-btn-incart');

                    buttonStyling.addClass('amalgamation-button-in-cart');
                    buttonStyling.addClass('amalgamation-btn-incart');
                    buttonStyling.removeClass('amalgamation-button');
                    //buttonStyling.find('i').addClass('fa fa-check');

                }
                                
                UpdateBar();

            },
            error: function (msg) {
                console.log("error");
                console.log(msg);
                console.log(msg.d);
            },
            complete: function (data) {

            },

        });

    }

function UpdateAmalgamationBar(currentCategoryId, currentFeaturedProductId) {
      
    var link = "amalgamationbar/" + currentCategoryId + "/" + currentFeaturedProductId + "";
    
    if (!$("#amalgamationBarSection").is(":visible")) {
        $("#amalgamationBarSection").show();
    }


    $.ajax({
        type: "POST",
        url: link,
        cache: false,
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            console.log("success");
            console.log(msg); //entire object
            //console.log(msg.qty);
                        
            var priceEach = msg.eachPrice > 1 ? "$" + msg.eachPrice : msg.eachPrice.replace(".", "") + "&cent;";

            //fill bar with data
            //single
            if (msg.totalCartons == 1) {
                $("#currentTierText").html("You've added " + msg.totalCartons + " " + msg.packType.toLowerCase() + " for $" + msg.cartTotalPrice + " (" + priceEach + " ea)");
            }//multiple
            else {
                $("#currentTierText").html("You've added " + msg.totalCartons + " " + msg.packType.toLowerCase() + "s for $" + msg.cartTotalPrice + " (" + priceEach + " ea)");
            }
            
            //check for BEST PRICING 
            if (msg.tierNext == "best") {
                $("#nextTierText").html("You have the best price in the market!");
            }
            else {

                var nextPrice = msg.tierNextEach > 1 ? "$" + msg.tierNextEach : msg.tierNextEach.replace(".", "") + "&cent;";
                $("#nextTierText").html("Add " + msg.tierNext + " more and pay just " + nextPrice + "");
            }        
            

            if(!msg.totalCartons > 0){
                $("#amalgamationBarSection").hide();
            }
            
        },
        error: function (msg) {
            console.log("error");
            console.log(msg);
            console.log(msg.d);
        },
        complete: function (data) {

        },

    });

    //console.log("cartCount : " + cartCount);

    //if (!$("#amalgamationBarSection").is(":visible") && cartCount > 0) {
    //    $("#amalgamationBarSection").show();
    //}

    //else {
    //    $("#amalgamationBarSection").hide();
    //}
        

}


//FOR GROUP PAGE USE
function AmalgamationIncreaseQuantity(e, curCartQty, initLoad, singleTotal) {

    var textbox = e;
    var id = $(textbox).attr('id');
    var productId = $(textbox).data('productid');
    var featuredProductId = $(textbox).data('featuredproductid');
    //var curCartQty = GetCurCartCountForThisCategory($(dropDown).data('master-category-id'));

    //console.log("id: " + id);
    //console.log("productId: " + productId);
    //console.log("curCartQty: " + curCartQty);

    console.log("increase 1");

    if (id <= 0)
        id = 0;
    // var pricechange = document.getElementById("pricescript").textContent;
    //var pricechange = $(".pricescript-value-" + e.dataset.productid).text();
    var pricechange = $(".pricescript-value-" + productId).text();
    
    //var quantity = parseInt(id) + 1;
    var quantity = $(e).val();
    console.log("quantity: " + quantity);
    //var isPriceChange = GetTierPrice(quantity, e.dataset.productid);

    var qtyForTier = 0;
    
    if (initLoad == true) {
        qtyForTier = parseInt(curCartQty);
    } else {
        //removing # from cart
        if (singleTotal > quantity) {
            qtyForTier = parseInt(curCartQty) - (parseInt(singleTotal) - parseInt(quantity));
        }//increasing # in cart
        else {
            qtyForTier = parseInt(curCartQty) + (parseInt(quantity) - parseInt(singleTotal));
        }               
    }

    //quantity will need to change to current cart quantity + quantity selected
    var isPriceChange = GetTierPrice(qtyForTier, featuredProductId); //change to featured produt id

    pricechange = isPriceChange > 0 ? isPriceChange : pricechange;


    var qtyValue = parseInt(id);
    //qtyValue = qtyValue + 1;
    qtyValue = quantity;

    var final = parseFloat(qtyValue * pricechange).toFixed(2);
    //document.getElementById(e.id).value = qtyValue;
    $(e).val(qtyValue);
    
    //  document.getElementById("eachprice").textContent = "(Ea. $"+document.getElementById("pricescript").textContent+")";
    //$(".eachprice-value-" + e.dataset.productid).text("(Ea. $" + pricechange + ")");
    $(".eachprice-value-" + productId).text("(Ea. $" + pricechange + ")");


    //document.getElementById("newprice").textContent = "$"+final;
    //$(".price-value-" + e.dataset.productid).text("$" + final);
    $(".price-value-" + productId).text("$" + final);

    //if (initLoad == true) {
    //    $(".amalgamation-textbox").trigger("input");
    //}

    console.log("increase 1 final price ea" + pricechange);

    return true;
}

function AmalgamationChangeQuantity(e) {
    console.log("AmalgamationChangeQuantity");
    
    var textbox = e;
    var id = $(textbox).attr('id');
    
    var qtyValue = 0;

    if ($(textbox).hasClass('in-cart')) {
        qtyValue = parseInt($(textbox).data('initqty'));
    }
    
    document.getElementById(e.id).value = qtyValue;
    
    return true;
}

function AmalIncreaseTextBoxQuantity(e) {

    console.log("AmalIncreaseTextBoxQuantity");

    var id = document.getElementById(e.id).value;

    var featuredProductId = $(e).data('featuredproductid');
    if (id <= 0)
        id = 0;   
   
    var quantity = parseInt(id) + 1;
    
    var qtyValue = parseInt(id);
    qtyValue = qtyValue + 1;

    $(e).val(qtyValue);       

    $("#" + $(e).attr('id')).trigger("input");

    return true;
}

function AmalIncreaseTextBoxQuantityGallery(e) {

    console.log("increase gal: ");

    var id = $(e).val();
    var featuredProductId = $(e).data('featuredproductid');
    if (id <= 0)
        id = 0;
    //console.log("id: " + id);

    var quantity = parseInt(id) + 1;
    var qtyValue = parseInt(id);
    qtyValue = qtyValue + 1;    
    $(e).val(qtyValue);      
    $("#" + e.attr('id')).trigger("input");
    
    return true;
}

function AmalDecreaseTextBoxQuantity(e) {

    var id = document.getElementById(e.id).value;
    
    if (id <= 0)
        id = 0;

    var qtyValue = parseInt(id);
    if (id <= 0) {
        qtyValue = 0;
    }
    else {
        qtyValue = qtyValue - 1;
    }
        
    document.getElementById(e.id).value = qtyValue;  
        
    $(".amalgamation-textbox").trigger("input");

    return true;
}

function AmalDecreaseTextBoxQuantityGallery(e) {

    console.log("decrease: ");

    var id = $(e).val();

    if (id <= 0)
        id = 0;

    var qtyValue = parseInt(id);
    if (id <= 0) {
        qtyValue = 0;
    }
    else {
        qtyValue = qtyValue - 1;
    }

    $(e).val(qtyValue);
    
    var initQty = $(e).data('initqty');
    //handle opacity on entering 0 on init load
    console.log("init: " + initQty + " qtyVal: " + qtyValue);
    if (initQty == 0 && qtyValue == 0 && !$(e).hasClass('in-cart')) {
        var idHandle = $(e).data('productid');
        //$("#" + idHandle).removeClass('readyforcart');
        var buttonStyling = $('#amalgamationButton' + idHandle);
        buttonStyling.css('opacity', .3);
    } else {
        $("#" + e.attr('id')).trigger("input");
    }  

    return true;
}

function GetCurCartCountForThisCategory(textbox, initLoad) {

    var masterCategoryId = $(textbox).data('master-category-id');
    var productId = $(textbox).data('productid');

    var link = "amalgamationgetcarttotal/" + masterCategoryId + "/" + productId + "";

    console.log("get cart total : " + masterCategoryId);

    var total = 0;

    $.ajax({
        type: "POST",
        url: link,
        cache: false,
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            console.log("success");
            console.log(msg); //entire object
            console.log(msg.cartTotal);
             
            console.log("msg.cartTotal: " + msg.cartTotal);

            //total = msg.cartTotal;

            AmalgamationIncreaseQuantity(textbox, msg.cartTotal, initLoad, msg.singleTotal);

        },
        error: function (msg) {
            console.log("error");
            console.log(msg);
            console.log(msg.d);
        },
        complete: function (data) {

        },

    });

    //console.log("total: " + total);

    //return total;
    return true;
}



//backups
//function AmalgamationIncreaseQuantity(e, curCartQty, initLoad, singleTotal) {

//    var textbox = e;
//    var id = $(textbox).attr('id');
//    var productId = $(textbox).data('productid');
//    var featuredProductId = $(textbox).data('featuredproductid');
//    //var curCartQty = GetCurCartCountForThisCategory($(dropDown).data('master-category-id'));

//    console.log("id: " + id);
//    console.log("productId: " + productId);
//    console.log("curCartQty: " + curCartQty);

//    if (id <= 0)
//        id = 0;
//    // var pricechange = document.getElementById("pricescript").textContent;
//    //var pricechange = $(".pricescript-value-" + e.dataset.productid).text();
//    var pricechange = $(".pricescript-value-" + productId).text();

//    //var quantity = parseInt(id) + 1;
//    var quantity = $(e).val();
//    //var isPriceChange = GetTierPrice(quantity, e.dataset.productid);

//    var qtyForTier = 0;

//    if (initLoad == true) {
//        qtyForTier = parseInt(curCartQty);
//    } else {
//        if (singleTotal > quantity) {
//            qtyForTier = parseInt(curCartQty) - (parseInt(singleTotal) - parseInt(quantity));
//        } else {
//            qtyForTier = parseInt(curCartQty) + parseInt(quantity);
//        }
//    }

//    //quantity will need to change to current cart quantity + quantity selected
//    var isPriceChange = GetTierPrice(qtyForTier, featuredProductId); //change to featured produt id

//    pricechange = isPriceChange > 0 ? isPriceChange : pricechange;


//    var qtyValue = parseInt(id);
//    //qtyValue = qtyValue + 1;
//    qtyValue = quantity;

//    var final = parseFloat(qtyValue * pricechange).toFixed(2);
//    //document.getElementById(e.id).value = qtyValue;
//    $(e).val(qtyValue);

//    //  document.getElementById("eachprice").textContent = "(Ea. $"+document.getElementById("pricescript").textContent+")";
//    //$(".eachprice-value-" + e.dataset.productid).text("(Ea. $" + pricechange + ")");
//    $(".eachprice-value-" + productId).text("(Ea. $" + pricechange + ")");


//    //document.getElementById("newprice").textContent = "$"+final;
//    //$(".price-value-" + e.dataset.productid).text("$" + final);
//    $(".price-value-" + productId).text("$" + final);

//    $(".amalgamation-textbox").unbind().trigger("input");

//    return true;
//}