(function ($) {
    $.Toolbar = function (options) {
        var defaults = {
            Previous: null,
            Location: null,
            Scope: null,
            Current: null
        };

        this.Options = $.extend(defaults, options);

    };

    $.Toolbar.prototype = {
        intit: function () {
            self = this;
            $('.toolbar span').tooltip();
            $('.toolbar .fa-arrow-circle-left').click(function () {
                self.Back();
            });
            $('.toolbar .fa-files-o').click(function () {
                self.New();
            });
            $('.toolbar .fa-save').click(function () {
                self.Save();
            });
            $('.toolbar .fa-undo').click(function () {
                self.Undo();
            });
            $('.toolbar .fa-filter').click(function () {
                self.Filter();
            });
        },
        New: function () {
            if (this.Options.Location != null && this.Options.Current != null) {
                self.Options.Scope.$apply(function () {
                    self.Options.Location.path(self.Options.Current.newitem);
                });
            }
        },
        Save: function () {
             
            self.Options.Current.scope.Save();
        },
        Undo: function () {

        },
        Filter: function () {

        },
        Back: function () {
            //calls the previous url of template.
            if (this.Options.Location != null && this.Options.Previous != null) {
                self.Options.Scope.$apply(function () {
                    debugger
                    self.Options.Location.$$url = self.Options.Previous.$$route.originalPath;
                    self.Options.Location.url(self.Options.Previous.originalPath);
                });

            }

        }
    }
}(jQuery));
