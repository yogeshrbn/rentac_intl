app.controller('InventoryController', ['$scope', '$http', '$stateParams', '$rootScope', '$location', '$window', 'WorkOrderFactory',
    'ModalFactory',
    'LedgerFactory', 'toaster', '$filter', '$crypto', '$state', 'ChallanService', 'WarehouseService', '$timeout', 'AuthenticationService',
    function ($scope, $http, $stateParams, $rootScope, $location, $window, WorkOrderFactory, ModalFactory, LedgerFactory,
        toaster, $filter, $crypto, $state, ChallanService, WarehouseService, $timeout, AuthenticationService) {
        // var grnId = $routeParams.grnId == undefined ? 0 : $routeParams.grnId;

        $scope.ChallanType = $state.current.data.challanType;
        var grnId = $stateParams.GRNId == undefined ? 0 : $stateParams.GRNId;
        //  $scope.JobCardId = $stateParams.JobCardId == undefined ? 0 : $stateParams.JobCardId;
        var JobCardId = $stateParams.JobCardId == undefined ? 0 : $stateParams.JobCardId;
        if (JobCardId) {
            var jcencKey = $crypto.decrypt(JobCardId).split(',');
            $scope.JobCardId = parseInt(jcencKey[0]);

            var contractId = parseInt(jcencKey[1]);
            if (contractId > 0) {
                var contract = new $.Contract();

                contract.GetById(function (e) {
                    if (e.data.Data) {
                        if ($scope.GRN) {
                            $scope.Filter.LedgerId = e.data.Data.Ledger.LedgerId;
                            $scope.Filter.LedgerSiteId = e.data.Data.LedgerSiteId;
                        }
                    }
                }, { ContractId: contractId });
            }

        }
        $scope.LastChallanNo = '';
        $scope.NextChallanNumber = '';
        function applyReceivingChallanPreview() {
            if (grnId !== 0) return;
            if (!$scope.NextChallanNumber) return;
            if ($scope.allowEditChallanNumber === true) return;
            if ($scope.GRN) {
             //   $scope.GRN.GRN = $scope.NextChallanNumber;
            }
        }
        function refreshChallanNumberHints() {
            if (grnId != 0) return;
            var woHint = new $.WorkOrder();
            woHint.GetLastReturnChallanNo(function (e) {
                $scope.LastChallanNo = e.data.Data;
            }, { ChallanType: $scope.ChallanType });
            woHint.GetNextReceivingChallanNumberPreview(function (e) {
                $scope.NextChallanNumber = '';
                if (e.data && (e.data.Code === 200 || e.data.Code === '200') && e.data.Data != null && e.data.Data !== '') {
                    $scope.NextChallanNumber = String(e.data.Data);
                }
                applyReceivingChallanPreview();
            }, { ChallanType: $scope.ChallanType });
        }
        $scope.editorOptions = {

            height: 100

        };
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

        if (grnId != 0)
            grnId = $crypto.decrypt(grnId);

        var site = new $.Site({});
        var _siteNames = [];
        var selectedGrnItemIndex = -1;

        var date = new Date();
        var token = $rootScope.getTokenInfo();
        var edit = false;
        $scope.edit = edit;

        if (token != null) {
            $scope.MinDate = token.FinYearStart;

        }
        function newRecord() {
            $scope.edit = false;
            grnId = 0;
            site = new $.Site({});
            _siteNames = [];
            selectedGrnItemIndex = -1;

            date = new Date();
            token = $rootScope.getTokenInfo();
            edit = false;
            init();
            refreshChallanNumberHints();
        }
        function undo() {
            $scope.edit = false;
            setTimeout(function () {
                $scope.$apply()
            }, 50);

            nav_onToolbarClick(last_command);
        }

        $rootScope.$on('onToolbarClick', function (e, args) {

            nav_onToolbarClick(args);
        });
        nav_Items = [];

        // nav_init();
        function nav_onToolbarClick(args) {
            switch (args) {
                case 'first': nav_first(); nav_onItemSelection(); break;
                case 'previous': nav_previous(); nav_onItemSelection(); break;
                case 'next': nav_next(); nav_onItemSelection(); break;
                case 'last': nav_last(); nav_onItemSelection(); break;
                case 'search': nav_search(); break;
                case 'print': nav_print(); break;
                case 'new': newRecord(); break;
                case 'undo': undo(); break;
                case 'edit': edit = !edit;
                    $scope.edit = edit;
                    setTimeout(function () {
                        $scope.$apply()
                    }, 50); break;
            }

            console.log('Navigation command', args, nav_selected);

        }
        function nav_init() {

            // feth list set nav_items
            //
            var w = new $.WorkOrder();
            w.GetWorkOrdersByCompany(function (e) {
                console.log('workOrders', e);
                nav_Items = e.data;
                nav_count = nav_Items.length;
                nav_first();
                nav_onItemSelection();
            }, $scope.Filter);


            nav_initialized = true;

        }
        //nav_init();

        function nav_search() {
            alert('nav_search');
        }
        function nav_print() {
            alert('nav_print');
        }

        function nav_onItemSelection() {

            if (nav_selectedIndex > -1) {

                //$scope.WorkOrder.LedgerId = nav_selected.LedgerId;
                //$scope.WorkOrder.SiteInfo.LedgerSiteId = nav_selected.LedgerSiteId;
                //$scope.GRN.WorkOrderId = nav_selected.WorkOrderId;
                grnId = nav_selected.GrnId;
                init();
                edit = false;
                $scope.edit = false;
                setInterval(function () {
                    $scope.$apply()
                }, 50);
            }
        }




        $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date), code: 'GRN', ChallanType: $scope.ChallanType };
        //nav_init();
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }

        //  $scope.Filter = { 'LedgerId': 0, From: convertDate(date) };
        var ledger = new $.Ledger({});
        $scope.InvalidInput = false;

        function newDamageRow(name, quantity, rate, cost) {
            return {
                rowId: 'r_' + Date.now() + '_' + Math.floor(Math.random() * 100000),
                name: name != null ? name : '',
                quantity: quantity != null && quantity !== '' ? quantity : '',
                rate: rate != null && rate !== '' ? rate : '',
                cost: cost != null && cost !== '' ? cost : ''
            };
        }
        function parseDamageComponentJson(val) {
            if (!val) return [];
            try {
                var parsed = JSON.parse(val);
                return Array.isArray(parsed) ? parsed : [];
            } catch (ex) {
                return [];
            }
        }
        function damageJsonIsLegacy(parsed) {
            if (!parsed || !parsed.length) return false;
            var first = parsed[0];
            return !!(first && (first.components || first.Components));
        }
        function normalizeDamageComponentDetails(item) {
            if (!item) return;
            var existing = item.DamageComponentDetails;
            if (!Array.isArray(existing) || existing.length === 0) {
                var parsed = parseDamageComponentJson(item.DamageComponent);
                existing = [];
                if (damageJsonIsLegacy(parsed)) {
                    parsed.forEach(function (p) {
                        var comps = (p && (p.components || p.Components)) || [];
                        comps.forEach(function (c) {
                            var name = typeof c === 'string' ? c : (c && (c.name || c.Name) ? (c.name || c.Name) : '');
                            var costVal = typeof c === 'object' && c ? (c.cost != null ? c.cost : c.Cost) : '';
                            existing.push(newDamageRow(name, '', '', costVal));
                        });
                    });
                } else {
                    parsed.forEach(function (c) {
                        if (!c || typeof c !== 'object') return;
                        existing.push(newDamageRow(
                            c.name || c.Name || '',
                            c.quantity != null ? c.quantity : (c.Quantity != null ? c.Quantity : ''),
                            c.rate != null ? c.rate : (c.Rate != null ? c.Rate : ''),
                            c.cost != null ? c.cost : (c.Cost != null ? c.Cost : '')
                        ));
                    });
                }
            }
            existing = existing || [];
            if (existing.length === 0) {
                existing.push(newDamageRow('', '', '', ''));
            }
            existing.forEach(function (r) {
                if (!r.rowId) {
                    r.rowId = 'r_' + Date.now() + '_' + Math.floor(Math.random() * 100000);
                }
                if (r.name == null) r.name = '';
                if (r.quantity == null) r.quantity = '';
                if (r.rate == null) r.rate = '';
                if (r.cost == null) r.cost = '';
            });
            item.DamageComponentDetails = existing;
        }
        function serializeDamageComponentDetails(item) {
            normalizeDamageComponentDetails(item);
            var lines = [];
            (item.DamageComponentDetails || []).forEach(function (d) {
                var name = (d.name || '').trim();
                if (!name) return;
                var q = parseFloat(d.quantity);
                var r = parseFloat(d.rate);
                var costExplicit = parseFloat(d.cost);
                var costNum;
                if (!isNaN(q) && !isNaN(r)) {
                    costNum = Math.round(q * r * 100) / 100;
                } else if (!isNaN(costExplicit)) {
                    costNum = costExplicit;
                } else {
                    costNum = 0;
                }
                lines.push({
                    name: name,
                    quantity: isNaN(q) ? 0 : q,
                    rate: isNaN(r) ? 0 : r,
                    cost: costNum
                });
            });
            item.DamageComponent = lines.length > 0 ? JSON.stringify(lines) : '';
        }
        $scope.damageRowCost = function (row) {
            if (!row) return 0;
            var q = parseFloat(row.quantity);
            var r = parseFloat(row.rate);
            if (!isNaN(q) && !isNaN(r)) {
                return Math.round(q * r * 100) / 100;
            }
            var c = parseFloat(row.cost);
            return isNaN(c) ? 0 : c;
        };
        $scope.onDamageQtyOrRateChange = function (row) {
            if (!row) return;
            var q = parseFloat(row.quantity);
            var r = parseFloat(row.rate);
            if (!isNaN(q) && !isNaN(r)) {
                row.cost = Math.round(q * r * 100) / 100;
            }
        };
        $scope.getDamageComponentSummary = function (item) {
            if (!item) return '';
            function summarizeDetails(details) {
                var cnt = 0, sum = 0;
                if (!details || !details.length) return '';
                details.forEach(function (d) {
                    if ((d.name || '').trim().length) {
                        cnt++;
                        sum += $scope.damageRowCost(d);
                    }
                });
                return cnt === 0 ? '' : (cnt + ' component(s) · ' + sum.toFixed(2));
            }
            if (item.DamageComponent) {
                var parsed = parseDamageComponentJson(item.DamageComponent);
                var cnt = 0;
                var sum = 0;
                if (damageJsonIsLegacy(parsed)) {
                    parsed.forEach(function (p) {
                        var comps = (p && (p.components || p.Components)) || [];
                        comps.forEach(function (c) {
                            var name = typeof c === 'string' ? c : (c && (c.name || c.Name) ? (c.name || c.Name) : '');
                            if ((name || '').trim().length) {
                                cnt++;
                                var co = typeof c === 'object' && c ? parseFloat(c.cost != null ? c.cost : c.Cost) : 0;
                                if (!isNaN(co)) sum += co;
                            }
                        });
                    });
                } else {
                    parsed.forEach(function (c) {
                        if (!c || typeof c !== 'object') return;
                        var name = c.name || c.Name || '';
                        if ((name || '').trim().length) {
                            cnt++;
                            var q = parseFloat(c.quantity != null ? c.quantity : c.Quantity);
                            var r = parseFloat(c.rate != null ? c.rate : c.Rate);
                            var co = parseFloat(c.cost != null ? c.cost : c.Cost);
                            if (!isNaN(q) && !isNaN(r)) {
                                sum += Math.round(q * r * 100) / 100;
                            } else if (!isNaN(co)) {
                                sum += co;
                            }
                        }
                    });
                }
                if (cnt > 0) {
                    return cnt + ' component(s) · ' + sum.toFixed(2);
                }
            }
            return summarizeDetails(item.DamageComponentDetails);
        };
        $scope.addDamageComponentRow = function () {
            if (!$scope.selectedDamageItem) return;
            if (!$scope.selectedDamageItem.DamageComponentDetails) {
                $scope.selectedDamageItem.DamageComponentDetails = [];
            }
            $scope.selectedDamageItem.DamageComponentDetails.push(newDamageRow('', '', '', ''));
        };
        $scope.removeDamageComponentRow = function (row) {
            if (!$scope.selectedDamageItem || !row) return;
            var rows = $scope.selectedDamageItem.DamageComponentDetails || [];
            if (rows.length <= 1) return;
            var idx = rows.indexOf(row);
            if (idx >= 0) {
                rows.splice(idx, 1);
            }
        };
        $scope.modalDamageGrandTotal = function () {
            if (!$scope.selectedDamageItem || !$scope.selectedDamageItem.DamageComponentDetails) return 0;
            var sum = 0;
            $scope.selectedDamageItem.DamageComponentDetails.forEach(function (r) {
                if ((r.name || '').trim().length === 0) return;
                var v = $scope.damageRowCost(r);
                if (!isNaN(v)) sum += v;
            });
            return sum;
        };
        $scope.openDamageComponentEditor = function (item) {
            normalizeDamageComponentDetails(item);
            $scope.selectedDamageItem = item;
            $('#damageComponentModal').modal('show');
        };
        $scope.saveDamageComponentEditor = function () {
            if ($scope.selectedDamageItem) {
                serializeDamageComponentDetails($scope.selectedDamageItem);
            }
            $('#damageComponentModal').modal('hide');
        };
        $scope.AddItem = function () {
            var challanItem = cloneObj($scope.RecItem);
            normalizeDamageComponentDetails(challanItem);
            serializeDamageComponentDetails(challanItem);
            $scope.addToChallan(challanItem);
            $scope.RecItem = new $.WorkOrderItem({});
            $('#itemSelect_value').focus();
            $('#itemSelect_value').val('');
            var items = $scope.GRN.Items;

            // $scop.GRN.Items = items.sort();
        }

        $scope.addToChallan = function (challanItem) {

            var m = challanItem.ProductId == 0;

            if (m) {
                return;
            }

            if (isNaN(parseFloat(challanItem.Quantity))) {
                challanItem.Quantity = 0;
            }
            if (parseInt(challanItem.Quantity) == 0 && parseInt(challanItem.Breakage) == 0
                && parseInt(challanItem.ExcessQty) == 0 && parseInt(challanItem.ShortQty) == 0) {
                return;
            }
            if ($scope.GRN.LedgerId == 0 || !$scope.GRN.LedgerId) {
                toaster.pop('error', "error", 'Please select party and site.');
                return;
            }
            if ($scope.BalanceItems) {

                var bItem = $scope.BalanceItems.find(o => o.ProductId == challanItem.ProductId);
                if ($scope.negativeReceiving == false) {
                    if (!bItem) {
                        toaster.pop('error', 'error', challanItem.Item + ' not available in sent list.');
                        return;
                    }

                }
            }
            challanItem.Deleted = 0;
            challanItem.Quantity = parseFloat(challanItem.Quantity);
            var item = $scope.GRN.Items.find(o => o.ProductId == challanItem.ProductId);
            //var itemCopy = $scope.GRNItems.find(o => o.ProductId == $scope.RecItem.ProductId);
            if (item) {
                item.Deleted = 0;
                var qty = parseFloat(item.Quantity);
                item.Quantity = qty + parseFloat(challanItem.Quantity);
                //    item.Quantity = item.ReceivingQty;
                return;
            }
            //  challanItem.NewQty = challanItem.ReceivingQty;
            //  challanItem.Quantity = challanItem.ReceivingQty;
            // challanItem.ShortQty = 0;
            //  challanItem.ExcessQty = 0;
            if (challanItem.BOM && challanItem.BOM.length > 0) {
                $scope.GRN.Items.unshift(challanItem);
            }
            else {
                $scope.GRN.Items.push(challanItem);
            }

            if ((challanItem.GroupItemId == 0 || !challanItem.GroupItemId) && $scope.showRateOf == 2) {
                challanItem.Rate = 0;
            }

            if (challanItem.BOM != null) {
                $.each(challanItem.BOM, function (index, value) {

                    value.Quantity = value.Quantity * challanItem.Quantity;
                    //  value.Quantity = value.Quantity * challanItem.ReceivingQty;
                    value.Item = value.Product;
                    value.Rate = 0;
                    value.GroupItemId = challanItem.ProductId;
                    value.Breakage = 0;
                    value.BreakageRate = 0;
                    value.DamageComponent = '';
                    value.DamageComponentDetails = [];
                    if (!$scope.showRateOf)
                        $scope.applyDefaultRate(value);

                    if ($scope.showRateOf == 2 || $scope.showRateOf == 3) {
                        $scope.applyDefaultRate(value);
                    }
                    $scope.addToChallan(value);
                });
            }
        }

        $scope.validateQty = function () {
            var newVal = $scope.RecItem;

            if (newVal.ProductId == 0) return;

            if ($scope.BalanceItems) {
                var bItem = $scope.BalanceItems.find(o => o.ProductId == newVal.ProductId);
                var modifiedItem = angular.copy(newVal);
                var totalReceived = (parseInt(newVal.Quantity, 10) || 0) + (parseInt(newVal.Breakage, 10) || 0);

                if ($scope.disableExcess) {
                    modifiedItem.ExcessQty = 0;
                }
                if (bItem) {
                    if (bItem.ClosingBalance < totalReceived) {
                        if ($scope.disableExcess) {
                            var brk = parseInt(newVal.Breakage, 10) || 0;
                            var clos = parseInt(bItem.ClosingBalance, 10) || 0;
                            modifiedItem.Quantity = Math.max(0, clos - brk);
                            modifiedItem.ExcessQty = 0;
                        } else {
                            modifiedItem.ExcessQty = Math.abs(bItem.ClosingBalance - totalReceived);
                            modifiedItem.Quantity = modifiedItem.Quantity - modifiedItem.ExcessQty;
                        }
                    }
                    //else {
                    //    modifiedItem.ExcessQty = 0;
                    //}
                }
                //else {
                //  //  modifiedItem.ExcessQty = totalReceived;
                //    modifiedItem.Quantity = 0;
                //}

                // Only update if the objects are different
                if (!angular.equals(newVal, modifiedItem)) {
                    $timeout(function () {
                        $scope.RecItem = modifiedItem;
                    });
                }
            }
        }

        $scope.$watch('Filter.LedgerSiteId', function () {

            //   fetchItemsToReceive();
            $scope.GetBalance();
            $scope.GetPartyRates();
        });
        $scope.GetPartyRates = function () {

            var model = {
                LedgerId: $scope.Filter.LedgerId,
                LedgerSiteId: $scope.Filter.LedgerSiteId
            }
            ledger.PartyWiseRates(function (e) {
                $scope.ProductRates = e.data;
            }, model);
        }

        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }

        $scope.$watch('Filter.LedgerId', function () {
            //$scope.Filter.LedgerId = obj.originalObject.LedgerId;
            if ($scope.Filter.LedgerId > 0) {
                // fetchItemsToReceive();
                $scope.GRN.LedgerId = $scope.Filter.LedgerId;
                $scope.getSites();
                //  $scope.GetBalance();
            }
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            $scope.getAllProducts();
        });
        //$scope.$watch('GRN.ReceivingDate', function () {
        //    //$scope.Filter.LedgerId = obj.originalObject.LedgerId;
        //    $scope.GRN.RentStopDate = $scope.GRN.ReceivingDate;
        //    $scope.GetBalance();
        //}, true);

        $scope.onReceivingDateChange = function () {
            $scope.GRN.RentStopDate = $scope.GRN.ReceivingDate;
            $scope.GetBalance();
        }
        function fetchItemsToReceive() {
            $scope.GRN.Items = [];
            loadBalanceToReceivce($scope.Filter.LedgerId);
        }
        function loadBalanceToReceivce(ledgerId) {
            if (ledgerId != undefined) {
                $scope.GRN.LedgerId = ledgerId;
                //  getSites();
                $scope.GetBalance();
            }
        }

        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            if (e.status != 200) {
                alert('Could not load parties');
                return;
            }
            $scope.Accounts = e.data.filter(o => o.AccountGroup == StaicData.SYS_ACCOUNT_GROUPS.SUNDRY_CREDITORS || o.AccountGroup == StaicData.SYS_ACCOUNT_GROUPS.SUNDRY_DEBTORS);
            $scope.selectedParty();

            if (e.data != null && e.data.length > 0 && grnId == 0) {
                $scope.initialValue = e.data[0];
            }
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
                //  $scope.GRN.LedgerId = $scope.Filter.LedgerId;
            }
        });

        $scope.RowSelected = function () { }

        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.GRN.LedgerId = item.LedgerId;
                $scope.Filter.LedgerId = item.LedgerId;
            }
        };

        $scope.selectedProduct = function (selected) {

            if (selected != undefined) {


                // var Product = this.$parent.item.Item = selected.originalObject;//this.$parent.item.Props.Item.Props;
                // $scope.RecItem.Item = selected.originalObject;//this.$parent.item.Props.Item.Props;
                $scope.RecItem.Item = selected.originalObject.Product;
                //  $scope.RecItem.ProductSizeId = selected.originalObject.ProductSizeId;
                $scope.RecItem.ProductId = selected.originalObject.ProductId;

                $scope.findDefaultRate($scope.RecItem.ProductId);
                $scope.getBomItems($scope.RecItem.ProductId);
            }

        };
        $scope.getSelectedItemBalance = function () {
            if (!$scope.RecItem || !$scope.RecItem.ProductId || !$scope.BalanceItems) return null;
            var bItem = $scope.BalanceItems.find(function (o) { return o.ProductId == $scope.RecItem.ProductId; });
            var bal = bItem ? bItem.ClosingBalance : null;
            return (bal !== undefined && bal !== null && !isNaN(parseFloat(bal))) ? (parseFloat(bal) || 0) : null;
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
                $scope.RecItem.BOM = e.data.Data.Details;
            }, { ProductId: productId });
        }
        $scope.OnItemDeleted = function (index) {
            $scope.$apply(function () {
                $scope.GRN.Items.splice(index - 1, 1);
            });
        }
        $scope.DeleteItem = function (index) {

            //var item = $scope.GRN.Items[index];
            //$scope.GRN.Items[index].Deleted = 1;
            //$scope.GRN.Items[index].ReceivingQty = 0
            //$scope.GRN.Items[index].Quantity = 0;
            ////  $scope.GRN.Items.splice(index, 1);
            //$scope.GRN.Items = $scope.GRN.Items.filter(o => o.GroupItemId != item.ProductId);

            var item = $scope.GRN.Items.filter(o => o.Deleted != 1)[index];

            $scope.getBomItems(item.ProductId, function (bom) {
                item.BOM = bom;
                $.each(item.BOM, function (index, value) {

                    var bomItem = $scope.GRN.Items.find(o =>
                        o.ProductId == value.ProductId
                    );
                    if (bomItem) {
                        bomItem.Quantity -= value.Quantity * item.Quantity;

                        if (bomItem.Quantity <= 0) {
                            bomItem.Deleted = 1;
                        }
                    }
                });
                $scope.GRN.Items.filter(o => o.Deleted != 1)[index].Quantity = 0;
                $scope.GRN.Items.filter(o => o.Deleted != 1)[index].Deleted = 1;



            });
        }
        $scope.getSites = function () {
            _siteNames = [];
            //ledger.GetSites(function (e) {

            //    for (var i in e.data) {
            //        _siteNames.push({ JobNumber: e.data[i].JobNumber, WorkOrderId: e.data[i].WorkOrderId, Site: e.data[i].Site });
            //    }
            //    $scope.Site = _siteNames;
            //}, $scope.Filter);

            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });


        }

        function init() {

            $scope.GRN = new $.GRN({ GRNId: grnId });
            $scope.GRN.ReceivingDate = convertDate(new Date());
            $scope.GRN.RentStopDate = convertDate(new Date());
            $scope.RecItem = new $.WorkOrderItem({});
            $scope.GRN.Items = [];//initializeArray();
            if ($scope.Filter) {
                loadBalanceToReceivce($scope.Filter.LedgerId);
            }
            _siteNames.push({ WorkOrderId: 0 });
            if ($scope.GRN.GRNId > 0) {
                getItemsByGrnId();
            }
            var product = new $.Product();
            product.GetSizeListByCompany(function (e) {
                $scope.AllSizes = e.data;

            });
        }
        $scope.createEwayBill = false;
        $scope.Save = function (createEwayBill = false) {
             
            var m = $('#form-grn').valid();
            if (!m) {
                return;
            }
            if ($scope.GRN.Items.length == 0) {
                toaster.pop('error', "error", 'Please provide line items.');
                return;
            }
            $scope.createEwayBill = createEwayBill;

            $scope.GRN.WorkOrderId = $scope.Filter.WorkOrderId;
            $scope.GRN.LedgerSiteId = $scope.Filter.LedgerSiteId;
            $scope.GRN.ChallanType = $scope.ChallanType;
            $scope.GRN.JobCardId = $scope.JobCardId;
            if ($scope.negativeReceiving == false) {
                if (!$scope.ValidateBalance()) {
                    return;
                }
            }

            var wsDate = new Date(moment($scope.GRN.ReceivingDate, "DD/MM/YYYY"));
            var bsDate = new Date(moment($scope.GRN.RentStopDate, "DD/MM/YYYY"));

            var days = dateDiff(wsDate, bsDate);
            if (days != 0) {

                var c = confirm('Rent will stop from ' + $scope.GRN.RentStopDate + '. Are you sure to proceed?');
                if (!c) {
                    return;
                }

            }

            if ($scope.disableExcess) {
                applyDisableExcessRules();
            }
            var model = cloneObj($scope.GRN);
            if (model.ShipFrom) {
                model.ShipFrom = htmlEncode(model.ShipFrom);
            }
            if (model.Items && model.Items.length > 0) {
                model.Items.forEach(function (x) {
                    serializeDamageComponentDetails(x);
                    delete x.DamageComponentDetails;
                });
            }
            if (model.GRNId == 0) {
                model.Items = model.Items.filter(o => o.Deleted == 0);
            }
            model.ReceivingDate = formatdate(model.ReceivingDate);
            model.RentStopDate = formatdate(model.RentStopDate);

            //debugger
            //model.Tnc = htmlEncode(model.Tnc);
            //model.Remarks = htmlEncode(model.Remarks);

            $scope.GRN.Add(function (e) {

                if (e.data.Code == 200) {
                    // showMessage(MessageClass.GRN_SAVED);
                    // toaster.pop('success', "success", MessageClass.GRN_SAVED);
                    init();

                    var o = e.data.Data;
                    WorkOrderFactory.PrintItemReceivingSlip([{ GRNId: o.GRNId }], function () {
                        if ($scope.createEwayBill == true) {
                            var k = '0,chl,' + o.GRNId + ',ret';
                            var ec = $crypto.encrypt(k);
                            if ($scope.fromContractInfoModal) {
                                $('#contractReturnChallanModal').modal('hide');
                            }
                            $state.go('addEditEwayBill', { key: ec });
                            return;
                        }
                        if ($scope.fromContractInfoModal && $scope.ChallanType === 11) {
                            $('#contractReturnChallanModal').modal('hide');
                            return;
                        }
                        if ($scope.ChallanType == 2) {
                            toaster.pop('success', "success", MessageClass.GRN_SAVED);
                            $state.go('recvdList');
                        }
                        else if ($scope.ChallanType == 13) {
                            toaster.pop('success', "success", 'Items un-hired successfully.');
                            $state.go('unhiredreg');
                        }
                    });
                } else {
                    //  showMessage(MessageClass.GRN_SAVE_ERROR);
                    toaster.pop('error', "error", e.data.Message);
                }
            }, model, null);
        }

        $scope.cancelGrn = function () {
            if ($scope.ChallanType === 2) {
                $state.go('recvdList');
                return;
            }
            if ($scope.ChallanType === 10) {
                $state.go('hiredreg');
                return;
            }
            if ($scope.ChallanType === 11) {
                if ($scope.fromContractInfoModal) {
                    $('#contractReturnChallanModal').modal('hide');
                    return;
                }
                var jcp = $stateParams.JobCardId;
                if (jcp) {
                    try {
                        var parts = $crypto.decrypt(jcp).split(',');
                        var contractId = parseInt(parts[1], 10);
                        if (contractId > 0) {
                            $state.go('contract', { id: contractId, qId: 0 });
                            return;
                        }
                    } catch (ex) { }
                }
                $state.go('contracts');
                return;
            }
            $window.history.back();
        };

        $scope.ValidateBalance = function () {


            const uniqueArray = $scope.GRN.Items.filter((value, index, self) =>
                index === self.findIndex((t) => (
                    t.ProductId === value.ProductId
                ))
            )

            for (var i = 0; i < uniqueArray.length; i++) {
                var i_item = uniqueArray[0];
                var item = $scope.BalanceItems.find(o => o.ProductId == i_item.ProductId);
                var itemCopy = $scope.GRNItems.find(o => o.ProductId == i_item.ProductId);

                if (item == null || item == undefined) {
                    toaster.pop('error', 'error', i_item.Item + ' not available in sent list.');
                    return false;
                }
                var recItem = $scope.GRN.Items.filter(o => o.ProductId == item.ProductId);
                var totalRecived = $filter('sumByKey')(recItem, 'Quantity');

                if (itemCopy) {
                    totalRecived -= itemCopy.Quantity;
                }


                if (totalRecived > item.ClosingBalance) {
                    toaster.pop('error', 'error', item.Item + ' Can not be received more than total balance');
                    return false;
                }
            }
            return true;
        }
        $scope.GetBalance = function () {
            // $scope.GRN.Items = [];//initializeArray();
            if ($scope.Filter.LedgerId <= 0 || $scope.Filter.LedgerSiteId <= 0) {
                return;
            }
            if (!$scope.Filter.LedgerSiteId) {
                return;
            }
            var model = cloneObj($scope.Filter);
            //model.From = new Date(moment(model.From, "MM/MM/YYYY"))
            //model.To = new Date(moment(model.To, "DD/MM/YYYY"))

            //model.From = convertDate(model.From);
            //model.To = convertDate(model.To);
            if ($scope.ChallanType == 10) {
                model.BalanceType = 'hire';
            }
            model.To = $scope.GRN.ReceivingDate;
            ledger.GetClientWiseItems(function (e) {
                mapStockBalance(e.data);
            }, model);

        }
        $scope.BalanceGroup1 = [];
        $scope.BalanceGroup2 = [];
        function mapStockBalance(data) {
            // $scope.BalanceGroup1 = [];
            // $scope.BalanceGroup2 = [];
            $scope.BalanceItems = data

        }

        function getItemsByGrnId() {
            $scope.GRN.GetById(function (e) {

                if (e.data.Code != 200) {
                    alert('Could not get details');
                    return;
                }
                $scope.GRNItems = e.data.Data.GrnItems;

                // $scope.GRNItems = JSON.parse(JSON.stringify(e.data));
                $scope.GRN.Items = e.data.Data.GrnItems;
                if ($scope.GRN.Items && $scope.GRN.Items.length > 0) {
                    $scope.GRN.Items.forEach(function (it) {
                        normalizeDamageComponentDetails(it);
                    });
                }
                applyDisableExcessRules();
                populateGRNDataForEdit(e.data.Data);
            });
        }
        $scope.GRNItems = [];
        $scope.Items = 0;
        function populateGRNDataForEdit(data) {


            if (data != undefined) {

                $scope.GRN.GRN = data.GRN;
                $scope.Filter.LedgerId = data.LedgerId;
                $scope.Filter.LedgerSiteId = data.LedgerSiteId;

                $scope.GRN.JobNumber = data.JobNumber;
                $scope.GRN.Sender = data.Sender;
                $scope.GRN.Receiver = data.Receiver;
                $scope.GRN.ShipFrom = data.ShipFrom || '';
                $scope.GRN.Remarks = data.Remarks;
                $scope.GRN.Freight = data.Freight;
                $scope.GRN.VehicleNo = data.VehicleNo;
                $scope.GRN.Driver = data.Driver;
                $scope.GRN.Tnc = data.Tnc;
                $scope.GRN.ApproximateValue = data.ApproximateValue || 0;
                $scope.GRN.Weight = data.Weight != null && data.Weight !== undefined ? data.Weight : 0;
                $scope.GRN.LRNumber = data.LRNumber || '';
                $scope.GRN.CRNumber = data.CRNumber || '';
                $scope.GRN.GRNumber = data.GRNumber || '';
                $scope.GRN.PONumber = data.PONumber || '';

                $scope.GRN.WarehouseId = data.WarehouseId;

                $scope.Filter.LedgerId = data.LedgerId;
                $scope.GRN.ReceivingDate = convertDate(data.ReceivingDate);
                $scope.GRN.RentStopDate = convertDate(data.RentStopDate);

                if ($scope.Accounts != null) {
                    var party = $scope.Accounts.find(x => x.LedgerId == data.LedgerId);
                    $scope.initialValue = party;

                }

            }
        }

        function mapProductsToEdit() {
            for (var i = 0; i < $scope.GRN.Items.length > 0; i++) {
                var item = $scope.GRN.Items[i];
                var editItem = $scope.GRNItems.find(o => o.ProductId == item.Item.ProductId);
                if (item.ProdutSizeId) {
                    if (parseInt(item.ProductSizeId, 0) > 0) {
                        editItem = $scope.GRNItems.find(o => o.ProductId == item.Item.ProductId && o.ProductSizeId == item.ProductSizeId);
                    }
                }
                if (editItem) {
                    $scope.GRN.Items[i].Quantity = editItem.Quantity;
                    $scope.GRN.Items[i].Breakage = editItem.Breakage;
                    $scope.GRN.Items[i].BreakageRate = editItem.BreakageRate;
                    if (editItem.DamageComponent != null) {
                        $scope.GRN.Items[i].DamageComponent = editItem.DamageComponent;
                    }
                    $scope.GRN.Items[i].GRNItemId = editItem.GRNItemId;
                    $scope.GRN.Items[i].ChargeReturnedDate = editItem.ChargeReturnedDate;
                }
            }

        }

        $scope.getAllProducts = function () {
            ledger.Props.LedgerId = $scope.GRN.LedgerId;
            ledger.GetProductRates(function (e) {
                $scope.ProductRates = e.data;
            });

        }
        $scope.findDefaultRate = function (productId) {
            $scope.RecItem.BreakageRate = 0;

            if ($scope.ProductRates) {
                var item = $scope.ProductRates.find(o => o.ProductId == productId);
                if (item) {
                    $scope.RecItem.BreakageRate = item.DamageRate;
                    $scope.RecItem.Rate = 0;
                    if ($scope.rateType) {
                        if ($scope.rateType == 2) {
                            $scope.RecItem.Rate = item.RentRate;
                        }
                        else if ($scope.rateType == 1) {
                            $scope.RecItem.Rate = item.SaleRate;
                        }
                        else if ($scope.rateType == 3) {
                            $scope.RecItem.Rate = item.LossRate;
                        }
                    }
                }
            }
        }
        $scope.applyDefaultRate = function (objItem) {


            if ($scope.ProductRates) {
                var item = $scope.ProductRates.find(o => o.ProductId == objItem.ProductId);
                if (item) {
                    objItem.BreakageRate = item.DamageRate;
                    objItem.Rate = 0;
                    if ($scope.rateType) {
                        if ($scope.rateType == 2) {
                            objItem.Rate = item.RentRate;
                        }
                        else if ($scope.rateType == 1) {
                            objItem.Rate = item.SaleRate;
                        }
                        else if ($scope.rateType == 3) {
                            objItem.Rate = item.LossRate;
                        }
                    }
                }
            }
        }
        function printSlip(o) {
            var printController = function ($scope) {
                $scope.Message = 'Are you sure to print?';
                $scope.OkButtonClick = function () {
                    WorkOrderFactory.PrintItemReceivingSlip([{ GRNId: o.GRNId }]);
                };
                $scope.closeDialog = function () {
                    ModalFactory.Dialog.hide();
                };
            };
            ModalFactory.ConfirmToPrint(printController);


        }
        FormsValidation.init();
        init();
        if (grnId == 0) {
            refreshChallanNumberHints();
        }



        var config = new $.Config({});
        $scope.negativeReceiving = true;
        $scope.allowEditChallanNumber = false;
        $scope.disableExcess = false;

        FormsValidation.init('frmBillConfig');
        function applyDisableExcessRules() {
            if (!$scope.disableExcess) return;
            if ($scope.RecItem) {
                $scope.RecItem.ExcessQty = 0;
            }
            if ($scope.GRN && $scope.GRN.Items && $scope.GRN.Items.length > 0) {
                $scope.GRN.Items.forEach(function (it) {
                    it.ExcessQty = 0;
                });
            }
        }
        function GetBillingConfig() {
            config.GetByCategory(function (e) {

                var response = e.data;
                if (response.Data != null && response.Data) {
                    if (response.Data.length > 0) {

                        var chResetInterval = response.Data.find(o => o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'negativeReceiving');

                        if (chResetInterval) {
                            $scope.negativeReceiving = chResetInterval.Value == 'true';
                        }
                        var allowEditChallanNumber = response.Data.find(o => o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'allowEditChallanNumber');
                        if (allowEditChallanNumber) {
                            $scope.allowEditChallanNumber = allowEditChallanNumber.Value == 'true';
                        }
                        var showRate = response.Data.find(o => o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'showRate');
                        if (showRate) {
                            $scope.showRate = showRate.Value == 'true';
                        }
                        var disableExcessRow = response.Data.find(o => o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'disableExcess');
                        if (disableExcessRow) {
                            var dex = disableExcessRow.Value;
                            $scope.disableExcess = dex == '1' || dex == 1 || dex === true || ('' + dex).toLowerCase() === 'true';
                        }
                        var tnc = response.Data.find(o => o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'tnc');
                        if (tnc) {
                            $scope.GRN.Tnc = tnc.Value;
                        }
                        var addInfo = response.Data.find(o => o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'addInfo');
                        if (addInfo) {
                            $scope.GRN.Remarks = addInfo.Value;
                        }

                        var rateType = response.Data.find(o => o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'rateType');
                        if (rateType) {
                            $scope.rateType = rateType.Value;
                        }
                        var showRateOf = response.Data.find(o => o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'showRateOf');
                        if (showRateOf) {
                            $scope.showRateOf = showRateOf.Value;
                        }
                    }

                }
                applyDisableExcessRules();
                applyReceivingChallanPreview();
            }, 'RECEIVINGCHALLAN');
        }
        GetBillingConfig();


        function getAllOtherCharges() {
            var grn = new $.GRN();
            grn.GetOtherCharges(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                else
                    $scope.GRN.OtherCharges = e.data.Data;
            },
                { GRNId: $scope.GRN.GRNId }
            );
        }
        getAllOtherCharges();
        $scope.checkShortQty = function (item) {

            //if (parseFloat(item.ShortQty) > 0 && parseFloat(item.ExcessQty) > 0) {
            //    item.ShortQty = 0;
            //    alert('Please enter either short or excess quantities');
            //}
            //if (parseFloat(item.ShortQty) > parseFloat(item.Quantity)) {
            //    item.ShortQty = item.Quantity;
            //    alert('Short quantity can not be more than receiveables');
            //}
        }
        $scope.checkExcessQty = function (item) {
            //if ($scope.disableExcess) {
            //    item.ExcessQty = 0;
            //}
            //if (item.ShortQty > 0 && item.ExcessQty > 0) {
            //    item.ExcessQty = 0;
            //    alert('Please enter either short or excess quantities');
            //}

        }

        //$scope.$watch('GRN.Items', function (newValue, oldValue) {
        //    $.each(newValue, function (index, el) {
        //        if (el.Breakage && el.Breakage > 0) {
        //            if (el.Breakage > el.Quantity && el.Deleted == 0) {
        //                el.Breakage = 0;
        //                alert('Breakage can not be more than receiveables');
        //                return;
        //            }
        //        }

        //    });
        //}, true);

        $scope.getWarehouses = function () {
            WarehouseService.getWarehouses().then(function (e) {
                $scope.warehouses = e.data.Data;
            });
        }
        ChallanService.initChallan().then(function (e) {
            if (e.data.Code == 200) {
                var comp = e.data.Data.company;
                if (!$scope.GRN.WarehouseId) {
                    $scope.GRN.WarehouseId = comp.DefaultWarehouseId;
                }
            }
        });
        $scope.getWarehouses();
    }]);


