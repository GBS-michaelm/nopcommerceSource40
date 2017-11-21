var OptionValue = function (optionValueModel) {
    this.id = optionValueModel.Id;
    this.defaultValue = optionValueModel.DefaultValue;
    this.preselected = optionValueModel.IsPreSelected;
    this.title = optionValueModel.Name;
    this.price = optionValueModel.PriceAdjustmentValue;
}

var SelectedOption = function (option, value) {
    this.option = option;
    this.value = value;
}


var SelectedValue = function (id, preselected, title, price) {
    this.id = id;
    this.preselected = preselected;
    this.title = title;
    this.price = price;
}

var JsonArray = function (jArray)
{
    this.jArray = [];
}
var Options = function (optionModel) {

    var optionTypes = {
        1: "list", // DropdownList
        2: "radio", // RadioList
        3: "checkbox", // Checkboxes
        4: "text", // TextBox
        10: "textarea", // MultilineTextbox
        20: "date", // Datepicker
        30: "file", // FileUpload
        40: "color", // ColorSquares
        45: "image",  // ImageSquares
        50: "readonlycheckbox" // ReadonlyCheckboxes
    };

    var self = this;

    self.id = optionModel.Id;
    self.description = optionModel.Description;
    self.prompt = optionModel.TextPrompt;
    self.title = optionModel.Name;
    self.type = optionTypes[optionModel.AttributeControlType];
    self.required = optionModel.IsRequired;
    self.defaultValue = optionModel.DefaultValue;
    self.values = {};
    if (Array.isArray(optionModel.Values)) {
        optionModel.Values.forEach(function (value) {
            self.values[value.Name] = new OptionValue(value);
            switch (self.type) {
                case "color":
                    if (value.ImageSquaresPictureModel != null && value.ImageSquaresPictureModel.FullSizeImageUrl != null) {
                        var url = value.ImageSquaresPictureModel.FullSizeImageUrl;
                        self.values[value.Name].image = url;
                    }
                    self.values[value.Name].color = value.ColorSquaresRgb;
                    break;
                case "image":
                    var url = value.ImageSquaresPictureModel.FullSizeImageUrl;
                    if (url != null)
                        self.values[value.Name].image = url;
                    break;
            }
        });
    }



}

var CreateJson = function (array, options) {
    var optionArray = {};
    var vArray = {};
    var bufferArray = {};
    var bufferArray2 = {};
    var valueArray = [];

    for (var member in options) {
        if (options[member] != null) {

            optionArray[member] = options[member];
           
        }
       
    }
    switch (options.type) {
        case "list": // DropdownList
            break;
        case "radio": // RadioList
        case "checkbox": // Checkboxes
            for (var item in options.Values)
            {
                if ($("#product_attribute_" + options.id + "_" +options.Values[item].Id).checked)
                {
                    valueArray.push( new SelectedValue(options.Values[item].Id, false,  options.Values[item].Name, options.Values[item].PriceAdjustmentValue));

                }
            }
         
            break;
        case "text": // TextBox
            //if ($("#product_attribute_" + options.id).val() != "") {
            //    valueArray.push(new SelectedValue(options.id, false));
            //    console.log(valueArray);
            //}
            break;
        case "textarea": // MultilineTextbox

            break;
        case "date": // Datepicker
            break;
        case "file": // FileUpload
            break;
        case "color": // ColorSquares
            break;
        case "image":  // ImageSquares
            break;
        case "readonlycheckbox": // ReadonlyCheckboxes
            break;
        default:
            break;
    }
  
    bufferArray[options.title] = new SelectedOption(optionArray, valueArray);
    console.log(bufferArray);
    array[options.title] = new SelectedOption(optionArray, valueArray);

}

var AddItem = function (dataJson, cartItemId, qty, prodId, cartImageSrc, editActive) {
    $.ajax({
        cache: false,
        url: "shoppingcart/submititem?productId=" + prodId + "&dataJson=" + dataJson + "&quantity=" + qty + "&cartImageSrc=" + cartImageSrc + "&editActive=" + editActive + "&formOptions={}&cartItemId=" + cartItemId ,
        type: "post",
        success: function (data) {
            window.location = '/cart';
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(xhr);
            console.log(ajaxOptions);
            console.log(thrownError);
        }
    });
}

var SetProductOption = function (input, url) {
    $.ajax({
        cache: false,
        url: "shoppingcart/setproductoptions?options=" + input,
        type: "post",
        success: function (data) {
            window.location = url;
        },
        error: function (xhr, ajaxOptions, thrownError) {

        }
    });
}