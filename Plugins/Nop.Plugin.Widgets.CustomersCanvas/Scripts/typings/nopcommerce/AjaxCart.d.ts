declare module AjaxCart {
    var loadWaiting: boolean;
    var usepopupnotifications: boolean;
    var topcartselector: string;
    var topwishlistselector: string;
    var flyoutcartselector: string;

    function init(usepopupnotifications: boolean, topcartselector: string, topwishlistselector: string, flyoutcartselector: string);
    function setLoadWaiting(display: boolean);
    function addproducttocart_catalog(urladd: string);
    function addproducttocart_details(urladd: string, formselector: string);
    function addproducttocomparelist(urladd: string);
    function success_process(response: IResponse);
    function resetLoadWaiting();
    function ajaxFailure();

    interface IResponse {
        updatetopcartsectionhtml?: string;
        updatetopwishlistsectionhtml?: string;
        updateflyoutcartsectionhtml?: string;
        message?: string;
        success?: boolean;
        redirect?: string;
    }
}

