
$(document).ready(function () {
    $(".increase-quantity.company-product-group").click(function () {
        //CompanyProductIncreaseTextBoxQuantityGallery($("#addtocart_" + $(this).data('productid') + "_EnteredQuantity"));
        //CompanyProductIncreaseQuantity

        console.log("comp prod inc: " + $(this).data('productid'));
    });

    $(".decrease-quantity.company-product-group").click(function () {
        //CompanyProductDecreaseTextBoxQuantityGallery($("#addtocart_" + $(this).data('textboxid') + "_EnteredQuantity"));
        //CompanyProductDecreaseQuantity
        console.log("comp prod dec " + $(this).data('productid'));
    })



    $("#btnUpdateDesigns").click(PreCheckCreatePreviews);



});

function PreCheckCreatePreviews() {

    var valid = true;
    //for each validation here

    //on pass field validation
    if (valid) {
        CreatePreviews(false);
    }
    

}

function CreatePreviews(isDefault) {
    $(".company-product").each(function (index, elem) {

        var id = elem.id;
        var sku = id.replace("company-product-", "");

        try {

            CreateSinglePreview(sku, isDefault);

        }
        catch (exeception) {
            console.log("fail on create preview");
            console.log("error: " + exeception);
        }
        
    });
}

function CreateSinglePreview(sku, isDefault) {

    //var userId = GLOB.UserId;
    var userId = "zxqg0uiw4hcuxn0cilcw1dtz";
    var postUrl = $("#canvasDomain").val() + "/api/Preview/GeneratePreview";
    var previewOptions = { "width": 400, "height": 400, "resizeMode": "Fit", "proofImageRendering": { "showStubContent": true } };
    var template = getTemplate($("#template-" + sku).val());
    var productDefinitions = [];

    //show loading gif
    //$("#dv-loading-" + wtpId).show();
    //$("#dv-loading-" + wtpId + " p.error").hide();
    //$("#dv-loading-" + wtpId + " img").show();
    //delete current img urls
    //$("#img-preview-" + wtpId).attr("src", "");
    //$("#a-preview-" + wtpId).attr("href", "");
    
    if (isDefault) {
        //default stuff
    }

    productDefinitions[0] = { "surfaces": ["NameBadges/" + template] };

    //Save Form Values for Proof Creation during Add to Cart
    //GLOB.Name = getOptionalFieldValue("lbl-field-Name");
    //GLOB.Title = getOptionalFieldValue("lbl-field-Title");
    //GLOB.Title2 = getOptionalFieldValue("lbl-field-Title2");
    ////GLOB.Office = getOptionalFieldValue("txt-office");
    ////GLOB.FullCompanyName = getOptionalFieldValue("txt-fullcompanyname");

    var itemsData = getItemsData(sku, isDefault);

    console.log("itemsData Look : " + itemsData);

    var data = JSON.stringify({ 'previewOptions': previewOptions, 'itemsData': itemsData, 'productDefinitions': productDefinitions, 'userId': userId });   

    $.ajax({
        type: "POST",
        url: postUrl,
        headers: { "X-CustomersCanvasAPIKey": "CustomersCanvas+NopCommerceSecurityKey" },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data
    })
    .fail(function (d) {
        console.log("ERROR during Preview Creation");
        //console.log(d);
        //console.log(d.statusText);

        //$("#dv-loading-" + wtpId + " p.error").show();
        //$("#dv-loading-" + wtpId + " p.error").html("Error");
        //$("#dv-loading-" + wtpId + " img").hide();
    })
    .done(function (d) {
        console.log("done")

        // List the URLs of proof images.
        d.forEach(function (link) {
            console.log(link);

            $("#img-preview-" + sku).unbind("load");
            $("#img-preview-" + sku).load(function () {
                //$("#dv-loading-" + sku).fadeOut();
            });

            $("#img-preview-" + sku).attr("src", link);
            $("#a-preview-" + sku).attr("href", link);
        });
    });


    
}

function getItemsData(wtpId, isDefault) {
    ////var itemsData = {"Name": {},"Title": {},"Title2": {},"Office": {},"FullCompanyName": {}};

    var itemsData = {}; //JSON.parse($("#dv-preview-data-" + wtpId).html());
    
    if (itemsData["Name"] == null) {
        itemsData["Name"] = {};
    }

    if (itemsData["Title"] == null) {
        itemsData["Title"] = {};
    }

    if (itemsData["Title2"] == null) {
        itemsData["Title2"] = {};
    }

    if (itemsData["Office"] == null) {
        itemsData["Office"] = {};
    }

    if (itemsData["FullCompanyName"] == null) {
        itemsData["FullCompanyName"] = {};
    }

    //itemsData["Office"].text = GLOB.Office;
    //itemsData["FullCompanyName"].text = GLOB.FullCompanyName;

    if (!isDefault) {
        itemsData["Name"].text = getOptionalFieldValue("txt-Name");
        itemsData["Title"].text = getOptionalFieldValue("txt-Title");
        itemsData["Title2"].text = getOptionalFieldValue("txt-Title2");
    }

    /* Check Bold and Italic */
    var fieldList = ["Name", "Title", "Title2", "Office", "FullCompanyName"];

    for (var i = 0; i < fieldList.length; i++) {
        var fieldName = fieldList[i];
        if (itemsData[fieldName].font == null) { itemsData[fieldName].font = {}; }


        if ($("#btn-bold-txt" + fieldName.toLowerCase()).hasClass("active")) {
            itemsData[fieldName].font.fauxBold = true;
        }
        else {
            itemsData[fieldName].font.fauxBold = false;
        }

        if ($("#btn-italic-txt" + fieldName.toLowerCase()).hasClass("active")) {
            itemsData[fieldName].font.fauxItalic = true;
        }
        else {
            itemsData[fieldName].font.fauxItalic = false;
        }
    }

    return itemsData;
}

/* BEGIN - Utility Functions */
function getOptionalFieldValue(fieldId) {
    var fieldVal = $("#" + fieldId).val();

    //console.log("getOptionalFieldValue(" + fieldId + ")", fieldVal);

    if (fieldVal == null) {
        fieldVal = "";
    }

    return fieldVal;
}

function getTemplate(baseName) {
    var name = getOptionalFieldValue("txt-Name");
    var title = getOptionalFieldValue("txt-Title");
    var title2 = getOptionalFieldValue("txt-Title2");
    var templateNum = "1";

    if (title2.trim() != "") {
        templateNum = "3";
    }
    else if (title.trim() != "") {
        templateNum = "2";
    }

    return baseName + "-" + templateNum;
}






//name badge enter quantity


