var CcHiResDownload;
(function (CcHiResDownload) {
    function init(id, url) {
        var $button = $("#" + id + ".ccHiResButton");
        var indication = new Indication($button);
        url = _fixUrlProtocol(url, window.location.protocol);
        $button.on("click", function () { return new Promise(function (resolve, reject) {
            indication.start();
            var xmlhttp = new XMLHttpRequest();
            xmlhttp.open("GET", url, true);
            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.status >= 400 && xmlhttp.status < 600)
                    reject(new RejectError(xmlhttp.status));
                if (xmlhttp.readyState === 3 /*process*/)
                    resolve(xmlhttp);
                if (xmlhttp.readyState === 4 && xmlhttp.status !== 0 && !(xmlhttp.status >= 200 && xmlhttp.status < 300))
                    reject(new RejectError(xmlhttp.status));
            };
            xmlhttp.onerror = reject;
            xmlhttp.send(null);
        })
            .then(function (request) {
            request.abort();
            indication.stop();
            window.location.href = url;
        })
            .catch(function (err) {
            indication.stop();
            if (err instanceof RejectError) {
                var rejectError = err;
                if (rejectError.status === 404) {
                    console.error("File not found");
                    return;
                }
                console.error("Network error " + rejectError.status);
                return;
            }
            console.error("Unable to prepare file, please check internet connection");
            console.error(err);
        }); });
    }
    CcHiResDownload.init = init;
    function _fixUrlProtocol(url, targetProtocol) {
        var a = document.createElement("a");
        a.href = url;
        if (targetProtocol.toLowerCase() === "https:" && a.protocol.toLowerCase() !== "https:") {
            a.protocol = targetProtocol;
            a.port = "443";
            return a.toString();
        }
        return url;
    }
    CcHiResDownload._fixUrlProtocol = _fixUrlProtocol;
    var RejectError = (function () {
        function RejectError(_status) {
            this._status = _status;
        }
        Object.defineProperty(RejectError.prototype, "status", {
            get: function () {
                return this._status;
            },
            enumerable: true,
            configurable: true
        });
        return RejectError;
    }());
    var Indication = (function () {
        function Indication(_$indicationEl) {
            this._$indicationEl = _$indicationEl;
            this._dots = 0;
            this._intervalId = null;
            this._prepareText = "PDF is being prepared";
            this._maxDots = 3;
        }
        Indication.prototype.start = function () {
            var _this = this;
            this._$indicationEl[0].setAttribute("disabled", "");
            this._origText = this._$indicationEl.text();
            this._origTextAlign = this._$indicationEl.css("text-align");
            this._origMinWidth = this._$indicationEl.css("min-width");
            this._$indicationEl.css("text-align", "left");
            this._$indicationEl.text(this._prepareText + Array(this._maxDots + 1).join("."));
            this._$indicationEl.css("min-width", this._$indicationEl.css("width"));
            this._intervalId = window.setInterval(function () {
                _this._dots++;
                _this._$indicationEl.text(_this._prepareText + Array(_this._dots + 1).join("."));
                if (_this._dots >= _this._maxDots) {
                    _this._dots = 0;
                }
            }, 500);
        };
        Indication.prototype.stop = function () {
            this._$indicationEl.text(this._origText);
            this._$indicationEl.css("text-align", this._origTextAlign);
            this._$indicationEl.css("min-width", this._origMinWidth);
            if (this._intervalId !== null) {
                window.clearInterval(this._intervalId);
                this._intervalId = null;
            }
            this._$indicationEl[0].removeAttribute("disabled");
        };
        return Indication;
    }());
})(CcHiResDownload || (CcHiResDownload = {}));
//# sourceMappingURL=ccHiResDownload.js.map