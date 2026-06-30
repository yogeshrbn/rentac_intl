app.controller('ContractsListController', function ($scope, $state, $rootScope, AuthenticationService,
    $crypto, toaster, LedgerFactory, FileSaver) {
    var CONTRACTS_LIST_FILTER_KEY = 'rentacContractsListFilter_v1';

    function persistContractsListFilter() {
        try {
            sessionStorage.setItem(CONTRACTS_LIST_FILTER_KEY, JSON.stringify({
                LedgerId: $scope.Filter.LedgerId,
                LedgerSiteId: $scope.Filter.LedgerSiteId,
                From: $scope.Filter.From,
                To: $scope.Filter.To,
                StatusId: $scope.Filter.StatusId,
                QuotationNumber: $scope.Filter.QuotationNumber,
                Extended: $scope.Filter.Extended,
                ExpringOn: $scope.Filter.ExpringOn,
                DueBillOn: $scope.Filter.DueBillOn,
                AreaMin: $scope.Filter.AreaMin,
                AreaMax: $scope.Filter.AreaMax,
                ContractValueMin: $scope.Filter.ContractValueMin,
                ContractValueMax: $scope.Filter.ContractValueMax,
                RateMin: $scope.Filter.RateMin,
                RateMax: $scope.Filter.RateMax
            }));
        } catch (ex) { }
    }

    function restoreContractsListFilter() {
        try {
            var raw = sessionStorage.getItem(CONTRACTS_LIST_FILTER_KEY);
            return raw ? JSON.parse(raw) : null;
        } catch (ex) {
            return null;
        }
    }

    var savedContractsFilter = restoreContractsListFilter();

    $scope.Filter = {
        LedgerId: 0, LedgerSiteId: 0, From: '', To: '', StatusId: 0, QuotationNumber: '', Extended: false, ExpringOn: 0, DueBillOn: 0,
        AreaMin: null, AreaMax: null, ContractValueMin: null, ContractValueMax: null, RateMin: null, RateMax: null
    };
    var token = $rootScope.getTokenInfo();
    $scope.installed = 0;
    $scope.lastInstallation = 0;
    $scope.dismantled = 0;
    $scope.latedismantling = 0;

    $scope.Contracts = [];
    if (token) {
        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;
    }

    if (savedContractsFilter) {
        angular.extend($scope.Filter, savedContractsFilter);
    } else {
        var toDate = new Date();
        var fromDate = new Date(toDate);
        fromDate.setMonth(fromDate.getMonth() - 1);
        $scope.Filter.To = convertDate(toDate);
        $scope.Filter.From = convertDate(fromDate);
        if (token && token.FinYearStart) {
            var fromParsed = moment($scope.Filter.From, 'DD/MM/YYYY', true);
            var fyStart = moment(token.FinYearStart);
            if (fromParsed.isValid() && fyStart.isValid() && fromParsed.isBefore(fyStart, 'day')) {
                $scope.Filter.From = convertDate(fyStart.toDate());
            }
        }
    }

    LedgerFactory.GetAllParties(function (e) {
        $scope.Accounts = e.data;
    });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId && !savedContractsFilter) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $scope.getSites();
        if (!$scope.Accounts) {
            return;
        }
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        var ledger = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);


    });
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }
    $scope.FilteredContracts = [];
    $scope.filter = function () {

        var contract = new $.Contract();
        var filterModel = JSON.parse(JSON.stringify($scope.Filter));

        filterModel.From = formatdate(filterModel.From);
        filterModel.To = formatdate(filterModel.To);
        ['AreaMin', 'AreaMax', 'ContractValueMin', 'ContractValueMax', 'RateMin', 'RateMax'].forEach(function (k) {
            if (filterModel[k] === '' || filterModel[k] === undefined)
                filterModel[k] = null;
            else if (filterModel[k] != null && filterModel[k] !== '') {
                var n = parseFloat(filterModel[k]);
                filterModel[k] = isNaN(n) ? null : n;
            }
        });

        contract.Filter(function (e) {
            $scope.FilteredContracts = e.data.Data;
            $scope.Contracts = e.data.Data;
            persistContractsListFilter();
        }, filterModel);
    }
    $scope.filter();
    $scope.edit = function (item) {
        if (item.StatusId != 1 && token.RoleId != 1) {
            alert('Only draft contracts can be edited');
            return;
        }

        persistContractsListFilter();
        var cId = $crypto.encrypt(item.ContractId);
        $state.go('contract', { id: cId });
    }

    $scope.preview = function (item) {
        $('#previewDialog').modal('show');
        $scope.SelectedContract = item;
        var strInput = "contractpreview," + item.ContractId
        var encrypedText = $crypto.encrypt(strInput);

        var econded = btoa(encrypedText);
        var report = new $.Reports();
        $scope.ContractPreview = 1;
        report.previewReport(function (e) {
            $scope.ContractPreview = null;
            $('#rpt').html(e.data);
        }, econded);
    }
    $scope.printPdf = function () {
        var item = $scope.SelectedContract;
        var strInput = "contractpreview," + + item.ContractId;
        var encrypedText = $crypto.encrypt(strInput);

        var econded = btoa(encrypedText);
        var report = new $.Reports();
        report.downloadReport(function (e) {

            FileSaver.saveAs(e.data, 'text.pdf');
        }, econded);

    }
    $scope.openStatusDialog = function (statusId) {

        var item = $scope.SelectedContract;
        if (statusId == 3) {
            if (item.StatusId > 1) {
                alert('Only draft contracts can be deleted');
                return;
            }
            var c = confirm('Are you sure you want to delete this contract');
            if (c) {
                $('#delDialog').modal('show');
            }
            return;
        }

    }
    $scope.deleteContract = function () {

        var ct = new $.Contract();
        var item = cloneObj($scope.SelectedContract);
        item.StatusId = 3;
        var c = confirm('Are you sure you want to delete this contract? Click OK to delete or click cancel to do nothing');
        if (!c) {
            return;
        }
        item.Remarks = item.DeletedRemarks;
        ct.UpdateStatus(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Contract deleted successfully');
            $scope.SelectedContract.StatusId = 3;
            $('#delDialog').modal('hide');
            return;
        }, item);
    }
    $scope.Publish = function () {

        var ct = new $.Contract();
        var item = cloneObj($scope.SelectedContract);
        item.StatusId = 2
        var c = confirm('Are you sure you want to publish this contract? Click OK to publish or click cancel to do nothing');
        if (!c) {
            return;
        }
        ct.UpdateStatus(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Contract published successfully');
            $scope.SelectedContract.StatusId = 2;
            return;
        }, item);
    }
    $scope.CompleteContract = function () {

        var ct = new $.Contract();
        var item = cloneObj($scope.SelectedContract);
        item.StatusId = 5
        var c = confirm('Are you sure you want to complete this contract? Click OK to complete or click cancel to do nothing');
        if (!c) {
            return;
        }
        ct.UpdateStatus(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Contract completed successfully');
            $scope.SelectedContract.StatusId = 5;
            return;
        }, item);
    }

    $scope.billContract = function () {
        //if ($scope.SelectedContract.StatusId != 2) {
        //    alert('Only published contracts can be billed');
        //    return;
        //}

        persistContractsListFilter();
        var cId = $crypto.encrypt($scope.SelectedContract.ContractId);
        $('#previewDialog').modal('hide');
        window.setTimeout(() => {
            $state.go('contractbill', { cId: cId });
        }, 600);

    }

    $scope.info = function (item) {
        persistContractsListFilter();
        var key = $crypto.encrypt(item.ContractId);
        $state.go('contractInfo', { cId: key });
    }

    $scope.applyByStatus = function (statusName) {
        if ($scope.Contracts) {
            $scope.FilteredContracts = $scope.Contracts.filter(o => o.StatusName && o.StatusName.toLowerCase() == statusName);
        }
        else
            $scope.FilteredContracts = [];
        var $tile = $(event.currentTarget).closest('.metrics-tile');
        $tile.closest('.row').find('.metrics-tile').removeClass('selected-tile');
        $tile.addClass('selected-tile');
    }
    $scope.applyFilter = function () {
        $('#advFilterDialog').modal('hide');
        $scope.filter();
    }

    $scope.clearFilter = function () {
        $scope.Filter.QuotationNumber = '';
        $scope.Filter.Extended = false;
        $scope.Filter.ExpringOn = 0;
        $scope.Filter.DueBillOn = 0;
        $scope.Filter.AreaMin = null;
        $scope.Filter.AreaMax = null;
        $scope.Filter.ContractValueMin = null;
        $scope.Filter.ContractValueMax = null;
        $scope.Filter.RateMin = null;
        $scope.Filter.RateMax = null;
        $scope.filter();
    }

});
app.controller('AddEditContractsController', function ($scope, $stateParams, LedgerFactory,
    AuthenticationService, $rootScope, $crypto, $state, EmployeeService, FileSaver) {

    $scope.TAXES = StaicData.TAX_CATEGORY;
    $scope.activeOnly = { Quantity: 0 };
    $scope.STORAGE_LOC = STORAGE_LOC;
    $scope.editorOptions = {
        height: 200
    };
    var zone = new $.Zone();
    var cId = $stateParams.id == undefined ? 0 : $stateParams.id;
    var qId = $stateParams.qId == undefined ? 0 : $stateParams.qId;

    var contract = new $.Contract({
        PurchaseId: 0, MeasureType: 'SQFT', ZoneId: 0, LocalityId: 0, SizeDescription: '', QuotationId: 0,
        Height: 0, Width: 0, ApproximateWeight: 0, PONumber: '', PODate: null
    });
    $scope.Contract = contract;
    $scope.Contract.Conditions = [];
    $scope.NewCondition = { ContractConditionId: 0, ContractId: 0, Condition: '' };


    $scope.getQuotationById = function () {

        var txn = new $.Transaction();
        txn.GetQutotationById(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            var qData = e.data.Data;
            if (qData) {
                $scope.Contract.LedgerId = qData.LedgerId;
                $scope.Contract.LedgerSiteId = qData.LedgerSiteId;
                $scope.Contract.ContractValue = qData.Total;
                if (qData.Area)
                    $scope.Contract.Area = qData.Area;
                if (qData.MeasureType)
                    $scope.Contract.MeasureType = qData.MeasureType;
                if (qData.From)
                    $scope.Contract.EffectiveFrom = convertDate(qData.From);
                if (qData.To)
                    $scope.Contract.ValidTill = convertDate(qData.To);
                syncContractSelectedLedger();
            }

        }, $scope.Contract.QuotationId);

    }
    if (qId != '0' && qId != '') {
        $scope.Contract.QuotationId = $crypto.decrypt(qId);

        $scope.getQuotationById();

        $scope.Contract.ContractType = 1
    }
    contract.MeasureType = 1;
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    $scope.SelectedLedger = null;

    function syncContractSelectedLedger() {
        var lid = parseInt($scope.Contract.LedgerId, 10) || 0;
        if (lid <= 0) {
            $scope.SelectedLedger = null;
            return;
        }
        if (!$scope.Accounts) {
            $scope.SelectedLedger = null;
            return;
        }
        var ledger = $scope.Accounts.find(function (o) { return o.LedgerId == lid; });
        if (ledger) {
            ledgerDTO.Props.StateCode = ledger.StateCode;
            $scope.SelectedLedger = ledger;
        } else {
            $scope.SelectedLedger = null;
        }
    }

    var loginData = AuthenticationService.getTokenInfo();
    getAllEmployees();
    $scope.zoneList = function () {
        zone.zoneList(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Zones = e.data.Data;
        }, {});
    }
    $scope.zoneList();

    $scope.$watch('Contract.ZoneId', function (x, y) {

        if ($scope.Contract.ZoneId == 0 || !$scope.Contract.ZoneId) {

            return;
        }
        $scope.Localities = [];
        zone.zoneById(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            if (e.data.Data) {
                $scope.Localities = e.data.Data.Localities;
            }
        }, { ZoneId: $scope.Contract.ZoneId });
    });
    $scope.onZoneChange = function () {


    }

    function getAllEmployees() {
        EmployeeService.GetAllEmployees(function (e) {
            $scope.Employees = e.data.Data;
        });
    }

    $scope.getTeamsList = function () {
        var employee = new $.Employee();
        employee.TeamList(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.TeamList = e.data.Data;
        });
    }
    $scope.getTeamsList();
    LedgerFactory.GetAllParties(function (e) {

        $scope.Accounts = e.data;
        syncContractSelectedLedger();
        if (cId != 0) {
            $scope.Contract.ContractId = $crypto.decrypt(cId);
            if ($scope.Contract.ContractId > 0) {
                $scope.GetDetails();
            }
        }
    });

    $scope.Doc1 = ''; $scope.Doc2 = ''; $scope.Doc3 = '';
    $scope.GetDetails = function () {
        var contract = new $.Contract();
        contract.GetById(function (e) {
            if (e.data.Data) {
                $scope.Contract = e.data.Data;
                $scope.Contract.EffectiveFrom = convertDate($scope.Contract.EffectiveFrom);
                $scope.Contract.ValidTill = convertDate($scope.Contract.ValidTill);
                if ($scope.Contract.PODate) {
                    $scope.Contract.PODate = convertDate($scope.Contract.PODate);
                }

                if ($scope.Contract.Conditions == null || $scope.Contract.Conditions.length == 0) {
                    $scope.Contract.Conditions = [];
                    $scope.Contract.Conditions.push(JSON.parse(JSON.stringify($scope.NewCondition)));

                    if ($scope.Contract.Doc1 != '') {
                        $scope.Doc1 = $scope.STORAGE_LOC + $scope.Contract.Doc1;
                        // $('#imgDoc1').attr('src', _loc1);
                    }
                    if ($scope.Contract.Doc2 != '') {
                        $scope.Doc2 = $scope.STORAGE_LOC + $scope.Contract.Doc2;
                        // $('#imgDoc1').attr('src', _loc1);
                    }
                    if ($scope.Contract.Doc3 != '') {
                        $scope.Doc3 = $scope.STORAGE_LOC + $scope.Contract.Doc3;
                        // $('#imgDoc1').attr('src', _loc1);
                    }
                }
                syncContractSelectedLedger();
            }
        }, { ContractId: $scope.Contract.ContractId });
    }
    $scope.editQuotation = function () {
        var key = $crypto.encrypt($scope.Contract.QuotationId);


        $state.go('editquotation', { key: key });

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetSizeListByCompany(function (e) {

            $scope.AllSizes = e.data;

        });
    }

    $scope.getAllProductSizesByCompany();



    $scope.Contract.Conditions.push(JSON.parse(JSON.stringify($scope.NewCondition)));
    $scope.Contract.Details = [];//initializeArray();
    init();
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            //$scope.Contract.Details.splice(index - 1, 1);
            $scope.Contract.Details[index - 1].Deleted = 1;
        });
    }
    $scope.DeleteCondition = function (cond) {

        cond.Deleted = 1;
        var activeConditins = $scope.Contract.Conditions.filter(o => o.Deleted != 1);
        if (activeConditins.length == 0) {
            $scope.Contract.Conditions.push(JSON.parse(JSON.stringify($scope.NewCondition)));
        }
    }


    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Contract.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Contract.LedgerId', function () {


        $scope.getSites();
        $rootScope.LedgerId = $scope.Contract.LedgerId;
        syncContractSelectedLedger();

    });


    $scope.onRowKeyDown = function ($event, item) {
        if ($event.keyCode == 13) {
            var fullRows = $scope.Contract.Conditions.filter(o => o.Condition.length >= 3).length;
            var totalRows = $scope.Contract.Conditions.length;
            if (fullRows == totalRows) {
                var cs = JSON.parse(JSON.stringify($scope.NewCondition));
                $scope.Contract.Conditions.push(cs);
            }
        }
    }

    //  $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    //GetTaxes();


    $scope.Find = function () {

    }

    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        console.log(selected);
    };

    $scope.RowSelected = function (index) {
        if (!$scope.Contract.Details) {
            return;
        }
        if ($scope.Contract.Details[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Contract.AppliedTaxes = [];
    $scope.Contract.FreightTax = 0;

    var modal

    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.ContractItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //   $scope.Contract = new $.LedgerTrasaction({});
        $scope.Contract.EntryType = $scope.TranType == 1 ? 8 : 9;
        //$scope.Contract.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Contract.Details.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmContract').valid();
        var model = cloneObj($scope.Contract);
        model.Doc1 = $scope.Doc1;
        model.Doc2 = $scope.Doc2;
        model.Doc3 = $scope.Doc3;

        if (!m) {
            // alert('Please provide all values');
            return;
        }



        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        //if (res.length < 1) {
        //    alert('Please add items to save.');
        //    return;
        //}

        model.EffectiveFrom = formatdate(model.EffectiveFrom);
        model.ValidTill = formatdate(model.ValidTill);
        model.PODate = model.PODate ? formatdate(model.PODate) : null;

        if (model.ContractType == 1 && (parseFloat(model.ContractValue) == 0 || !model.ContractValue)) {
            alert('Please enter contract value');
            return;
        }
        if (model.ContractType == 2 && (parseFloat(model.Rate) == 0 || !model.Rate)) {
            alert('Please enter contract rate');
            return;
        }


        contract.Save(function (e) {

            if (e.data.Code == 200) {

                alert('saved');
                $scope.warnOnLeave = false;
                $state.go('contracts');
            } else {
                showMessage(e.data.Message);
            }

        }, model);

    };

    //add new item to be issued
    $scope.addItem = function () {

        if (!$scope.ContractItem.ProductId || $scope.ContractItem.ProductId <= 0 || $scope.ContractItem.Quantity == 0) {
            alert("Please provide all details.");
            return;
        }


        var itemExist = $scope.Contract.Details.find(o => o.ProductId == $scope.ContractItem.ProductId);
        if (itemExist) {
            if (itemExist.Deleted && itemExist.Deleted == 1) {
                itemExist.Quantity = parseInt($scope.ContractItem.Quantity);
                itemExist.Deleted = 0;
            } else
                itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.ContractItem.Quantity);


        } else {
            $scope.ContractItem.SubTotal = $scope.ContractItem.Quantity * $scope.ContractItem.Rate;

            $scope.Contract.Details.push($scope.ContractItem);
        }



        $scope.ContractItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }




    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.ContractItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.ContractItem.Product = selected.originalObject.Product;

        }
    };
    $scope.DefaultRate = 0.0;


    FormsValidation.init('frmContract');

    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Contract.LedgerId });
    }

    $scope.browseFile = function (item) {

        $('#' + item).click();
        $('#' + item).bind('change', function (e) {

            //  var imgItem = $('#' + image)[0];
            $scope.onFileBrowse(e, function (res) {
                if (item == 'doc1') {
                    $scope.Doc1 = res;
                    $scope.Contract.Doc1 = res;
                }
                if (item == 'doc2') {
                    $scope.Doc2 = res;
                    $scope.Contract.Doc2 = res;
                }
                if (item == 'doc3') {
                    $scope.Doc3 = res;
                    $scope.Contract.Doc3 = res;
                }
                $scope.$apply();
            });


        });
    }

    $scope.onFileBrowse = function (event, cb) {
        return readURL(event.currentTarget, cb);
    };

    function readURL(input, cb) {
        var file = input.files[0];
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                cb(e.target.result);
            }
            var dataUrl = reader.readAsDataURL(input.files[0]);
            return dataUrl;
        }
    }
    $scope.downloadFile = function (doc) {
        downloadFile(doc, FileSaver);
    }
});

