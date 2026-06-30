app.controller('MaterialTransferController', ['$scope', '$rootScope', '$route', '$state', '$stateParams', '$crypto', 'LedgerFactory', 'EmployeeService', 'CompanyService', 'WorkOrderFactory',
    function ($scope, $rootScope, $route, $state, $stateParams, $crypto, LedgerFactory, EmployeeService, CompanyService, WorkOrderFactory) {
        var editWorkOrderId = 0;
        var editGrnId = 0;
        if ($stateParams.WorkOrderId) {
            try {
                var dec = $crypto.decrypt($stateParams.WorkOrderId);
                editWorkOrderId = parseInt(dec, 10) || 0;
            } catch (ex) {
                editWorkOrderId = 0;
            }
        }
        $scope.editMode = editWorkOrderId > 0;
        var _date = new Date();
        $scope.Transfer = {
            DestLedgerId: 0, DestLedgerSiteId: 0, ReceivingDate: convertDate(_date), OtherCharges: [],
            Freight: 0, Weight: 0, Vehicle: '', Driver: '', ChallanNumber: '', Remarks: ''
        };
        $scope.TransferList = [];

        $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, From: '', To: convertDate(new Date()) };
        var token = $rootScope.getTokenInfo();
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }

        function getSites(cb) {
            LedgerFactory.GetMasterSites(function (e) {
                $scope.LedgerSites = e.data.Data;
                if (cb) cb();
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        function getDestinationSites(cb) {
            LedgerFactory.GetMasterSites(function (e) {
                $scope.DestLedgerSites = e.data.Data;
                if (cb) cb();
            }, { LedgerId: $scope.Transfer.DestLedgerId });
        }
        function getAllOtherCharges() {
            WorkOrderFactory.GetOtherCharges(0, function (e) {
                $scope.Transfer.OtherCharges = e.data;
            });
        }
        function getOtherChargesForEdit() {
            WorkOrderFactory.GetOtherCharges(editWorkOrderId, function (e) {
                $scope.Transfer.OtherCharges = e.data;
            });
        }

        function getSiteBalance(cb) {

            var filter = { LedgerId: 0, LedgerSiteId: 0, To: '' };

            filter.LedgerId = $scope.Transfer.SourceLedgerId;
            filter.LedgerSiteId = $scope.Transfer.SourceLedgerSiteId;

            filter.From = formatdate($scope.Transfer.ReceivingDate);
            filter.To = filter.From;
            //LedgerFactory.PartyStockBalance(filter, function (e) {
            //  c
            //    if (cb) cb();
            //});

            $scope.ClientWiseItemsList = [];
            ledger.GetClientWiseItems(function (e) {


                if (e.data.length == 0) {
                    return;
                }
                $scope.Balance = e.data;
                if (cb) cb();
                // $scope.ClientWiseItemsList = e.data;


            }, filter);

        }

        function applySavedQuantities(rows) {

            if (!rows || !$scope.Balance || !$scope.Balance.length) {
                return;
            }
            rows.forEach(function (r) {
                var pid = r.ProductId;
                var psz = r.ProductSizeId || 0;
                var b = $scope.Balance.find(function (x) {
                    return x.ProductId == pid && (x.ProductSizeId || 0) == psz;
                });
                if (b) {
                    b.ClosingBalance += r.Quantity;
                    b.Quantity = r.Quantity;

                }
            });
        }

        function loadMatTransferEdit() {
            var wo = new $.WorkOrder({ WorkOrderId: editWorkOrderId });
            wo.MatTransferById(function (e) {
                if (!e.data || e.data.Code != 200) {
                    alert((e.data && e.data.Message) ? e.data.Message : 'Could not load transfer challan');
                    return;
                }
                var rows = e.data.Data.WorkOrder.Items;
                if (!rows || !rows.length) {
                    alert('No line items found for this transfer.');
                    return;
                }

                var g = e.data.Data.GRN;
                var h = e.data.Data.WorkOrder;
                editGrnId = g.GRNId;
                $scope.Transfer.GRNId = editGrnId;
                $scope.Transfer.WorkOrderId = h.WorkOrderId;
                $scope.Transfer.SourceLedgerId = g.LedgerId;
                $scope.Transfer.SourceLedgerSiteId = g.LedgerSiteId;
                $scope.Transfer.DestLedgerId = h.LedgerId;
                $scope.Transfer.DestLedgerSiteId = h.LedgerSiteId;
                $scope.Transfer.ReceivingDate = convertDate(g.ReceivingDate);
                $scope.Transfer.ChallanNumber = h.Number || '';
                $scope.Transfer.Remarks = h.Remarks || '';
                $scope.Transfer.Freight = g.Freight != null ? g.Freight : 0;
                $scope.Transfer.Weight = h.Weight != null ? h.Weight : 0;
                $scope.Transfer.Driver = h.Driver || '';
                $scope.Transfer.Vehicle = h.Vehicle || '';

                $scope.Filter.LedgerId = g.LedgerId;
                $scope.Filter.LedgerSiteId = g.LedgerSiteId;
                if (rows && rows.length > 0) {
                    var firstRow = rows[0];
                    $scope.Transfer.Freight = firstRow.Freight != null ? firstRow.Freight : 0;
                    $scope.Transfer.Driver = firstRow.Driver || '';
                }
                getSites(function () {
                    getDestinationSites(function () {
                        getSiteBalance(function () {
                            applySavedQuantities(rows);
                        });
                    });
                });
            }, wo);
        }

        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if (e.data != null && e.data.length > 0) {
                $scope.initialValue = e.data[0];
            }
            if ($scope.editMode) {
                getOtherChargesForEdit();
                loadMatTransferEdit();
            } else {
                getAllOtherCharges();
            }
        });

        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Filter.LedgerId = item.LedgerId;
            }
        };
        $scope.$watch('Filter.LedgerId', function () {
            $scope.Transfer.SourceLedgerId = $scope.Filter.LedgerId;
            if ($scope.editMode) {
                return;
            }
            $scope.Balance = [];
            getSites();
        });
        $scope.$watch('Transfer.DestLedgerId', function () {
            getDestinationSites();
        });

        $scope.$watch('Filter.LedgerSiteId', function () {
            $scope.Transfer.SourceLedgerSiteId = $scope.Filter.LedgerSiteId;
            if ($scope.editMode) {
                return;
            }
            $scope.Balance = [];
            $scope.TransferList = [];
            if ($scope.Transfer.SourceLedgerSiteId > 0) {
                getSiteBalance();
            }
        });
        $scope.itemSelected = function (index) {
            var item = $scope.Balance[index - 1];
            if (item) {
                $scope.IssueItem.Item = item.Item;
                $scope.IssueItem.ProductId = item.ProductId;
                $scope.IssueItem.ProductSizeId = item.ProductSizeId;
            }
        };
        $scope.addToList = function (index) {

            if ($scope.IssueItem.Quantity > 0) {
                var found = $scope.TransferList.find(o => o.ProductId == $scope.IssueItem.ProductId);
                var sourceItem = $scope.Balance.find(o => o.ProductId == $scope.IssueItem.ProductId);

                if (sourceItem && sourceItem.ClosingBalance < $scope.IssueItem.Quantity) {
                    alert('Can not transfer more than balance');
                    return;
                }

                if (found) {
                    found.Quantity += $scope.IssueItem.Quantity;
                }
                else {
                    $scope.TransferList.push($scope.IssueItem);
                }
                $scope.IssueItem = {};
                $scope.IssueItem.ProductId = 0;
            }
        };
        $scope.remove = function (index) {
            $scope.TransferList.splice(index, 1);
        }

        FormsValidation.init();
        $scope.TransferMaterial = function () {

            var m = $('#form-matTransfer').valid();
            if (!m) {
                return;
            }

            $scope.Transfer.Items = ($scope.Balance || []).filter(function (o) { return o.Quantity != 0 && o.Quantity != null; });
            if ($scope.Transfer.Items.length == 0) {
                alert('Please select items to transfer');
                return;
            }
            if ($scope.Filter.LedgerSiteId == $scope.Transfer.DestLedgerSiteId) {
                alert('Can not transfer to same site');
                return;
            }
            var wo = new $.WorkOrder({});
            var model = cloneObj($scope.Transfer);
            model.ReceivingDate = formatdate(model.ReceivingDate);
            if ($scope.Config.allowEditChallanNumber) {
                if (!model.ChallanNumber || model.ChallanNumber.length < 1) {
                    alert('Please enter the challan number');
                    return;
                }
            }
            if ($scope.editMode) {
                model.WorkOrderId = editWorkOrderId;
                model.GRNId = editGrnId;
            }
            wo.TransferMaterial(function (e) {
                if (e.data.Code == 200) {
                    alert($scope.editMode ? 'Transfer challan updated' : 'Material Transferred');
                    $state.go('issuedList', {}, { reload: true });
                    //if ($scope.editMode && $state && $state.go) {
                    //    $state.go('issuedList', {}, { reload: true });
                    //} else {
                    //    $route.reload();
                    //}
                }
                else if (e.data.Code == 500) {
                    alert(e.data.Message);
                }
            }, model);
        }

        $scope.Config = {
            rateType: 2, showRateOf: 2, applyTax: true, maintainInventory: 1,
            allowEditChallanNumber: false, freightTax: false, otherChargesTax: false
        };
        function getChallanConfig() {
            var config = new $.Config();
            config.GetAllConfig(function (e) {

                var response = e.data;
                if (response.Data != null && response.Data) {
                    if (response.Data.length > 0) {

                        var inventoryConfig = response.Data.filter(o => o.Category == 'inventory');
                        var general = response.Data.filter(o => o.Category == 'general');

                        var challanConfig = response.Data.filter(o => o.Category == 'ISSUECHALLAN');
                        var rateType = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'rateType');

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

                        if (addInfo && $scope.WorkOrder) {
                            $scope.Config.addInfo = $scope.WorkOrder.Remarks = addInfo.Value;
                        }

                        var tnc = challanConfig.find(o => o.SubCategory == 'ISSUECHALLAN' &&
                            o.Key == 'tnc');

                        if (tnc && $scope.WorkOrder) {
                            $scope.Config.tnc = $scope.WorkOrder.Tnc = tnc.Value;
                        }


                    }

                }
            });
        }

        getChallanConfig();
    }]);
