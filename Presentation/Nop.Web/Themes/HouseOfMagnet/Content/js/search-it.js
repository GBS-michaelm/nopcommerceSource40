﻿/* search-it v0.1.1 | http://mirjamsk.github.io/search-it */
(function (c, f, g, h) {
    var e = function (a, b) { this.options = b; this.$container = c(a); this.items = [] }; e.prototype = {
        defaults: { $searchInput: null, inputLabelValue: "Search", itemSelector: ".collapsible-body a", searchTemplate: '<div class="input-field"><label for="search">Search</label><input type="text" id="search"></div>', headerIdentifier: ".collapsible-header", useMaterializeCollapsible: !1, toggle: function (a, b) { a.is_match = b; a.$item.toggle(b) }, complete: function () { } }, init: function () {
            var a = this, b = jQuery.extend(!0, {}, this.defaults);
            b.searchTemplate = b.searchTemplate.replace("search", "search-" + this.$container.attr("id")); this.config = c.extend({}, b, this.options); this.config.$searchInput || (this.$searchBar = c(this.config.searchTemplate), this.config.$searchInput = this.$searchBar.find("input"), this.$container.before(this.$searchBar), this.options.inputLabelValue && this.$searchBar.find("label").text(this.options.inputLabelValue)); this.$container.find(this.config.itemSelector).each(function () { a.items.push({ $item: c(this), is_match: !0 }) }); this.bindKeyboardInput(this)
        },
        getQuery: function () { return this.config.$searchInput.val().toLowerCase().split(/\s+/)[0] }, getText: function (a) { return a.text().toLowerCase() }, queryRegularContainer: function () { for (var a = this.getQuery(), b = 0; b < this.items.length; b++) { var d = this.getText(this.items[b].$item), d = "" === a ? !0 : 0 <= d.indexOf(a); this.config.toggle(this.items[b], d) } this.config.complete() }, queryMaterializeCollapsibleContainer: function () {
            for (var a = this.getQuery(), b = 0; b < this.items.length; b++) {
                var d = !0, c = this.getText(this.items[b].$item);
                "" !== a ? (d = 0 <= c.indexOf(a), this.$container.find(this.config.headerIdentifier + ":not(.active)").trigger("click")) : (this.$container.find(this.config.headerIdentifier + ".active").trigger("click"), this.$container.find(this.config.headerIdentifier + ":first").trigger("click")); this.config.toggle(this.items[b], d)
            } this.config.complete()
        }, bindKeyboardInput: function (a) {
            if (a.config.useMaterializeCollapsible) a.config.$searchInput.on("keyup.searchIt change.searchIt", function () { a.queryMaterializeCollapsibleContainer() });
            else a.config.$searchInput.on("keyup.searchIt change.searchIt", function () { a.queryRegularContainer() })
        }
    }; e.defaults = e.prototype.defaults; c.fn.searchIt = function (a) { return this.each(function () { (new e(this, a)).init() }) }
})(jQuery, window, document);