app.controller('BillContractsController', function ($scope, $stateParams, LedgerFactory, AuthenticationService, $rootScope, $crypto, toaster) {
    $scope.TAXES = StaicData.TAX_CATEGORY;
    $scope.AppliedTax = null;
    var cId = $stateParams.id == undefined ? 0 : $stateParams.id;
    var contract = new $.Contract();
    contract.MeasureType = 1;
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });

    var loginData = AuthenticationService.getTokenInfo();
    contract.ContractId = $crypto.decrypt(cId);
    $scope.Contract = contract;

    var contract = new $.Contract();
    $scope.GetDetails = function () {
        contract.GetById(function (e) {
            if (e.data.Data) {
                $scope.Contract = e.data.Data;
                $scope.Contract.EffectiveFrom = convertDate($scope.Contract.EffectiveFrom);
                $scope.GetSiteInfo();
            }
        }, { ContractId: $scope.Contract.ContractId });
    }
    if ($scope.Contract.ContractId > 0)
        $scope.GetDetails();

    $scope.GetSiteInfo = function () {
        LedgerFactory.GetMasterSiteById(function (e) {
            $scope.SiteInfo = e.data.Data;

            if ($scope.Contract.TaxCategoryId >= 0) {
                $scope.AppliedTax = $scope.TAXES.find(o => o.TaxId == $scope.Contract.TaxCategoryId);
                $scope.Contract.IGSTRate = $scope.AppliedTax.IGST;
                $scope.Contract.SGSTRate = $scope.AppliedTax.SGST;
                $scope.Contract.CGSTRate = $scope.AppliedTax.CGST;

                if ($scope.Contract.Company.StateId != $scope.SiteInfo.StateId) {
                    $scope.Contract.SGSTRate = 0;
                    $scope.Contract.CGSTRate = 0;

                }
                else {
                    $scope.Contract.IGSTRate = 0;
                }

            }
        }, { LedgerSiteId: $scope.Contract.LedgerSiteId });
    }

    FormsValidation.init('frmContractBill');
    $scope.$watch('Contract.SubTotal', function () {
        if ($scope.AppliedTax) {
            $scope.Contract.IGST = parseFloat($scope.Contract.IGSTRate * $scope.Contract.SubTotal / 100);
            $scope.Contract.CGST = parseFloat($scope.Contract.CGSTRate * $scope.Contract.SubTotal / 100);
            $scope.Contract.SGST = parseFloat($scope.Contract.SGSTRate * $scope.Contract.SubTotal / 100);
            $scope.Contract.Total = parseFloat($scope.Contract.SubTotal) + $scope.Contract.IGST + $scope.Contract.CGST + $scope.Contract.SGST;
        }

    });

    $scope.generateBill = function () {
        var m = $('#frmContractBill').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }
        var model = cloneObj($scope.Contract);

        contract.GenerateBill(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
            } else {
                alert('Bill Saved');
                $state.go('billList');
            }
        }, model);
    };

});
app.controller('EditBillContractsController', function ($scope, $stateParams, LedgerFactory, AuthenticationService, $state, $crypto, toaster) {
    $scope.TAXES = StaicData.TAX_CATEGORY;
    $scope.AppliedTax = null;
    if ($stateParams.id == undefined) {
        alert('Invalid key');
        $state.go('billList');
        return;
    }

    var cId = $crypto.decrypt($stateParams.id).split(',');

    var contract = new $.Contract();
    contract.MeasureType = 1;
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });

    var loginData = AuthenticationService.getTokenInfo();
    //   contract.InvoiceId = cId[0];
    contract.ContractId = cId[1];

    $scope.Contract = contract;


    $scope.GetDetails = function () {

        contract.GetById(function (e) {

            if (e.data.Data) {
                $scope.Contract = e.data.Data;
                $scope.Contract.InvoiceId = cId[0];
                $scope.Contract.EffectiveFrom = convertDate($scope.Contract.EffectiveFrom);
                $scope.getBillById();
                $scope.GetSiteInfo();
            }
        }, { ContractId: $scope.Contract.ContractId });
    }
    if ($scope.Contract.ContractId > 0)
        $scope.GetDetails();

    $scope.GetSiteInfo = function () {
        LedgerFactory.GetMasterSiteById(function (e) {
            $scope.SiteInfo = e.data.Data;

            if ($scope.Contract.TaxCategoryId >= 0) {
                $scope.AppliedTax = $scope.TAXES.find(o => o.TaxId == $scope.Contract.TaxCategoryId);
                $scope.Contract.IGSTRate = $scope.AppliedTax.IGST;
                $scope.Contract.SGSTRate = $scope.AppliedTax.SGST;
                $scope.Contract.CGSTRate = $scope.AppliedTax.CGST;

                if ($scope.Contract.Company.StateId != $scope.SiteInfo.StateId) {
                    $scope.Contract.SGSTRate = 0;
                    $scope.Contract.CGSTRate = 0;

                }
                else {
                    $scope.Contract.IGSTRate = 0;
                }

            }
        }, { LedgerSiteId: $scope.Contract.LedgerSiteId });
    }

    FormsValidation.init('frmContractBill');
    $scope.$watch('Contract.SubTotal', function () {
        if ($scope.AppliedTax) {
            $scope.Contract.IGST = parseFloat($scope.Contract.IGSTRate * $scope.Contract.SubTotal / 100);
            $scope.Contract.CGST = parseFloat($scope.Contract.CGSTRate * $scope.Contract.SubTotal / 100);
            $scope.Contract.SGST = parseFloat($scope.Contract.SGSTRate * $scope.Contract.SubTotal / 100);
            $scope.Contract.Total = parseFloat($scope.Contract.SubTotal) + $scope.Contract.IGST + $scope.Contract.CGST + $scope.Contract.SGST;
        }

    });


    $scope.getBillById = function () {
        var billing = new $.Billing();
        billing.ById(function (e) {
            if (e.data.Code != 200) {
                alert('Could not load invoice' + e.data.Message);
                return;
            }
            var billData = e.data.Data;
            billData.Items = e.data.Data.BillableItems
            billData.InvoiceDate = convertDate(e.data.Data.InvoiceDate);
            $scope.Contract.SubTotal = billData.SubTotal;
            $scope.Contract.Remarks = billData.Remarks;
        }, { InvoiceId: $scope.Contract.InvoiceId });
    }


    $scope.generateBill = function () {
        var m = $('#frmContractBill').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }
        var model = cloneObj($scope.Contract);

        contract.GenerateBill(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
            } else {
                alert('Bill Saved');
                $state.go('billList');
            }
        }, model);
    };

});

