//Purchase Register 
app.controller('PurchaseRegisterController', function ($scope, $rootScope, $state, $window, $rootScope, $crypto, $mdDialog,
    ReportService, FileUploadService, FileSaver) {

    var purchase = new $.Transaction({ PurchaseId: 0 });

    var date = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
    });
    var ledger = new $.Ledger({});

    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);

    $scope.PurchaseRegister = function () {

        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.PurchaseType = 1;
        purchase.PurchaseRegister(function (e) {
            $scope.Register = e.data;

        }, filter)
    };
    $scope.PurchaseRegister();
    $scope.PurchaseItemsTax = function (index) {

        loadTaxes(index.PurchaseId);
    };
    function loadTaxes(PurchaseId) {
        purchase.PurchaseId = PurchaseId;
        purchase.PurchaseItemsTax(function (e) {
            $scope.AppliedTaxes = e.data;
            var div = '<div style="width:90%;height:70%"></div>';

            $(div).load('templ/taxItems.html', function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        AppliedTaxes: $scope.AppliedTaxes
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: function (AppliedTaxes) {
                        $scope.closeSliderModal = function () {
                            $mdDialog.hide();
                        }
                    }
                });
            });
        });
    }
    $scope.ShowPurchaseItems = function (index) {

        loadItems(index.PurchaseId);
    };
    function loadItems(purchaseId) {

        var div = '<div style="width:90%;height:70%"></div>';

        $(div).load('templ/dialogs/purchaseItems.html', function () {
            var html = $(this).html();

            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    purchaseId: purchaseId
                },
                template: html,
                parent: angular.element(document.body),
                controller: 'PurchaseItemsController'
            });
        });
    }

    //$scope.print = function (item, format) {
    //    var filter = { 'PurchaseId': item.PurchaseId, 'FileFormat': format };
    //    //  purchase.PurchaseId = item.PurchaseId;
    //    purchase.PrintPurchaseReceipt(function (e) {
    //        //debugger
    //        var filePath = SERVER_RPT_URL + 'temp/' + e.data;
    //        // $window.target = '_blank';
    //        $window.open(filePath);
    //    }, filter);
    //}
    //PurchaseItemsList

    $scope.edit = function (item) {
        var key = $crypto.encrypt(item.PurchaseId);

        $('#previewDialog').modal('hide');
        setTimeout(() => {
            $state.go('editpurchase', { key: key });
        }, 500);

    }
    $scope.delete = function (item) {
        var c = confirm('Are you sure to delete this record');
        if (!c) {
            return;
        }
        var model = { PurchaseId: item.PurchaseId };

        purchase.DeletePurchase(function (o) {
            if (o.data.Code != 200) {
                alert(e.data.Message);
            }
            else {
                alert('Record deleted successfully.');
                $('#previewDialog').modal('hide');
                $scope.PurchaseRegister();
            }
        }, model);

    }

    $scope.Preview = 0;
    $scope.SelectedItem;
    $scope.preview = function (item) {
        $scope.SelectedItem = item;
        $scope.Preview = 1;
        $('#previewDialog').modal('show');

        var strInput = "purchasebill," + item.PurchaseId
        var encrypedText = $crypto.encrypt(strInput);

        //var econded = btoa(encrypedText);
        //    var report = new $.Reports();

        ReportService.loadPreviewFromReportServer(function (e) {
            $scope.Preview = null;
            $('#rpt').html(e.data);

        }, encrypedText);
    }

    $scope.printPdf = function () {

        var item = $scope.SelectedItem;
        var strInput = "purchasebill," + + item.PurchaseId;
        var encrypedText = $crypto.encrypt(strInput);

        //var econded = btoa(encrypedText);
        //var report = new $.Reports();
        ReportService.printFromReportServer(encrypedText);
    }

    $scope.downloadAttachment = function (item) {
        debugger
        var model = cloneObj(item);
        model.DocType = 'purchase';
        model.FileName = item.Doc1;
        FileUploadService.downloadAsBlob(model).then(function (e) {
            if (e.status == 200) {
                FileSaver.saveAs(e.data, item.Doc1);
            }
            else {
                alert(e.statusText);
            }
        });
    }
});
//-- end of purchase register
app.controller('PurchaseItemsController', function ($scope, $mdDialog, purchaseId) {

    var purchase = new $.Transaction({ PurchaseId: purchaseId });
    purchase.PurchaseItemsList(function (e) {
        $('#loadersite').hide();
        $scope.Items = e.data;
    });

    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
});
//Purchase Entry
app.controller('PurchaseController', function ($scope, $rootScope, FileUploadService, $state, $http, PurchaseService, $uibModal, WarehouseService, AuthenticationService) {

    var purchase = new $.Transaction({ PurchaseId: 0, PurchaseType: 1 });
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});

    $scope.Trans = purchase;
    $scope.Trans.Items = [];//initializeArray();
    init();
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    accGroup.GetAccountsByGroup(function (e) {
        $scope.Banks = e.data;

    }, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    //accGroup.GetAccountsByGroup(function (e) {

    //    $scope.SundryDebtors = e.data;

    //}, { AccountGroupId: Enums.SUNDRY_CREDITORS });
    ledgerDTO.GetAllByGroups(function (e) {

        $scope.SundryDebtors = e.data.Data;


    }, DEBTORS_AND_CREDITORS);
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Trans.LedgerId', function () {
        $rootScope.LedgerId = $scope.Trans.LedgerId;
        $scope.Ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
    });
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
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        // $scope.addWorkOrder(fileList);
        var model = cloneObj($scope.Trans);
        model.PurchaseDate = formatdate(model.PurchaseDate);

        PurchaseService.save(model).then(function (e) {
            if (e.status != 200) {
                alert(e.data.Message);
                return;
            }
            alert('saved');
            $scope.warnOnLeave = false;
            $state.go('purchaseReg');
        });

        //var purchase = new $.Transaction();
        //purchase.SavePurchase(function (e) {
        //    if (e.status == 200) {

        //        alert('saved');
        //        $scope.warnOnLeave = false;
        //        $state.go('purchaseReg');
        //    } else {
        //        showMessage(MessageClass.SAVED);
        //    }

        //}, model, fileList);


    };

    $scope.onFileSelected = function (files) {
        $scope.Trans.Doc1 = null;
        if (files) {
            if (files.length > 0) {
                FileUploadService.upload(files[0]).then(function (e) {
                    if (e.data.Code == 200) {
                        $scope.Trans.Doc1 = e.data.Data;
                    }
                });
            }
        }

    };


    $scope.addWorkOrder = function (fileList) {

    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        //$scope.SubTotal(0);
    });
    function getItemSubTotal(item) {
        if (item.PurchaseUnitId > 0) {
            item.SubTotal = item.Unit1Quantity * item.Rate;
        }
        else {
            item.SubTotal = item.Quantity * item.Rate;
        }
        return item.SubTotal;
    }
    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function () {
        var pId = $scope.TransItem.ProductId || 0;
        if (pId <= 0 || $scope.TransItem.Quantity == 0) {

            //  alert("Please provide all details1.");
            return;
        }

        if ($scope.TransItem.Rate < 0) {
            //  alert("Rate can't be 0 or less.");
            return;
        }

        var itemExist = $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
            itemExist.Unit1Quantity = parseInt(itemExist.Unit1Quantity) + parseInt($scope.TransItem.Unit1Quantity);

            //   itemExist.Unit1Quantity = parseFloat(itemExist.Unit1Quantity || 0) + parseFloat($scope.TransItem.Unit1Quantity || 0);
            //  itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;
            itemExist.SubTotal = getItemSubTotal($scope.TransItem);
        } else {
            //if ($scope.TransItem.Unit1Quantity > 0) {
            //    $scope.TransItem.SubTotal = $scope.TransItem.Unit1Quantity * $scope.TransItem.Rate;
            //}
            //else {
            //    $scope.TransItem.SubTotal = $scope.TransItem.Quantity * $scope.TransItem.Rate;
            //}
            $scope.TransItem.SubTotal = getItemSubTotal($scope.TransItem);
            $scope.Trans.Items.push($scope.TransItem);
        }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');

        event.stopPropagation();
    }

    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;

        if ($scope.Trans.Items) {
            // $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}
            if ($scope.Trans.Items) {
                for (var i = 0; i < $scope.Trans.Items.length; i++) {

                    var item = $scope.Trans.Items[i];
                    // item.SubTotal = item.Quantity * item.Rate;
                    item.SubTotal = getItemSubTotal(item);
                    $scope.applyDiscount(item);
                    $scope.applyTaxRate(item.ProductId);
                }
            }
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {

                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        $scope.calculateDiscount();
        //}
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        return $scope.Trans.SubTotal;

    };

    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {

            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.CostPrice;
            $scope.TransItem.UOM = selected.originalObject.UOM;
            $scope.TransItem.Unit = selected.originalObject.Unit

            $scope.TransItem.PurchaseUnitId = selected.originalObject.PurchaseUnitId || 0;
            $scope.TransItem.PurchaseUnitName = selected.originalObject.PurchaseUnitName
        }
    };
    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {

        //   $scope.TransItem.Rate = 0;
        var taxes = StaicData.TAX_CATEGORY;
        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);

                var partyStateIdForGST = $scope.Ledger.StateId;
                var token = $rootScope.getTokenInfo();
                if (tax) {
                    for (var i = 0; i < lineItems.length; i++) {
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        lineItems[i].IGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.IGST / 100;

                        lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                        lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        lineItems[i].TaxName = tax.TaxName;

                        lineItems[i].CGSTRate = tax.CGST;
                        lineItems[i].SGSTRate = tax.SGST;
                        lineItems[i].IGSTRate = tax.IGST;

                        if (partyStateIdForGST == token.CompanyStateId) {

                            lineItems[i].IGST = 0;
                            lineItems[i].IGSTRate = 0;
                        }
                        else {
                            lineItems[i].CGST = 0;
                            lineItems[i].SGST = 0;
                            lineItems[i].CGSTRate = 0;
                            lineItems[i].SGSTRate = 0;

                        }

                        //lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                        //lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        //lineItems[i].TaxName = tax.TaxName;
                    }
                }
            }
        }

    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');
    $scope.warehouses = [];
    WarehouseService.getWarehouses().then(function (wres) {
        if (wres.data && wres.data.Data) {
            $scope.warehouses = wres.data.Data;
        }
        var tok = AuthenticationService.getTokenInfo();
        if (tok && tok.DefaultWareHouseId && (!$scope.Trans.PurchaseId || $scope.Trans.PurchaseId === 0)) {
            if (!$scope.Trans.WarehouseId) {
                $scope.Trans.WarehouseId = tok.DefaultWareHouseId;
            }
        }
    });
});


