interface JQuery {
    /**
	 * Removes mask from the element(s). Accepts both single and multiple selectors.
	 */
    unmask();

    /**
	 * Displays loading mask over selected element(s). Accepts both single and multiple selectors.
	 *
	 * @param label Text message that will be displayed on top of the mask besides a spinner (optional). 
	 * 				If not provided only mask will be displayed without a label or a spinner.  	
	 * @param delay Delay in milliseconds before element is masked (optional). If unmask() is called 
	 *              before the delay times out, no mask is displayed. This can be used to prevent unnecessary 
	 *              mask display for quick processes.   	
	 */
    mask(label?: string, delay?: number);

    isMasked():boolean;
}