app.controller('ContractInfoController', function ($scope, $stateParams, LedgerFactory, FileSaver, $filter, $crypto, $state,
    EmployeeService, $window, FileSaver, ModalFactory, $timeout, $q, ReportService) {

    $scope.activeOnly = { Quantity: 0 };
    $scope._currentDate = new Date();
    var cId = $stateParams.cId == undefined ? 0 : $stateParams.cId;
    var contract = new $.Contract({
        PurchaseId: 0, MeasureType: 'SQFT'
    });
    contract.MeasureType = 1;
    //var ledgerDTO = new $.Ledger({ LedgerId: 0 });

    //var loginData = AuthenticationService.getTokenInfo();

    $scope.Contract = contract;
    $scope.Contract.Conditions = [];
    $scope.NewCondition = { ContractConditionId: 0, ContractId: 0, Condition: '' };
    $scope.ActivityTypes = StaicData.CONTRACT_ACTIVITY_TYPES;

    EmployeeService.GetAllEmployees(function (e) {

        if (e.data.Code == 200) {
            $scope.Employees = e.data.Data;
        }
        else {
            alert(e.data.Message);
        }
    });

    $scope.ContractInventory = [];
    $scope.Bills = [];
    $scope.BilledAmount = 0;
    $scope.ContractReceipts = [];
    $scope.paymentReceivedTotal = 0;
    $scope.amountReceivable = 0;
    $scope.quickReceiptModalOpen = false;
    $scope.measureBillModalOpen = false;
    $scope.measureBillSessionKey = 0;
    $scope.billQuotationIds = [];
    $scope.billQuotationSummary = '';
    $scope.billQuotationPickerOpen = false;
    $scope.billQuotationPickerRows = [];
    $scope.billQuotationPickerSelectAll = true;
    $scope.deliveryChallanModalOpen = false;
    $scope.returnChallanModalOpen = false;
    $scope.editQuotationModalOpen = false;
    $scope.newQuotationModalOpen = false;
    $scope.editQuotationTargetId = null;
    $scope.SelectedQuotationForPreview = null;
    $scope.Preview = null;

    $scope.summaryByJob = [];
    $scope.summaryLoading = false;
    $scope.summaryBillsVsPaymentsBalance = 0;

    $scope.ContractDocuments = [];
    $scope.ContractHeaderDocuments = [];
    $scope.contractDocumentsLoading = false;
    $scope.STORAGE_LOC = typeof STORAGE_LOC !== 'undefined' ? STORAGE_LOC : '';

    $scope.contractDocUrl = function (path) {
        if (!path) return '#';
        var base = $scope.STORAGE_LOC || '';
        if (!base) return path;
        if (base.charAt(base.length - 1) !== '/' && path.charAt(0) !== '/')
            return base + '/' + path;
        return base + path;
    };

    $scope.buildContractHeaderDocuments = function () {
        var c = $scope.Contract || {};
        var slots = [
            { label: 'Document 1', path: c.Doc1 },
            { label: 'Document 2', path: c.Doc2 },
            { label: 'Document 3', path: c.Doc3 }
        ];
        $scope.ContractHeaderDocuments = slots
            .filter(function (s) { return s.path && String(s.path).trim() !== ''; })
            .map(function (s) {
                var path = String(s.path).trim();
                return {
                    label: s.label,
                    path: path,
                    fileName: path.split('/').pop() || s.label,
                    url: $scope.contractDocUrl(path)
                };
            });
    };

    $scope.downloadContractDoc = function (doc) {
        if (!doc || !doc.url) return;
        downloadFile(doc.url, FileSaver);
    };

    $scope.documentFileUrl = function (doc) {
        if (!doc || !doc.StoragePath) return '#';
        var base = $scope.STORAGE_LOC || '';
        var path = doc.StoragePath;
        if (!base) return path;
        if (base.charAt(base.length - 1) !== '/' && path.charAt(0) !== '/')
            return base + '/' + path;
        return base + path;
    };

    $scope.loadContractDocuments = function () {
        if (!$scope.Contract || !$scope.Contract.ContractId) {
            $scope.ContractDocuments = [];
            $scope.ContractHeaderDocuments = [];
            return;
        }
        $scope.buildContractHeaderDocuments();
        $scope.contractDocumentsLoading = true;
        var c = new $.Contract();
        c.GetContractDocuments(function (e) {
            $scope.$applyAsync(function () {
                $scope.contractDocumentsLoading = false;
                if (e.data && e.data.Code == 200 && e.data.Data) {
                    $scope.ContractDocuments = e.data.Data;
                } else {
                    $scope.ContractDocuments = [];
                    if (e.data && e.data.Code != 200 && e.data.Message) {
                        console.warn(e.data.Message);
                    }
                }
            });
        }, { ContractId: $scope.Contract.ContractId });
    };

    $scope.summaryActivityLabel = function (job) {
        if (!job) return '';
        var t = job.TypeId == 1 ? 'Install' : job.TypeId == 2 ? 'Dismantle' : job.TypeId == 3 ? 'Others' : 'Activity';
        return t + (job.JobCardId ? ' #' + job.JobCardId : '');
    };

    $scope.recomputeSummaryBillPaymentBalance = function () {
        var b = parseFloat($scope.BilledAmount);
        var p = parseFloat($scope.paymentReceivedTotal);
        $scope.summaryBillsVsPaymentsBalance = (isNaN(b) ? 0 : b) - (isNaN(p) ? 0 : p);
    };

    $scope.loadContractSummary = function () {
        if (!$scope.Contract || !$scope.Contract.ContractId) {
            $scope.summaryByJob = [];
            return;
        }
        $scope.summaryLoading = true;

        var pBills = $q(function (resolve) {
            contract.GetBills(function (e) {
                if (e.data && e.data.Data) {
                    $scope.ContractBills = e.data.Data;
                    $scope.BilledAmount = $filter('sumByKey')($scope.ContractBills, 'Total');
                } else {
                    $scope.ContractBills = $scope.ContractBills || [];
                }
                resolve();
            }, { ContractId: $scope.Contract.ContractId });
        });

        var pLedger = $q(function (resolve) {
            var txn = new $.LedgerTrasaction({});
            txn.GetContractReceiptPayments(function (e) {
                $scope.ContractReceipts = e.data || [];
                $scope.paymentReceivedTotal = $filter('sumByKey')($scope.ContractReceipts, 'TransactionAmount');
                var cv = parseFloat($scope.Contract.ContractValue);
                var pr = parseFloat($scope.paymentReceivedTotal);
                $scope.amountReceivable = (isNaN(cv) ? 0 : cv) - (isNaN(pr) ? 0 : pr);
                resolve();
            }, { ContractId: $scope.Contract.ContractId });
        });

        var jobs = $scope.Contract.JobCards || [];
        //var jobPromises = [];
        //for (var ji = 0; ji < jobs.length; ji++) {
        //    (function (job) {
        //        jobPromises.push($q(function (resolve) {
        //            var jcDel = new $.JobCard();
        //            jcDel.JobCardDelChallanItems(function (e) {
        //                var delData = (e.data && e.data.Data) ? e.data.Data : [];
        //                var jcRet = new $.JobCard();
        //                jcRet.JobCardRetChallanItems(function (e2) {
        //                    var retData = (e2.data && e2.data.Data) ? e2.data.Data : [];
        //                    resolve({ job: job, delData: delData, retData: retData });
        //                }, { JobCardId: job.JobCardId });
        //            }, { JobCardId: job.JobCardId });
        //        }));
        //    })(jobs[ji]);
        //}

        var contractPromisses = [];
        var c = new $.Contract();
        contractPromisses.push(c.ContractDelChallanItems(null, { ContractId: $scope.Contract.ContractId }, true));
        contractPromisses.push(c.ContractRetChallanItems(null, { ContractId: $scope.Contract.ContractId }, true));


        $q.all([pBills, pLedger].concat(contractPromisses)).then(function (results) {
            debugger
            var jobResults = results.slice(2);
            var delData = jobResults[0].data.Data || [];
            var retData = jobResults[1].data.Data || [];
            var deliveryChallans = uniqueObjects($.map(delData, function (obj) {
                var items = delData.filter(function (o) { return o.WorkOrderId == obj.WorkOrderId; });
                return {
                    WorkOrderId: obj.WorkOrderId, ChallanNumber: obj.ChallanNumber, SentDate: obj.SentDate, Items: items,
                    SiteId: obj.SiteId
                };
            }), 'ChallanNumber');
            var returnChallans = uniqueObjects($.map(retData, function (obj) {
                var items = retData.filter(function (o) { return o.GRNId == obj.GRNId; });
                return { GRNId: obj.GRNId, ChallanNumber: obj.ChallanNumber, ChallanDate: obj.ChallanDate, Items: items };
            }), 'ChallanNumber');
            $scope.summaryByJob = {
                deliveryChallans: deliveryChallans,
                returnChallans: returnChallans

            };

            $scope.recomputeSummaryBillPaymentBalance();
            $scope.summaryLoading = false;
        }, function () {
            $scope.summaryByJob = [];
            $scope.summaryLoading = false;
            $scope.recomputeSummaryBillPaymentBalance();
        });
    };

    $scope.loadContractLedger = function () {
        if (!$scope.Contract || !$scope.Contract.ContractId) {
            $scope.ContractReceipts = [];
            $scope.paymentReceivedTotal = 0;
            $scope.amountReceivable = 0;
            return;
        }
        var txn = new $.LedgerTrasaction({});
        txn.GetContractReceiptPayments(function (e) {
            $scope.ContractReceipts = e.data || [];
            $scope.paymentReceivedTotal = $filter('sumByKey')($scope.ContractReceipts, 'TransactionAmount');
            var cv = parseFloat($scope.Contract.ContractValue);
            var pr = parseFloat($scope.paymentReceivedTotal);
            $scope.amountReceivable = (isNaN(cv) ? 0 : cv) - (isNaN(pr) ? 0 : pr);
            if ($scope.recomputeSummaryBillPaymentBalance) {
                $scope.recomputeSummaryBillPaymentBalance();
            }
        }, { ContractId: $scope.Contract.ContractId });
    };

    $scope.openReceivePaymentModal = function () {
        if (!$scope.Contract || !$scope.Contract.ContractId) {
            return;
        }
        if (!$scope.Contract.LedgerId) {
            alert('This contract has no party linked. Set the party on the contract before receiving payment.');
            return;
        }
        $scope.quickReceiptModalOpen = true;
        $timeout(function () {
            $('#contractQuickReceiptModal').modal('show');
            if (typeof FormsValidation !== 'undefined' && FormsValidation.init) {
                FormsValidation.init('form-receipt');
            }
        }, 50);
    };

    $timeout(function () {
        $('#contractQuickReceiptModal').on('hidden.bs.modal', function () {
            $scope.$applyAsync(function () {
                $scope.quickReceiptModalOpen = false;
            });
        });
        $('#contractMeasureBillModal').on('hidden.bs.modal', function () {
            $scope.$applyAsync(function () {
                $scope.measureBillModalOpen = false;
                $scope.billQuotationIds = [];
                $scope.billQuotationSummary = '';
                if ($scope.refreshContractInfoLists) {
                    $scope.refreshContractInfoLists();
                }
                if ($scope.GetDetails) {
                    $scope.GetDetails();
                }
            });
        });
        $('#contractBillQuotationPickerModal').on('hidden.bs.modal', function () {
            $scope.$applyAsync(function () {
                $scope.billQuotationPickerOpen = false;
                $scope.billQuotationPickerRows = [];
            });
        });
        $('#contractDeliveryChallanModal').on('hidden.bs.modal', function () {
            $scope.$applyAsync(function () {
                $scope.deliveryChallanModalOpen = false;
                if ($scope.refreshContractInfoLists) {
                    $scope.refreshContractInfoLists();
                }
            });
        });
        $('#contractReturnChallanModal').on('hidden.bs.modal', function () {
            $scope.$applyAsync(function () {
                $scope.returnChallanModalOpen = false;
                if ($scope.refreshContractInfoLists) {
                    $scope.refreshContractInfoLists();
                }
            });
        });
        $('#contractEditQuotationModal').on('hidden.bs.modal', function () {
            $scope.$applyAsync(function () {
                $scope.editQuotationModalOpen = false;
                $scope.editQuotationTargetId = null;
                if ($scope.refreshContractInfoLists) {
                    $scope.refreshContractInfoLists();
                }
                if ($scope.GetDetails) {
                    $scope.GetDetails();
                }
            });
        });
        $('#contractNewQuotationModal').on('hidden.bs.modal', function () {
            $scope.$applyAsync(function () {
                $scope.newQuotationModalOpen = false;
                if ($scope.refreshContractInfoLists) {
                    $scope.refreshContractInfoLists();
                }
                if ($scope.GetDetails) {
                    $scope.GetDetails();
                }
            });
        });
    }, 0);

    $scope.GetDetails = function () {
        var contract = new $.Contract();
        contract.GetById(function (e) {

            if (e.data.Data) {

                $scope.Contract = e.data.Data;
                var cq = $scope.Contract.ContractQuotations || $scope.Contract.contractQuotations;
                $scope.Contract.ContractQuotations = angular.isArray(cq) ? cq : [];
                $scope.ContractType = StaicData.CONTRACT_TYPES.find(o => o.Id == $scope.Contract.ContractType);
                $scope.ContractMeasureType = StaicData.CONTRACT_MEASUREMENTS.find(o => o.Id == $scope.Contract.MeasureType);

                // $scope.Contract.EffectiveFrom = convertDate($scope.Contract.EffectiveFrom);
                if ($scope.Contract.Conditions == null || $scope.Contract.Conditions.length == 0) {
                    $scope.Contract.Conditions = [];
                    $scope.Contract.Conditions.push(JSON.parse(JSON.stringify($scope.NewCondition)));
                }
                $scope.buildContractHeaderDocuments();
                $scope.loadContractSummary();
            }
        }, { ContractId: $scope.Contract.ContractId });




    }
    function getBillableContractQuotations() {
        var list = [];
        if ($scope.Contract) {
            list = $scope.Contract.ContractQuotations || $scope.Contract.contractQuotations || [];
        }
        if (!angular.isArray(list)) {
            list = [];
        }
        if (!list.length && $scope.Contract && $scope.Contract.QuotationId > 0) {
            list = [{
                QuotationId: $scope.Contract.QuotationId,
                QuotationNumber: $scope.Contract.QuotationNumber || ('Q' + $scope.Contract.QuotationId),
                InvoiceId: 0
            }];
        }
        return list;
    }

    $scope.isQuotationAlreadyBilled = function (q) {
        return q && parseInt(q.InvoiceId, 10) > 0;
    };

    function openContractMeasureBillModal() {
        $scope.measureBillSessionKey = ($scope.measureBillSessionKey || 0) + 1;
        $scope.measureBillModalOpen = true;
        $timeout(function () {
            $('#contractMeasureBillModal').modal('show');
            if (typeof FormsValidation !== 'undefined' && FormsValidation.init) {
                FormsValidation.init('frmPurchase');
            }
        }, 50);
    }

    function initBillQuotationPickerRows() {
        var quotes = getBillableContractQuotations();
        $scope.billQuotationPickerRows = [];
        var unbilledCount = 0;
        for (var i = 0; i < quotes.length; i++) {
            var q = quotes[i];
            var billed = $scope.isQuotationAlreadyBilled(q);
            if (!billed) {
                unbilledCount++;
            }
            $scope.billQuotationPickerRows.push({
                quotation: q,
                selected: !billed
            });
        }
        $scope.billQuotationPickerSelectAll = unbilledCount > 0;
    }

    $scope.toggleBillQuotationPickerAll = function () {
        var checked = !!$scope.billQuotationPickerSelectAll;
        angular.forEach($scope.billQuotationPickerRows, function (row) {
            if (!$scope.isQuotationAlreadyBilled(row.quotation)) {
                row.selected = checked;
            }
        });
    };

    $scope.onBillQuotationRowToggle = function () {
        var rows = $scope.billQuotationPickerRows || [];
        var selectable = rows.filter(function (r) { return !$scope.isQuotationAlreadyBilled(r.quotation); });
        if (!selectable.length) {
            $scope.billQuotationPickerSelectAll = false;
            return;
        }
        $scope.billQuotationPickerSelectAll = selectable.every(function (r) { return r.selected; });
    };

    $scope.confirmBillQuotationPicker = function () {
        var selected = [];
        var labels = [];
        angular.forEach($scope.billQuotationPickerRows || [], function (row) {
            if (row.selected && row.quotation && row.quotation.QuotationId > 0) {
                selected.push(row.quotation.QuotationId);
                labels.push(row.quotation.QuotationNumber || ('Q' + row.quotation.QuotationId));
            }
        });
        if (!selected.length) {
            alert('Select at least one quotation that is not already billed.');
            return;
        }
        $scope.billQuotationIds = selected;
        $scope.billQuotationSummary = labels.join(', ');
        $('#contractBillQuotationPickerModal').modal('hide');
        openContractMeasureBillModal();
    };

    $scope.billContract = function () {
        if (!$scope.Contract || !$scope.Contract.ContractId) {
            return;
        }
        var quotes = getBillableContractQuotations();
        if (!quotes.length) {
            $scope.billQuotationIds = [];
            $scope.billQuotationSummary = '';
            openContractMeasureBillModal();
            return;
        }
        initBillQuotationPickerRows();
        $scope.billQuotationPickerOpen = true;
        $timeout(function () {
            $('#contractBillQuotationPickerModal').modal('show');
        }, 50);
    }

    $scope.editQuotation = function (quotationId) {
        var qid = quotationId != null && quotationId > 0 ? quotationId : ($scope.Contract && $scope.Contract.QuotationId);
        if (!$scope.Contract || !qid) {
            return;
        }
        $scope.editQuotationTargetId = qid;
        $scope.editQuotationModalOpen = true;
        $timeout(function () {
            $('#contractEditQuotationModal').modal('show');
            if (typeof FormsValidation !== 'undefined' && FormsValidation.init) {
                FormsValidation.init('frmQuotation');
            }
        }, 50);
    };

    $scope.openNewContractQuotation = function () {
        if (!$scope.Contract || !$scope.Contract.ContractId) {
            return;
        }
        $scope.newQuotationModalOpen = true;
        $timeout(function () {
            $('#contractNewQuotationModal').modal('show');
            if (typeof FormsValidation !== 'undefined' && FormsValidation.init) {
                FormsValidation.init('frmQuotation');
            }
        }, 50);
    };

    $scope.previewContractQuotation = function (item) {
        if (!item || !item.QuotationId) {
            return;
        }
        $scope.SelectedQuotationForPreview = item;
        $scope.Preview = 1;
        $('#contractQuotationPreviewDialog').modal('show');
        var strInput = 'quotation,' + item.QuotationId;
        var encrypedText = $crypto.encrypt(strInput);
        ReportService.loadPreviewFromReportServer(function (e) {
            $scope.$applyAsync(function () {
                $scope.Preview = null;
                $('#contractQuotationRpt').html(e.data);
            });
        }, encrypedText);
    };

    $scope.printContractQuotationPdf = function (item) {
        var row = item || $scope.SelectedQuotationForPreview;
        if (!row || !row.QuotationId) {
            return;
        }
        var strInput = 'quotation,' + row.QuotationId;
        var encrypedText = $crypto.encrypt(strInput);
        var client = (row.Client || 'Quotation').replace(/[\\/:*?"<>|]/g, '-');
        var fileName = client + '-' + (row.QuotationNumber || row.QuotationId) + '.pdf';
        ReportService.printFromReportServer(encrypedText, fileName);
    };

    $scope.editContractQuotationFromPreview = function () {
        var row = $scope.SelectedQuotationForPreview;
        if (!row || !row.QuotationId) {
            return;
        }
        $('#contractQuotationPreviewDialog').modal('hide');
        $timeout(function () {
            $scope.editQuotation(row.QuotationId);
        }, 300);
    };
    $scope.showExtendDialog = function () {

        $('#extendConractDialog').modal('show');
        FormsValidation.init('frmExtendConract');
    }
    $scope.ExtendContract = function () {

        var m = $('#frmExtendConract').valid();
        if (!m) {
            return;
        }

        var model = $scope.ContractExt;
        if (model == null || model == undefined) {
            alert('Please enter all input');
            return;
        }
        model.ContractId = $scope.Contract.ContractId;
        model.ValidTill = formatdate(model.ValidTill);
        contract.Extend(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Contract extended successfully.');
            $('#extendConractDialog').modal('hide');
            $scope.GetDetails();
            $scope.ContractExt = null;
        }, model);
    }
    $scope.getInventory = function () {
        contract.ContractInventory(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            if (e.data.Data) {
                var _inventory = e.data.Data;

                var _uniqueProdsInventory = uniqueItems($.map(_inventory, function (o) {
                    return o.Product;
                }));

                $.each(_uniqueProdsInventory,

                    function (index, value) {
                        var pd = { Product: '', Sent: 0, Returned: 0, Balance: 0 };
                        var _sentItems = _inventory.filter(o => o.ChallanType == 1 && o.Product == value);
                        pd.Product = value;
                        pd.Sent = $filter('sumByKey')(_sentItems, 'Quantity');

                        var _returnItems = _inventory.filter(o => o.ChallanType == 11 && o.Product == value);
                        pd.Returned = $filter('sumByKey')(_returnItems, 'Quantity');
                        pd.Balance = pd.Sent - pd.Returned;
                        $scope.ContractInventory.push(pd);
                    });
            }
        }, { ContractId: $scope.Contract.ContractId });

    }
    $scope.getBills = function () {
        contract.GetBills(function (e) {

            if (e.data.Data) {

                $scope.ContractBills = e.data.Data;
                $scope.BilledAmount = $filter('sumByKey')($scope.ContractBills, 'Total');
                if ($scope.recomputeSummaryBillPaymentBalance) {
                    $scope.recomputeSummaryBillPaymentBalance();
                }
                // $scope.Contract.Balance = $scope.Contract.ContractValue - $scope.Contract.BilledAmount;
            }
        }, { ContractId: $scope.Contract.ContractId });
    }
    $scope.refreshContractInfoLists = function () {
        $scope.ContractInventory = [];
        $scope.getInventory();
        if ($scope.SelectedJob) {
            $scope.viewJobDetails($scope.SelectedJob);
        }
        $scope.loadContractSummary();
        $scope.loadContractDocuments();
    };
    if (cId != 0) {
        $scope.Contract.ContractId = $crypto.decrypt(cId);
        if ($scope.Contract.ContractId > 0) {
            $scope.GetDetails();
            $scope.getInventory();

        }
    }


    $scope.Contract.Details = [];//initializeArray();
    init();

    $scope.PrintDeliveryChallan = function (workOrderId) {

        var wOrder = new $.WorkOrder({});
        wOrder.PrintIssuedReceipt(function (e) {

            var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
        }, [{ WorkOrderId: workOrderId }]);
    }
    $scope.PrintAllDeliveryChallans = function () {

        var wOrder = new $.WorkOrder({});

        if ($scope.DeliveryChallans.length > 0) {
            wOrder.PrintIssuedReceipt(function (e) {

                var filePath = SERVER_RPT_URL + 'temp/' + e.data;
                // $window.target = '_blank';
                $window.open(filePath);
            }, $scope.DeliveryChallans);
        }
    }

    $scope.editDeliveryChallan = function (worder) {

        //  $scope.params = worder;
        var worderId = $crypto.encrypt(worder.WorkOrderId.toString());
        $state.go('editIsCh', { WorkOrderId: worderId });


    }
    $scope.deleteDeliveryChallan = function (worder) {
        worder.LedgerId = $scope.Contract.LedgerId;
        worder.LedgerSiteId = $scope.Contract.LedgerSiteId;


        $scope.Message = 'Are you sure to delete this challan record';

        ModalFactory.ConfirmDelete($scope, function () {

            var wd = new $.WorkOrder();


            wd.DeleteChallan(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                ModalFactory.hideDialog();
                $scope.viewJobDetails($scope.SelectedJob);
                $scope.getInventory();
            }, worder);

        });

        //  $scope.params = worder;

    }
    $scope.editReturnChallan = function (grn) {
        //  $scope.params = worder;      
        var grnId = $crypto.encrypt(grn.GRNId.toString());
        $state.go('editgrn', { GRNId: grnId });
    }
    $scope.deleteReturnChallan = function (objGrn) {

        $scope.Message = 'Are you sure to delete this challan record';

        ModalFactory.ConfirmDelete($scope, function () {

            var grn = new $.GRN({});
            grn.DeleteChallan(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                ModalFactory.hideDialog();
                alert('Challan deleted successfully');
                $scope.viewJobDetails($scope.SelectedJob);
                $scope.getInventory();
            }, objGrn);

        });
    }
    $scope.PrintReturnChallan = function (grnId) {

        var wOrder = new $.WorkOrder({});
        wOrder.ItemsReceived_Print([{ GRNId: grnId }], function (e) {

            var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
        });
    }
    $scope.PrintAllReturnChallans = function () {
        var wOrder = new $.WorkOrder({});
        wOrder.ItemsReceived_Print($scope.ReturnChallans, function (e) {

            var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
        });
    }
    $scope.printBill = function (bill) {
        //var encrypedText = $crypto.encrypt(invoiceId);

        //var billing = new $.Billing();
        //billing.InvoiceId = invoiceId;
        //billing.PrintContractBill(function (e) {

        //    var filePath = SERVER_RPT_URL + 'temp/' + e.data;
        //    // $window.target = '_blank';
        //    $window.open(filePath);
        //});
        var encrypedText = $crypto.encrypt(bill.InvoiceId);

        var econded = btoa(encrypedText);
        var report = new $.Reports();
        report.downloadReportFromHtml(function (e) {
            FileSaver.saveAs(e.data, bill.InvoiceNumber + '.pdf');
        }, 'PrintContractBill', econded);
    };
    $scope.printContractSummaryPdf = function () {
        if (!$scope.Contract || !$scope.Contract.ContractId) {
            return;
        }
        var cid = $scope.Contract.ContractId.toString();
        var comp = ($scope.Contract.CompanyId !== undefined && $scope.Contract.CompanyId !== null)
            ? $scope.Contract.CompanyId.toString() : '0';
        var encrypedText = $crypto.encrypt(cid + '|' + comp);
        var econded = btoa(encrypedText);
        var report = new $.Reports();
        var namePart = ($scope.Contract.Title || ('Contract-' + $scope.Contract.ContractId)).replace(/[\\/:*?"<>|]/g, '_');
        report.downloadReportFromHtml(function (e) {
            FileSaver.saveAs(e.data, 'ContractSummary-' + namePart + '.pdf');
        }, 'PrintContractSummary', econded);
    };
    $scope.addJobDialog = function () {
        $scope.JobCard = {};
        $('#addJobDialog').modal('show');
    }
    $scope.editJobCard = function (job) {
        $scope.JobCard = cloneObj(job);
        if ($scope.JobCard.Employees) {
            var arrEmps = $scope.JobCard.Employees.split(',');
            $.each(arrEmps, function (index, value) {
                var emp = $scope.Employees.find(o => o.EmployeeId == value);
                if (emp) {
                    emp.Selected = true;
                }
            });
        }

        $scope.JobCard.EstimatedStartDate = convertDate(new Date(moment(job.EstimatedStartDate)));
        $scope.JobCard.EstimatedEndDate = convertDate(new Date(moment(job.EstimatedEndDate)));

        $('#addJobDialog').modal('show');
    }

    //  $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init('fromWoJobCard');
    //GetTaxes();





    //-- taxes

    function init() {
        $scope.ContractItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //   $scope.Contract = new $.LedgerTrasaction({});
        $scope.Contract.EntryType = $scope.TranType == 1 ? 8 : 9;
        //$scope.Contract.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.createReturnChallan = function () {
        if (!$scope.SelectedJob || !$scope.Contract || !$scope.Contract.ContractId) {
            alert('Please open an activity (VIEW) first.');
            return;
        }
        $scope.returnChallanModalOpen = true;
        $timeout(function () {
            $('#contractReturnChallanModal').modal('show');
            if (typeof FormsValidation !== 'undefined' && FormsValidation.init) {
                FormsValidation.init('form-grn');
            }
        }, 50);
    }
    $scope.createDelChallan = function () {
        if (!$scope.SelectedJob || !$scope.Contract || !$scope.Contract.ContractId) {
            alert('Please open an activity (VIEW) first.');
            return;
        }
        $scope.deliveryChallanModalOpen = true;
        $timeout(function () {
            $('#contractDeliveryChallanModal').modal('show');
            if (typeof FormsValidation !== 'undefined' && FormsValidation.init) {
                FormsValidation.init('form-workorder');
            }
        }, 50);
    }
    //add new item to be issued
    $scope.toggleDeliveryChallans = function (challan) {
        $('#' + challan).collapse('toggle');
    }
    $scope.toggleRetDeliveryChallans = function (challan) {
        $('#' + challan).collapse('toggle');
    }
    var jobCard = new $.JobCard();
    $scope.SelectedJob = null;
    $scope.DeliveryChallans = [];
    $scope.viewJobDetails = function (item) {
        $scope.SelectedJob = item;
        $scope.SelectedJob.Items = [];
        var jobCard = new $.JobCard();
        jobCard.JobCardDelChallanItems(function (e) {

            $scope.SelectedJob.Items = e.data.Data;
            $scope.DeliveryChallans = uniqueObjects($.map(e.data.Data, function (obj) {
                var items = e.data.Data.filter(o => o.WorkOrderId == obj.WorkOrderId);
                return {
                    WorkOrderId: obj.WorkOrderId, ChallanNumber: obj.ChallanNumber, SentDate: obj.SentDate, Items: items
                    , SiteId: obj.SiteId
                };
            }), 'ChallanNumber');

        }, { JobCardId: item.JobCardId });

        jobCard.JobCardRetChallanItems(function (e) {
            $scope.SelectedJob.Items = e.data.Data;
            $scope.ReturnChallans = uniqueObjects($.map(e.data.Data, function (obj) {
                var items = e.data.Data.filter(o => o.GRNId == obj.GRNId);
                return { GRNId: obj.GRNId, ChallanNumber: obj.ChallanNumber, ChallanDate: obj.ChallanDate, Items: items };
            }), 'ChallanNumber');

        }, { JobCardId: item.JobCardId })

    }
    $scope.SaveJobCard = function () {

        var m = $('#fromWoJobCard').valid();
        if (!m) {
            //  alert('Please enter all job card information');
            return;
        }
        var model = cloneObj($scope.JobCard);
        model.JobCardType = 'contract';
        model.JobCardTypeKey = $scope.Contract.ContractId;
        model.LedgerSiteId = $scope.Contract.LedgerSiteId;
        model.EstimatedStartDate = formatdate(model.EstimatedStartDate);
        model.EstimatedEndDate = formatdate(model.EstimatedEndDate);

        if ($scope.selectedEmployees) {
            model.Employees = $.map($scope.selectedEmployees, function (val) {
                return val.EmployeeId;
            }).join(',');
        }
        jobCard.Save(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $('#addJobDialog').modal('hide');
            $scope.GetDetails();
        }, model);

    }

    FormsValidation.init('frmContract');

    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Contract.LedgerId });
    }

    $scope.browseFile = function (image, item) {

        $('#' + item).click();
        $('#' + item).bind('change', function (e) {
            var img = $scope.onFileBrowse(e, $('#' + image)[0]);
        });

    }

    $scope.onFileBrowse = function (event, image) {
        readURL(event.currentTarget, image);
    };
    function readURL(input, image) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                image.src = e.target.result;
            }

            var dataUrl = reader.readAsDataURL(input.files[0]);


            return dataUrl;
        }
    }
});