app.controller('PurchaseEditController', function ($scope, $rootScope, $stateParams, $state, $crypto, PurchaseService, $uibModal
    , FileUploadService, WarehouseService, AuthenticationService) {

    var purchase = new $.Transaction({ PurchaseId: 0, PurchaseType: 1 });

    if (!$stateParams.key) {
        alert('Invalid key to edit');
        return;
    }
    purchase.PurchaseId = $crypto.decrypt($stateParams.key);

    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});





    $scope.Trans = purchase;
    $scope.Trans.Items = [];//initializeArray();
    init();



    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    accGroup.GetAccountsByGroup(function (e) {
        $scope.Banks = e.data;

    }, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    ledgerDTO.GetAllByGroups(function (e) {

        $scope.SundryDebtors = e.data.Data;
        if (purchase.PurchaseId > 0) {
            purchase.GetPurchaseById(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.Trans = e.data.Data;
                $scope.Trans.PurchaseDate = convertDate(new Date(moment(e.data.Data.PurchaseDate)));
                $scope.Trans.DiscountValue = $scope.Trans.DiscountPercent;
                $scope.Ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
            }, purchase.PurchaseId);
        }

    }, DEBTORS_AND_CREDITORS);
    //accGroup.GetAccountsByGroup(function (e) {

    //    $scope.SundryDebtors = e.data;

    //}, { AccountGroupId: Enums.SUNDRY_CREDITORS });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;

    }
    $scope.$watch('Trans.LedgerId', function () {
        $rootScope.LedgerId = $scope.Trans.LedgerId;

    });
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
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        // $scope.addWorkOrder(fileList);
        var model = cloneObj($scope.Trans);
        model.PurchaseDate = formatdate(model.PurchaseDate);
        PurchaseService.save(model).then(function (e) {
            if (e.status != 200) {
                alert(e.data.Message);
                return;
            }
            alert('saved');
            $scope.warnOnLeave = false;
            $state.go('purchaseReg');
        });


    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    $scope.addWorkOrder = function (fileList) {
        var model = cloneObj($scope.Trans);
        model.PurchaseDate = formatdate(model.PurchaseDate);
        var purchase = new $.Transaction();
        purchase.SavePurchase(function (e) {

            if (e.status == 200) {

                alert('saved');

                $state.go('purchaseReg');
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        //$scope.SubTotal(0);
    });
    function getItemSubTotal(item) {
        if (item.PurchaseUnitId > 0) {
            item.SubTotal = item.Unit1Quantity * item.Rate;
        }
        else {
            item.SubTotal = item.Quantity * item.Rate;
        }
        return item.SubTotal;
    }
    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate < 0) {
            alert("Rate can't be 0 or less.");
            return;
        }

        var itemExist = $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
            itemExist.Unit1Quantity = parseInt(itemExist.Unit1Quantity) + parseInt($scope.TransItem.Unit1Quantity);

            //   itemExist.Unit1Quantity = parseFloat(itemExist.Unit1Quantity || 0) + parseFloat($scope.TransItem.Unit1Quantity || 0);
            //  itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;
            itemExist.SubTotal = getItemSubTotal($scope.TransItem);
        } else {
            //if ($scope.TransItem.Unit1Quantity > 0) {
            //    $scope.TransItem.SubTotal = $scope.TransItem.Unit1Quantity * $scope.TransItem.Rate;
            //}
            //else {
            //    $scope.TransItem.SubTotal = $scope.TransItem.Quantity * $scope.TransItem.Rate;
            //}
            $scope.TransItem.SubTotal = getItemSubTotal($scope.TransItem);
            $scope.Trans.Items.push($scope.TransItem);
        }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');

        event.stopPropagation();
    }

    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;

        if ($scope.Trans.Items) {
            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}

            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}
            if ($scope.Trans.Items) {
                for (var i = 0; i < $scope.Trans.Items.length; i++) {

                    var item = $scope.Trans.Items[i];
                    // item.SubTotal = item.Quantity * item.Rate;
                    item.SubTotal = getItemSubTotal(item);
                    $scope.applyDiscount(item);
                    $scope.applyTaxRate(item.ProductId);
                }
            }
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        $scope.calculateDiscount();
        //}
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        return $scope.Trans.SubTotal;

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.CostPrice;
            $scope.TransItem.UOM = selected.originalObject.UOM;
            $scope.TransItem.Unit = selected.originalObject.Unit

            $scope.TransItem.PurchaseUnitId = selected.originalObject.PurchaseUnitId || 0;
            $scope.TransItem.PurchaseUnitName = selected.originalObject.PurchaseUnitName

        }
    };
    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {

        //$scope.TransItem.Rate = 0;
        var taxes = StaicData.TAX_CATEGORY;
        if (!$scope.Ledger) {
            return;
        }
        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);

                var partyStateIdForGST = $scope.Ledger.StateId;
                var token = $rootScope.getTokenInfo();
                if (tax) {
                    for (var i = 0; i < lineItems.length; i++) {
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        lineItems[i].IGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.IGST / 100;

                        lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                        lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        lineItems[i].TaxName = tax.TaxName;

                        lineItems[i].CGSTRate = tax.CGST;
                        lineItems[i].SGSTRate = tax.SGST;
                        lineItems[i].IGSTRate = tax.IGST;

                        if (partyStateIdForGST == token.CompanyStateId) {

                            lineItems[i].IGST = 0;
                            lineItems[i].IGSTRate = 0;
                        }
                        else {
                            lineItems[i].CGST = 0;
                            lineItems[i].SGST = 0;
                            lineItems[i].CGSTRate = 0;
                            lineItems[i].SGSTRate = 0;

                        }

                        //lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                        //lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        //lineItems[i].TaxName = tax.TaxName;
                    }
                }
            }
        }

    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');
    $scope.warehouses = [];
    WarehouseService.getWarehouses().then(function (wres) {
        if (wres.data && wres.data.Data) {
            $scope.warehouses = wres.data.Data;
        }
        var tok = AuthenticationService.getTokenInfo();
        if (tok && tok.DefaultWareHouseId && (!$scope.Trans.PurchaseId || $scope.Trans.PurchaseId === 0)) {
            if (!$scope.Trans.WarehouseId) {
                $scope.Trans.WarehouseId = tok.DefaultWareHouseId;
            }
        }
    });
    $scope.SelectedFiles = null;
    $scope.onFileSelected = function (files) {
        $scope.SelectedFiles = files;
        //if (files) {
        //    if (files.length > 0) {
        //        FileUploadService.upload(files[0]).then(function (e) {

        //            if (e.data.Code == 200) {
        //                $scope.Trans.Doc1 = e.data.Data;
        //            }
        //        });
        //    }
        //}

    };
    $scope.deleteAttachment = function () {
        $scope.Trans.Doc1 = null;
    }
    $scope.onFileDeleted = function (file) {
        //  $scope.Trans.Doc1 = null;
        $scope.SelectedFiles = null;
    }
    $scope.onOkClicked = function () {

        if ($scope.SelectedFiles) {
            if ($scope.SelectedFiles.length > 0) {
                FileUploadService.upload($scope.SelectedFiles[0]).then(function (e) {

                    if (e.data.Code == 200) {
                        $scope.Trans.Doc1 = e.data.Data;
                        $('#fileUploadDialog').modal('hide');
                    }
                });
            }
        }
    }
});

