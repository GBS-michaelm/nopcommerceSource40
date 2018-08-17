var editorAdminDriver = {
    _info: {
        version: '0.0.10',
        platform: 'nopcomerce'
    },

    getAttributesValues: function(productId) {
        var self = this;
        var data = { productId: productId };
        $.ajax({
            cache: false,
            url: _getAttrUrl,
            type: 'post',
            data: data,
            success: function(xhr, ajaxOptions, thrownError) {
                var values = xhr.data;
                self._render(values);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.error('Error get attributes from NopCommere!');
                console.error("filed to load" + this.url, textStatus, xhr.status, thrownError, xhr.responseText);
            }
        });

    },

    _removeAttrUrl: "",
    _saveUrl: "",
    _getAttrUrl: "",
    _ccUrl: "",

    _productId: "",
    _parent: {},
    _configText: "",

    init: function(ccUrl, saveUrl, getAttrUrl, removeAttrUrl) {
        if (ccUrl)
            this._ccUrl = ccUrl;
        else
            console.error("Customers Canvas url not defined!");

        if (saveUrl)
            _saveUrl = saveUrl;
        else
            console.error("SaveSpecificationAttribute url not defined!");

        if (getAttrUrl)
            _getAttrUrl = getAttrUrl;
        else
            console.error("GetSpecificationAttribute url not defined!");

        if (removeAttrUrl)
            _removeAttrUrl = removeAttrUrl;
        else
            console.error("RemoveSpecificationAttribute url not defined!");
    },

    _render: function(attributeValues) {
        var self = this;
        var configText = JSON.stringify(self._configText);
        var elem = document.createElement("editor-admin");
        elem.setAttribute("product-id", self._productId);
        elem.driver = self;
        elem.setAttribute("cc-url", self._ccUrl);
        elem.setAttribute("editor-config", configText);
        elem.setAttribute("default-values", JSON.stringify(attributeValues));
        self._parent.empty();
        self._parent.append(elem);
    },

    render: function(parent, productId, configText) {
        var self = this;
        this._productId = productId;
        this._parent = parent;
        this._configText = configText;

        if (!!configText) {
            this.getAttributesValues(productId);
        }
    },

    submitAttributes: function(productId, attributes) {
        Object.keys(attributes).forEach(function(key) {
            var param = {
                ProductId: productId,
                attributeName: key,
                attributeValue: attributes[key]
            };
            $.ajax({
                cache: false,
                url: _saveUrl,
                type: 'post',
                data: param,
                success: function(xhr, ajaxOptions, thrownError) {
                    var specGrid = $("#specificationattributes-grid").data('kendoGrid');
                    specGrid.dataSource.read();

                    var attrGrid = $("#productattributemappings-grid").data('kendoGrid');
                    attrGrid.dataSource.read();
                },
                error: function(xhr, textStatus, thrownError) {
                    console.error('Error add attributes to NopCommerce!');
                    console.error("filed to load" + this.url, textStatus, xhr.status, thrownError, xhr.responseText);
                }
            });
        });
    },

    removeAttributes: function(productId, attributes) {
        Object.keys(attributes).forEach(function(key) {
            var param = {
                ProductId: productId,
                attributeName: key,
            };
            $.ajax({
                cache: false,
                url: _removeAttrUrl,
                type: 'post',
                data: param,
                success: function(xhr, ajaxOptions, thrownError) {
                    var specGrid = $("#specificationattributes-grid").data('kendoGrid');
                    specGrid.dataSource.read();

                    var attrGrid = $("#productattributemappings-grid").data('kendoGrid');
                    attrGrid.dataSource.read();
                },
                error: function(xhr, textStatus, thrownError) {
                    console.error('Error delete attributes from NopCommerce!');
                    console.error("filed to load" + this.url, textStatus, xhr.status, thrownError, xhr.responseText);
                }
            });
        });
    },
};
window["editorAdminDriver"] = editorAdminDriver;
