$(document).ready(function () {

    //init load stuff
    var $tabs = $('#tabs').tabs();
    var countPlus = 0;
    var buttonCount = 0;



    LoadPlusAndMinus();
    ButtonsNextLoad();

   

    //iterate through each textboxes and add keyup
    //handler to trigger sum event
    $("#videoQuantityCalculatorSection .txt").each(function () {     

        $(this).keyup(function () {
            calculateSum();
        });
    });

    //EVENT FUNCTIONS
    $('#videoQuantityCalculatorSection .next-tab, #videoQuantityCalculatorSection .prev-tab').click(function () {
        $tabs.tabs('select', $(this).attr("rel"));
        return false;
    });

    $('#videoQuantityCalculatorSection input').bind('change keyup input', function (event) {
        var currValue = $(this).val();

        if (currValue.search(/[^0-9]/) != -1) {
            // Change this to something less obnoxious
            //alert('Only numerical inputs please');
        }

        $(this).val(currValue.replace(/[^0-9]/, ''));
    });

    //$(".numbers-row").append('<div class="inc button">+</div><div class="dec button">-</div>');
    $("#videoQuantityCalculatorSection .button").live("click", function () {

        var $button = $(this);
        var oldValue = $button.parent().find("input").val();

        if ($button.text() == "+") {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            // Don't allow decrementing below zero
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            } else {
                newVal = 0;
            }
        }

        $button.parent().find("input").val(newVal);

        calculateSum();
    });


    //CALLED FUNCTIONS

    function calculateSum() {

        var sum = 0;

        //iterate through each textboxes and add the values
        $("#videoQuantityCalculatorSection input.txt").each(function () {

            


            var multipliedqty = ($(this).attr('data-quantity'));

            //add only if the value is number
            if (!isNaN(this.value) && this.value.length != 0) {
                sum += parseFloat(this.value) * multipliedqty;
            
            }

        });
        //.toFixed() method will roundoff the final sum to 2 decimal places
        $("#videoQuantityCalculatorSection #sum").html("You'll need <strong>" + sum + " </strong> schedules");
    }

    function LoadPlusAndMinus() {

        if (countPlus == 0) {
            $("#videoQuantityCalculatorSection .numbers-row").append('<div class="inc button">+</div><div class="dec button">-</div>');
        }
        countPlus++;

    }

    function ButtonsNextLoad() {
        if (buttonCount == 0) {
            $("#videoQuantityCalculatorSection .ui-tabs-panel").each(function (i) {

                var totalSize = $("#videoQuantityCalculatorSection .ui-tabs-panel").size() - 1;

                if (i != totalSize) {
                    next = i + 2;
                    $(this).append("<a href='#' class='next-tab mover' rel='" + next + "'>Next &gt;</a>");
                }

                if (i != 0) {
                    prev = i;
                    $(this).append("<a href='#' class='prev-tab mover' rel='" + prev + "'>&lt; Prev</a>");
                }

            });
        }

        buttonCount++;

    }


});