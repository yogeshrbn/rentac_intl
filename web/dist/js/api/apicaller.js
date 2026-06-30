(function ($) {
    $.ApiCaller = function (options) {
        var defaults = {
            http: null,
            cache: false,
            promiseOnly: false
        };

        this.Options = $.extend(defaults, options);
        this.Options.o = null;
        
    };


    $.ApiCaller.prototype = {
        spinner: ' <i class="fa fa-spinner fa-spin" style="color:#fff;"></i>',
        init: function (o) {
            this.Options.o = o;
        },
      
        getObject: function () {

        },

        prepareGet: function (action) {
            var _url = API_URL + action;
            var req = {
                method: 'GET',
                url: _url,

            };
            return req;
        },
        preparePost: function (action, data) {
            var _url = API_URL + action;
            var req = {
                method: 'POST',
                url: _url,
                data: data
                // , data: json
            };
            return req;
        },


        Exec: function (action, httpMethod, dataType, files, download) {
            var self = this;

            var _url = API_URL + action;

            var req = {
                method: httpMethod == null ? 'POST' : httpMethod,
                url: _url

                // , data: json
            }

            if (download != null && download != undefined) {

                req["responseType"] = 'blob'

            }
            //  req.cache = this.Options.cache;
            // debugger
            if (dataType == null || dataType == undefined) {
                //req["headers"] = {
                //    'Content-Type': 'application/json'
                //}

                if (this.Options && this.Options.o) {
                    req["data"] = this.BuildJSON();
                    if ($.isEmptyObject(req.data)) {
                        if ($.isArray(this.Options.o)) {
                            req['data'] = this.Options.o;
                        }
                    }
                }

            } else {
                var formData = new FormData();
                if (files != null) {


                    var dtoVal = (typeof this.Options.o === 'string') ? this.Options.o : JSON.stringify(this.Options.o);
                    formData.append('dto', dtoVal);
                    for (var i = 0; i < files.length; i++) {
                        if (files[i]) {
                            formData.append('doc' + (i + 1), files[i]);
                        }
                    }
                    // formData.append('file', files[0]);
                    req["data"] = formData;
                    req['headers'] = {
                        'Content-Type': undefined
                    };

                    req["processData"] = false;
                } else {
                    // req["data"] = "'" + this.Options.o + "'";
                    var dtoOnly = (typeof this.Options.o === 'string') ? this.Options.o : JSON.stringify(this.Options.o);
                    formData.append('dto', dtoOnly);
                    req["data"] = formData;
                    req['headers'] = {
                        'Content-Type': undefined
                    };
                    req["processData"] = false;
                }
            }

            var event = window.event;

            if (event) {
                var target = event.currentTarget.type;
                if (target == "submit" || target == 'button') {

                    $(event.currentTarget).append(this.spinner);
                }
            }
           
            //const lru = $cacheFactory('my-cache', { capacity: 10 });
            if (this.Options.promiseOnly == true) {
                return this.Options.http(req);
            }
            this.Options.http(req).then(this.Success).then(this.Error).then(function (e) {


                $('body').find('.fa-spin').remove()
            });;

        },
        Success: function (e) {

        },
        Error: function (e) {
            //  console.log('Error:' + e);
        },
        BuildJSON: function () {
            if (this.Options.o.Props == undefined || this.Options.o.Props == null) {
                return this.BuildJSONArray(this.Options.o);
            } else {
                return this.BuildJSONArray(this.Options.o.Props);
            }

        },
        DownloadReport: function (action, httpMethod, dataType, files, download) {
            var self = this;

            var _url = REPORT_SERVER + action;

            var req = {
                method: httpMethod == null ? 'POST' : httpMethod,
                url: _url

                // , data: json
            }

            if (download != null && download != undefined) {

                req["responseType"] = 'blob'

            }
            //  req.cache = this.Options.cache;
            // debugger
            if (dataType == null || dataType == undefined) {
                if (this.Options && this.Options.o) {
                    req["data"] = this.BuildJSON();
                    if ($.isEmptyObject(req.data)) {
                        if ($.isArray(this.Options.o)) {
                            req['data'] = this.Options.o;
                        }
                    }
                    req["headers"] = req["headers"] || {};
                    req["headers"]["Content-Type"] = "application/json";
                }

            } else {
                var formData = new FormData();
                if (files != null) {


                    formData.append('dto', this.Options.o);
                    for (var i = 0; i < files.length; i++) {
                        formData.append('file', files[i]);
                    }
                    // formData.append('file', files[0]);
                    req["data"] = formData;
                    req['headers'] = {
                        'Content-Type': undefined
                    };

                    req["processData"] = false;
                } else {
                    // req["data"] = "'" + this.Options.o + "'";
                    formData.append('dto', this.Options.o);
                    req["data"] = formData;
                    req['headers'] = {
                        'Content-Type': undefined
                    };
                    req["processData"] = false;
                }
            }

            var event = window.event;

            if (event) {
                var target = event.currentTarget.type;
                if (target == "submit" || target == 'button' || target == 'input') {
                    $(event.currentTarget).append(this.spinner);
                }
            }
            //const lru = $cacheFactory('my-cache', { capacity: 10 });
            this.Options.http(req).then(this.Success).then(this.Error).then(function (e) {
                $('body').find('.fa-spin').remove()
            });;

        },
        BuildJSONArray: function (obj) {
            var _jSONObj = [];
            var _jSONItem = {};

            $.each(obj, function (key, value) {

                if ((value instanceof Function) || typeof (value) == 'object') {
                    //do nothing
                    if ($.isArray(value)) {
                        //var jsonData = new $.ApiCaller().BuildJSONArray(value);;
                        _jSONItem[key] = value;
                    }
                } else {
                    _jSONItem[key] = value;
                }

            });
            return _jSONItem;
        }

    };


}(jQuery));