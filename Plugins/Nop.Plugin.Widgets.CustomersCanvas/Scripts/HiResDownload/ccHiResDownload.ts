module CcHiResDownload{
    export function init(id: string, url: string) {
        var $button = $(`#${id}.ccHiResButton`);

        var indication = new Indication($button);

        url = _fixUrlProtocol(url, window.location.protocol);

        $button.on("click", () => new Promise<XMLHttpRequest>((resolve, reject) => {
                indication.start();

                var xmlhttp = new XMLHttpRequest();
                xmlhttp.open("GET", url, true);

                xmlhttp.onreadystatechange = () => {
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
            .then(request => {
                request.abort();
                indication.stop();

                window.location.href = url;
            })
            .catch(err => {
                indication.stop();

                if (err instanceof RejectError) {
                    var rejectError = <RejectError>err;

                    if (rejectError.status === 404) {
                        console.error("File not found");
                        return;
                    }

                    console.error(`Network error ${rejectError.status}`);
                    return;
                }

                console.error("Unable to prepare file, please check internet connection");

                console.error(err);
            }));
    }

    export function _fixUrlProtocol(url: string, targetProtocol: string): string {
        const a = document.createElement("a");
        a.href = url;

        if (targetProtocol.toLowerCase() === "https:" && a.protocol.toLowerCase() !== "https:") {
            a.protocol = targetProtocol;
            a.port = "443";

            return a.toString();
        }

        return url;
    }

    class RejectError {
        constructor(private _status: number) {
            
        }

        get status() {
            return this._status;
        }
    }

    class Indication {
        private _origText: string;
        private _origTextAlign: string;
        private _origMinWidth: string;
        private _dots = 0;
        private _intervalId: number = null;
        private _prepareText = "PDF is being prepared";

        private _maxDots = 3;

        constructor(private _$indicationEl: JQuery) { }

        start() {
            this._$indicationEl[0].setAttribute("disabled", "");

            this._origText = this._$indicationEl.text();
            this._origTextAlign = this._$indicationEl.css("text-align");
            this._origMinWidth = this._$indicationEl.css("min-width");
            this._$indicationEl.css("text-align", "left");

            this._$indicationEl.text(this._prepareText + Array(this._maxDots + 1).join("."));
            this._$indicationEl.css("min-width", this._$indicationEl.css("width"));

            this._intervalId = window.setInterval(() => {
                this._dots++;

                this._$indicationEl.text(this._prepareText + Array(this._dots + 1).join("."));

                if (this._dots >= this._maxDots) {
                    this._dots = 0;
                }
            }, 500);
        }

        stop() {
            this._$indicationEl.text(this._origText);
            this._$indicationEl.css("text-align", this._origTextAlign);
            this._$indicationEl.css("min-width", this._origMinWidth);

            if (this._intervalId !== null) {
                window.clearInterval(this._intervalId);

                this._intervalId = null;
            }

            this._$indicationEl[0].removeAttribute("disabled");
        }
    }
}