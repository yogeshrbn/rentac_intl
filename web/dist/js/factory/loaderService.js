app.factory('loaderService', function () {
    return {
        isLoading: false,
        requests: 0,

        show: function () {
            this.requests++;
            this.isLoading = true;
            //console.log('Loader shown. Active requests:', this.requests);
        },

        hide: function () {
            if (this.requests > 0) {
                this.requests--;
            }
            if (this.requests <= 0) {
                this.requests = 0;
                this.isLoading = false;
            }
         //   console.log('Loader hidden. Active requests:', this.requests);
        }
    };
});