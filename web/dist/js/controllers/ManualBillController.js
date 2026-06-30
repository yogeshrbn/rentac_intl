app.controller('ManualBillController', ['$scope', '$location', '$routeParams', '$route', '$http', '$mdDialog', '$rootScope', '$uibModal', 'LedgerFactory',
    function ($scope, $location, $routeParams, $route, $http, $mdDialog, $rootScope, $uibModal, LedgerFactory) {
        //


        var type = $routeParams.type == undefined ? 2 : $routeParams.type;
        var wId = $routeParams.wId == undefined ? 0 : $routeParams.wId;
        $scope.AddFromSite = wId > 0;

        newBill();
        $scope.Billing.WorkOrderId = wId;

        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            if (e.data != null && e.data.length > 0) {
                $scope.initialValue = e.data[0];
            }
        });
        var ledger = new $.Ledger({});
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Billing.LedgerId = $rootScope.LedgerId;
        }
        var token = $rootScope.getTokenInfo();

        if (token) {
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
            $scope.Billing.InvoiceDate = convertDate(new Date());
        }
        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Billing.LedgerId = item.LedgerId;
            }
        };
        $scope.$watch('Billing.LedgerId', function () {
            if ($scope.Billing) {
                getSites();
                ledger.LedgerId = $scope.Billing.LedgerId;
                getAllProducts();
                $rootScope.LedgerId = $scope.Billing.LedgerId;
            }
        });
        $scope.$watch('Billing.WorkOrderId', function () {
            if ($scope.Site) {
                var site = $scope.Site.find(o => o.WorkOrderId == $scope.Billing.WorkOrderId);
                if (site) {
                    $scope.Billing.SiteAddress = site.Site;
                }
            }
        });

        function getAllProducts() {
            ledger.GetProductRates(function (e) {
                $scope.ProductRates = e.data;
            });

        }
        $scope.itemSelected = function (itemId) {
            if (itemId != undefined) {
                var item = $scope.ProductRates.find(o => o.ProductId == itemId);
                $scope.IssueItem.Product = item.Product;
                //$scope.TransItem.Product = selected.originalObject.Name;//this.$parent.item.Props.Item.Props;
                $scope.IssueItem.ProductId = itemId;//selected.originalObject.ProductId;
                $scope.ProductSizes = [{}];
                $scope.ProductSizes = $.map($scope.AllSizes, function (value, key) {
                    if (value.ProductId == itemId) {
                        return value;
                    }
                });
            }
        };
        $scope.$watch('IssueItem.ProductSizeId', function () {

            if ($scope.IssueItem.ProductSizeId != null) {
                if ($scope.AllSizes) {
                    var size = $scope.AllSizes.find(o => o.ProductSizeId == $scope.IssueItem.ProductSizeId);
                    if (size) {
                        $scope.IssueItem.Size = size.Size;
                    }
                }
            }
        });
        $scope.DeleteItem = function (index) {

            $scope.$apply(function () {
                $scope.Billing.Items.splice(index - 1, 1);
            });
        }
        getAllProductSizesByCompany();
        function getAllProductSizesByCompany() {
            var product = new $.Product();
            product.GetSizeListByCompany(function (e) {
                $scope.AllSizes = e.data;
            });
        }

        $scope.NewBill = function () {
            newBill();
        }

        $scope.GenBill = function () {

            $scope.Billing.GenerateBill(function (e) {
                if (e.data.Code != undefined) {
                    alert(e.data.Description);
                    return;
                }

                $scope.BillData = $scope.Billing.Items = e.data;
                if (e.data.length > 0) {
                    displaySave(true);
                    getBreakageForBill();
                }
            });
        }

        function newBill() {

            $scope.Billing = new $.Billing({});
            $scope.Billing.InvoiceType = type; //-- rent purpose
            $scope.IssueItem = new $.WorkOrderItem({ Rate: $scope.DefaultRate });

            $scope.Billing.Items = [];
            GetTaxes();
            $scope.CurrentBill = new $.Billing({ BillNumber: '<NEW>' });

            $scope.Breakages = null;
            $scope.AllTaxes = null;

        }

        // $scope.CurrentBill = new $.Billing({BillNumber:'<NEW>'});
        $scope.SaveBill = function () {
            //  $scope.Billing.InvoiceType = type;
            //  $scope.Billing.WorkOrderId = wId;


            $scope.Billing.SaveBreakageBill(function (e) {
                if (e.statusText == 'OK') {
                    debugger
                    $scope.CurrentBill = e.data;
                    // alert('Bill generated successfully');
                    // newBill();
                    chooseAndPrintBill($scope.CurrentBill.InvoiceId);
                    $route.reload();
                    newBill();
                }

            });
        }
        function chooseAndPrintBill(invoiceId) {
            ModalFactory.BillType(function ($scope, $mdDialog) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                },
                    $scope.OkButtonClick = function () {
                        var headerTypes = [];

                        if ($scope.HeaderType1) {
                            headerTypes.push(1);
                        }
                        if ($scope.HeaderType2) {
                            headerTypes.push(2);
                        }
                        if ($scope.HeaderType3) {
                            headerTypes.push(3);
                        }
                        $(event.currentTarget).find('.fa-spin').show();
                        printBill($scope.HeaderType, invoiceId);
                    }
            });
        }
        function printBill(headerType, invoiceId) {

            $scope.Billing.InvoiceId = invoiceId;
            //   $scope.Billing.BillCopyType = headerType;
            //  $scope.Billing.InvoiceNumber = e.data.InvoiceNumber;
            $scope.Billing.HeaderTypes = headerType;
            $scope.Billing.PrintBill(function (e) {
                $mdDialog.hide();
                var filePath = SERVER_URL + 'temp/' + e.data;
                // $window.target = '_blank';
                $window.open(filePath);
            });

            $route.reload();
        }
        //--Loading site information
        var _siteNames = [];
        _siteNames.push({ JobNumber: '', SiteId: 0 });
        var site = new $.Site({});


        function getSites() {
            _siteNames = [];

            ledger.GetSites(function (e) {

                for (var i in e.data) {
                    _siteNames.push({ WorkOrderId: e.data[i].WorkOrderId, JobNumber: e.data[i].JobNumber, Site: e.data[i].Site });
                }
                $scope.Site = _siteNames;
            }, $scope.Billing);
        }

        $scope.CalculateSum = function () {

            var total = 0;
            if ($scope.Billing.Items != undefined) {
                for (var i = 0; i <= $scope.Billing.Items.length - 1; i++) {
                    if ($scope.Billing.InvoiceType != 4) {
                        $scope.Billing.Items[i].Amount = $scope.Billing.Items[i].Quantity * $scope.Billing.Items[i].Rate;
                        if ($scope.Billing.Items[i].Days && $scope.Billing.Items[i].Days > 0) {
                            $scope.Billing.Items[i].Amount = $scope.Billing.Items[i].Quantity * $scope.Billing.Items[i].Rate * $scope.Billing.Items[i].Days;
                        }
                    }
                    var v = $scope.Billing.Items[i];

                    total += parseFloat(v.Amount);

                }
            }

            $scope.Billing.SubTotal = total;
            $scope.Billing.Total1 = parseFloat($scope.Billing.SubTotal) + parseFloat($scope.Billing.Freight);
            return total;
        }
        $scope.CalSubTotal = function () {
            var subTotal = 0, taxAmount = 0;
            $scope.Billing.FreightTax = 0;

            $scope.CalculateSum();
            if ($scope.AppliedTaxes != null) {
                for (var i = 0; i < $scope.AppliedTaxes.length; i++) {

                    //  taxAmount += (  $scope.Billing.Total1 )* $scope.Billing.Taxes[i].Rate / 100.00;
                    taxAmount += $scope.AppliedTaxes[i].TaxAmount

                }
            }
            if ($scope.Billing.Taxes) {
                for (var i = 0; i < $scope.Billing.Taxes.length; i++) {
                    var tax = $scope.Billing.Taxes[i];
                    if (tax.Applicable) {
                        $scope.Billing.FreightTax += ($scope.Billing.Freight) * $scope.Billing.Taxes[i].DefaultRate / 100.00;
                    }
                }
            }
            $scope.Billing.TaxAmount = taxAmount + $scope.Billing.FreightTax;

            $scope.Billing.Total = parseFloat($scope.Billing.Total1) + $scope.Billing.TaxAmount;//  parseFloat($scope.Billing.SubTotal) + parseFloat(taxAmount) + parseFloat($scope.Billing.Freight) ;
            if ($scope.Billing.RoundOff == true) {
                $scope.Billing.Total = $scope.Billing.Total.toFixed()
            }
            return $scope.Billing.Total;

        }
        function GetTaxes() {
            var Tax = new $.Tax({ ItemId: 0 });
            $scope.WorkOrder = new $.WorkOrder({});
            Tax.GetTaxes($scope.WorkOrder, function (e) {

                $scope.Billing.Taxes = e.data;
            });
            Tax.GetAllTaxes(function (e) {

                $scope.AllTaxes = e.data;
            });
        }
        $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = [];
        $scope.Billing.FreightTax = 0;
        $scope.Billing.BreakageTax = 0;


        $scope.ApplyTax = function (o) {
            var taxId = o.tax.TaxId;
            debugger
            //var itemsToApplyTax = $scope.AllTaxes.find(n=>n.TaxId == o.tax.TaxId);
            var itemsToApplyTax = jQuery.grep($scope.AllTaxes, function (n, i) {
                return (n.TaxId == taxId);
            });
            o.tax.TaxAmount = 0;
            if (!o.tax.Applicable) {

                $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = jQuery.grep($scope.Billing.AppliedTaxes, function (n, i) {
                    return (n.TaxId != taxId);
                });
                return;
            }
            for (var i = 0; i < itemsToApplyTax.length; i++) {
                var billingItems = jQuery.grep($scope.Billing.Items, function (n, j) {
                    return (n.ProductId == itemsToApplyTax[i].ItemValue);
                });
                if (billingItems && billingItems.length > 0) {
                    for (var j = 0; j < billingItems.length; j++) {
                        var txAmount = (billingItems[j].Amount * itemsToApplyTax[i].Rate / 100.0);
                        debugger
                        if ($scope.Billing.RoundOffTaxes == true) {

                            txAmount = parseFloat(txAmount.toFixed(), 0);
                        }
                        o.tax.TaxAmount += txAmount
                        var appliedTax = { 'TaxId': taxId, 'TaxName': o.tax.Name, 'Item': billingItems[j].Item, 'ProductId': billingItems[j].ProductId, 'Amount': billingItems[j].Amount, 'TaxRate': itemsToApplyTax[i].Rate, 'TaxAmount': txAmount };
                        addToAppliedTaxes(appliedTax);
                    }
                }
            }



        };

        $scope.RoundOffTaxes = function (o) {
            $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = [];
            $($scope.Billing.Taxes).each(function (e) {

                if (this.Applicable) {
                    var x = {};
                    x.tax = this;

                    $scope.ApplyTax(x);
                }

            });
        }

        function addToAppliedTaxes(o) {
            var taxes = $scope.Billing.AppliedTaxes;
            var updated = false;

            //debugger
            for (var i = 0; i < taxes.length; i++) {
                if (o.ProductId == taxes[i].ProductId && o.TaxId == parseInt(taxes[i].TaxId)) {
                    taxes[i].Amount += o.Amount;
                    taxes[i].TaxAmount += o.TaxAmount;
                    updated = true;
                }

            }
            if (!updated) {
                taxes.push(o);
            }
            $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = taxes;
        }


        //add new item to be issued
        $scope.addItem = function () {


            //default calculation of amount
            if ($scope.Billing.InvoiceType != 4) {
                $scope.IssueItem.Amount = $scope.IssueItem.Quantity * $scope.IssueItem.Rate;
            }


            if ($scope.Billing.InvoiceType == 1) {
                if ($scope.IssueItem.From && $scope.IssueItem.To) {

                    var from = $scope.IssueItem.From;
                    var to = $scope.IssueItem.To;
                    from = from.replace(/\//g, '-');
                    to = to.replace(/\//g, '-');
                    from = stringToDate(from);
                    to = stringToDate(to);
                    $scope.IssueItem.Days = dateDiff(from, to) + 1; // add extra 1 to include the from date for rent. 
                    // $scope.IssueItem.From = convertDate( $scope.IssueItem.From)
                    //  $scope.IssueItem.To = convertDate( $scope.IssueItem.To)
                    $scope.IssueItem.Amount = $scope.IssueItem.Quantity * $scope.IssueItem.Rate * $scope.IssueItem.Days;
                }
            }

            debugger
            $scope.Billing.Items.push($scope.IssueItem);

            $scope.IssueItem = new $.WorkOrderItem({ Rate: $scope.DefaultRate });

            $('#itemSelect_value').focus();
            $('#itemSelect_value').val('');
        }
        $scope.selectedProduct = function (selected) {
            if (selected != undefined) {
                //debugger
                $scope.IssueItem.Item = selected.originalObject;//this.$parent.item.Props.Item.Props;
                $scope.IssueItem.ProductId = selected.originalObject.ProductId;
                //findDefaultRate($scope.IssueItem.Item.ProductId);
            }
        };
    }]);