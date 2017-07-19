define('ecommerce-driver', function () {

    var _info = {
        version: '0.0.10',
        platform: 'nopcommerce'
    };

    var Attribute = function (specAttributeModel) {
        this.id = specAttributeModel.SpecificationAttributeId;
        this.title = specAttributeModel.SpecificationAttributeName;
        this.value = specAttributeModel.ValueRaw;
    };

    var PluginSettings = function (settingsModel) {
        this.ccIdAttributeId = settingsModel.CcIdAttributeId;
        this.customersCanvasBaseUrl = settingsModel.ServerHostUrl;
        if (!this.customersCanvasBaseUrl) {
            throw "Customer's Canvas base URL is not specified in the plugin settings. Go to Administration -> Configuration -> Plugins -> Local Plugins -> CustomersCanvas -> Configure and set a URL to the Customer's Canvas instance there.";
        }
    }

    var Option = function (optionModel) {

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

        Object.defineProperty(this, "free", {
            get: function () {
                return Object.keys(this.values).every(function (itemname) {
                    return this.values[itemname].price == 0;
                }, this);
            }
        });
    }

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

    var Product = function (productModel, editor, configuration) {
        var self = this;
        self.id = productModel.Id;
        self.title = productModel.Name;
        self.description = productModel.ShortDescription;
        self.attributes = {};
        self.updateCartItemId = productModel.updateCartItemId;
        self.price = productModel.ProductPrice.PriceValue;
        self.sku = productModel.Sku;
       
        self.skuPlaceholders = [];
        self.pricePlaceholders = [];

        productModel.ProductSpecifications.forEach(function (spec) {
            self.attributes[spec.SpecificationAttributeName] = new Attribute(spec);
        });
        
        // TODO: Extract quantities through the productModel
        self.quantities = {
            min: -1,
            max: -1,
            values: []
        };

        self.options = {};
        productModel.ProductAttributes.forEach(function (option) {
            self.options[option.Name] = new Option(option);
        });

        self._editor = editor;
        self._currentStep = "";
        self._config = configuration.config;

        var processConfig = function (config, title) {

            var result = null;
            switch (true) {
                case Array.isArray(config):
                    result = [];
                    config.forEach(function (item) {
                        result.push(processConfig(item));
                    });
                    break;
                case typeof (config) === "object":
                    var result = {};
                    Object.keys(config).forEach(function (key) {
                        if (title == 'steps')
                            self._currentStep = key;
                        result[key] = processConfig(config[key], key);
                    });
                    break;
                case typeof (config) === "number" || typeof (config) === "boolean":
                    return config;
                default:
                    result = config;
                    var placeholders = result.match(/%[^%]+%/g);
                    if (Array.isArray(placeholders)) {
                        placeholders.forEach(function (placeholder) {
                            if (placeholder.toLowerCase().indexOf('%product.sku%') >= 0) {
                                self.skuPlaceholders.push({
                                    'title': title,
                                    'placeholder': placeholder,
                                });
                            }
                            if (placeholder.toLowerCase().indexOf('%product.price%') >= 0) {
                                self.pricePlaceholders.push({
                                    'title': title,
                                    'placeholder': placeholder,
                                });
                            }

                            var doCommon = true;
                            var mainRegex = new RegExp("%.*#([a-zA-Z_0-9]+.?)+#.*%", "g");
                            var line = placeholder.match(mainRegex);
                            if (line != null) {
                                var regex = new RegExp("#([a-zA-Z_0-9]+.?)+#", "g");
                                var str = line[0].match(regex)[0];
                                var clearStr = str.replace("#", "").replace("#", "");
                                var path = clearStr.split('.');
                                var value = self._config;
                                path.forEach(function (item) {
                                    value = value[item];
                                });
                                value = value.name;
                                if (!!self.attributes[value] && !!self.attributes[value].value)
                                {
                                	var ecommerceValue = self.attributes[value].value;
                                	var elem = document.createElement('textarea');
                                	elem.innerHTML = ecommerceValue;
                                	replaceValue = elem.value;
                                    doCommon = false;
                                } else {
                                    placeholder = placeholder.replace(str, value);
                                    result = result.replace(str, value);
                                }
                            }
                            if (doCommon) {
                                var vars = placeholder.slice(1, -1).split(".");
                                var obj = null;
                                var objName = vars.shift().toLowerCase();
                                switch (objName) {
                                    case "product":
                                        obj = self;
                                        break;
                                    default:
                                        throw "The placeholder " + placeholder + " is not supported";
                                }
                                var replaceValue = obj;
                                while (vars.length > 0) {
                                    var name = vars.shift();
                                    replaceValue = replaceValue[name];
                                    if (typeof (replaceValue) == "undefined") {
                                        throw "The placeholder " + placeholder + " is invalid: " + name + " not found!";
                                    }
                                }
                                if (typeof (replaceValue) === "object") {
                                    if (replaceValue instanceof Attribute) {
                                        replaceValue = replaceValue.value;
                                    } else if (replaceValue instanceof Option || replaceValue instanceof OptionValue) {
                                        replaceValue = replaceValue.title
                                    } else {
                                        throw "You try to insert object as a placeholder value for " + placeholder;
                                    }
                                }
                            }
                            result = result.replace(placeholder, replaceValue);
                        });
                    }
            }
            return result;
        }

        if (!configuration || !configuration.config) {
            throw "The configuration " + configuration.title + "  is invalid (the 'config' section is missing).";
        }

        self.configuration = processConfig(configuration.config);

        delete self._config;
        delete self._currentStep;

        self.renderEditor = function (parent) {
            self._editor.render(parent, self);
        }
    };

    var Order = function (product, options, data, images, downloadUrls, quantity) {
        var self = this;
        self._quantity = quantity;
        self.id = null;
        self.designId = parseInt(_currentProduct.options.CcId.defaultValue);
        self.updateCartItemId = _currentProduct.updateCartItemId;

        self.data = data;
        self.options = options;
        
        self.product = product;
        self.price = null;
        self.images = images;
        self.downloadUrls = downloadUrls;

        self.onPriceChanging = null;
        self.onPriceChanged = null;

        self.setOptionByTitle = function (optionName, value) {
            if (typeof (_currentProduct.options[optionName]) === "undefined") {
                throw "Option " + optionName + " is not found in the product " + _currentProduct.title;
            }

            if (Object.prototype.toString.call(value) !== "[object Array]") {
                value = [value];   
            }
            self.options[optionName] = new SelectedOption(_currentProduct.options[optionName], value);
            self.updatePrice();
        };

        self.removeOptionByTitle = function (optionName) {
            if (typeof (_currentProduct.options[optionName]) === "undefined") {
                throw "Option " + optionName + " is not found in the product " + _currentProduct.title;
            }
            delete self.options[optionName]
        },

        self.getOptionStr = function(option) {
            var result = [];
            for (var i = 0; i < option.value.length; i++) {
                var val = option.value[i];
                if (val)
                    if (Object.prototype.toString.call(val) === "[object String]") {
                        if (option.option)
                            result.push("product_attribute_" + option.option.id + "=" + encodeURIComponent(val));
                        else 
                            result.push("product_attribute_" + option.id + "=" + encodeURIComponent(val));
                    } else {
                        if (option.option)
                            result.push("product_attribute_" + option.option.id + "=" + val.id);
                        else 
                            result.push("product_attribute_" + option.id + "=" + val.id);
                    }
            }
            return result.join("&");
        };

        self.updatePrice = function () {
            
            var values = [];
            for (var i in this.options) {
                values.push(this.getOptionStr(this.options[i]));
            }
            values.push("addtocart_" + this.product.id + ".EnteredQuantity=" + this.quantity);
            
            var attributesStr = values.join("&");
            
            self._raise(self.onPriceChanging);
            $.ajax({
                cache: false,
                url: ccWidgetBaseUrl + "shoppingcart/productdetails_attributechange?productId="+this.product.id+"&validateAttributeConditions=False&loadPicture=True",
                type: "post",
                data: attributesStr,
                success: function (data) {
                    self.price = parseFloat(data.price.replace(/[^0-9\.]+/g, ""));
                    self._raise(self.onPriceChanged, (self.price * self.quantity).toFixed(2), data.sku);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    _notImplemented("update price failed responce handling not implemented");
                }
            });
        };

        self._raise = function (cb) {
            if (cb !== null && typeof (cb) === 'function') {
                var args = Array.prototype.slice.call(arguments);
                args.shift();
                cb.apply(self, args);
            }
        }

        self.submit = function () {
        	var self = this;
        	var formOptions = self._getFormOptions();
        	Object.keys(formOptions).forEach(function (attrKey) {
        		if (formOptions[attrKey] == "") {
        			delete formOptions[attrKey];
        		}
        	});
        	var jsonOptions = JSON.stringify(this.options);
        	Object.keys(this.options).forEach(function (optionName) {
        		var attrName = "product_attribute_" + self.options[optionName].option.id;
        		delete formOptions[attrName];
        	});

            var jsonData = JSON.stringify(this.data);
            var jsonImages = JSON.stringify(this.images);
            var jsonDownloadUrls = JSON.stringify(this.downloadUrls);
	        var jsonformOptions = JSON.stringify(formOptions);

            if (typeof (self.designId) !== undefined && self.designId > 0) {
                $.ajax({
                    cache: false,
                    url: ccWidgetBaseUrl + "plugins/ccwidget/UpdateItem",
                    data: {
                        "productId": this.product.id, "designId": self.designId, "updateCartItemId": self.updateCartItemId,
                        "dataJson": jsonData, "imagesJson": jsonImages, "optionsJson": jsonOptions, "downloadUrlsJson": jsonDownloadUrls,
                        "quantity": this.quantity, "formOptions": jsonformOptions
                    },
                    type: "post",
                    success: function (data) {
                        location.assign(ccWidgetBaseUrl + "cart");
                    },
                    error: function (xhr, textStatus, thrownError) {
                    	console.error("filed to load" + this.url, xhr.status, thrownError, xhr.responseText);
                    }
                });
            }
            else {
                $.ajax({
                    cache: false,
                    url: ccWidgetBaseUrl + "plugins/ccwidget/submititem",
                    data: {
                        "productId": this.product.id, "dataJson": jsonData, "imagesJson": jsonImages, "optionsJson": jsonOptions,
                        "downloadUrlsJson": jsonDownloadUrls, "quantity": this.quantity, "formOptions": jsonformOptions
                    },
                    type: "post",
                    success: function (data) {
                        location.assign(ccWidgetBaseUrl + "cart");
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                    	console.error("filed to load" + this.url, xhr.status, thrownError, xhr.responseText);
                    }
                });
            }
        }

        self._getFormOptions = function() {
		        function getQueryParameters(str) {
			        return (str || document.location.search).replace(/(^\?)/, '').split("&").map(function(n) {
				        return n = n.split("="), this[n[0]] = n[1], this
			        }.bind({}))[0];
		        }

		        function getParameterByName(name, url) {
			        if (!url) url = window.location.href;
			        name = name.replace(/[\[\]]/g, "\\$&");
			        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
				        results = regex.exec(url);
			        if (!results) return null;
			        if (!results[2]) return '';
			        return decodeURIComponent(results[2].replace(/\+/g, " "));
		        }
				
		        var queryParams = getQueryParameters();

		        var optArray = {};
		        Object.keys(queryParams).forEach(function (item) {
			        var index = item.indexOf("product_attribute_");
			        if (index> -1) {
			        	optArray[item.substr(index)] = getParameterByName(item);
			        }
		        });
		        return optArray;
	        },

        Object.defineProperty(this, "quantity", {
            get: function () {
                return this._quantity;
            },
            set: function (value) {
                self._quantity = value;
                self.updatePrice();
            }
        });        
    }

    var User = function (userModel) {
        if (!!userModel.UserId)
            this.id = userModel.UserId;
        else this.id = 0;
        if (!!userModel.UserGuid)
            this.guid = userModel.UserGuid;
        else this.guid = "abcd";
        if (!!userModel.IsAuthenticated)
            this.isLogged = userModel.IsAuthenticated;
        else this.isLogged = false;
    };

    var _notImplemented = function (name) {
        throw "The " + name + " is not implemented in " + _info.platform + " plugin " + " version " + _info.version;
    };

    var _currentProduct = null;
    var _currentOrder = null;
    var _pluginSetting = null;
    var _user = null;

    return {

        get info() { return _info; }, 
      
        init: function (productModel, editor, config, pluginSettings, order, quantity, user) {
            _pluginSetting = new PluginSettings(pluginSettings);
            _currentProduct = new Product(productModel, editor, config);
            if (!!order) {
                _currentOrder = order;
            } else {
                this.orders.create();
            }
            if (!!quantity) {
                _currentOrder.quantity = quantity;
            }
            _user = new User(user);

        },

        get settings() {
            return _pluginSetting;
        },

        get products() {
            return {
                get current() { return _currentProduct },

                // TODO: Implement all and find(id)
                get all() { _notImplemented("products.all property"); },
                find: function (id) { _notImplemented("products.find(id) method"); },
            };
        },

        get orders() {
            return {
                get current() {
                    return _currentOrder;
                },
                create: function ()
                {
                    _currentOrder = new Order(_currentProduct, {}, null, [], [], 1);
                    return _currentOrder;
                },
                find: function(id) {
                    _notImplemented("orders.find(id) method");
                }            
            };
        },

        get user() {
            return _user;
        }

    };
});