app.controller('StockAdjustmentController', ['$scope', '$http', '$stateParams', '$rootScope',
    'toaster', '$state', 'InventoryFactory', '$crypto',
    function ($scope, $http, $stateParams, $rootScope, toaster, $state, inventoryFactory, $crypto) {
        var headerId = $stateParams.HeaderId == undefined ? 0 : $stateParams.HeaderId;





        var site = new $.Site({});
        var _siteNames = [];
        var selectedGrnItemIndex = -1;

        var date = new Date();
        var token = $rootScope.getTokenInfo();
        var edit = false;
        if (headerId != 0) {
            headerId = $crypto.decrypt(headerId);
            edit = true;
        }
        $scope.edit = edit;

        if (token != null) {
            $scope.MinDate = token.FinYearStart;

        }
        $scope.Filter = { onDate: convertDate(date) };


        var product = new $.Product();
        product.GetSizeListByCompany(function (e) {
            $scope.AllSizes = e.data;

        });

        if (headerId > 0) {

            inventoryFactory.StockAdjustmentDetails({ StockTransactionHeaderId: headerId }, function (e) {

                if (e.data.Code == 200) {
                    $scope.StockTrans = e.data.Data;
                    $scope.StockTrans.Items = e.data.Data.Items;
                    $scope.StockTrans.PostingDate = convertDate(e.data.Data.PostingDate);
                    $scope.StockTrans.PostingType = e.data.Data.PostingType.toString();
                }

            });
        }

        $scope.TransItem = { ProductId: 0, Quantity: 0 };
        $scope.StockTrans = { StockTransactionHeaderId: headerId, Items: [] };
        $scope.addItem = function () {

            var m = $scope.TransItem.ProductId == 0 || $scope.TransItem.Quantity == 0 || $scope.TransItem.Quantity == undefined;
            if (m) {

                toaster.pop('error', "error", 'Please provide all details');
                return;
            }

            var item = $scope.StockTrans.Items.find(o => o.ProductId == $scope.TransItem.ProductId);
            if (item) {
                var qty = parseFloat(item.Quantity);
                item.Quantity = qty + parseFloat($scope.TransItem.Quantity);
                return;
            }
            if ($scope.Stock) {
                var stock = $scope.Stock.find(o => o.ProductId == $scope.TransItem.ProductId);
                if (stock) {
                    $scope.TransItem.InHandQty = stock.ClosingBalance;
                }
            }
            $scope.TransItem.QtyNow = $scope.TransItem.InHandQty - $scope.TransItem.Quantity;


            $scope.StockTrans.Items.push($scope.TransItem);
            $scope.TransItem = new $.WorkOrderItem({});
            $('#itemSelect_value').focus();
            $('#itemSelect_value').val('');
        }
        //$scope.selectedParty = function (selected) {
        //    if (selected != undefined) {
        //        var item = selected.originalObject;
        //        console.log('$scope.selectedParty', selected);
        //        $scope.StockTrans.LedgerId = item.LedgerId;
        //        $scope.Filter.LedgerId = item.LedgerId;
        //    }
        //};

        $scope.selectedProduct = function (selected) {

            if (selected != undefined) {


                // var Product = this.$parent.item.Item = selected.originalObject;//this.$parent.item.Props.Item.Props;
                // $scope.RecItem.Item = selected.originalObject;//this.$parent.item.Props.Item.Props;
                $scope.TransItem.Product = selected.originalObject.Product;
                //  $scope.RecItem.ProductSizeId = selected.originalObject.ProductSizeId;
                $scope.TransItem.ProductId = selected.originalObject.ProductId;
            }

        };
        $scope.OnItemDeleted = function (index) {
            $scope.$apply(function () {
                $scope.StockTrans.Items.splice(index - 1, 1);
            });
        }
        $scope.DeleteItem = function (index) {

            $scope.$apply(function () {
                //$scope.StockTrans.Items.splice(index - 1, 1);
                $scope.StockTrans.Items[index - 1].Deleted = 1;
            });
        };
        $scope.filterDeleted = function (item) {
            return item.Deleted != 1;
        };
        $scope.saveStockAdjust = function () {

            var m = $('#frmStockAdjust').valid();
            if (!m) {
                return;
            }
            if ($scope.StockTrans.Items.length == 0) {
                toaster.pop('error', "error", 'Please provide line items.');
                return;
            }

            $scope.StockTrans.PostingDate = formatdate($scope.StockTrans.PostingDate)

            var zeroLenth = $scope.StockTrans.Items.filter(o => o.Quantity < 1 || o.Quantity == undefined).length;
            if (zeroLenth > 0) {
                toaster.pop('error', "error", 'Correct the line items quantities');
                return;
            }
            inventoryFactory.PostStockTxn($scope.StockTrans, function (e) {
                if (e.data.Code == 200) {
                    toaster.pop('success', "success", 'Saved successfully.');
                    $state.go('stockadjustlist');
                }
                else {
                    toaster.pop('error', "error", e.data.Message);
                }
            });
        }

        GetStockInhand = function () {

            inventoryFactory.GetStockInhand(function (e) {

                $scope.Stock = e.data;
                console.log('GetStockInhand', e.data);
                $scope.LastRow = e.data[e.data.length - 1];
            }, $scope.Filter);
        }
        GetStockInhand();
        FormsValidation.init('frmStockAdjust');
    }]);