function rentacContractsDashRestoreFilter(key) {
    try {
        var raw = sessionStorage.getItem(key);
        return raw ? JSON.parse(raw) : null;
    } catch (ex) {
        return null;
    }
}
function rentacContractsDashPersistFilter(key, payload) {
    try {
        sessionStorage.setItem(key, JSON.stringify(payload));
    } catch (ex) { }
}
function rentacContractsDashApplyDefaultDates(savedFilter, token, filter) {
    if (savedFilter) {
        angular.extend(filter, savedFilter);
        return;
    }
    var toDate = new Date();
    var fromDate = new Date(toDate);
    fromDate.setMonth(fromDate.getMonth() - 1);
    filter.To = convertDate(toDate);
    filter.From = convertDate(fromDate);
    if (token && token.FinYearStart) {
        var fromParsed = moment(filter.From, 'DD/MM/YYYY', true);
        var fyStart = moment(token.FinYearStart);
        if (fromParsed.isValid() && fyStart.isValid() && fromParsed.isBefore(fyStart, 'day')) {
            filter.From = convertDate(fyStart.toDate());
        }
    }
}

app.controller('ActivityTrackerController', function ($scope, $state, $rootScope, AuthenticationService,
    $crypto, toaster, LedgerFactory, FileSaver, formatDateFilter) {
    var ACT_TRACKER_FILTER_KEY = 'rentacActTrackerListFilter_v1';
    var savedActTrackerFilter = rentacContractsDashRestoreFilter(ACT_TRACKER_FILTER_KEY);

    function persistActTrackerFilter() {
        rentacContractsDashPersistFilter(ACT_TRACKER_FILTER_KEY, {
            LedgerId: $scope.Filter.LedgerId,
            LedgerSiteId: $scope.Filter.LedgerSiteId,
            From: $scope.Filter.From,
            To: $scope.Filter.To,
            StatusId: $scope.Filter.StatusId
        });
    }

    $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, From: '', To: '', StatusId: 0 };
    var token = $rootScope.getTokenInfo();

    $scope.Contracts = [];
    if (token) {
        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;
    }
    rentacContractsDashApplyDefaultDates(savedActTrackerFilter, token, $scope.Filter);

    LedgerFactory.GetAllParties(function (e) {

        $scope.Accounts = e.data;

    });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId && !savedActTrackerFilter) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $scope.getSites();
        if (!$scope.Accounts) {
            return;
        }
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        var ledger = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);


    });
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }

    $scope.filter = function () {

        var contract = new $.Contract();
        var filterModel = JSON.parse(JSON.stringify($scope.Filter));

        filterModel.From = formatdate(filterModel.From);
        filterModel.To = formatdate(filterModel.To);

        contract.ActivityTracker(function (e) {

            //$scope.Activities = e.data.Data;
            $scope.gridApi.setGridOption("rowData", e.data.Data);
            persistActTrackerFilter();
        }, filterModel);
    }


    $scope.openStatusDialog = function (statusId) {

        var item = $scope.SelectedContract;
        if (statusId == 3) {
            if (item.StatusId > 1) {
                alert('Only draft contracts can be deleted');
                return;
            }
            var c = confirm('Are you sure you want to delete this contract');
            if (c) {
                $('#delDialog').modal('show');
            }
            return;
        }

    }



    $scope.applyFilter = function () {
        $('#advFilterDialog').modal('hide');
        $scope.filter();
    }
    const myTheme = agGrid.themeQuartz.withParams({
        borderColor: "#00000054",
        headerHeight: "30px",
        headerTextColor: "#000",
        fontSize: 13,
        headerFontSize: 13,
        wrapperBorderRadius: 0,
        headerColumnBorder: true,
        rowHeight: 30,
        headerBackgroundColor: "#d7d7d7",

        headerCellMovingBackgroundColor: "rgb(80, 40, 140)",
        headerCellHoverBackgroundColor: "#98CCF8",
    });
    var gridApi;

    //Grid Options: Contains all of the grid configurations
    $scope.gridOptions = {
        theme: myTheme,
        rowNumbers: true,
        autoSizeStrategy: {
            type: 'fitCellContents',
        },
        getRowStyle: params => {
            if (params.data.Status == 'Completed') {
                return { color: 'green', background: '#e3ffe3' };
            }
            if (params.data.Status == 'Delayed') {
                return { color: 'orange', background: '#fff5c49e' };
            }
        },
        loading: false,
        rowData: null,
        domLayout: 'autoHeight',
        // Columns to be displayed (Should match rowData properties)
        alwaysShowHorizontalScroll: true,
        columnDefs: [
            { headerName: "Quotation #", field: "QuotationNumber" },
            {
                headerName: "Planning Date",
                valueGetter: (p) => {
                    if (!p.data.EstimatedStartDate) {
                        return '';
                    }
                    return convertDate(p.data.EstimatedStartDate);
                }
            },
            { field: "Client" },
            { field: "Address" },
            { field: "AreaZone", headerName: "Zone Area" },
            { field: "Locality" },
            { field: "Area" },

            { field: "SizeDescription", headerName: "Total Area" },
            { field: "Area" },
            { field: "TypeOfWork" },
            {
                headerName: "EstimatedStartDate",
                valueGetter: (p) => {
                    if (!p.data.EstimatedStartDate) {
                        return '';
                    }
                    return convertDate(p.data.EstimatedStartDate);
                }
            },
            {
                headerName: "EstimatedEndDate",
                valueGetter: (p) => {
                    if (!p.data.EstimatedEndDate) {
                        return '';
                    }
                    return convertDate(p.data.EstimatedEndDate);
                }
            },
            { field: "Employees" },
            { field: "Status" }
            //{ field: "Status" },
            //{
            //    headerName: 'Action',
            //    cellRenderer: CustomButtonComponent,
            //    cellRendererParams: {
            //        label: 'Change Status',
            //        onLoad: (btn, params) => {
            //            if (params.data.Status == 'Completed') {
            //                $(btn).attr('disabled', true);
            //            }

            //        },
            //        onClick: function (o) {
            //            $('#changeStatusDialog').modal('show');
            //            $scope.onChangeStatusClick(o.data);
            //        }
            //    },
            //    pinned: 'right'
            //}

        ],

        defaultColDef: {

            autoHeaderHeight: true,
            resizable: false,


        },
    };
    $scope.JobCard;
    $scope.onChangeStatusClick = function (data) {
        if (data.Status == 'Completed') {
            return;
        }
        $scope.JobCard.JobCardId = data.JobCardId;
    };

    $scope.updateStatus = function () {
        var jobCard = new $.JobCard();
        jobCard.UpdateStatus(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $('#advFilterDialog').modal('hide');
        }, $scope.JobCard);

    }
    // Create Grid: Create new grid within the #myGrid div, using the Grid Options object
    $scope.gridApi = agGrid.createGrid(document.querySelector("#myGrid"), $scope.gridOptions);
    $scope.filter();
});
app.controller('EmployeeDPRController', function ($scope, $state, $rootScope, EmployeeService,
    $crypto, toaster, LedgerFactory, FileSaver, formatDateFilter) {
    var EMP_DPR_FILTER_KEY = 'rentacEmpDprListFilter_v1';
    var savedEmpDprFilter = rentacContractsDashRestoreFilter(EMP_DPR_FILTER_KEY);

    function persistEmpDprFilter() {
        rentacContractsDashPersistFilter(EMP_DPR_FILTER_KEY, {
            LedgerId: $scope.Filter.LedgerId,
            LedgerSiteId: $scope.Filter.LedgerSiteId,
            EmployeeId: $scope.Filter.EmployeeId,
            From: $scope.Filter.From,
            To: $scope.Filter.To,
            StatusId: $scope.Filter.StatusId
        });
    }

    $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, EmployeeId: 0, From: '', To: '', StatusId: 0 };
    $scope.dprDocuments = [];
    $scope.STORAGE_LOC = typeof STORAGE_LOC !== 'undefined' ? STORAGE_LOC : '';
    $scope.documentFileUrl = function (doc) {
        if (!doc || !doc.StoragePath) return '#';
        var base = $scope.STORAGE_LOC || '';
        var path = doc.StoragePath;
        if (!base) return path;
        if (base.charAt(base.length - 1) !== '/' && path.charAt(0) !== '/')
            return base + '/' + path;
        return base + path;
    };
    var token = $rootScope.getTokenInfo();

    $scope.Contracts = [];
    if (token) {
        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;
    }
    rentacContractsDashApplyDefaultDates(savedEmpDprFilter, token, $scope.Filter);

    LedgerFactory.GetAllParties(function (e) {

        $scope.Accounts = e.data;

    });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId && !savedEmpDprFilter) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $scope.getSites();
        if (!$scope.Accounts) {
            return;
        }
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        var ledger = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);


    });
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }

    $scope.filter = function () {

        var contract = new $.Contract();
        var filterModel = JSON.parse(JSON.stringify($scope.Filter));

        filterModel.From = formatdate(filterModel.From);
        filterModel.To = formatdate(filterModel.To);

        contract.EmployeeDPR(function (e) {

            //$scope.Activities = e.data.Data;
            $scope.gridApi.setGridOption("rowData", e.data.Data.detail);
            $scope.summaryGridApi.setGridOption("rowData", e.data.Data.summary);

            $scope.summary = e.data.Data.summary;
            $scope.dprDocuments = (e.data.Data && e.data.Data.documents) ? e.data.Data.documents : [];
            persistEmpDprFilter();
        }, filterModel);
    }


    $scope.openStatusDialog = function (statusId) {

        var item = $scope.SelectedContract;
        if (statusId == 3) {
            if (item.StatusId > 1) {
                alert('Only draft contracts can be deleted');
                return;
            }
            var c = confirm('Are you sure you want to delete this contract');
            if (c) {
                $('#delDialog').modal('show');
            }
            return;
        }

    }



    $scope.applyFilter = function () {
        $('#advFilterDialog').modal('hide');
        $scope.filter();
    }
    const myTheme = agGrid.themeQuartz.withParams({
        borderColor: "#00000054",
        headerHeight: "30px",
        headerTextColor: "#000",
        fontSize: 13,
        headerFontSize: 13,
        wrapperBorderRadius: 0,
        headerColumnBorder: true,
        rowHeight: 30,
        headerBackgroundColor: "#d7d7d7",

        headerCellMovingBackgroundColor: "rgb(80, 40, 140)",
        headerCellHoverBackgroundColor: "#98CCF8",
    });
    var gridApi;

    //Grid Options: Contains all of the grid configurations
    $scope.gridOptions = {
        theme: myTheme,
        rowNumbers: true,
        autoSizeStrategy: {
            type: 'fitCellContents',
        },
        getRowStyle: params => {
            if (params.data.Status == 'Completed') {
                return { color: 'green', background: '#e3ffe3' };
            }
            if (params.data.Status == 'Delayed') {
                return { color: 'orange', background: '#fff5c49e' };
            }
        },
        loading: false,
        rowData: null,
        domLayout: 'autoHeight',
        // Columns to be displayed (Should match rowData properties)
        alwaysShowHorizontalScroll: true,
        columnDefs: [
            /*{ headerName: "Quotation #", field: "QuotationNumber" },*/
            { field: "Employee", headerName: 'Team Member', pinned: 'left' },
            {
                headerName: "Date",
                valueGetter: (p) => {
                    if (!p.data.EstimatedStartDate) {
                        return '';
                    }
                    return convertDate(p.data.EstimatedStartDate);
                }
            },


            { field: "Client" },
            { field: "Address" },


            { field: "SizeDescription", headerName: "Size" },
            { field: "Area", headerName: "Total Area" },
            { field: "TypeOfWork", headerName: 'Scope' },
            { field: "AreaCovered", headerName: 'OutPut' },


        ],

        defaultColDef: {

            autoHeaderHeight: true,
            resizable: false,


        },
    };
    $scope.JobCard;
    $scope.onChangeStatusClick = function (data) {
        if (data.Status == 'Completed') {
            return;
        }
        $scope.JobCard.JobCardId = data.JobCardId;
    };

    $scope.updateStatus = function () {
        var jobCard = new $.JobCard();
        jobCard.UpdateStatus(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $('#advFilterDialog').modal('hide');
        }, $scope.JobCard);

    }
    // Create Grid: Create new grid within the #myGrid div, using the Grid Options object
    $scope.gridApi = agGrid.createGrid(document.querySelector("#myGrid"), $scope.gridOptions);


    $scope.summaryGridOptions = {
        theme: myTheme,
        rowNumbers: true,
        autoSizeStrategy: {
            type: 'fitCellContents',
        },
        getRowStyle: params => {
            if (params.data.Status == 'Completed') {
                return { color: 'green', background: '#e3ffe3' };
            }
            if (params.data.Status == 'Delayed') {
                return { color: 'orange', background: '#fff5c49e' };
            }
        },
        loading: false,
        rowData: null,
        domLayout: 'autoHeight',
        // Columns to be displayed (Should match rowData properties)
        alwaysShowHorizontalScroll: true,
        columnDefs: [
            /*{ headerName: "Quotation #", field: "QuotationNumber" },*/
            { field: "Employee", headerName: 'Team Member' },
            { field: "AreaCovered", headerName: 'Area Covered' },
        ],

        defaultColDef: {

            autoHeaderHeight: true,
            resizable: false,


        },
    };
    $scope.summaryGridApi = agGrid.createGrid(document.querySelector("#summaryGrid"), $scope.summaryGridOptions);

    function getAllEmployees() {
        EmployeeService.GetAllEmployees(function (e) {
            $scope.Employees = e.data.Data;
        });
    }
    getAllEmployees();
    $scope.filter();
});

