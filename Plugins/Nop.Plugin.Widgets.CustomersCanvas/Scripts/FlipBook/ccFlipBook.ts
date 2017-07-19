class CcFlipBook {

    private _conf: ICcFlipbookConfiguration;
    private _flipbookWindow: kendo.ui.Window;

    private _savedOverflowValue;

    private _windowOpened = false;

    constructor(conf: ICcFlipbookConfiguration) {
        this._conf = conf;

        this._initWindow();
        
        this._initBtn();
    }

    private _initWindow() {
        var $windowElement = $(`#${this._conf.id}.flipbookWindow`);
        
        this._flipbookWindow = $windowElement.kendoWindow({
            draggable: false,
            resizable: false,
            modal: true,
            actions: ["Close"],
            animation: false,
            visible: false,
            pinned: true,
            close: () => this._onWindowClose()
        }).data("kendoWindow");

        this._flipbookWindow.wrapper.addClass('ccModal');
        this._flipbookWindow.wrapper.css("margin-top", "-1%");

        this._flipbookWindow.center();
    }

    private _initFlipbook(hardImg: HTMLImageElement) {
        var $bookElement = $(`#${this._conf.id}.flipbook`);
        var $flipbookWindow = this._flipbookWindow.wrapper;
        var $windowContent = $flipbookWindow.find(".k-window-content");

        var windowHeight = $bookElement.parent().height();
        var windowWidth = $bookElement.parent().width();

        var imgRatio = hardImg.naturalHeight / hardImg.naturalWidth;

        var imgWidth = windowHeight / imgRatio;

        var bookWidth = imgWidth * 2;

        var contentHorizontalPadding = parseFloat($windowContent.css("padding-left")) + parseFloat($windowContent.css("padding-right"));
        var contentVerticalPadding = parseFloat($windowContent.css("padding-top")) + parseFloat($windowContent.css("padding-bottom"));

        var newWindowHeight: number;
        if (bookWidth > (windowWidth + contentHorizontalPadding)) {
            bookWidth = windowWidth - contentHorizontalPadding;

            imgWidth = bookWidth / 2;
            var imgHeight = imgWidth * imgRatio;

            newWindowHeight = imgHeight + contentVerticalPadding;
        }
        
        var newWindowWidth = bookWidth + contentHorizontalPadding;
        
        $bookElement.parent().width(bookWidth);

        $flipbookWindow.width(newWindowWidth);

        if (newWindowHeight != null)
            $flipbookWindow.height(newWindowHeight);

        this._flipbookWindow.center();

        //$bookElement.turn({
        //    width: "100%",
        //    height: "100%",
        //    autoCenter: true,
        //    acceleration: false,
        //    when: {
        //        turned: (event, page, view) => {

        //            var loadPagesImages = (pagesNumber: number[]): Promise<any> => {
        //                var loadPromises: Promise<any>[] = [];

        //                pagesNumber.forEach(n => {
        //                    var $page = $bookElement.find(`.p${n}`);
        //                    var img = <HTMLImageElement>$page.find(`img`)[0];

        //                    if (img != null && img.hasAttribute("data-src")) {
        //                        //console.log(`promise hide img of ${$page.selector}`);
        //                        var loadPromise = new Promise<JQuery>((resolve, reject) => {
        //                            if (img.hasAttribute("data-src")) {
        //                                img.addEventListener("load", () => resolve($page));
        //                                img.addEventListener("error", reject);

        //                                img.src = img.getAttribute("data-src");
        //                                img.removeAttribute("data-src");
        //                            }
        //                        });

        //                        loadPromises.push(loadPromise);
        //                    }

        //                });

        //                return Promise.all(loadPromises);
        //            };

        //            loadPagesImages(view).then(() => {
        //                var lastViewPageNumber = view[view.length - 1];

        //                return loadPagesImages([lastViewPageNumber + 1, lastViewPageNumber + 2]);
        //            });
        //        }
        //    }
        //});
        
        $bookElement.css("visibility", "");

        $(document).keydown(e => {
            if (!this._windowOpened)
                return;

            var previous = 37, next = 39, esc = 27;

            switch (e.keyCode) {
            case previous:

                // left arrow
                //$bookElement.turn('previous');
                e.preventDefault();

                break;
            case next:

                //right arrow
                //$bookElement.turn('next');
                e.preventDefault();

                break;
            }
        });

        $bookElement.click((e) => {
            var $pageElement = $(e.target).parent(".page");

            if ($pageElement.hasClass("odd")) {
                //$bookElement.turn('next');
            }
            else if ($pageElement.hasClass("even")) {
                //$bookElement.turn('previous');
            }
        });
    }

    private _openWindow() {
        var $body = $("body");
        this._savedOverflowValue = $body.css("overflow");
        $body.css("overflow", "hidden");

        this._flipbookWindow.center();
        this._flipbookWindow.open();

        this._windowOpened = true;
    }

    private _onWindowClose() {
        $("body").css("overflow", this._savedOverflowValue);

        this._windowOpened = false;
    }

    private  _initBtn() {
        var btnElement = document.querySelector(`#${this._conf.id}.flipbookBtn`);
        var $btnElement = $(btnElement);

        var $loader = $btnElement.find(".loader");

        var btnImg = btnElement.getElementsByTagName("img")[0];

        new Promise<HTMLImageElement>((resolve, reject) => {
                btnImg.onload = () => resolve(btnImg);
                btnImg.onerror = reject;

                btnImg.src = this._conf.proofs[0];
            })
            .then(img => {
                $btnElement.removeClass("flipbookBtnBorder");

                $(img).show();

                $loader.hide();
                img.style.cursor = "pointer";

                var flipbookInitialized = false;

                btnElement.addEventListener("click", () => {
                    this._openWindow();

                    if (!flipbookInitialized)
                        this._initFlipbook(img);

                    flipbookInitialized = true;
                });
            });
    }
}

interface ICcFlipbookConfiguration {
    id: string;
    proofs: string[];
}