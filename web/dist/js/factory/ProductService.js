app.factory('ProductTaxClassicationService', function ($rootScope) {
    var selectedTaxes = [];
    return {
        getTaxes: function () {
            return selectedTaxes;
        },
        setTaxes: function (args, taxes) {
            selectedTaxes = taxes;
            $rootScope.$broadcast('taxesUpdated', { args: args, taxes: selectedTaxes });
        }
    };
});