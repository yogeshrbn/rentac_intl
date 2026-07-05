app.controller('RentChallanController', function ($scope, $rootScope, $route, $state, $stateParams, $mdDialog, $window, $filter,
    LedgerFactory, StorageService,
    InventoryFactory, ModalFactory, CompanyService, EmployeeService, WorkOrderFactory, toaster,
    $crypto, CompanyService, WarehouseService, ChallanService, AuthenticationService, $q, ChallanTaxService, TaxService) {
    var wId = $stateParams.WorkOrderId == undefined ? 0 : $stateParams.WorkOrderId;

    var JobCardId = $stateParams.JobCardId == undefined ? 0 : $stateParams.JobCardId;
    $scope.NextRentInvoiceNumber = '';
    $scope.NextChallanNumber = '';
    $scope.Config = {
        rateType: 2, showRateOf: 2, applyTax: true, maintainInventory: 1,
        allowEditChallanNumber: false, freightTax: false, otherChargesTax: false,
        showNextChallanInvoiceNumbers: false
    };
    $scope.WorkOrderId = 0;
    if (JobCardId) {
        var jcencKey = $crypto.decrypt(JobCardId).split(',');
        $scope.JobCardId = parseInt(jcencKey[0]);

        var contractId = parseInt(jcencKey[1]);
        if (contractId > 0) {
            var contract = new $.Contract();

            contract.GetById(function (e) {
                if (e.data.Data) {
                    if ($scope.WorkOrder && $scope.WorkOrder.SiteInfo) {
                        $scope.WorkOrder.LedgerId = e.data.Data.Ledger.LedgerId;
                        $scope.WorkOrder.SiteInfo.LedgerSiteId = e.data.Data.LedgerSiteId;
                    }
                }
            }, { ContractId: contractId });
        }

    }

    $scope.ChallanType = $state.current.data.challanType;
    // Available warehouses data
    var token = $rootScope.getTokenInfo();
    if (wId != 0)
        wId = $crypto.decrypt(wId);

    //  var sId = $stateParams.SiteId == undefined ? 0 : $stateParams.SiteId;
    var edit = wId > 0 ? true : false;
    $scope.edit = edit;
    $scope.AllowEdit = true;
    var config = new $.Config();
    var employee = new $.Employee();
    var transDto = new $.Transporter();
    var wo = new $.WorkOrder();
    var proConfig = config.GetAllConfig(null, true);
    var promTeams = employee.TeamList(null, null, true);
    var proWareHouse = WarehouseService.getWarehouses();
    var proTrans = transDto.GetAll(null, true);
     
    var lastChallan = wo.LastChallanNumber(null, { ChallanType: $scope.ChallanType }, true);
    var nextChallanPreview = !edit
        ? wo.NextChallanNumberPreview(null, { ChallanType: $scope.ChallanType }, true)
        : $q.when({ data: { Data: '' } });
    $scope.editorOptions = {
        height: 100
    };
    $scope.remarksEditorOptions = { height: 100 };
    $scope.warehouses = [];
    $scope.TeamList = [];
    $scope.Transporters = [];
    $q.all([proConfig, promTeams, proWareHouse, proTrans, lastChallan, nextChallanPreview]).then(function (e) {

        if (e) {
            
            getChallanConfig(e[0].data);
            $scope.TeamList = e[1].data.Data;
            $scope.warehouses = e[2].data.Data;
            $scope.Transporters = e[3].data;
            $scope.LastChallan = e[4].data.Data;
            var nextPreview = '';
            if (e[5] && e[5].data && (e[5].data.Code === 200 || e[5].data.Code === '200')) {
                var nd = e[5].data.Data;
                if (nd != null && nd !== undefined && nd !== '') {
                    nextPreview = String(nd);
                }
            }
            $scope.NextChallanNumber = nextPreview;
            if (!edit && nextPreview && $scope.Config.allowEditChallanNumber === false) {
               // $scope.WorkOrder.ChallanNumber = nextPreview;
            }

            //if (!$scope.edit && $scope.Config.showNextChallanInvoiceNumbers) {
            //    var nextInv = new $.NextId();
            //    nextInv.GetRentInvoiceNumber(function (res) {
            //        var n = res.data;
            //        if (n == null || n === '') {
            //            return;
            //        }
            //        $scope.NextRentInvoiceNumber = String(n);
            //    });
            //}
        }
        if (!edit) {
            // getNextWorkOrderNumber();
        }
    });
    $scope.comp = { EwayBillEnabled: false };
    var loginData = AuthenticationService.getTokenInfo();



    function getInfo() {

        var company = new $.Company();
        company.Props.CompanyId = loginData.DefaultCompanyId;
        company.GetDetails(function (e) {
            if (e.status != 200) {
                return;
            }
            if (!e.data) {
                return;
            }
            $scope.comp = e.data;
        });
    }
    getInfo();

    //$scope.teamslist = function () {
    //    var employee = new $.Employee();
    //    employee.TeamList(function (e) {
    //        if (e.data.Code != 200) {
    //            alert(e.data.Message);
    //            return;
    //        }
    //        $scope.TeamList = e.data.Data;
    //    });
    //}
    //$scope.getWarehouses = function () {
    //    WarehouseService.getWarehouses().then(function (e) {
    //        $scope.warehouses = e.data.Data;
    //    });
    //}
    //$scope.GetAllTransporter = function () {
    //    var transDto = new $.Transporter();
    //    transDto.GetAll(function (e) {
    //        $scope.Transporters = e.data;
    //    });
    //}
    nav_Items = [];

    //$scope.teamslist();
    //$scope.GetAllTransporter();
    //$scope.getWarehouses();


    $scope.$watch('edit', function (e) {

        //$scope.AllowEdit =  e && $scope.AccessData.Add ? true : false;
        if (e && $scope.AccessData.Edit) {
            $scope.AllowEdit = true;
        } else if (!e && $scope.AccessData.Add) {
            $scope.AllowEdit = true;
        } else
            $scope.AllowEdit = false;
    });



    function newRecord() {

        $scope.edit = wId > 0;

        worder = new $.WorkOrder({
            WorkOrderId: wId,
            Number: 'New'
        });
        var _cDate = new Date();
        worder.WorkOrderDate = convertDate(new Date());
        worder.IssueTime = new Date(1970, 0, 1, _cDate.getHours(), _cDate.getMinutes(), 0);
        ledgerDTO = new $.Ledger({
            LedgerId: 0
        });
        worder.WarehouseId = token.DefaultWareHouseId;
        $scope.WorkOrderId = wId;

        $scope.WorkOrder = worder;
        worder.RentStartDate = worder.WorkOrderDate;
        $scope.WorkOrder.Items = []; //initializeArray();
        sId = 0;
        //  wId = 0;
        $scope.WorkOrder.SiteInfo = new $.Site({
            WorkOrderId: $scope.WorkOrderId,
            SiteId: sId,
            VehicleId: 0,
            DriverId: 0,
            Total: 0,
            Freight: 0
        });
        $scope.Stock = [];
        $scope.MaxDate = new Date();

    }

    function undo() {
        $scope.edit = false;
        setTimeout(function () {
            $scope.$apply()
        }, 50);

        nav_onToolbarClick(last_command);
    }

    function nav_search() {
        alert('nav_search');
    }

    function nav_print() {
        $scope.printWorkOrder($scope.WorkOrder.WorkOrderId);
    }
    $scope.ParentItems = [];
    var worder = new $.WorkOrder({
        WorkOrderId: wId
    });
    worder.WorkOrderDate = convertDate(new Date());
    var ledgerDTO = new $.Ledger({
        LedgerId: 0
    });
  
    $scope.WorkOrder = worder;
    $scope.WorkOrder.Items = []; //initializeArray();
    $scope.Stock = [];
    $scope.initialValue = null;

    //parentItems is the item whose child items's rent won't be charged. Only parent item rent will be charged.

    $scope.DeleteItem = function (index) {
        //   $scope.$apply(function () {
        // $scope.WorkOrder.Items.splice(index - 1, 1);
        //var item = $scope.WorkOrder.Items[index]
        //$scope.WorkOrder.Items.splice(index, 1);
        //$scope.WorkOrder.Items = $scope.WorkOrder.Items.filter(o => o.GroupItemId != item.ProductId);

        var item = $scope.WorkOrder.Items.filter(o => o.Deleted != 1)[index];

        $scope.getBomItems(item.ProductId, function (bom) {
            item.BOM = bom;
            $.each(item.BOM, function (index, value) {

                var bomItem = $scope.WorkOrder.Items.find(o =>
                    o.ProductId == value.ProductId
                );
                if (bomItem) {
                    bomItem.SentQty -= value.Quantity * item.SentQty;
                    bomItem.Rate = 0;
                    if (bomItem.SentQty <= 0) {
                        bomItem.Deleted = 1;
                    }
                }
            });
            $scope.WorkOrder.Items.filter(o => o.Deleted != 1)[index].SentQty = 0;
            $scope.WorkOrder.Items.filter(o => o.Deleted != 1)[index].Deleted = 1;


            //$scope.WorkOrder.Items.splice(index, 1);
            //   $scope.WorkOrder.Items = $scope.WorkOrder.Items.filter(o => o.GroupItemId != item.ProductId);
            //$scope.WorkOrder.Items = $scope.WorkOrder.Items.filter(o => o.GroupItemId != item.ProductId);
        });
        //$scope.WorkOrder.Items.filter(o => o.Deleted != 1)[index].Deleted = 1;
        //$scope.WorkOrder.Items.filter(o => o.Deleted != 1)[index].SentQty = 0;

        //$scope.WorkOrder.Items[index].Deleted = 1;
        //$scope.WorkOrder.Items[index].SentQty = 0
        // });
    }
    // $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });

    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();

    hideWorkOrderTab();

    $scope.Find = function () {

    }

    //  nav_init();
    function ItemStock() {
        var filter = { OnDate: convertDate(new Date()) };
        InventoryFactory.GetStockInhand(function (e) {

            if (e.data) {
                $scope.ItemStock = e.data;
            }
        }, filter);
    }

    function PartyStockBalance() {
        var token = $rootScope.getTokenInfo();
        var filter = {
            LedgerId: $scope.WorkOrder.LedgerId,
            LedgerSiteId: $scope.WorkOrder.SiteInfo && $scope.WorkOrder.SiteInfo.LedgerSiteId ? $scope.WorkOrder.SiteInfo.LedgerSiteId : 0,
            From: convertDate(token.FinYearStart),
            To: (worder.WorkOrderDate)
        };
        //InventoryFactory.PartyStockBalance_BySize(filter, function (e) {
        //    $scope.PartyBalance = e.data;
        //});

        var ld = new $.Ledger();
        if ($scope.ChallanType == 10) {
            filter.BalanceType = 'hire';
        }
        ld.GetClientWiseItems(function (e) {
            $scope.PartyBalance = e.data;
        }, filter);
    }

    $scope.selectedParty = function (selected) {

        if (selected != undefined) {

            var item = selected.originalObject;
            $scope.Ledger = item;
            $scope.WorkOrder.LedgerId = item.LedgerId;
        }
    };
    function recalculateChallanTaxes() {
        if (!$scope.WorkOrder) {
            return $q.when();
        }

        var result = ChallanTaxService.calculateChallanTaxes({
            items: $scope.WorkOrder.Items,
            allSizes: $scope.AllSizes,
            taxCategories: $scope.TaxCategories,
            applyGst: $scope.ApplyGST !== false && $scope.Config.applyTax !== false,
            freight: $scope.WorkOrder.SiteInfo ? $scope.WorkOrder.SiteInfo.Freight : 0,
            otherCharges: $filter('sumByKey')($scope.WorkOrder.OtherCharges, 'Amount'),
            freightTax: $scope.Config.freightTax,
            otherChargesTax: $scope.Config.otherChargesTax
        });

        $scope.WorkOrder.AppliedTaxes = result.appliedTaxes || [];
        $scope.WorkOrder.TaxAmount = result.taxAmount || 0;
        $scope.WorkOrder.Taxes = result.legacyTaxes || {
            IGST: 0, IGSTAmount: 0,
            SGST: 0, SGSTAmount: 0,
            CGST: 0, CGSTAmount: 0
        };

        return $q.when();
    }

    $scope.SubTotal = function (_total) {
        $scope.WorkOrder.TaxAmount = 0;
        if ($scope.WorkOrder == undefined) {
            return;
        }
        if ($scope.WorkOrder.SiteInfo == undefined) {
            return;
        }
        if ($scope.WorkOrder.Items == undefined) {
            return;
        }

        var subTotal = 0;
        for (var i = 0; i < $scope.WorkOrder.Items.length; i++) {

            var item = $scope.WorkOrder.Items[i];
            if (item.Deleted == 1) {
                continue;
            }
            $scope.WorkOrder.Items[i].SubTotal = 0;
            if ($scope.WorkOrder.Items[i].SentQty != null) {
                $scope.WorkOrder.Items[i].SubTotal = parseFloat($scope.WorkOrder.Items[i].SentQty) * parseFloat($scope.WorkOrder.Items[i].Rate);
                subTotal += $scope.WorkOrder.Items[i].SubTotal;
            }
        }
        $scope.WorkOrder.SubTotal = $scope.WorkOrder.SiteInfo.SubTotal = subTotal;
        $scope.WorkOrder.SubTotal1 = $scope.WorkOrder.SubTotal + parseFloat($scope.WorkOrder.SiteInfo.Freight);
        recalculateChallanTaxes().then(function () {
            var otherCharges = $filter('sumByKey')($scope.WorkOrder.OtherCharges, 'Amount');
            $scope.WorkOrder.Total = $scope.WorkOrder.SubTotal
                + parseFloat($scope.WorkOrder.SiteInfo.Freight)
                + ($scope.WorkOrder.TaxAmount || 0)
                + otherCharges;

            if (_total == 1) {
                $scope.WorkOrder.Total = $scope.WorkOrder.SiteInfo.Total = $scope.WorkOrder.Total;
            }
        });

        if (_total == 1) {
            return $scope.WorkOrder.Total;
        }

        return subTotal;
    };

    $scope.CalcTax = function () {
        recalculateChallanTaxes();
    };
    $scope.itemSelected = function (itemId) {
        //here itemId is selected index

        if (itemId <= 0) return;
        var index = itemId - 1; //-- subtract the index of first element in the dropdown which is "Please select";
        //  $scope.IssueItem.ProductId = itemId;
        //var item = $scope.ProductRates.find(o=>o.ProductId == itemId);
        var item = $scope.AllSizes[index];
        $scope.IssueItem.ProductId = item.ProductId;

        $scope.IssueItem.Product = item.Product;
        $scope.IssueItem.Code = item.Code;

        $scope.IssueItem.ProductSizeId = item.ProductSizeId;


        $scope.ItemInStock = $filter('sumByKey')($filter('filter')($scope.ItemStock, {
            ProductId: $scope.IssueItem.ProductId
        }), 'ClosingBalance');
        $scope.PartyItemInStock = $filter('sumByKey')($filter('filter')($scope.PartyBalance, {
            ProductId: $scope.IssueItem.ProductId
        }), 'ClosingBalance');

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            var item = selected.originalObject;
            $scope.IssueItem.ProductId = item.ProductId;

            $scope.IssueItem.Product = item.Product;
            $scope.IssueItem.Code = item.Code;
            //  findDefaultRate($scope.IssueItem.ProductId);

            $scope.IssueItem.ProductSizeId = item.ProductSizeId;
            $scope.ItemInStock = $filter('sumByKey')($filter('filter')($scope.ItemStock, {
                ProductId: $scope.IssueItem.ProductId
            }), 'ClosingBalance');
            $scope.PartyItemInStock = $filter('sumByKey')($filter('filter')($scope.PartyBalance, {
                ProductId: $scope.IssueItem.ProductId
            }), 'ClosingBalance');

            $scope.getBomItems($scope.IssueItem.ProductId);
        }
    };
    $scope.getSelectedItemBalance = function () {
        if (!$scope.IssueItem || !$scope.IssueItem.ProductId) return null;
        if ($scope.PartyBalance && $scope.WorkOrder && $scope.WorkOrder.SiteInfo && $scope.WorkOrder.SiteInfo.LedgerSiteId) {
            var siteBalance = $filter('sumByKey')($filter('filter')($scope.PartyBalance, {
                ProductId: $scope.IssueItem.ProductId
            }), 'ClosingBalance');
            return (siteBalance !== undefined && siteBalance !== null && !isNaN(siteBalance)) ? siteBalance : null;
        }
        return null;
    };
    $scope.getBomItems = function (productId, o) {
        var p = new $.Product();
        p.BOMByProductId(function (e) {
            if (e.data.Code != 200) {
                alert('Could not featch BOM details');
                return;
            }
            if (o) {
                o(e.data.Data.Details);
                return;
            }
            $scope.IssueItem.BOM = e.data.Data.Details;
        }, { ProductId: productId });
    }

    $scope.groupItemSelected = function (itemId) {
        if (itemId > 0) {
            $scope.IssueItem.Rate = 0;
        }
    }

    $scope.DefaultRate = 0.0;

    function findDefaultRate(issueItem) {
        var productId = issueItem.ProductId;

        issueItem.Rate = 0;
        //var product = $scope.AllSizes.find(o => o.ProductId == productId);
        var product = $scope.ProductRates.find(o => o.ProductId == productId);


        var rateApplied = false;
        if ($scope.Config) {

            if ($scope.Config.rateType == 1) {
                issueItem.Rate = parseFloat(product.SalePrice);
                rateApplied = true;
            }
            if ($scope.Config.rateType == 3) {
                issueItem.Rate = parseFloat(product.LossRate);
                rateApplied = true;
            }
            if (!issueItem.Rate || isNaN(issueItem.Rate)) {
                issueItem.Rate = 0;
            }
            if (issueItem.GroupItemId > 0) {
                if ($scope.Config.showRateOf == 2) {
                    var groupedItem = $scope.WorkOrder.Items.find(o => o.ProductId == issueItem.GroupItemId);
                    if (groupedItem) {
                        groupedItem.Rate = 0;

                    }
                }
                else {
                    issueItem.Rate = 0;
                    return;
                }

            }

        }

        if ($scope.ProductRates && rateApplied == false) {
            var item = $scope.ProductRates.find(o => o.ProductId == productId);
            if (item) {
                issueItem.Rate = item.RentRate;
            }
            return item;
        }
    }

    function getNextWorkOrderNumber() {
        debugger
        if (!$scope.Config.showNextChallanInvoiceNumbers) {
            return;
        }
        var nextId = new $.NextId();
        nextId.GetWorkOrderNumber(function (e) {
            var n = e.data;
            if (n == null || n === '') {
                return;
            }
            n = String(n);
            $scope.WorkOrder.Number = n;
            $scope.WorkOrder.ChallanNumber = n;
        }, { ChallanType: $scope.ChallanType });
    }

    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.WorkOrder.LedgerId = $rootScope.LedgerId;
    }

    $scope.ApplyGST = true;
    $scope.$watch('ApplyGST', function () {
        if ($scope.ApplyGST === false) {
            $scope.WorkOrder.AppliedTaxes = [];
            $scope.WorkOrder.TaxAmount = 0;
            $scope.WorkOrder.Taxes = null;
        }

        $scope.ApplySiteConfig();
    }, true);
    $scope.ApplySiteConfig = function () {
        $scope.ProductRates = [];

        if (!$scope.WorkOrder.SiteInfo.LedgerSiteId) {
            return;
        }

        getAllProducts();
        recalculateChallanTaxes().then(function () {
            $scope.SubTotal(0);
        });
    };


    function getLedgerDetails() {
        // $scope.WorkOrder.SiteInfo.Site="";
        ledgerDTO.GetDetails(function (e) {
            if (worder.WorkOrderId == 0) {
                $scope.WorkOrder.SiteInfo.Site = e.data.ShippingAddress.FullAddress
            }
        });
    }

    function getAllProducts() {

        var model = {
            LedgerId: $scope.WorkOrder.LedgerId,
            LedgerSiteId: $scope.WorkOrder.SiteInfo.LedgerSiteId
        }
        ledgerDTO.PartyWiseRates(function (e) {
            $scope.ProductRates = e.data;
        }, model);

    }
    $scope.TaxCategories = [];

    function loadTaxCategories() {
        TaxService.getTaxCategories(null, true).then(function (response) {
            if (response.data && response.data.Code === 200) {
                $scope.TaxCategories = response.data.Data || [];
            }
        });
    }

    getAllProductSizesByCompany();
    loadTaxCategories();

    function getAllProductSizesByCompany() {
        var product = new $.Product();
        product.GetSizeListByCompany(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }

    $scope.CompanySelection = function (obj) {
        if (obj != undefined) {
            $scope.WorkOrder.CompanyId = obj.originalObject.CompanyId;
        }
    };

    //$scope.focusIn = function (selected) {
    //    // $scope.Product = this.item.Props.Item.Props;
    //    console.log(selected);
    //};

    $scope.RowSelected = function (index) {

        if ($scope.WorkOrder.Items[index] != undefined) {
            //  $scope.$digest(function () {
            selectedWorkOrderItemIndex = index;
            //  $scope.Product = $scope.WorkOrder.Items[index].Item.Props;
            // });
        }
        //LoadSites(index, 0);

    };

    $scope.GetSelected = function () {
        var v = $scope[selectedProject];
    };
    /*
    function GetTaxes() {
        var Tax = new $.Tax({
            ItemId: 0
        });

        Tax.GetTaxes($scope.WorkOrder, function (e) {

            $scope.WorkOrder.Taxes = e.data;
        });
    }
    */
    function initializeArray() {
        var WorkOrders = [];
        for (var i = 0; i <= 20; i++) {
            WorkOrders.push(new $.WorkOrderItem({}));
        }
        return WorkOrders;
    }

    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
            // $scope.ApplySiteConfig();
        }, {
            LedgerId: $scope.WorkOrder.LedgerId
        });
    }

    $scope.NewSite = function () {
        if ($scope.WorkOrder.LedgerId) {
            if ($scope.WorkOrder.LedgerId > 0) {
                openSiteDialog({
                    LedgerId: $scope.WorkOrder.LedgerId,
                    LedgerSiteId: 0
                });
            }
        }
    };
    var onSiteAdded = $rootScope.$on("OnSiteAdded", function (evt, data) {
        getSites();
    });

    var onLedgerAdded = $rootScope.$on("OnLedgerAdded", function (evt, data) {
        if (data && data.quotationIdForLink) {
            return;
        }
        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            var newLedger = e.data.find(x => x.Code == data.Code);
            $scope.WorkOrder.LedgerId = newLedger.LedgerId;
            $scope.Ledger = item;

        });
    });

    function openSiteDialog(data) {
        ModalFactory.AddEditClientSite('AddEditClientSiteController', data);
    }

    $scope.NewLedger = function () {

        ModalFactory.AddEditLedger('AddEditLedgerController', {
            LedgerId: 0
        });

    }



    getAllVehicles();

    function getAllVehicles() {
        CompanyService.getAllVehicle().then(function (e) {
            $scope.Vehicles = e.data.Data;
        });
    }
    getAllEmployees();

    function getAllEmployees() {
        EmployeeService.GetAllEmployees(function (e) {
            $scope.Drivers = e.data.Data;
        });
    }

    function getAllOtherCharges() {

        WorkOrderFactory.GetOtherCharges(worder.WorkOrderId, function (e) {
            $scope.WorkOrder.OtherCharges = e.data;

        });
    }
    $scope.createEwayBill = false;
    $scope.Save = function (createewayBill = false) {
        $scope.createEwayBill = createewayBill;
        var res = $scope.WorkOrder.Items;
        // $scope.WorkOrder.ChallanType = CHALLAN_TYPE.RENT_DELIVERY; // for rent purpose
        $scope.WorkOrder.ChallanType = $scope.ChallanType;
        var m = $('#form-workorder').valid();
        // var site = $('#form-site').valid();

        var time = new Date($scope.WorkOrder.IssueTime);
        var wsDate = new Date(moment($scope.WorkOrder.WorkOrderDate, "DD/MM/YYYY"));
        var bsDate = new Date(moment($scope.WorkOrder.RentStartDate, "DD/MM/YYYY"));

        var model = cloneObj($scope.WorkOrder);
        //model.WorkOrderDate = wsDate.toLocaleString();
        //model.RentStartDate = bsDate.toLocaleString();
        model.WorkOrderDate = formatdate($scope.WorkOrder.WorkOrderDate);
        model.RentStartDate = formatdate($scope.WorkOrder.RentStartDate);
        if ($scope.WorkOrder.PODate) {
            model.PODate = formatdate($scope.WorkOrder.PODate);
        }
        if (model.PONumber && model.PONumber.length > 0) {
            if (!model.PODate) {
                alert('Please enter PO Date');
                return;
            }
        }
        if (model.PODate) {
            if (!model.PONumber || model.PONumber.length <= 0) {
                alert('Please enter PO Number');
                return;
            }
        }

        var rsDate = new Date(moment($scope.WorkOrder.SiteInfo.RecoveryDate, "DD/MM/YYYY"));
        if (rsDate != "Invalid Date") {
            model.SiteInfo.RecoveryDate = formatdate($scope.WorkOrder.SiteInfo.RecoveryDate);
        }
        model.JobCardId = $scope.JobCardId;

        if (model.WorkOrderId == 0) {
            model.Items = model.Items.filter(o => o.Deleted != 1);
        }

        var days = dateDiff(wsDate, bsDate);
        if (days != 0) {
            if (days < 0) {
                alert('Rent can not start before issue date');
                return;
            } else {
                var c = confirm('Rent will start from ' + $scope.WorkOrder.RentStartDate + '. Are you sure to proceed?');
                if (!c) {
                    return;
                }
            }
        }

        //if (site && sId == 0) {
        //    if ($scope.WorkOrder.SiteInfo.JobNumber == null || $scope.WorkOrder.SiteInfo.JobNumber == '') {
        //        alert('Job number is required');
        //        return;
        //    }
        //}
        if (m) {
            EnableToolbar(0);
            //   var reader = new FileReader();

            var fileList = [];
            //fileList = addFileToList(fileList, 'fileSitePic');
            //fileList = addFileToList(fileList, 'Doc1');
            //fileList = addFileToList(fileList, 'Doc2');
            //fileList = addFileToList(fileList, 'Doc3');

            if (res.length < 1) {
                alert('Work order can not be empty.');
                return;
            }
            if ($scope.Config.allowEditChallanNumber == true) {
                if (!model.ChallanNumber) {
                    alert('Please enter the challan number.');
                    return;
                }
                if (model.ChallanNumber.length < 1) {
                    alert('Challan number must be minimum 1 characters long');
                    return;
                }

            }

            model.Tnc = htmlEncode(model.Tnc);
            model.Remarks = htmlEncode(model.Remarks);
            if (model.SezDescription)
                model.SezDescription = htmlEncode(model.SezDescription);
            if (model.ShipFrom)
                model.ShipFrom = htmlEncode(model.ShipFrom);

            model.AppliedTaxes = $scope.WorkOrder.AppliedTaxes || [];
            $scope.addWorkOrder(fileList, model);
        }
    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    $scope.addWorkOrder = function (fileList, model) {
        $scope.WorkOrder.Transaction = $scope.Trans;
        $scope.WorkOrder.Edit = edit;

        $scope.WorkOrder.Add(function (e) {

            var res = e.data;
            if (res.Code == 200) {
                if ($scope.ChallanType == 2) {
                    toaster.pop('success', "success", res.Message);
                }
                if ($scope.ChallanType == 10) {
                    toaster.pop('success', "success", 'Items hired successfully.');
                }
                $scope.warnOnLeave = false;
                $scope.WorkOrder.WorkOrderId = res.Data.WorkOrderId;

                $scope.print($scope.WorkOrder.WorkOrderId)

                wId = 0;
                init();

            } else if (res.Code == 500) {
                toaster.pop('error', "error", res.Message);

            }

        }, fileList, model);
    }

    $scope.print = function (workOrderId) {

        $scope.WorkOrder.PrintIssuedReceipt(function (e) {
            var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
            if ($scope.createEwayBill) {
                var k = '0,chl,' + workOrderId + ',del';
                var ec = $crypto.encrypt(k);
                if ($scope.fromContractInfoModal) {
                    $('#contractDeliveryChallanModal').modal('hide');
                }
                $state.go('addEditEwayBill', { key: ec });
                return;
            }
            if ($scope.fromContractInfoModal && $scope.ChallanType === 1) {
                $('#contractDeliveryChallanModal').modal('hide');
                return;
            }
            if ($scope.ChallanType == 2) {
                $state.go('issueChallan');
            }
            else if ($scope.ChallanType == 10) {
                $state.go('hirechallan');
            }
        }, [{
            WorkOrderId: workOrderId
        }]);
    }


    function openChallanHeaderSelectinDialog(okButton, cancelButton, message) {
        var div = '<div style="width:50%;height:10%"></div>';

        $(div).load('templ/dialogs/messagewithConfirm.html?d=' + new Date().getTime(), function () {
            var html = $(this).html();
            $scope.Message = message;
            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                height: '200px',
                template: html,
                parent: angular.element(document.body),
                controller: function ($scope, $mdDialog) {
                    $scope.OkButtonClick = function () {
                        $mdDialog.hide();
                        okButton($scope.HeaderType);
                    }
                    $scope.closeDialog = function () {

                        $mdDialog.hide();
                        if (cancelButton) {
                            cancelButton(null);
                        }
                    }
                }
            });
        });
    }

    function hideWorkOrderTab() {
        $scope.HideWorkOrder = 0;
        if (wId > 0) {


            $('#tbWorkOrder').addClass('disabled');
            //$('#tbWorkOrder').next().addClass('active');
            $('#tbWorkOrder a').removeAttr('data-toggle');

            $('a[href="#siteInfo"]').tab('show');
        }
    }

    var parentItems = [];
    //add new item to be issued
    $scope.addItem = function ($event) {

        var item = cloneObj($scope.IssueItem)

        if (wId == 0) {

            if ($scope.Config.maintainInventory == 2) {
                if ($scope.ItemStock) {
                    var sItem = $scope.ItemStock.find(o => o.ProductId == item.ProductId);
                    if (sItem && sItem.ClosingBalance < parseInt(item.SentQty)) {
                        alert('Insufficient Quantity');
                        return;
                    }
                }
            }
        }

        if ($scope.Config) {
            if ($scope.Config.warnchallanwithoutrate == 'yes') {
                if ($scope.ProductRates) {
                    var rate = $scope.ProductRates.find(o => o.ProductId == item.ProductId);
                    if (!rate) {
                        var c = confirm('Rate is not set for this item. Are you sure to continue?');
                        if (!c) {
                            return;
                        }
                    }
                    else if (rate && (rate.ProductRateId == 0 || rate.Rate <= 0)) {
                        var c = confirm('Rate is not set for this item. Are you sure to continue?');
                        if (!c) {
                            return;
                        }
                    }

                }
            }
        }

        $scope.AddToChallan(item);

        var lastProductId = item.ProductId;
        var lastProductName = item.Product;
        var lastProductCode = item.Code;
        var lastGroupItemId = item.GroupItemId;

        $scope.IssueItem = new $.WorkOrderItem({
            ProductId: lastProductId,
            Product: lastProductName,
            Code: lastProductCode,
            Rate: $scope.DefaultRate,
            SentQty: 0

        });
        if (lastGroupItemId) {
            if (lastGroupItemId > 0) {
                $scope.IssueItem.GroupItemId = lastGroupItemId;
            }
        }
        $('#itemSelect_value').focus();




        $event.preventDefault();
        return;
    }
    $scope.AddToChallan = function (issueItem) {
        if (issueItem.ProductId <= 0 || issueItem.SentQty == 0 || !issueItem.SentQty) {
            alert("Product not found, Pleaes type the product name and select from the list.");
            return;
        }

        //if ($scope.IssueItem.Rate <= 0 && !$scope.IssueItem.GroupItemId) {
        //    alert("Rate can't be 0 or less.");
        //    return;
        //}
        issueItem.Deleted = 0;
        var exits = false;
        for (var i = 0; i < $scope.WorkOrder.Items.length; i++) {
            var item = $scope.WorkOrder.Items[i];
            if (item.ProductId == issueItem.ProductId) {
                item.Deleted = 0;
                item.SentQty = parseFloat(item.SentQty) + parseFloat(issueItem.SentQty);
                exits = true;
                break;
            }
        }

        var lastProductId = issueItem.ProductId;
        var lastProductName = issueItem.Product;
        var lastGroupItemId = issueItem.GroupItemId;
        if (!issueItem.GroupItemId && issueItem.BOM && issueItem.BOM.length > 0) {

            parentItems.push({
                ProductId: lastProductId,
                Product: lastProductName
            });
            $scope.ParentItems = parentItems;
        } else {
            issueItem.Rate = 0;
        }

        if (!exits) {
            // $scope.WorkOrder.Items.push(issueItem);
            //if (issueItem.BOM && issueItem.BOM.length > 0) {
            //    $scope.WorkOrder.Items.unshift(issueItem);
            //}
            //else {
            $scope.WorkOrder.Items.push(issueItem);
            //}
        }

        findDefaultRate(issueItem);

        if (issueItem.BOM != null) {
            $.each(issueItem.BOM, function (index, value) {

                value.SentQty = value.Quantity * issueItem.SentQty;
                value.Rate = 0;
                value.Quantity = 0;
                value.BOM = null;
                value.GroupItemId = issueItem.ProductId;
                $scope.AddToChallan(value);
            });
        }
    }

    $scope.sizeTextFocused = function (item) {

        item.EditSize = true;
        item.ItemSizes = $.map($scope.AllSizes, function (value, key) {
            if (value.ProductId == item.ProductId) {
                return value;
            }
        });
        // $(event.target).next().show();
    };


    $scope.OnSizeOut = function (item) {

        item.EditSize = false;
        if ($scope.AllSizes) {
            var size = $scope.AllSizes.find(o => o.ProductSizeId == item.ProductSizeId);
            if (size) {
                item.Size = size.Size;
            }
        }
        // $(event.target).hide();
    }

    function loadSavedChallanTaxes(siteId) {
        if (!siteId) {
            return;
        }

        var taxLoader = new $.WorkOrder({ SiteId: siteId });
        taxLoader.GetTaxes(function (e) {
            var lineTaxes = e.data || [];
            var taxesByItem = {};

            lineTaxes.forEach(function (tax) {
                if (!taxesByItem[tax.WorkOrderItemId]) {
                    taxesByItem[tax.WorkOrderItemId] = [];
                }
                taxesByItem[tax.WorkOrderItemId].push(tax);
            });

            ($scope.WorkOrder.Items || []).forEach(function (item) {
                item.LineTaxes = taxesByItem[item.WorkOrderItemId] || [];
            });

            $scope.WorkOrder.AppliedTaxes = ChallanTaxService.aggregateUniqueTaxes(lineTaxes);
            $scope.WorkOrder.TaxAmount = $scope.WorkOrder.AppliedTaxes.reduce(function (sum, tax) {
                return sum + (parseFloat(tax.Amount) || 0);
            }, 0);
            $scope.WorkOrder.Taxes = ChallanTaxService.buildLegacyTaxSummary($scope.WorkOrder.AppliedTaxes);
        });
    }

    function init() {

        newRecord();
        $scope.WorkOrder.Items = []; //initializeArray();
        $scope.IssueItem = new $.WorkOrderItem({
            Rate: $scope.DefaultRate, SentQty: 0
        });
        $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        //GetTaxes();
        getAllOtherCharges();
        $scope.WorkOrder.AppliedTaxes = [];
        $scope.WorkOrder.Taxes = {
            IGST: 0, IGSTAmount: 0, SGST: 0,
            SGSTAmount: 0, CGST: 0, CGSTAmount: 0
        };

        //load workorde details on edit.
        if (worder.WorkOrderId > 0) {
            worder.GetDetail(function (e) {

                // $scope.WorkOrder.Items = worder.Items = e.data;
                $scope.WorkOrder.Items = e.data.Items;
                if ($scope.AllSizes) {
                    $scope.WorkOrder.Items.forEach(function (it) {
                        if (!it.Code) {
                            var sz = $scope.AllSizes.find(o => o.ProductId === it.ProductId && o.ProductSizeId === it.ProductSizeId) || $scope.AllSizes.find(o => o.ProductId === it.ProductId);
                            if (sz) it.Code = sz.Code;
                        }
                    });
                }
                var wOrder = e.data;
                if (e.data) {
                    $scope.WorkOrder.Tnc = wOrder.Tnc;
                    // var siteObj = e.data[0];
                    var siteObj = e.data.Items[0];
                    $scope.WorkOrder.SiteInfo.Freight = siteObj.Freight;
                    $scope.WorkOrder.WorkOrderDate = convertDate(siteObj.SentDate);
                    $scope.WorkOrder.RentStartDate = convertDate(siteObj.RentStartDate);
                    $scope.WorkOrder.RecoveryDate = convertDate(wOrder.RecoveryDate);



                    $scope.WorkOrder.SiteInfo.Weight = wOrder.Weight;
                    $scope.WorkOrder.SiteInfo.ApproximateValue = wOrder.ApproximateValue  || 0;
                    $scope.WorkOrder.SiteInfo.LRNumber = wOrder.LRNumber || siteObj.LRNumber || '';
                    $scope.WorkOrder.SiteInfo.CRNumber = wOrder.CRNumber || siteObj.CRNumber || '';
                    $scope.WorkOrder.SiteInfo.GRNumber = wOrder.GRNumber || siteObj.GRNumber || '';
                    $scope.WorkOrder.SiteInfo.Site = siteObj.Site;
                    $scope.WorkOrder.SiteInfo.SiteId = siteObj.SiteId;
                    $scope.WorkOrder.SiteInfo.Vehicle = siteObj.Vehicle;
                    $scope.WorkOrder.SiteInfo.Driver = siteObj.Driver;
                    $scope.WorkOrder.SiteInfo.DriverId = siteObj.DriverId;
                    $scope.WorkOrder.SiteInfo.VehicleId = siteObj.VehicleId;
                    $scope.WorkOrder.SiteInfo.TransporterId = siteObj.TransporterId;
                    $scope.WorkOrder.SiteInfo.TeamId = siteObj.TeamId;

                    $scope.WorkOrder.SiteInfo.LedgerSiteId = siteObj.LedgerSiteId;
                    $scope.WorkOrder.ChallanNumber = siteObj.ChallanNumber;
                    $scope.WorkOrder.SubTotal = siteObj.SubTotal;
                    $scope.WorkOrder.Total = siteObj.Total;
                    $scope.WorkOrder.LedgerId = siteObj.LedgerId;
                    $scope.WorkOrder.Taxes = {
                        IGST: wOrder.IGSTRate, IGSTAmount: wOrder.IGSTAmount, SGST: wOrder.SGSTRate,
                        SGSTAmount: wOrder.SGSTAmount, CGST: wOrder.CGSTRate, CGSTAmount: wOrder.CGSTAmount
                    };

                    loadSavedChallanTaxes(siteObj.SiteId);

                    $scope.WorkOrder.Remarks = wOrder.Remarks;
                    $scope.WorkOrder.SezDescription = wOrder.SezDescription || '';
                    $scope.WorkOrder.ShipFrom = wOrder.ShipFrom || '';
                    $scope.WorkOrder.RefNo = wOrder.RefNo;
                    $scope.WorkOrder.WarehouseId = wOrder.WarehouseId;

                    //--po number and date
                    $scope.WorkOrder.PODate = convertDate(wOrder.PODate);
                    $scope.WorkOrder.PONumber = wOrder.PONumber;

                    var _wDate = new Date(e.data.WorkOrderDate);
                    // $scope.WorkOrder.IssueTime = new Date(1970, 0, 1, _wDate.getHours(), _wDate.getMinutes(), 0);

                    $scope.selectParty();
                }

            });
        }


        var ledger = new $.Ledger({});

        LedgerFactory.GetAllParties(function (e) {
            if (e.status != 200) {
                alert('Could not load parties');
                return;
            }
            $scope.Accounts = e.data.filter(o => o.AccountGroup == StaicData.SYS_ACCOUNT_GROUPS.SUNDRY_CREDITORS || o.AccountGroup == StaicData.SYS_ACCOUNT_GROUPS.SUNDRY_DEBTORS);
            $scope.selectParty();
        });

        ItemStock();
        var token = $rootScope.getTokenInfo();
        if (token != null) {
            $scope.MinDate = token.FinYearStart;

            // convertDate(token.FinYearEnd);
        }
    }
    init();

    ChallanService.initChallan().then(function (e) {
        if (e.data.Code == 200) {

            var comp = e.data.Data.company;
            if (!$scope.WorkOrder.WarehouseId) {
                $scope.WorkOrder.WarehouseId = comp.DefaultWarehouseId;
            }
        }
    });

    $scope.selectParty = function () {
        if ($scope.WorkOrder.LedgerId != null && $scope.Accounts != null) {
            $scope.initialValue = $scope.Accounts.find(o => o.LedgerId == $scope.WorkOrder.LedgerId);
        }
    }
    $scope.$watch('WorkOrder.OtherCharges', function () {
        $scope.SubTotal(0);
    }, true);

    function getChallanConfig(response) {
        //   var config = new $.Config();
        //config.GetAllConfig(function (e) {

        //   var response = e.data;
        if (response.Data != null && response.Data) {
            if (response.Data.length > 0) {
                var challanConfig = response.Data.filter(o => o.Category == 'ISSUECHALLAN');
                var inventoryConfig = response.Data.filter(o => o.Category == 'inventory');
                var general = response.Data.filter(o => o.Category == 'general');
                var rateType = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'rateType');
                try {
                    var tnc = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' &&
                        o.Key == 'tnc');

                    if (tnc) {
                        $scope.Config.tnc = $scope.WorkOrder.Tnc = tnc.Value;
                    }
                }
                catch (error) {
                    alert(error);
                }


                if (rateType) {
                    $scope.Config.rateType = rateType.Value;
                }
                var showRateOf = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'showRateOf');

                if (showRateOf) {
                    $scope.Config.showRateOf = showRateOf.Value;
                }
                var applyTax = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'applyTax');

                if (applyTax) {
                    $scope.Config.applyTax = $scope.ApplyGST = applyTax.Value == 1;
                }
                var allowNegative = general.find(o => o.SubCategory == 'inventory' && o.Key == 'allowNegativeSale');

                if (allowNegative) {
                    $scope.Config.maintainInventory = parseInt(allowNegative.Value);
                }
                var showNextChallanInvoice = general.find(o => o.SubCategory == 'issuechallan' &&
                    o.Key == 'showNextChallanInvoiceNumbers');
                if (showNextChallanInvoice) {
                    $scope.Config.showNextChallanInvoiceNumbers = showNextChallanInvoice.Value == 'true';
                }
                var allowEditChallanNumber = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' &&
                    o.Key == 'allowEditChallanNumber');

                if (allowEditChallanNumber) {
                    $scope.Config.allowEditChallanNumber = allowEditChallanNumber.Value == 1;
                }

                var freightTax = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' &&
                    o.Key == 'freightTax');

                if (freightTax) {
                    $scope.Config.freightTax = freightTax.Value == 'true';
                }

                var otherChargesTax = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' &&
                    o.Key == 'otherChargesTax');

                if (otherChargesTax) {
                    $scope.Config.otherChargesTax = otherChargesTax.Value == 'true';
                }

                var addInfo = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' &&
                    o.Key == 'addInfo');

                if (addInfo) {
                    $scope.Config.addInfo = $scope.WorkOrder.Remarks = addInfo.Value;
                }


                var warnchallanwithoutrate = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' &&
                    o.Key == 'warnchallanwithoutrate');

                if (warnchallanwithoutrate) {
                    $scope.Config.warnchallanwithoutrate = warnchallanwithoutrate.Value;
                }

            }

        }
        //});
    }
    // getChallanConfig();

    $scope.onQuotationSelected = function (val) {
        // alert(val);
    };


    $scope.$watch('WorkOrder.Items', function () {

        $scope.SubTotal(0);
    }, true);

    $scope.$watch('AllSizes', function (sizes) {
        if (sizes && sizes.length && $scope.WorkOrder && $scope.WorkOrder.Items) {
            $scope.WorkOrder.Items.forEach(function (it) {
                if (!it.Code) {
                    var sz = sizes.find(o => o.ProductId === it.ProductId && o.ProductSizeId === it.ProductSizeId) || sizes.find(o => o.ProductId === it.ProductId);
                    if (sz) it.Code = sz.Code;
                }
            });
        }
    }, true);


    $scope.$watch('WorkOrder.SiteInfo.Freight', function () {
        //if ($scope.WorkOrder.SiteInfo.Freight > 0 && $scope.WorkOrder.SiteInfo.VehicleId == 0) {
        //    $scope.WorkOrder.SiteInfo.Freight = 0;
        //    alert('You must select vehicle to enter cartriage');
        //}
        // $scope.CalcTax();
        //$scope.WorkOrder.Total = $scope.WorkOrder.SubTotal +
        //    parseFloat($scope.WorkOrder.SiteInfo.Freight) + $scope.WorkOrder.TaxAmount;
        $scope.SubTotal(0);
    });
    $scope.$watch('WorkOrder.SiteInfo.VehicleId', function () {
        if ($scope.WorkOrder.SiteInfo.VehicleId == 0) {
            $scope.WorkOrder.SiteInfo.Freight = 0;

        }
    });
    $scope.$watch('WorkOrder.LedgerId', function () {

        if ($scope.WorkOrder.LedgerId != null) {
            ledgerDTO.Props.LedgerId = $scope.WorkOrder.LedgerId;
            //   $scope.WorkOrder.LedgerId =ledgerDTO.Props.LedgerId = obj.originalObject.LedgerId;
            //    $scope.IssueItem.Rate =   $scope.DefaultRate = obj.originalObject.DefaultRate;
            // getLedgerDetails();
            if ($scope.Accounts) {
                $scope.Ledger = $scope.Accounts.find(o => o.LedgerId == $scope.WorkOrder.LedgerId);
            }
            if ($scope.Accounts)
                $scope.Ledger = $scope.Accounts.find(o => o.LedgerId == $scope.WorkOrder.LedgerId);

            PartyStockBalance();
            $rootScope.LedgerId = $scope.WorkOrder.LedgerId;
            getSites();
        }
    });

    $scope.$watch('WorkOrder.SiteInfo.LedgerSiteId', function () {
        if ($scope.WorkOrder.SiteInfo.LedgerSiteId) {
            getAllProducts();
            $scope.ApplySiteConfig();
            PartyStockBalance();
        }
    });
    //$scope.$watch('WorkOrder.WorkOrderDate', function () {
    //    $scope.WorkOrder.RentStartDate = $scope.WorkOrder.WorkOrderDate;
    //});
    $scope.onWorkOrderDateChange = function () {
        $scope.WorkOrder.RentStartDate = $scope.WorkOrder.WorkOrderDate;
    };

    $scope.$watch('IssueItem.ProductSizeId', function () {

        if ($scope.IssueItem.ProductSizeId != null) {
            if ($scope.AllSizes) {
                var size = $scope.AllSizes.find(o => o.ProductSizeId == $scope.IssueItem.ProductSizeId);
                if (size) {
                    $scope.IssueItem.Size = size.Size;
                    $scope.ItemInStockSize = $filter('sumByKey')($filter('filter')($scope.ItemStock, {
                        ProductId: $scope.IssueItem.ProductId,
                        ProductSizeId: $scope.IssueItem.ProductSizeId
                    }), 'Quantity');
                    $scope.PartyItemInStockSize = $filter('sumByKey')($filter('filter')($scope.PartyBalance, {
                        ProductId: $scope.IssueItem.ProductId,
                        ProductSizeId: $scope.IssueItem.ProductSizeId
                    }), 'ClosingBalance');
                }
            }
        }
    });
});