//purchase return controllers
app.controller('PurchaseReturnListController', function ($scope, $rootScope, $crypto, $state, $window, $rootScope, $mdDialog, ReportService) {

    var purchase = new $.Transaction({ PurchaseId: 0 });

    var date = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
    });
    var ledger = new $.Ledger({});
    //ledger.GetAll(function (e) {

    //    $scope.Accounts = e.data;
    //    if ($scope.Filter.LedgerId == null) {
    //        $scope.Filter.LedgerId = e.data[0].LedgerId;
    //    }
    //});
    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = $scope.Accounts[0].LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);
    $scope.PurchaseRegister = function () {

        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.PurchaseType = 2;
        purchase.PurchaseRegister(function (e) {
            $scope.Register = e.data;

        }, filter)
    };
    $scope.PurchaseRegister();
    $scope.PurchaseItemsTax = function (index) {

        loadTaxes(index.PurchaseId);
    };
    function loadTaxes(PurchaseId) {
        purchase.PurchaseId = PurchaseId;
        purchase.PurchaseItemsTax(function (e) {
            $scope.AppliedTaxes = e.data;
            var div = '<div style="width:90%;height:70%"></div>';

            $(div).load('templ/taxItems.html', function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        AppliedTaxes: $scope.AppliedTaxes
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: function (AppliedTaxes) {
                        $scope.closeSliderModal = function () {
                            $mdDialog.hide();
                        }
                    }
                });
            });
        });
    }
    $scope.ShowPurchaseItems = function (index) {

        loadItems(index.PurchaseId);
    };
    function loadItems(purchaseId) {

        var div = '<div style="width:90%;height:70%"></div>';

        $(div).load('templ/dialogs/purchaseItems.html', function () {
            var html = $(this).html();

            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    purchaseId: purchaseId
                },
                template: html,
                parent: angular.element(document.body),
                controller: 'PurchaseItemsController'
            });
        });
    }

    $scope.print = function (item, format) {
        var filter = { 'PurchaseId': item.PurchaseId, 'FileFormat': format };
        //  purchase.PurchaseId = item.PurchaseId;
        purchase.PrintPurchaseReceipt(function (e) {
            //debugger
            var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
        }, filter);
    }
    //PurchaseItemsList


    $scope.edit = function (item) {
        var key = $crypto.encrypt(item.PurchaseId);

        $('#previewDialog').modal('hide');
        setTimeout(() => {
            $state.go('editpurchasereturn', { key: key });
        }, 500);

    }
    $scope.delete = function (item) {
        var c = confirm('Are you sure to delete this record');
        if (!c) {
            return;
        }
        var model = { PurchaseId: item.PurchaseId };

        purchase.DeletePurchase(function (o) {
            if (o.data.Code != 200) {
                alert(e.data.Message);
            }
            else {
                alert('Record deleted successfully.');
                $('#previewDialog').modal('hide');
                $scope.PurchaseRegister();
            }
        }, model);

    }
    $scope.Preview = 0;
    $scope.SelectedItem;
    $scope.preview = function (item) {
        $scope.SelectedItem = item;
        $scope.Preview = 1;
        $('#previewDialog').modal('show');

        var strInput = "purchasereturn," + item.PurchaseId
        var encrypedText = $crypto.encrypt(strInput);

        //var econded = btoa(encrypedText);
        //    var report = new $.Reports();

        ReportService.loadPreviewFromReportServer(function (e) {
            $scope.Preview = null;
            $('#rpt').html(e.data);

        }, encrypedText);
    }

    $scope.printPdf = function () {

        var item = $scope.SelectedItem;
        var strInput = "purchasereturn," + + item.PurchaseId;
        var encrypedText = $crypto.encrypt(strInput);

        //var econded = btoa(encrypedText);
        //var report = new $.Reports();
        ReportService.printFromReportServer(encrypedText);
    }
});
app.controller('PurchaseReturnController', function ($scope, $rootScope, $state, $uibModal, WarehouseService,
    AuthenticationService, PurchaseService) {

    var purchase = new $.Transaction({ PurchaseId: 0, PurchaseType: 2 });
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});

    $scope.Trans = purchase;
    $scope.Trans.Items = [];//initializeArray();
    init();
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    accGroup.GetAccountsByGroup(function (e) {
        $scope.Banks = e.data;

    }, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    //accGroup.GetAccountsByGroup(function (e) {

    //    $scope.SundryDebtors = e.data;

    //}, { AccountGroupId: Enums.SUNDRY_CREDITORS });
    ledgerDTO.GetAllByGroups(function (e) {

        $scope.SundryDebtors = e.data.Data;


    }, DEBTORS_AND_CREDITORS);
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Trans.LedgerId', function () {
        $rootScope.LedgerId = $scope.Trans.LedgerId;
    });
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
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        $scope.addWorkOrder(fileList);



    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    $scope.addWorkOrder = function (fileList) {

        var model = cloneObj($scope.Trans);
        model.PurchaseDate = formatdate(model.PurchaseDate);
        if (model.VendorCreditNoteDate)
            model.VendorCreditNoteDate = formatdate(model.VendorCreditNoteDate);

        model.PurchaseType = 2;
        PurchaseService.save(model).then(function (e) {
            if (e.status != 200) {
                alert(e.data.Message);
                return;
            }
            alert('saved');
            $scope.warnOnLeave = false;
            $state.go('purretlist');
        });

        //$scope.Trans.SavePurchase(function (e) {
        //    if (e.status == 200) {

        //        alert('saved');
        //        $scope.warnOnLeave = false;
        //        $state.go('purretlist');
        //        //   $state.reload();
        //    } else {
        //        showMessage(MessageClass.SAVED);
        //    }

        //}, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        //$scope.SubTotal(0);
    });

    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate < 0) {
            alert("Rate can't be 0 or less.");
            return;
        }

        var itemExist = $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId && o.Rate == $scope.TransItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
            itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;

        } else {
            $scope.TransItem.SubTotal = $scope.TransItem.Quantity * $scope.TransItem.Rate;

            $scope.Trans.Items.push($scope.TransItem);
        }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }

    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;

        if ($scope.Trans.Items) {
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}
            if ($scope.Trans.Items) {
                for (var i = 0; i < $scope.Trans.Items.length; i++) {
                    var item = $scope.Trans.Items[i];
                    $scope.applyDiscount(item);
                    $scope.applyTaxRate(item.ProductId);
                }
            }
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        $scope.calculateDiscount();
        //}
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        return $scope.Trans.SubTotal;

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.CostPrice;

        }
    };
    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {

        //  $scope.TransItem.Rate = 0;
        var taxes = StaicData.TAX_CATEGORY;

        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);

                if (tax) {
                    for (var i = 0; i < lineItems.length; i++) {
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                        lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        lineItems[i].TaxName = tax.TaxName;
                    }
                }
            }
        }

    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');
    $scope.warehouses = [];
    WarehouseService.getWarehouses().then(function (wres) {
        if (wres.data && wres.data.Data) {
            $scope.warehouses = wres.data.Data;
        }
        var tok = AuthenticationService.getTokenInfo();
        if (tok && tok.DefaultWareHouseId && (!$scope.Trans.PurchaseId || $scope.Trans.PurchaseId === 0)) {
            if (!$scope.Trans.WarehouseId) {
                $scope.Trans.WarehouseId = tok.DefaultWareHouseId;
            }
        }
    });
});

