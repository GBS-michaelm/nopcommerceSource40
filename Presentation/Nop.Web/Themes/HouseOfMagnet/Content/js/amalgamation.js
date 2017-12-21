
    $(document).ready(function () {
             
        


        $(".amalgamation-dropdown").unbind().change(function () {

            console.log("drop down changed");

            var productId = $(this).data('productid');
            var cartType = $("#" + productId).data('cart-type');
            var qty = $(this).val();
            var obj = jQuery.parseJSON('{ "productId": "' + productId + '","cartType": "' + cartType + '" , "quantity": "' + qty + '" }');

            var dropdown = $(this);
            var addToCartButton = $("#" + productId);
            var buttonStyling = $('#amalgamationButton' + productId);

            //change button text to update unless they dont have an in cart class tag then leave as add to cart
            if (dropdown.hasClass('in-cart')) {

                console.log("in-cart change entered");

                dropdown.removeClass('in-cart');

                addToCartButton.removeClass('in-cart');
                addToCartButton.addClass('readyforcart');
                addToCartButton.val('Update'); //button text

                buttonStyling.addClass('amalgamation-button');
                buttonStyling.removeClass('amalgamation-button-in-cart');
                buttonStyling.css('opacity', 1);
                buttonStyling.find('i').removeClass('fa fa-check');

            }

            if (!dropdown.hasClass('in-cart')) {

                console.log("NOT in-cart change entered");

                addToCartButton.addClass('readyforcart');

                buttonStyling.css('opacity', 1);

            }                      

            CreateCartLink(obj);

            if (dropdown.hasClass('group-amalgamation-dropdown')) {
                console.log("is group page dropdown");
                //AmalgamationIncreaseQuantity(dropdown);
                GetCurCartCountForThisCategory(dropdown, false);
            }
            
        });

        $(".amalgamation-product-box-add-to-cart-button").unbind().click(function () {
            console.log("Add to cart clicked");

            var addToCartButton = $(this);
            //ignore button clicks on items already in cart that havent had their quantities changed
            if (!addToCartButton.hasClass('in-cart') && addToCartButton.hasClass('readyforcart')) {
                AmalgamationAddToCart(addToCartButton);
            }

        });


        $('.group-amalgamation-dropdown').each(function (index, value) {

            var dropdown = $(this);
            if (dropdown.hasClass('in-cart')) {
                var curInCart = dropdown.data('initqty');
                
                dropdown.val(curInCart);//.prop('selected', true);

                //AmalgamationIncreaseQuantity(dropdown);
                GetCurCartCountForThisCategory(dropdown, true);
            }

        });


        LoadBar();
                
    });

    function UpdateBar() {
        console.log("bar update entered");
            var currentCategoryId = $("#AmalgamationBarWrap").data('category-id');
            var currentFeaturedProductId = $("#AmalgamationBarWrap").data('featured-id');

            console.log("catid: " + currentCategoryId);
            console.log("featuredId: " + currentFeaturedProductId);

            UpdateAmalgamationBar(currentCategoryId, currentFeaturedProductId);
               
    }

    function LoadBar() {
        //check on init load if page should display amalgamation bar(items for amalgamtion already exist in cart)
        $('.amalgamation-dropdown').each(function (index, value) {
        
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

    //function UpdateCartLink(obj) {
    //    //modify cart



    //}

    function AmalgamationAddToCart(addToCartButton) {

        var dropdown = $("#dropdown-qty-" + addToCartButton.attr('id'));
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
                    //set text To Add To Cart change styling to orange style and opacity
                    dropdown.removeClass('in-cart');

                    addToCartButton.val("Add To Cart");
                    addToCartButton.removeClass('readyforcart');

                    buttonStyling.addClass('amalgamation-button');
                    buttonStyling.removeClass('amalgamation-button-in-cart');
                    buttonStyling.css('opacity', 0.3);
                    buttonStyling.find('i').removeClass('fa fa-check');

                } else {
                    //Set Text to In Cart and set to green stlying                       
                    dropdown.addClass('in-cart');

                    addToCartButton.val("In Cart");
                    addToCartButton.removeClass('readyforcart');

                    buttonStyling.addClass('amalgamation-button-in-cart');
                    buttonStyling.removeClass('amalgamation-button');
                    buttonStyling.find('i').addClass('fa fa-check');

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
            $("#currentTierText").html("You've added " + msg.totalCartons + " cartons for $" + msg.cartTotalPrice + " (" + priceEach + " ea)");

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
//pass in dropdown? or just pass in drop down value or dropdown name  //, featuredProductId
function AmalgamationIncreaseQuantity(e, curCartQty, initLoad, singleTotal) {

    var dropDown = e;

    var id = $(dropDown).attr('id');
    var productId = $(dropDown).data('productid');
    var featuredProductId = $(dropDown).data('featuredproductid');
    //var curCartQty = GetCurCartCountForThisCategory($(dropDown).data('master-category-id'));

    console.log("id: " + id);
    console.log("productId: " + productId);
    console.log("curCartQty: " + curCartQty);

    if (id <= 0)
        id = 0;
    // var pricechange = document.getElementById("pricescript").textContent;
    //var pricechange = $(".pricescript-value-" + e.dataset.productid).text();
    var pricechange = $(".pricescript-value-" + productId).text();
    
    //var quantity = parseInt(id) + 1;
    var quantity = $(e).val();
    //var isPriceChange = GetTierPrice(quantity, e.dataset.productid);

    var qtyForTier = 0;
    
    if (initLoad == true) {
        qtyForTier = parseInt(curCartQty);
    } else {
        if (singleTotal > quantity) {

            qtyForTier = parseInt(curCartQty) - (parseInt(singleTotal) - parseInt(quantity));

        } else {

            qtyForTier = parseInt(curCartQty) + parseInt(quantity);

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


    return true;
}

function GetCurCartCountForThisCategory(dropdown, initLoad) {

    var masterCategoryId = $(dropdown).data('master-category-id');
    var productId = $(dropdown).data('productid');

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

            AmalgamationIncreaseQuantity(dropdown, msg.cartTotal, initLoad, msg.singleTotal);

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

}

