declare function OpenWindow(query: string, w: number, h: number, scroll: boolean);
declare function setLocation(url: string);
declare function displayAjaxLoading(display: boolean);
declare function displayPopupNotification(message: string, messageType: string, modal?: boolean);
declare function displayBarNotification(message: string, messageType: string, timeout: number);
declare function htmlEncode(value: string): string;
declare function htmlDecode(value: string): string;