app.controller('PurchaseReturnEditController', function ($scope, $rootScope, $stateParams, $state, $crypto,
    $location, $uibModal, WarehouseService, AuthenticationService, PurchaseService) {

    var purchase = new $.Transaction({ PurchaseId: 0, PurchaseType: 2 });

    if (!$stateParams.key) {
        alert('Invalid key to edit');
        return;
    }
    purchase.PurchaseId = $crypto.decrypt($stateParams.key);

    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});

    $scope.Trans = purchase;
    $scope.Trans.Items = [];//initializeArray();
    init();

    if (purchase.PurchaseId > 0) {
        purchase.GetPurchaseById(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Trans = e.data.Data;
            $scope.Trans.PurchaseDate = convertDate(new Date(moment(e.data.Data.PurchaseDate)));
            $scope.Trans.VendorCreditNoteDate = convertDate(new Date(moment(e.data.Data.VendorCreditNoteDate)));
            $scope.Trans.DiscountValue = $scope.Trans.DiscountPercent;

        }, purchase.PurchaseId);
    }

    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    accGroup.GetAccountsByGroup(function (e) {
        $scope.Banks = e.data;

    }, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    //accGroup.GetAccountsByGroup(function (e) {

    //    $scope.SundryDebtors = e.data;

    //}, { AccountGroupId: Enums.SUNDRY_CREDITORS });
    ledgerDTO.GetAllByGroups(function (e) {

        $scope.SundryDebtors = e.data.Data;


    }, DEBTORS_AND_CREDITORS);
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Trans.LedgerId', function () {
        $rootScope.LedgerId = $scope.Trans.LedgerId;
    });
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
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        $scope.addWorkOrder(fileList);



    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    $scope.addWorkOrder = function (fileList) {
        var model = cloneObj($scope.Trans);
        model.PurchaseDate = formatdate(model.PurchaseDate);
        if (model.VendorCreditNoteDate)
            model.VendorCreditNoteDate = formatdate(model.VendorCreditNoteDate);

      //  var purchase = new $.Transaction();
        PurchaseService.save(model).then(function (e) {
            if (e.status != 200) {
                alert(e.data.Message);
                return;
            }
            alert('saved');
            $scope.warnOnLeave = false;
            $state.go('purretlist');
        });
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        //$scope.SubTotal(0);
    });

    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate < 0) {
            alert("Rate can't be 0 or less.");
            return;
        }

        var itemExist = $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId && o.Rate == $scope.TransItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
            itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;

        } else {
            $scope.TransItem.SubTotal = $scope.TransItem.Quantity * $scope.TransItem.Rate;

            $scope.Trans.Items.push($scope.TransItem);
        }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }

    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;

        if ($scope.Trans.Items) {
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}
            if ($scope.Trans.Items) {
                for (var i = 0; i < $scope.Trans.Items.length; i++) {
                    var item = $scope.Trans.Items[i];
                    $scope.applyDiscount(item);
                    $scope.applyTaxRate(item.ProductId);
                }
            }
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        $scope.calculateDiscount();
        //}
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        return $scope.Trans.SubTotal;

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.CostPrice;

        }
    };
    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {

        //$scope.TransItem.Rate = 0;
        var taxes = StaicData.TAX_CATEGORY;

        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);

                if (tax) {
                    for (var i = 0; i < lineItems.length; i++) {
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                        lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        lineItems[i].TaxName = tax.TaxName;
                    }
                }
            }
        }

    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');
    $scope.warehouses = [];
    WarehouseService.getWarehouses().then(function (wres) {
        if (wres.data && wres.data.Data) {
            $scope.warehouses = wres.data.Data;
        }
        var tok = AuthenticationService.getTokenInfo();
        if (tok && tok.DefaultWareHouseId && (!$scope.Trans.PurchaseId || $scope.Trans.PurchaseId === 0)) {
            if (!$scope.Trans.WarehouseId) {
                $scope.Trans.WarehouseId = tok.DefaultWareHouseId;
            }
        }
    });
});

