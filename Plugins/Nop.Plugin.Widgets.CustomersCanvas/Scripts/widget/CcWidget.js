var CcWidget = (function () {
    function CcWidget() {
    }
    CcWidget.prototype.init = function () {
        var _this = this;
        document.addEventListener("DOMContentLoaded", function () { return _this._onLoad(); });
    };
    CcWidget.prototype.replaceCartItemImage = function (id, index, imageSource, version) {
        var columnIndex = 2;
        if (version === "3.90")
            columnIndex = 3;
        var input = $("input[name=\"removefromcart\"][value=\"" + id + "\"]");
        var row;
        var img = null;
        if (input.length > 0) {
            row = input.parent().parent();
            img = $("td img", row);
        }
        else if (index > -1) {
            row = $("div.order-summary-content table.cart tbody tr:nth-child(" + (index + 1) + ")");
            img = $("td.product-picture img", row);
        }
        if (img != null) {
            //     img.attr("style", "max-width: 80px; max-height: 80px");
            img.attr("src", imageSource);
        }
    };
    CcWidget.prototype.replaceReturnToEditUrl = function (id, oldUrl, url, version) {
        var columnIndex = 3;
        if (version === "3.90")
            columnIndex = 4;
        var input = $("input[name=\"removefromcart\"][value=\"" + id + "\"]");
        var row;
        var link = null;
        link = $('a[href="' + oldUrl + '"]');
        if (link.length < 1) {
            if (input.length > 0) {
                row = input.parent().parent();
                link = $("td:nth-child(" + columnIndex + ") .edit-item a", row);
                if (link.length < 1) {
                    var div = document.createElement("div");
                    div.classList.add("edit-item");
                    var a = document.createElement("a");
                    a.href = url;
                    var text = document.createTextNode("Edit");
                    a.appendChild(text);
                    div.appendChild(a);
                    $("td:nth-child(" + columnIndex + ")", row).append(div);
                }
            }
        }
        else {
            link.attr("href", url);
        }
    };
    CcWidget.prototype._updateFlyoutCartItems = function () {
        if (this._isFlyoutCartAlreadyUpdated())
            return;
        this._getCartItemsData();
    };
    CcWidget.prototype._getCartItemsData = function () {
        var _this = this;
        $.ajax({
            cache: false,
            url: ccWidgetBaseUrl + "plugins/ccwidget/getcartitemsdata",
            data: {},
            type: "get",
            success: function (data) {
                _this._onGetCartItemsData(data);
            },
            error: function (xhr, textStatus, thrownError) {
                console.error("filed to load " + ccWidgetBaseUrl + "plugins/ccwidget/getcartitemsdata", textStatus, xhr.status, thrownError, xhr.responseText);
            }
        });
    };
    CcWidget.prototype._onLoad = function () {
        this._updateFlyoutCartItems();
    };
    CcWidget.prototype._isFlyoutCartAlreadyUpdated = function () {
        var flyoutCart = $("#flyout-cart");
        var flag = flyoutCart.attr("data-cc-flag");
        return flag === "1";
    };
    CcWidget.prototype._onGetCartItemsData = function (data) {
        if (this._isFlyoutCartAlreadyUpdated())
            return;
        this._setCartUpdated();
        if (data.Items && data.Items.length > 0) {
            var flyoutCartItems = $("#flyout-cart .mini-shopping-cart .items");
            for (var i = 0; i < data.Items.length; i++) {
                var item = data.Items[i];
                this._updateFlyoutCartItem(flyoutCartItems, data.Count - item.Index, item.ImageSource);
            }
        }
    };
    CcWidget.prototype._setCartUpdated = function () {
        var flyoutCart = $("#flyout-cart");
        flyoutCart.attr("data-cc-flag", 1);
    };
    CcWidget.prototype._updateFlyoutCartItem = function (flyoutCartItems, index, imageSource) {
        var img = $("div.item:nth-child(" + index + ") div.picture img", flyoutCartItems);
        img.attr("src", imageSource);
    };
    return CcWidget;
}());
var ccWidget = new CcWidget();
ccWidget.init();
//# sourceMappingURL=CcWidget.js.map