app.controller('OperationsContractsController', function ($scope, $state, $rootScope,
    $crypto, LedgerFactory, EmployeeService, FileSaver) {
    var OPS_CONTRACTS_FILTER_KEY = 'rentacOpsContractsListFilter_v1';
    var savedOpsContractsFilter = rentacContractsDashRestoreFilter(OPS_CONTRACTS_FILTER_KEY);

    function persistOpsContractsFilter() {
        rentacContractsDashPersistFilter(OPS_CONTRACTS_FILTER_KEY, {
            LedgerId: $scope.Filter.LedgerId,
            LedgerSiteId: $scope.Filter.LedgerSiteId,
            From: $scope.Filter.From,
            To: $scope.Filter.To,
            StatusId: $scope.Filter.StatusId,
            ActivityType: $scope.Filter.ActivityType,
            ActivityStatusId: $scope.Filter.ActivityStatusId
        });
    }

    $scope.Filter = {
        LedgerId: 0,
        LedgerSiteId: 0,
        From: '',
        To: '',
        StatusId: 0,
        ActivityType: 0,
        ActivityStatusId: 0
    };
    var token = $rootScope.getTokenInfo();

    $scope.Contracts = [];
    if (token) {
        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;
    }
    rentacContractsDashApplyDefaultDates(savedOpsContractsFilter, token, $scope.Filter);

    LedgerFactory.GetAllParties(function (e) {

        $scope.Accounts = e.data;

    });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId && !savedOpsContractsFilter) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $scope.getSites();
        if (!$scope.Accounts) {
            return;
        }
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        var ledger = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);


    });
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }
    $scope.Installations = 0;
    $scope.Dismantle = 0;
    $scope.Others = 0;

    $scope.filter = function () {

        var contract = new $.Contract();
        var filterModel = JSON.parse(JSON.stringify($scope.Filter));
        filterModel.From = formatdate(filterModel.From);
        filterModel.To = formatdate(filterModel.To);
        filterModel.FilterFor = "ops";
        contract.Filter(function (e) {

            $scope.Contracts = e.data.Data;
            $scope.Installations = $scope.Contracts.filter(o => o.Activity == 'INSTALL').length;
            $scope.Dismantle = $scope.Contracts.filter(o => o.Activity == 'DISMANTLE').length;
            $scope.ReInstall = $scope.Contracts.filter(o => o.Activity == 'REINSTALL').length;
            $scope.Pickup = $scope.Contracts.filter(o => o.Activity == 'PICKUP').length;

            $scope.Others = $scope.Contracts.filter(o => o.Activity == 'OTHER').length;
            $scope.FilteredContracts = e.data.Data;
            persistOpsContractsFilter();
        }, filterModel);
    }
    $scope.filter();
    $scope.edit = function (item) {
        if (item.StatusId != 1 && token.RoleId != 1) {
            alert('Only draft contracts can be edited');
            return;
        }

        persistOpsContractsFilter();
        var cId = $crypto.encrypt(item.ContractId);
        $state.go('contract', { id: cId });
    }
    $scope.applyByStatus = function (statusName) {

        var $tile = $(event.currentTarget).closest('.metrics-tile');
        $tile.closest('.row').find('.metrics-tile').removeClass('selected-tile');
        $tile.addClass('selected-tile');
        if ($scope.Contracts && statusName != '') {
            $scope.FilteredContracts = $scope.Contracts.filter(o => o.Activity && o.Activity.toLowerCase() == statusName);
        }
        if ($scope.Contracts && statusName == '') {
            $scope.FilteredContracts = $scope.Contracts;
        }

    }


    $scope.info = function (item) {
        persistOpsContractsFilter();
        var key = $crypto.encrypt(item.ContractId);
        $state.go('contractInfo', { cId: key });
    }
    function getAllEmployees() {
        EmployeeService.GetAllEmployees(function (e) {
            $scope.Employees = e.data.Data;
        });
    }
    $scope.applyFilter = function () {
        $('#advFilterDialog').modal('hide');
        $scope.filter();
    }
    function rentacMapActivityStatusToStatusId(statusStr) {
        var s = (statusStr == null ? '' : String(statusStr)).toLowerCase();
        if (s === 'completed') return 2;
        if (s === 'delayed') return 3;
        if (s.indexOf('progress') >= 0) return 4;
        return 1;
    }
    $scope.installActivityImageDataUrl = '';
    $scope.installActivityImagePreview = '';
    $scope.browseInstallActivityImage = function () {
        var el = document.getElementById('installActivityImageFile');
        if (!el) return;
        el.value = '';
        el.onchange = function (ev) {
            $scope.$apply(function () {
                $scope.onInstallActivityImageSelected(ev);
            });
        };
        el.click();
    };
    $scope.onInstallActivityImageSelected = function (event) {
        var input = event.target;
        if (!input.files || !input.files[0]) return;
        var f = input.files[0];
        if (!f.type || f.type.indexOf('image/') !== 0) {
            alert('Please select an image file.');
            return;
        }
        var reader = new FileReader();
        reader.onload = function (e) {
            $scope.$apply(function () {
                $scope.installActivityImageDataUrl = e.target.result;
                $scope.installActivityImagePreview = e.target.result;
            });
        };
        reader.readAsDataURL(f);
    };
    $scope.clearInstallActivityImage = function () {
        $scope.installActivityImageDataUrl = '';
        $scope.installActivityImagePreview = '';
        var el = document.getElementById('installActivityImageFile');
        if (el) el.value = '';
    };
    $scope.JobCard = {};
    $scope.updateStatusDialog = function (job) {
        if (job.Activity.toUpperCase() == 'INSTALL' && job.DeliveryChallans < 1) {
            alert('Material not delivered yet. Can not update status');
            return;
        }

        $scope.clearInstallActivityImage();
        $scope.JobCard = {
            JobCardId: job.JobCardId,
            StatusId: rentacMapActivityStatusToStatusId(job.ActivityStatus),
            AreaCovered: job.ActivtyAreaCovered,
            CompletionDate: job.ActivityCompletionDate,
            Remarks: job.Remarks || '',
            Activity: job.Activity
        };
        getAllEmployees();
        $('#changeStatusDialog').modal('show');
    }

    $scope.updateStatus = function () {
        var jobCard = new $.JobCard();
        var model = cloneObj($scope.JobCard);
        model.CompletionDate = formatdate(model.CompletionDate);
        model.Activity = $scope.JobCard.Activity;
        if ($scope.JobCard.Activity === 'INSTALL' && $scope.installActivityImageDataUrl) {
            model.ActivityImageDocument = $scope.installActivityImageDataUrl;
        }
        if ($scope.selectedEmployees) {
            model.Employees = $.map($scope.selectedEmployees, function (val) {
                return val.EmployeeId;
            }).join(',');
        }

        jobCard.UpdateStatus(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $('#changeStatusDialog').modal('hide');
            $scope.clearInstallActivityImage();
            $scope.filter();
        }, model);

    }
    //$scope.createDeliveryChallan = function (item) {
    //    $state.go('contractDelchallan', { JobCardId: item.JobCardId });
    //}
    //$scope.createReturnChallan = function (item) {
    //    $state.go('contractRetChallan', { JobCardId: item.JobCardId });
    //}
    $scope.createReturnChallan = function (item) {
        persistOpsContractsFilter();
        var jcKey = item.JobCardId + ',' + item.ContractId;
        var encKey = $crypto.encrypt(jcKey);
        $state.go('contractRetChallan', { JobCardId: encKey });
    }
    $scope.createDeliveryChallan = function (item) {
        persistOpsContractsFilter();
        var jcKey = item.JobCardId + ',' + item.ContractId;
        var encKey = $crypto.encrypt(jcKey);
        $state.go('contractDelchallan', { JobCardId: encKey });
    }
});
app.controller('SalesContractsController', function ($scope, $state, $rootScope,
    $crypto, LedgerFactory, EmployeeService, FileSaver) {
    var SALES_CONTRACTS_FILTER_KEY = 'rentacSalesContractsListFilter_v1';
    var savedSalesContractsFilter = rentacContractsDashRestoreFilter(SALES_CONTRACTS_FILTER_KEY);

    function persistSalesContractsFilter() {
        rentacContractsDashPersistFilter(SALES_CONTRACTS_FILTER_KEY, {
            LedgerId: $scope.Filter.LedgerId,
            LedgerSiteId: $scope.Filter.LedgerSiteId,
            From: $scope.Filter.From,
            To: $scope.Filter.To,
            StatusId: $scope.Filter.StatusId,
            ActivityType: $scope.Filter.ActivityType,
            ActivityStatusId: $scope.Filter.ActivityStatusId
        });
    }

    $scope.Filter = {
        LedgerId: 0,
        LedgerSiteId: 0,
        From: '',
        To: '',
        StatusId: 0,
        ActivityType: 0,
        ActivityStatusId: 0
    };

    $scope.activityStatus = ['delay', 'delayed', 'pending'];


    var token = $rootScope.getTokenInfo();
    $scope.installFilter = function (item) {

        return $scope.activityStatus.includes(item.InstallStatus.toLowerCase());
    }
    $scope.dismantleFilter = function (item) {
        return $scope.activityStatus.includes(item.DismantleStatus.toLowerCase()
            && item.InstallStatus.toLowerCase() == 'installed'
        );
    }

    $scope.Contracts = [];
    if (token) {
        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;
    }
    rentacContractsDashApplyDefaultDates(savedSalesContractsFilter, token, $scope.Filter);

    LedgerFactory.GetAllParties(function (e) {

        $scope.Accounts = e.data;

    });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId && !savedSalesContractsFilter) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $scope.getSites();
        if (!$scope.Accounts) {
            return;
        }
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        var ledger = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);


    });
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }
    $scope.Installations = 0;
    $scope.Dismantle = 0;
    $scope.Others = 0;

    $scope.filter = function () {
        var contract = new $.Contract();
        var filterModel = JSON.parse(JSON.stringify($scope.Filter));
        filterModel.From = formatdate(filterModel.From);
        filterModel.To = formatdate(filterModel.To);
        filterModel.FilterFor = "sales";
        contract.Filter(function (e) {

            $scope.Contracts = e.data.Data;
            $scope.Installations = $scope.Contracts.filter(o => o.Activity == 'INSTALL').length;
            $scope.Dismantle = $scope.Contracts.filter(o => o.Activity == 'DISMANTLE').length;
            $scope.Others = $scope.Contracts.filter(o => o.Activity == 'OTHER').length;

            $scope.FilteredContracts = e.data.Data;
            persistSalesContractsFilter();

        }, filterModel);
    }
    $scope.filter();
    $scope.edit = function (item) {
        if (item.StatusId != 1 && token.RoleId != 1) {
            alert('Only draft contracts can be edited');
            return;
        }

        persistSalesContractsFilter();
        var cId = $crypto.encrypt(item.ContractId);
        $state.go('contract', { id: cId });
    }
    $scope.applyByStatus = function (statusName) {

        var $tile = $(event.currentTarget).closest('.metrics-tile');
        $tile.closest('.row').find('.metrics-tile').removeClass('selected-tile');
        $tile.addClass('selected-tile');
        if ($scope.Contracts && statusName != '') {
            $scope.FilteredContracts = $scope.Contracts.filter(o => o.Activity && o.Activity.toLowerCase() == statusName);
        }
        if ($scope.Contracts && statusName == '') {
            $scope.FilteredContracts = $scope.Contracts;
        }

    }


    $scope.info = function (item) {
        persistSalesContractsFilter();
        var key = $crypto.encrypt(item.ContractId);
        $state.go('contractInfo', { cId: key });
    }
    function getAllEmployees() {
        EmployeeService.GetAllEmployees(function (e) {
            $scope.Employees = e.data.Data;
        });
    }
    $scope.applyFilter = function () {
        $('#advFilterDialog').modal('hide');
        $scope.filter();
    }
    function rentacMapActivityStatusToStatusIdSales(statusStr) {
        var s = (statusStr == null ? '' : String(statusStr)).toLowerCase();
        if (s === 'completed') return 2;
        if (s === 'delayed') return 3;
        if (s.indexOf('progress') >= 0) return 4;
        return 1;
    }
    $scope.installActivityImageDataUrl = '';
    $scope.installActivityImagePreview = '';
    $scope.browseInstallActivityImage = function () {
        var el = document.getElementById('installActivityImageFile');
        if (!el) return;
        el.value = '';
        el.onchange = function (ev) {
            $scope.$apply(function () {
                $scope.onInstallActivityImageSelected(ev);
            });
        };
        el.click();
    };
    $scope.onInstallActivityImageSelected = function (event) {
        var input = event.target;
        if (!input.files || !input.files[0]) return;
        var f = input.files[0];
        if (!f.type || f.type.indexOf('image/') !== 0) {
            alert('Please select an image file.');
            return;
        }
        var reader = new FileReader();
        reader.onload = function (e) {
            $scope.$apply(function () {
                $scope.installActivityImageDataUrl = e.target.result;
                $scope.installActivityImagePreview = e.target.result;
            });
        };
        reader.readAsDataURL(f);
    };
    $scope.clearInstallActivityImage = function () {
        $scope.installActivityImageDataUrl = '';
        $scope.installActivityImagePreview = '';
        var el = document.getElementById('installActivityImageFile');
        if (el) el.value = '';
    };
    $scope.JobCard = {};
    $scope.updateStatusDialog = function (job) {
        $scope.clearInstallActivityImage();
        $scope.JobCard = {
            JobCardId: job.JobCardId,
            StatusId: rentacMapActivityStatusToStatusIdSales(job.ActivityStatus),
            AreaCovered: job.ActivtyAreaCovered,
            CompletionDate: job.ActivityCompletionDate,
            Remarks: job.Remarks || '',
            Activity: job.Activity
        };
        getAllEmployees();
        $('#changeStatusDialog').modal('show');
    }

    $scope.updateStatus = function () {
        var jobCard = new $.JobCard();
        var model = cloneObj($scope.JobCard);
        model.CompletionDate = formatdate(model.CompletionDate);
        model.Activity = $scope.JobCard.Activity;
        if ($scope.JobCard.Activity === 'INSTALL' && $scope.installActivityImageDataUrl) {
            model.ActivityImageDocument = $scope.installActivityImageDataUrl;
        }
        if ($scope.selectedEmployees) {
            model.Employees = $.map($scope.selectedEmployees, function (val) {
                return val.EmployeeId;
            }).join(',');
        }

        jobCard.UpdateStatus(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $('#changeStatusDialog').modal('hide');
            $scope.clearInstallActivityImage();
            $scope.filter();
        }, model);

    }

    $scope.getClassName = function (status) {

        var statusName = status.toLowerCase();
        if (statusName == 'delay') {
            return 'delay';
        }
        if (statusName == 'delayed') {
            return 'delayed';
        }
        if (statusName == 'installed' || statusName == 'dismantled' || statusName == 'picked-up') {
            return 'completed';
        }
    }
});