app.controller('StockAdjustmentListController', ['$scope', '$http', '$stateParams', '$rootScope',
    'toaster', '$state', 'InventoryFactory', '$crypto',
    function ($scope, $http, $stateParams, $rootScope, toaster, $state, inventoryFactory, $crypto) {
        // var grnId = $routeParams.grnId == undefined ? 0 : $routeParams.grnId;

        $scope.AjustList = [];

        var date = new Date();
        var token = $rootScope.getTokenInfo();


        if (token) {

            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }
        $scope.Filter = { From: convertDate($scope.MinDate), To: convertDate(date) };

        $scope.filter = function () {
            var m = $('#frmStockAdjustList').valid();
            if (!m) {
                return;
            }



            inventoryFactory.StockAdjustmentList($scope.Filter, function (e) {
                if (e.data.Code == 200) {
                    $scope.AjustList = e.data.Data;
                }
                else {
                    toaster.pop('error', "error", e.data.Message);
                }
            });
        }

        $scope.edit = function (item) {

            var _headerId = $crypto.encrypt(item.StockTransactionHeaderId.toString());
            $state.go('editstockadjust', { HeaderId: _headerId });
        }
        FormsValidation.init('frmStockAdjustList');
        window.setTimeout(() => {
            $scope.filter();
        }, 100);

        $scope.delete = function (item) {

            var cnf = confirm('Are you sure to delete this record');
            if (!cnf) {
                return;
            }
            inventoryFactory.DeleteStockAdjustment({ StockTransactionHeaderId: item.StockTransactionHeaderId }, function (e) {
                if (e.data.Code == 200) {
                    toaster.pop('success', "success", 'Record deleted successfully');
                    $scope.filter();
                }
                else {
                    toaster.pop('error', "error", e.data.Message);
                }
            });
        }

    }]);