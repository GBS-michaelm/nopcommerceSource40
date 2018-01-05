declare var ccWidgetBaseUrl: string;

class CcWidget {
    init() {
        document.addEventListener("DOMContentLoaded", () => this._onLoad());
    }

    replaceCartItemImage(id, index, imageSource, version) {
        var columnIndex = 2;
        if (version === "3.90")
            columnIndex = 3;

        const input = $(`input[name="removefromcart"][value="${id}"]`);
        let row: JQuery;
        let img: JQuery = null;
        if (input.length > 0) {
            row = input.parent().parent();
            img = $("td img", row);
        }
        else if (index > -1) {
            row = $(`div.order-summary-content table.cart tbody tr:nth-child(${index + 1})`);
            img = $("td.product-picture img", row);
        }
        if (img != null) {
         //   img.attr("style", "max-width: 80px; max-height: 80px");
            img.attr("src", imageSource);
        }
    }

    replaceReturnToEditUrl(id, oldUrl: string, url: string, version: string) {
        var columnIndex = 3;
        if (version === "3.90")
            columnIndex = 4;

        const input = $(`input[name="removefromcart"][value="${id}"]`);
        let row: JQuery;
        let link: JQuery = null;
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
                    $("td:nth-child(" + columnIndex +")", row).append(div);
                }
            }
        } else {
            link.attr("href", url);
        }
    }

    private _updateFlyoutCartItems() {
        if (this._isFlyoutCartAlreadyUpdated())
            return;
        
        this._getCartItemsData();
    }

    private _getCartItemsData() {
        $.ajax({
            cache: false,
            url: ccWidgetBaseUrl + "plugins/ccwidget/getcartitemsdata",
            data: {},
            type: "get",
            success: data => {
                this._onGetCartItemsData(data);
            },
            error: (xhr, textStatus, thrownError) => {
                console.error("filed to load " + ccWidgetBaseUrl + "plugins/ccwidget/getcartitemsdata", textStatus, xhr.status, thrownError, xhr.responseText);
            }
        });
    }

    private _onLoad() {        
        this._updateFlyoutCartItems();
    }

    private _isFlyoutCartAlreadyUpdated() {
        const flyoutCart = $("#flyout-cart");
        const flag = flyoutCart.attr("data-cc-flag");
        return flag === "1";
    }

    private _onGetCartItemsData(data) {
        if (this._isFlyoutCartAlreadyUpdated())
            return;

        this._setCartUpdated();
       
        if (data.Items && data.Items.length > 0) {
            const flyoutCartItems = $("#flyout-cart .mini-shopping-cart .items");

            for (let i = 0; i < data.Items.length; i++) {
                const item = data.Items[i];
                this._updateFlyoutCartItem(flyoutCartItems, data.Count - item.Index, item.ImageSource);
            }
        }
    }

    private _setCartUpdated() {
        const flyoutCart = $("#flyout-cart");
        flyoutCart.attr("data-cc-flag", 1);
    }

    private _updateFlyoutCartItem(flyoutCartItems: JQuery, index: number, imageSource: string) {
        const img = $(`div.item:nth-child(${index}) div.picture img`, flyoutCartItems);
        img.attr("src", imageSource);
    }
}

const ccWidget = new CcWidget();
ccWidget.init();
 