app.controller('InvoiceListController', function ($scope, $routeParams, $http, $mdDialog, $rootScope) {
    var company = new $.Company({ CompanyId: 0 });
    var invoice = new $.Invoice({});

    $scope.Company = company;
    $scope.search = { Number: '', Site: '', Client: '', JobNumber: '' };

    GetAll();
    function GetAll() {
        //worder.Number = $scope.search.Number;
        //worder.JobNumber = $scope.search.JobNumber;
        //worder.Site = $scope.search.Site;
        //worder.Client = $scope.search.Client;
        //worder.Closed = $scope.worder.Closed
        invoice.GetList($scope.Company, function (e) {
            debugger
            $scope.InvoiceList = e.data;
        });
    }

    $scope.RowSelected = function (index) {

    };
    $scope.Find = function () {
        GetAll();


    };

});

app.controller('InvoiceController', function ($scope, $routeParams, $http, $location) {
    var wId = $routeParams.wId == undefined ? 0 : $routeParams.wId;
    var sId = $routeParams.sId == undefined ? 0 : $routeParams.sId;
    $scope.WorkOrder = new $.WorkOrder({ WorkOrderId: wId });
    debugger
    var x = JSON.stringify($scope.WorkOrder);
    var _siteNames = [];
    _siteNames.push({ JobNumber: '', SiteId: 0 });


    $scope.WorkOrder.Items = initializeArray();


    var site = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    //$scope.WorkOrder.SiteInfo =site;
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    GetTaxes();


    $scope.Find = function () {

    }

    $scope.SubTotal = function (_total) {

        var subTotal = 0;

        for (var i = 0; i < Object.keys($scope.WorkOrder.Items).length; i++) {
            if ($scope.WorkOrder.Items[i].PurchaseQty != null) {
                subTotal += parseFloat($scope.WorkOrder.Items[i].PurchaseQty) * parseFloat($scope.WorkOrder.Items[i].Rate);
            }
        }
        $scope.WorkOrder.SubTotal = subTotal;
        //  $scope.WorkOrder.SubTotal1 =  $scope.WorkOrder.SubTotal+parseFloat($scope.WorkOrder.SiteInfo.Freight);
        var taxAmount = 0;
        if (_total == 1) {

            if ($scope.WorkOrder.Taxes != null) {
                for (var i = 0; i < $scope.WorkOrder.Taxes.length; i++) {
                    if ($scope.WorkOrder.Taxes[i].Applicable) {
                        taxAmount += ($scope.WorkOrder.SubTotal) * $scope.WorkOrder.Taxes[i].Rate / 100.00;
                    }
                }
            }
            $scope.WorkOrder.Total = $scope.WorkOrder.SubTotal + taxAmount;
            return $scope.WorkOrder.Total;
        }
        else {
            return subTotal;
        }


    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {

            /// this.$parent.item.Props.Item.Props.MRP = selected.originalObject.MRP;
            var Product = this.$parent.item.Item = selected.originalObject;//this.$parent.item.Props.Item.Props;
            //  $scope.WorkOrder.Items[selectedWorkOrderItemIndex].Size = parseFloat(Product.Size);
            // $scope.WorkOrder.Items[selectedWorkOrderItemIndex].Rate = parseFloat(Product.Rate);
        }
    };
    site.GetJobNumbers(function (e) {
        debugger
        for (var i in e.data) {
            _siteNames.push({ JobNumber: e.data[i].JobNumber, SiteId: e.data[i].SiteId });
        }
        $scope.Site = _siteNames;

    });

    $scope.CompanySelection = function (obj) {
        if (obj != undefined) {
            $scope.WorkOrder.CompanyId = obj.originalObject.CompanyId;
        }
    };

    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        console.log(selected);
    };

    $scope.RowSelected = function (index) {

        if ($scope.WorkOrder.Items[index] != undefined) {
            //  $scope.$digest(function () {
            selectedWorkOrderItemIndex = index;
            //  $scope.Product = $scope.WorkOrder.Items[index].Item.Props;
            // });
        }
        // LoadSites(index, 0);

    };

    $scope.GetSelected = function () {
        var v = $scope[selectedProject];
    };

    function GetTaxes() {
        var Tax = new $.Tax({ ItemId: 0 });

        Tax.GetTaxes($scope.WorkOrder, function (e) {

            $scope.WorkOrder.Taxes = e.data;
        });
    }

    function initializeArray() {
        var WorkOrders = [];
        for (var i = 0; i < 10; i++) {
            WorkOrders.push(new $.WorkOrderItem({}));
        }
        return WorkOrders;
    }

    $scope.Save = function () {
        debugger
        var res = $scope.WorkOrder.Items.filter(function (v) {
            if (v.Item != undefined) {
                return v.Item.ProductId > 0;
            }
            else
                return false;
        });


        //var m = $('#form-workorder').valid();
        //var site = $('#form-site').valid();


        //if (m && site) {
        EnableToolbar(0);

        if (res.length < 1) {
            alert('Work order can not be empty.');
            return;
        }

        addInvoice();


    };



    function addInvoice() {


        $scope.WorkOrder.InvoiceDate = new Date($scope.WorkOrder.InvoiceDate).toLocaleDateString();

        $scope.WorkOrder.AddInvoice(function (e) {

            if (e.statusText == 'OK' && e.data == true) {
                alert('invoice created');
                // $location.path('/woInfo').search({ wId: e.data.WorkOrderId });
                // $scope.$apply();
            } else {
                showMessage(MessageClass.SAVED);
            }

        });
    }






});