//Purcahse Order controllers
app.controller('PurchaseOrderListController', function ($scope, $rootScope, $crypto, $state, $window, $rootScope, $mdDialog, ReportService) {

    var purchase = new $.Transaction({ PurchaseId: 0, PurchaseType: 3 });

    var date = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
    });
    var ledger = new $.Ledger({});
    //ledger.GetAll(function (e) {

    //    $scope.Accounts = e.data;
    //    if ($scope.Filter.LedgerId == null) {
    //        $scope.Filter.LedgerId = e.data[0].LedgerId;
    //    }
    //});
    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = $scope.Accounts.LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);
    $scope.PurchaseRegister = function () {

        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.PurchaseType = 3;
        purchase.PurchaseRegister(function (e) {
            $scope.Register = e.data;

        }, filter)
    };
    $scope.PurchaseRegister();
    $scope.PurchaseItemsTax = function (index) {

        loadTaxes(index.PurchaseId);
    };
    function loadTaxes(PurchaseId) {
        purchase.PurchaseId = PurchaseId;
        purchase.PurchaseItemsTax(function (e) {
            $scope.AppliedTaxes = e.data;
            var div = '<div style="width:90%;height:70%"></div>';

            $(div).load('templ/taxItems.html', function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        AppliedTaxes: $scope.AppliedTaxes
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: function (AppliedTaxes) {
                        $scope.closeSliderModal = function () {
                            $mdDialog.hide();
                        }
                    }
                });
            });
        });
    }
    $scope.ShowPurchaseItems = function (index) {

        loadItems(index.PurchaseId);
    };
    function loadItems(purchaseId) {

        var div = '<div style="width:90%;height:70%"></div>';

        $(div).load('templ/dialogs/purchaseItems.html', function () {
            var html = $(this).html();

            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    purchaseId: purchaseId
                },
                template: html,
                parent: angular.element(document.body),
                controller: 'PurchaseItemsController'
            });
        });
    }

    $scope.print = function (item, format) {
        var filter = { 'PurchaseId': item.PurchaseId, 'FileFormat': format };
        //  purchase.PurchaseId = item.PurchaseId;
        purchase.PrintPurchaseReceipt(function (e) {
            //debugger
            var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
        }, filter);
    }
    //PurchaseItemsList

    $scope.edit = function (item) {
        var key = $crypto.encrypt(item.PurchaseId);

        $('#previewDialog').modal('hide');
        setTimeout(() => {
            $state.go('editpurchaseorder', { key: key });
        }, 500);

    }
    $scope.delete = function (item) {
        var c = confirm('Are you sure to delete this record');
        if (!c) {
            return;
        }
        var model = { PurchaseId: item.PurchaseId };

        purchase.DeletePurchase(function (o) {
            if (o.data.Code != 200) {
                alert(e.data.Message);
            }
            else {
                alert('Record deleted successfully.');
                $('#previewDialog').modal('hide');
                $scope.PurchaseRegister();
            }
        }, model);

    }
    $scope.Preview = 0;
    $scope.SelectedItem;
    $scope.preview = function (item) {
        $scope.SelectedItem = item;
        $scope.Preview = 1;
        $('#previewDialog').modal('show');

        var strInput = "purchaseorder," + item.PurchaseId
        var encrypedText = $crypto.encrypt(strInput);

        //var econded = btoa(encrypedText);
        //    var report = new $.Reports();

        ReportService.loadPreviewFromReportServer(function (e) {
            $scope.Preview = null;
            $('#rpt').html(e.data);

        }, encrypedText);
    }

    $scope.printPdf = function () {

        var item = $scope.SelectedItem;
        var strInput = "purchaseorder," + + item.PurchaseId;
        var encrypedText = $crypto.encrypt(strInput);

        //var econded = btoa(encrypedText);
        //var report = new $.Reports();
        ReportService.printFromReportServer(encrypedText);
    }
});
app.controller('PurchaseOrderController', function ($scope, $rootScope, $state, $uibModal, WarehouseService,
    AuthenticationService, PurchaseService) {

    var purchase = new $.Transaction({ PurchaseId: 0, PurchaseType: 3 });
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});

    $scope.Trans = purchase;
    $scope.Trans.Items = [];//initializeArray();
    init();
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    accGroup.GetAccountsByGroup(function (e) {
        $scope.Banks = e.data;

    }, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    //accGroup.GetAccountsByGroup(function (e) {

    //    $scope.SundryDebtors = e.data;

    //}, { AccountGroupId: Enums.SUNDRY_CREDITORS });
    ledgerDTO.GetAllByGroups(function (e) {

        $scope.SundryDebtors = e.data.Data;


    }, DEBTORS_AND_CREDITORS);
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Trans.LedgerId', function () {
        $rootScope.LedgerId = $scope.Trans.LedgerId;
    });
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
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        $scope.addWorkOrder(fileList);



    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    $scope.addWorkOrder = function (fileList) {

        var model = cloneObj($scope.Trans);
        model.PurchaseDate = formatdate(model.PurchaseDate);

        model.PurchaseType = 3;
        PurchaseService.save(model).then(function (e) {
            if (e.status != 200) {
                alert(e.data.Message);
                return;
            }
            alert('saved');
            $scope.warnOnLeave = false;
            $state.go('polist');
        });
        //$scope.Trans.SavePurchase(function (e) {
        //    if (e.status == 200) {

        //        alert('saved');
        //        $scope.warnOnLeave = false;
        //        $state.go('polist');
        //    } else {
        //        showMessage(MessageClass.SAVED);
        //    }

        //}, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        //$scope.SubTotal(0);
    });

    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate < 0) {
            alert("Rate can't be 0 or less.");
            return;
        }

        var itemExist = $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId && o.Rate == $scope.TransItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
            itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;

        } else {
            $scope.TransItem.SubTotal = $scope.TransItem.Quantity * $scope.TransItem.Rate;

            $scope.Trans.Items.push($scope.TransItem);
        }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }

    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;

        if ($scope.Trans.Items) {
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}
            if ($scope.Trans.Items) {
                for (var i = 0; i < $scope.Trans.Items.length; i++) {
                    var item = $scope.Trans.Items[i];
                    $scope.applyDiscount(item);
                    $scope.applyTaxRate(item.ProductId);
                }
            }
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        $scope.calculateDiscount();
        //}
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        return $scope.Trans.SubTotal;

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.CostPrice;

        }
    };
    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {

        // $scope.TransItem.Rate = 0;
        var taxes = StaicData.TAX_CATEGORY;

        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);

                if (tax) {
                    for (var i = 0; i < lineItems.length; i++) {
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                        lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        lineItems[i].TaxName = tax.TaxName;
                    }
                }
            }
        }

    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');
    $scope.warehouses = [];
    WarehouseService.getWarehouses().then(function (wres) {
        if (wres.data && wres.data.Data) {
            $scope.warehouses = wres.data.Data;
        }
        var tok = AuthenticationService.getTokenInfo();
        if (tok && tok.DefaultWareHouseId && (!$scope.Trans.PurchaseId || $scope.Trans.PurchaseId === 0)) {
            if (!$scope.Trans.WarehouseId) {
                $scope.Trans.WarehouseId = tok.DefaultWareHouseId;
            }
        }
    });
});

