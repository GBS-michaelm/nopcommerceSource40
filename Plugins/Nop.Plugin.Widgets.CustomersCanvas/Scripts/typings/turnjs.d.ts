interface JQuery {
    turn(options: ITurnOptions)
}

//(event, page, pageObject) => void
interface ITurnOptions {
    height?: number|string;
    width?: number|string;

    when?: {
        /**
         * @param view  A view contains the pages that are visible.
         */
        turned?: (event, page: number, view: number[]) => void;
    };

    /**
     * Sets the hardware acceleration mode, for touch devices this value must be true.
     */
    acceleration?: boolean;

    /**
     * Centers the flipbook depending on how many pages are visible.
     */
    autoCenter?: boolean;

    /**
     * Sets the page when initializing the flipbook.
     */
    pages?: number;

    /**
     * Sets the corners to be used when turning the page from methods like page, next or previous.
     * Possible values bl,br or tl,tr or bl,tr
     */
    turnCorners?: string;
}