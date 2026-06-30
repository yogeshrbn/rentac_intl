var settings = function () {
    var uiInit = function () {
         
        $('.input-datepicker').datepicker({ weekStart: 1 });
    };
    return {
        init: function () {
            uiInit(); // Initialize UI Code
        }
    };
}();
/* Initialize app when page loads */
//$(function () { settings.init(); });