app.controller('PurchaseOrderEditController', function ($scope, $rootScope, $stateParams, $state, $crypto, $location,
    $uibModal, WarehouseService, AuthenticationService, PurchaseService) {

    var purchase = new $.Transaction({ PurchaseId: 0, PurchaseType: 3 });

    if (!$stateParams.key) {
        alert('Invalid key to edit');
        return;
    }
    purchase.PurchaseId = $crypto.decrypt($stateParams.key);

    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});





    $scope.Trans = purchase;
    $scope.Trans.Items = [];//initializeArray();
    init();

    if (purchase.PurchaseId > 0) {
        purchase.GetPurchaseById(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Trans = e.data.Data;
            $scope.Trans.PurchaseDate = convertDate(new Date(moment(e.data.Data.PurchaseDate)));

            $scope.Trans.DiscountValue = $scope.Trans.DiscountPercent;

        }, purchase.PurchaseId);
    }

    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    accGroup.GetAccountsByGroup(function (e) {
        $scope.Banks = e.data;

    }, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    //accGroup.GetAccountsByGroup(function (e) {

    //    $scope.SundryDebtors = e.data;

    //}, { AccountGroupId: Enums.SUNDRY_CREDITORS });
    ledgerDTO.GetAllByGroups(function (e) {

        $scope.SundryDebtors = e.data.Data;


    }, DEBTORS_AND_CREDITORS);
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Trans.LedgerId', function () {
        $rootScope.LedgerId = $scope.Trans.LedgerId;
    });
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
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        $scope.addWorkOrder(fileList);



    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    $scope.addWorkOrder = function (fileList) {
        var model = cloneObj($scope.Trans);
        model.PurchaseDate = formatdate(model.PurchaseDate);


      //  var purchase = new $.Transaction();
        PurchaseService.save(model).then(function (e) {
            if (e.status != 200) {
                alert(e.data.Message);
                return;
            }
            alert('saved');
            $scope.warnOnLeave = false;
            $state.go('polist');
        });
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        //$scope.SubTotal(0);
    });

    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate < 0) {
            alert("Rate can't be 0 or less.");
            return;
        }

        var itemExist = $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId && o.Rate == $scope.TransItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
            itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;

        } else {
            $scope.TransItem.SubTotal = $scope.TransItem.Quantity * $scope.TransItem.Rate;

            $scope.Trans.Items.push($scope.TransItem);
        }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }

    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;

        if ($scope.Trans.Items) {
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}
            if ($scope.Trans.Items) {
                for (var i = 0; i < $scope.Trans.Items.length; i++) {
                    var item = $scope.Trans.Items[i];
                    $scope.applyDiscount(item);
                    $scope.applyTaxRate(item.ProductId);
                }
            }
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        $scope.calculateDiscount();
        //}
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        return $scope.Trans.SubTotal;

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.CostPrice;

        }
    };
    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {

        //$scope.TransItem.Rate = 0;
        var taxes = StaicData.TAX_CATEGORY;

        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);

                if (tax) {
                    for (var i = 0; i < lineItems.length; i++) {
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                        lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        lineItems[i].TaxName = tax.TaxName;
                    }
                }
            }
        }

    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');
    $scope.warehouses = [];
    WarehouseService.getWarehouses().then(function (wres) {
        if (wres.data && wres.data.Data) {
            $scope.warehouses = wres.data.Data;
        }
        var tok = AuthenticationService.getTokenInfo();
        if (tok && tok.DefaultWareHouseId && (!$scope.Trans.PurchaseId || $scope.Trans.PurchaseId === 0)) {
            if (!$scope.Trans.WarehouseId) {
                $scope.Trans.WarehouseId = tok.DefaultWareHouseId;
            }
        }
    });
});