app.controller("ChallanListController", ['$scope', '$rootScope', '$rootScope', '$routeParams', '$window', '$http', 'ModalFactory',
    function ($scope, $rootScope, $rootScope, $routeParams, $window, $http, ModalFactory) {

        var challan = new $.Challan({});
        $scope.Filter = { LedgerId: 0, From: '', To: '' };
        $scope.Challans = [];
        var date = new Date();
        var token = $rootScope.getTokenInfo();
        $scope.Filter.To = convertDate(date);
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }
        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
        });

        $scope.getList = function () {

            challan.GetList(function (e) {

                $scope.Challans = e.data;
            }, $scope.Filter);
        }

        $scope.PrintReceipt = function (item, event) {
            debugger
            event.preventDefault();
            var workOrder = new $.WorkOrder({ WorkOrderId: item.WorkOrderId });
            challan.WorkOrderId = item.WorkOrderId;

            ModalFactory.BillType(function ($scope, $mdDialog) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                },
                    $scope.OkButtonClick = function () {
                        $(event.currentTarget).find('.fa-spin').show();
                        // loadBillPdf($scope.HeaderType,invoiceId);
                        challan.ChallanHeaderType = $scope.HeaderType;
                        challan.PrintDeliveryChallanReceipt(function (e) {

                            var filePath = SERVER_URL + 'temp/' + e.data;
                            // $window.target = '_blank';
                            $window.open(filePath);
                        });
                    }
            });


        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
        });
    }]);
app.controller("ChallanController", ['$scope', '$rootScope', '$rootScope', '$routeParams', '$window', '$http', 'ModalFactory',
    function ($scope, $rootScope, $rootScope, $routeParams, $window, $http, ModalFactory) {
        var wId = $routeParams.wId == undefined ? 0 : parseInt($routeParams.wId);
        var edit = $routeParams.edit == undefined ? false : $routeParams.edit;

        var ledgerDTO = new $.Ledger({ WorkOrderId: wId });

        var challan = new $.Challan({ WorkOrderId: wId });
        challan.Items = [];
        $scope.Challan = challan;
        reset();
        $scope.Challan.Date = convertDate(new Date());
        $scope.DeleteItem = function (index) {
            $scope.$apply(function () {
                $scope.Challan.Items.splice(index - 1, 1);
            });
        }
        //parent will be 0 for new challans if not being added from wInfo.
        $scope.Challan.ParentWorkOrderId = 0;

        //load item details to edit a challan.
        if (edit == "true" && wId > 0) {
            $scope.Edit = true;
            loadDetailsToEdit();
        }
        else if (wId > 0 && edit == false) {
            $scope.Challan.ParentWorkOrderId = wId;
        }
        $scope.addItem = function () {

            $scope.TransItem.Amount = $scope.TransItem.Quantity * $scope.TransItem.Rate;
            $scope.Challan.Items.push($scope.TransItem);
            var lastProductId = $scope.TransItem.ProductId;
            var lastProductName = $scope.TransItem.Product;
            //var lastSizeId = $scope.TransItem.ProductSizeId;
            //var lastSize =  $scope.TransItem.Size;
            reset();

            $('#itemSelect_value').focus();
            $('#itemSelect_value').val('');
            $scope.TransItem.ProductId = lastProductId;
            $scope.TransItem.Product = lastProductName;
            //$scope.TransItem.ProductSizeId = lastSizeId;
            //$scope.TransItem.Size = lastSize;
        }
        function reset() {
            $scope.TransItem = new $.TransItem({});
            $('#itemSelect_value').val('');
        }
        $scope.itemSelected = function (itemId) {
            if (itemId != undefined) {
                var item = $scope.ProductRates.find(o => o.ProductId == itemId);
                $scope.TransItem.Product = item.Product;
                //$scope.TransItem.Product = selected.originalObject.Name;//this.$parent.item.Props.Item.Props;
                $scope.TransItem.ProductId = itemId;//selected.originalObject.ProductId;
                $scope.ProductSizes = [{}];
                $scope.ProductSizes = $.map($scope.AllSizes, function (value, key) {
                    if (value.ProductId == itemId) {
                        return value;
                    }
                });
            }
        };
        $scope.$watch('TransItem.ProductSizeId', function () {

            if ($scope.TransItem.ProductSizeId != null) {
                if ($scope.AllSizes) {
                    var size = $scope.AllSizes.find(o => o.ProductSizeId == $scope.TransItem.ProductSizeId);
                    if (size) {
                        $scope.TransItem.Size = size.Size;
                    }
                }
            }
        });
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Challan.LedgerId = $rootScope.LedgerId;
        }

   
        $scope.$watch('Challan.LedgerId', function () {

            if ($scope.Challan.LedgerId != null) {
                $scope.ProductRates = [];
                getAllProducts();
            }
            $rootScope.LedgerId = $scope.Challan.LedgerId;
        });
        function getAllProducts() {
            ledgerDTO.GetProductRates(function (e) {
                $scope.ProductRates = e.data;
            });

        }
        getAllProductSizesByCompany();
        function getAllProductSizesByCompany() {
            var product = new $.Product();
            product.GetSizeListByCompany(function (e) {
                $scope.AllSizes = e.data;
            });
        }

        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Challan.LedgerId == null) {
                $scope.Challan.LedgerId = e.data[0].LedgerId;
            }
        });

        $scope.Save = function () {
            $scope.Challan.Add(function (e) {
                $scope.PrintReceipt(e.data);
            });
        }
        var stateCity = new $.StateCity({});
        loadStates();

        function loadStates() {
            stateCity.GetStates(function (e) {
                $scope.States = e.data;

            });
        }

        $scope.PrintReceipt = function (workOrderId) {

            var challan = new $.Challan({ WorkOrderId: workOrderId });
            ModalFactory.BillType(function ($scope, $mdDialog) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                },
                    $scope.OkButtonClick = function () {
                        $(event.currentTarget).find('.fa-spin').show();
                        // loadBillPdf($scope.HeaderType,invoiceId);
                        challan.ChallanHeaderType = $scope.HeaderType;
                        challan.PrintDeliveryChallanReceipt(function (e) {

                            var filePath = SERVER_URL + 'temp/' + e.data;
                            // $window.target = '_blank';
                            $window.open(filePath);
                        });
                    }
            });


        }

        function loadDetailsToEdit() {
            var worder = new $.WorkOrder({ WorkOrderId: wId });
            if (worder.WorkOrderId > 0) {
                worder.GetItems(function (e) {

                    $scope.Challan.Items = worder.Items = e.data;


                    if (e.data.length > 0) {
                        debugger
                        var siteObj = e.data[0];
                        $scope.Challan.Freight = siteObj.Freight;
                        $scope.Challan.SubTotal = siteObj.ApproxValue;
                        $scope.Challan.Date = convertDate(siteObj.SentDate);
                        $scope.Challan.Site = e.data[0].Site;
                        $scope.Challan.SiteId = siteObj.SiteId;
                        $scope.Challan.Vehicle = siteObj.Vehicle;
                        $scope.Challan.Driver = siteObj.Driver;
                        $scope.Challan.Number = siteObj.JobNumber;
                        $scope.Challan.ChallanNumber = siteObj.ChallanNumber;
                        ;

                        $scope.Challan.LedgerId = siteObj.LedgerId;
                        $scope.Challan.State = siteObj.State;

                    }
                });
            }
        }
    }]);