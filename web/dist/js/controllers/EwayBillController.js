
app.controller("AddEwayBillController", ['$scope', '$rootScope', '$state', '$stateParams', '$http', 'CompanyService', '$crypto',
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, $crypto, NavigationHistory) {

        var transDto = new $.Transporter();
        $scope.Transporters = [];
        $scope.EwayBill = { InvoiceId: 0 };
        $scope.SubTypes = StaicData.EWAY_TXN_SUB_TYPE;
        $scope.TransacionTypes = StaicData.EWAY_TXN_TYPES;
        var token = $rootScope.getTokenInfo();

        function challanApproximateValue(woOrSite) {
            if (!woOrSite) return 0;
            var v = Number(woOrSite.ApproximateValue);
            if (isFinite(v) && v > 0) return v;
            if (woOrSite.SiteInfo) {
                v = Number(woOrSite.SiteInfo.ApproximateValue);
                if (isFinite(v) && v > 0) return v;
            }
            return 0;
        }
        function setEwayApproximateFromChallan(challanApprox, onlyIfEmpty) {
            var v = Number(challanApprox);
            if (!isFinite(v) || v <= 0) return;
            if (onlyIfEmpty) {
                var cur = $scope.EwayBill.ApproximateValue;
                if (cur != null && cur !== '' && Number(cur) > 0) return;
            }
            $scope.EwayBill.ApproximateValue = v;
        }

        var k = $stateParams.key;
        if (!k) {
            alert('Invalid input data provided, please re-visit');
            $state.go('billList');
            return;
        }
        var dck = $crypto.decrypt(k);
        if (!dck) {
            alert('Invalid input data provided, please re-visit');
            $state.go('billList');
            return;
        }

        var pdata = dck.split(',');
        $scope.Search = { Name: '' }
        $scope.EwayBill.EwayBillId = pdata[0];
        $scope.EwayBill.DocType = pdata[1];
        $scope.EwayBill.InvoiceId = pdata[2]
        $scope.EwayBill.docSubType = '';
        if (pdata.length == 4) {
            $scope.EwayBill.docSubType = pdata[3];
        }
        if ($scope.EwayBill.InvoiceId == 0) {
            alert('Invalid input data provided, please re-visit');
            $state.go('billList');
            return;
        }
        $scope.GetAll = function () {

            transDto.GetAll(function (e) {
                $scope.Transporters = e.data;

            });
        }
        $scope.GetAll();

        $scope.Save = function () {
            var ewayBillObj = new $.EwayBill();
            var ewayBill = $scope.EwayBill;
            if (ewayBill.DocType == "chl" && ewayBill.SubTypeId != 8) {
                alert('Please select "Others" in transaction sub type list for challans');
                return;
            }
            if (ewayBill.DocType == 'chl') {
                var invalid = $scope.validateChallanItems();
                if (invalid > 0) {
                    alert('Please enter line items for e-way bill.');
                    return;
                }
            }
            //if (ewayBill.SubTypeId == 8) {
            //    if (ewayBill.OtherTypeDesc && ewayBill.OtherTypeDesc.length < 1) {
            //        alert('Please enter line items for e-way bill.');
            //        return;
            //    }

            //}
            var m = $('#frmEwayBill').valid();

            if (m) {
                if (!ewayBill.TransportationMode) {
                    alert('Please select mode of transportation');
                    return;
                }
                if (ewayBill.TransportationMode == 1) {
                    if (!ewayBill.VehicleNo || ewayBill.VehicleNo.length < 7) {
                        alert('Please enter a valid vehicle no.');
                        return;
                    }
                    if (!ewayBill.VehicleType || ewayBill.VehicleType == null) {
                        alert('Please select a vehicle type.');
                        return;
                    }
                }
                if (ewayBill.TransportationMode == 2 || ewayBill.TransportationMode == 3 || ewayBill.TransportationMode == 4) {
                    if (!ewayBill.TransporterDocNo || ewayBill.TransporterDocNo.length < 5) {
                        alert('Please enter a document no.');
                        return;
                    }
                    if (!ewayBill.TransporterDocDate) {
                        alert('Please select a Document date.');
                        return;
                    }
                }
                ewayBillObj.Save(function (e) {

                    if (e.data.Code == 200) {
                        showMessage('E-way bill created successfully.');
                        // $('#addTransModel').modal('hide');
                        $state.go('ewayBills');
                    } else {

                        showMessage(e.data.Message);
                    }

                }, ewayBill);
            }
        }
        FormsValidation.init('frmEwayBill');


        $scope.validateChallanItems = function () {
            var invalid = 0;
            $.each($scope.EwayBill.Items, function (index, value) {

                value.IGST = round((value.Quantity * value.Rate) * value.IGSTRate / 100);
                value.CGST = round((value.Quantity * value.Rate) * value.CGSTRate / 100);
                value.SGST = round((value.Quantity * value.Rate) * value.SGSTRate / 100);

                value.SubTotal = (value.Quantity * value.Rate) + value.IGST + value.CGST + value.SGST;
                //if (value.SubTotal <= 0) {
                //    invalid++;
                //}
                if (value.Product.length < 1) {
                    invalid++;
                }
                if (value.HSNCode.length < 4) {
                    invalid++;
                }
            });
            return invalid;
        }

        $scope.getDetails = function () {
            if ($scope.EwayBill.EwayBillId > 0) {
                var ewayBillObj = new $.EwayBill();
                ewayBillObj.GetInfo(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    $scope.EwayBill = e.data.Data;
                }, { EwayBillId: $scope.EwayBill.EwayBillId });
            }
        }

        //  $scope.getDetails();
        if ($scope.EwayBill.InvoiceId > 0) {
            if ($scope.EwayBill.DocType == "inv") {
                $scope.EwayBill.docSubType = 'inv';
                var billing = new $.Billing();
                billing.ById(function (e) {

                    var _data = e.data.Data;
                    $scope.Billing = e.data.Data;
                    $scope.EwayBill.Items = _data.BillableItems.map((val, index) => {
                        return {
                            ProductId: val.ProductId,
                            Product: val.Item,                            
                            Quantity: val.Quantity,
                            HSNCode: val.HSNCode,
                            Rate: val.Rate,
                            IGST: val.IGST,
                            CGST: val.CGST,
                            SGST: val.SGST,
                            IGSTRate: val.IGSTRate,
                            CGSTRate: val.CGSTRate,
                            SGSTRate: val.SGSTRate,
                            Unit: val.Unit,
                            SubTotal: val.Quantity * val.Rate
                        }
                    });
                    //----- load shipping details
                    var _partyFilter = { LedgerId: _data.LedgerId, LedgerSiteId: _data.LedgerSiteId || 0 };
                    var ledger = new $.EwayBill();
                 
                    ledger.getPartyInfo(function (e) {
                        var clientInfo = e.data;
                        $scope.EwayBill.ShipToAddress = clientInfo.Address;
                        $scope.EwayBill.ShipToCity = clientInfo.City;
                        $scope.EwayBill.ShipToZipCode = clientInfo.ZipCode;
                        $scope.EwayBill.ShipToStateId = clientInfo.StateId;
                        $scope.EwayBill.ShipToStateCode = clientInfo.StateCode;
                    }, _partyFilter);
                    CompanyService.getCompanyDetails({ CompanyId: _data.CompanyId }).then(function (e) {
                        if (e.status != 200) {
                            alert('Seller information not loaded');
                            return;
                        }
                        var sellerInfo = e.data;
                        $scope.EwayBill.ShipFromAddress = sellerInfo.Address1 + ' ' + sellerInfo.Address2 + ' ' + sellerInfo.City;
                        $scope.EwayBill.ShipFromCity = sellerInfo.City;
                        $scope.EwayBill.ShipFromZipCode = sellerInfo.ZipCode;
                        $scope.EwayBill.ShipFromStateId = sellerInfo.StateId;
                        $scope.EwayBill.ShipFromStateCode = sellerInfo.StateCode;

                    });

                },
                    {
                        InvoiceId: $scope.EwayBill.InvoiceId
                    });
            }
            else if ($scope.EwayBill.DocType == "chl") {
                if ($scope.EwayBill.docSubType == '' || $scope.EwayBill.docSubType == 'del') {
                    $scope.EwayBill.docSubType = 'del';
                    var wo = new $.WorkOrder({ WorkOrderId: $scope.EwayBill.InvoiceId });
                    wo.GetDetail(function (e) {

                        $scope.Billing = e.data;
                        $scope.EwayBill.SubTypeId = 8;
                        $scope.EwayBill.Distance = 0;
                        $scope.EwayBill.TransportationMode = 1;
                        $scope.EwayBill.VehicleNo = $scope.Billing.Vehicle;
                        $scope.EwayBill.VehicleType = 'R';
                        $scope.EwayBill.TransporterId = $scope.Billing.TransporterId;
                        $scope.EwayBill.Items = e.data.Items;
                        setEwayApproximateFromChallan(challanApproximateValue(e.data), false);

                        //----- load shipping details
                      
                         
                        var _partyFilter = { LedgerId: $scope.Billing.LedgerId, LedgerSiteId: $scope.Billing.LedgerSiteId };
                        var ebill = new $.EwayBill();
                        ebill.getPartyInfo(function (e) {
                             
                            var clientInfo = e.data;
                            $scope.EwayBill.ShipToAddress = clientInfo.Address;
                            $scope.EwayBill.ShipToCity = clientInfo.City;
                            $scope.EwayBill.ShipToZipCode = clientInfo.ZipCode;
                            $scope.EwayBill.ShipToStateId = clientInfo.StateId;
                            $scope.EwayBill.ShipToStateCode = clientInfo.StateCode;
                        }, _partyFilter);

                        CompanyService.getCompanyDetails({ CompanyId: e.data.CompanyId }).then(function (e) {
                            if (e.status != 200) {
                                alert('Seller information not loaded');
                                return;
                            }
                            var sellerInfo = e.data;
                            $scope.EwayBill.ShipFromAddress = sellerInfo.Address1 + ' ' + sellerInfo.Address2 + ' ' + sellerInfo.City;
                            $scope.EwayBill.ShipFromCity = sellerInfo.City;
                            $scope.EwayBill.ShipFromZipCode = sellerInfo.ZipCode;
                            $scope.EwayBill.ShipFromStateId = sellerInfo.StateId;
                            $scope.EwayBill.ShipFromStateCode = sellerInfo.StateCode;

                        });

                    },
                    );
                }
                if ($scope.EwayBill.docSubType == 'ret') {

                    var grn = new $.GRN({ GRNId: $scope.EwayBill.InvoiceId });
                    grn.GetById(function (e) {
                        var _data = e.data.Data;
                        $scope.Billing =
                        {
                            Number: _data.GRN,
                            WorkOrderDate: convertDate(_data.ReceivingDate),
                            Client: _data.GrnItems[0].Client
                        };

                        $scope.EwayBill.SubTypeId = 8;
                        $scope.EwayBill.Distance = 0;
                        $scope.EwayBill.TransportationMode = 1;
                        $scope.EwayBill.VehicleNo = $scope.Billing.VehicleNo;
                        $scope.EwayBill.VehicleType = 'R';
                        $scope.EwayBill.TransporterId = $scope.Billing.TransporterId;

                        $scope.EwayBill.Items = _data.GrnItems.map((val, index) => {
                            return {
                                ProductId: val.ProductId,
                                Product: val.Item,
                                Quantity: val.Quantity,
                                HSNCode: val.HSNCode,
                                Rate: val.Rate,
                                IGSTRate: 0,
                                CGSTRate: 0,
                                SGSTRate: 0,
                                Unit: val.Unit,
                                SubTotal: val.Quantity * val.Rate
                            }
                        });
                        setEwayApproximateFromChallan(_data.ApproximateValue, false);

                        //----- load shipping details
                   
                        var _partyFilter = { LedgerId: _data.LedgerId, LedgerSiteId: _data.LedgerSiteId || 0 };
                        var ledger = new $.EwayBill();
                        ledger.getPartyInfo(function (e) {
                            var clientInfo = e.data;
                            $scope.EwayBill.ShipFromAddress = clientInfo.Address;
                            $scope.EwayBill.ShipFromCity = clientInfo.City;
                            $scope.EwayBill.ShipFromZipCode = clientInfo.ZipCode;
                            $scope.EwayBill.ShipFromStateId = clientInfo.StateId;
                            $scope.EwayBill.ShipFromStateCode = clientInfo.StateCode;
                        }, _partyFilter);


                        CompanyService.getCompanyDetails({ CompanyId: _data.CompanyId }).then(function (e) {
                            if (e.status != 200) {
                                alert('Buyer information not loaded');
                                return;
                            }
                            var sellerInfo = e.data;
                            $scope.EwayBill.ShipToAddress = sellerInfo.Address1 + ' ' + sellerInfo.Address2 + ' ' + sellerInfo.City;
                            $scope.EwayBill.ShipToCity = sellerInfo.City;
                            $scope.EwayBill.ShipToZipCode = sellerInfo.ZipCode;
                            $scope.EwayBill.ShipToStateId = sellerInfo.StateId;
                            $scope.EwayBill.ShipToStateCode = sellerInfo.StateCode;

                        });
                    },
                    );
                }
            }
        }

        $scope.$watch('EwayBill.Items', function () {
            $.each($scope.EwayBill.Items, function (index, value) {
                value.IGST = round((value.Quantity * value.Rate) * value.IGSTRate / 100);
                value.CGST = round((value.Quantity * value.Rate) * value.CGSTRate / 100);
                value.SGST = round((value.Quantity * value.Rate) * value.SGSTRate / 100);

                value.SubTotal = (value.Quantity * value.Rate) + value.IGST + value.CGST + value.SGST;
            });
        }, true);
        $scope.onIGSTRateChange = function (item) {
            item.CGSTRate = 0;
            item.SGSTRate = 0;
        }
        $scope.onCGSTRateChange = function (item) {
            item.SGSTRate = item.CGSTRate;
            item.IGSTRate = 0;
        }
        $scope.onSGSTRateChange = function (item) {
            item.CGSTRate = item.SGSTRate;
            item.IGSTRate = 0;
        }
        $scope.DeleteItem = function (index) {
            $scope.EwayBill.Items.splice(index, 1);
        }
        var newChallanItem = { IGSTRate: 0, CGSTRate: 0, SGSTRate: 0, Quantity: 1, Rate: 0, SubTotal: 0, Product: '', HSNCode: '' };
        $scope.addItem = function () {
            var invalid = $scope.validateChallanItems();
            if (invalid == 0) {
                var newItem = cloneObj(newChallanItem);
                $scope.EwayBill.Items.push(newItem);
            }
        }

        $scope.back = function () {
            if ($scope.EwayBill.docSubType == 'ret') {
                $state.go('recvdList');
            }
            else if ($scope.EwayBill.docSubType == 'del') {
                $state.go('issuedList');
            }
            else {
                $state.go('billList');
            }

        }
    }]);

app.controller("EditEwayBillController", ['$scope', '$rootScope', '$state', '$stateParams', '$http', 'CompanyService', '$crypto',
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, $crypto) {



        var transDto = new $.Transporter();
        $scope.Transporters = [];
        $scope.EwayBill = { InvoiceId: 0 };
        $scope.SubTypes = StaicData.EWAY_TXN_SUB_TYPE;
        $scope.TransacionTypes = StaicData.EWAY_TXN_TYPES;
        var token = $rootScope.getTokenInfo();

        function challanApproximateValue(woOrSite) {
            if (!woOrSite) return 0;
            var v = Number(woOrSite.ApproximateValue);
            if (isFinite(v) && v > 0) return v;
            if (woOrSite.SiteInfo) {
                v = Number(woOrSite.SiteInfo.ApproximateValue);
                if (isFinite(v) && v > 0) return v;
            }
            return 0;
        }
        function setEwayApproximateFromChallan(challanApprox, onlyIfEmpty) {
            var v = Number(challanApprox);
            if (!isFinite(v) || v <= 0) return;
            if (onlyIfEmpty) {
                var cur = $scope.EwayBill.ApproximateValue;
                if (cur != null && cur !== '' && Number(cur) > 0) return;
            }
            $scope.EwayBill.ApproximateValue = v;
        }

        var k = $stateParams.key;
        if (!k) {
            alert('Invalid input data provided, please re-visit');
            $state.go('billList');
            return;
        }
        var dck = $crypto.decrypt(k);
        if (!dck) {
            alert('Invalid input data provided, please re-visit');
            $state.go('billList');
            return;
        }
        var pdata = dck.split(',');
        $scope.Search = { Name: '' }
        $scope.EwayBill.EwayBillId = pdata[0];
        $scope.EwayBill.DocType = pdata[1];
        $scope.EwayBill.InvoiceId = pdata[2]
        if ($scope.EwayBill.InvoiceId == 0) {
            alert('Invalid input data provided, please re-visit');
            $state.go('billList');
            return;
        }
        $scope.GetAll = function () {

            transDto.GetAll(function (e) {
                $scope.Transporters = e.data;

            });
        }
        $scope.GetAll();
        $scope.Save = function () {
            var ewayBillObj = new $.EwayBill();



            var ewayBill = $scope.EwayBill;
            //if (ewayBill.DocType == "chl" && ewayBill.SubTypeId != 8) {
            //    alert('Please select "Others" in transaction sub type list for challans');
            //    return;
            //}
            if (ewayBill.DocType == 'chl') {
                var invalid = $scope.validateChallanItems();
                if (invalid > 0) {
                    alert('Please enter line items for e-way bill.');
                    return;
                }
            }
            var m = $('#frmEwayBill').valid();

            if (m) {
                if (!ewayBill.TransportationMode) {
                    alert('Please select mode of transportation');
                    return;
                }
                if (ewayBill.TransportationMode == 1) {
                    if (!ewayBill.VehicleNo || ewayBill.VehicleNo.length < 7) {
                        alert('Please enter a valid vehicle no.');
                        return;
                    }
                    if (!ewayBill.VehicleType || ewayBill.VehicleType == null) {
                        alert('Please select a vehicle type.');
                        return;
                    }
                }
                if (ewayBill.TransportationMode == 2 || ewayBill.TransportationMode == 3 || ewayBill.TransportationMode == 4) {
                    if (!ewayBill.TransporterDocNo || ewayBill.TransporterDocNo.length < 5) {
                        alert('Please enter a document no.');
                        return;
                    }
                    if (!ewayBill.TransporterDocDate) {
                        alert('Please select a Document date.');
                        return;
                    }
                }
                ewayBillObj.Save(function (e) {
                    if (e.data.Code == 200) {
                        showMessage('E-way bill updated successfully.');
                        // $('#addTransModel').modal('hide');
                        $state.go('ewayBills');
                    } else {
                        showMessage(e.data.Message);
                    }

                }, ewayBill);
            }
        }
        FormsValidation.init('frmEwayBill');

        //$scope.edit = function (item) {
        //    $scope.TP = item;
        //    $('#addTransModel').modal('show');
        //}
        $scope.closeModal = function () {
            $('#addTransModel').modal('hide');
        }
        $scope.validateChallanItems = function () {
            var invalid = 0;
            $.each($scope.EwayBill.Items, function (index, value) {

                value.IGST = round((value.Quantity * value.Rate) * value.IGSTRate / 100);
                value.CGST = round((value.Quantity * value.Rate) * value.CGSTRate / 100);
                value.SGST = round((value.Quantity * value.Rate) * value.SGSTRate / 100);

                value.SubTotal = (value.Quantity * value.Rate) + value.IGST + value.CGST + value.SGST;
                if (value.SubTotal <= 0) {
                    invalid++;
                }
                if (value.Product.length < 1) {
                    invalid++;
                }
                if (value.HSNCode.length < 4) {
                    invalid++;
                }
            });
            return invalid;
        }

        $scope.getDetails = function () {
            if ($scope.EwayBill.EwayBillId > 0) {
                var ewayBillObj = new $.EwayBill();
                ewayBillObj.GetInfo(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    $scope.EwayBill = e.data.Data;

                    if ($scope.EwayBill.InvoiceId > 0) {
                        if ($scope.EwayBill.DocType == "inv") {
                            var billing = new $.Billing();
                            billing.GetBillInfo(function (e) {
                                $scope.Billing = e.data.Data;
                            },
                                {
                                    InvoiceId: $scope.EwayBill.InvoiceId
                                });
                        }
                        else if ($scope.EwayBill.DocType == "chl" && $scope.EwayBill.DocSubType == 'del') {
                            var wo = new $.WorkOrder({ WorkOrderId: $scope.EwayBill.InvoiceId });
                            wo.GetDetail(function (e) {

                                $scope.Billing = e.data;
                                setEwayApproximateFromChallan(challanApproximateValue(e.data), true);
                            })
                        }
                        else if ($scope.EwayBill.DocType == "chl" && $scope.EwayBill.DocSubType == 'ret') {
                            var grn = new $.GRN({ GRNId: $scope.EwayBill.InvoiceId });
                            grn.GetById(function (e) {
                                var _data = e.data.Data;
                                $scope.Billing =
                                {
                                    Number: _data.GRN,
                                    WorkOrderDate: convertDate(_data.ReceivingDate),
                                    Client: _data.GrnItems[0].Client
                                };
                                setEwayApproximateFromChallan(_data.ApproximateValue, true);
                            });
                        }


                    }
                }, { EwayBillId: $scope.EwayBill.EwayBillId });
            }
        }
        $scope.getDetails();

        $scope.$watch('EwayBill.Items', function () {
            $.each($scope.EwayBill.Items, function (index, value) {
                value.IGST = round((value.Quantity * value.Rate) * value.IGSTRate / 100);
                value.CGST = round((value.Quantity * value.Rate) * value.CGSTRate / 100);
                value.SGST = round((value.Quantity * value.Rate) * value.SGSTRate / 100);

                value.SubTotal = (value.Quantity * value.Rate) + value.IGST + value.CGST + value.SGST;
            });
        }, true);
        $scope.onIGSTRateChange = function (item) {
            item.CGSTRate = 0;
            item.SGSTRate = 0;
        }
        $scope.onCGSTRateChange = function (item) {
            item.SGSTRate = item.CGSTRate;
            item.IGSTRate = 0;
        }
        $scope.onSGSTRateChange = function (item) {
            item.CGSTRate = item.SGSTRate;
            item.IGSTRate = 0;
        }
        $scope.DeleteItem = function (index) {
            $scope.EwayBill.Items.splice(index, 1);
        }
        var newChallanItem = { IGSTRate: 0, CGSTRate: 0, SGSTRate: 0, Quantity: 1, Rate: 0, SubTotal: 0, Product: '', HSNCode: '' };
        $scope.addItem = function () {
            var invalid = $scope.validateChallanItems();
            if (invalid == 0) {
                var newItem = cloneObj(newChallanItem);
                $scope.EwayBill.Items.push(newItem);
            }
        }

        $scope.back = function () {
            $state.go('ewayBills');
        }

    }]);

app.controller("EwayBillsController", ['$scope', '$rootScope', '$state', '$stateParams', 'LedgerFactory', 'CompanyService',
    'FileSaver', 'Blob', '$crypto',
    function ($scope, $rootScope, $state, $stateParams, LedgerFactory, CompanyService, FileSaver, Blob, $crypto) {
        var EWAY_BILLS_LIST_FILTER_KEY = 'rentacEwayBillsListFilter_v1';
        var savedEwayBillsFilter = typeof rentacContractsDashRestoreFilter === 'function'
            ? rentacContractsDashRestoreFilter(EWAY_BILLS_LIST_FILTER_KEY)
            : null;

        function persistEwayBillsFilter() {
            if (typeof rentacContractsDashPersistFilter !== 'function') {
                return;
            }
            rentacContractsDashPersistFilter(EWAY_BILLS_LIST_FILTER_KEY, {
                From: $scope.filter.From,
                To: $scope.filter.To,
                LedgerId: $scope.filter.LedgerId,
                DocType: $scope.filter.DocType,
                Status: $scope.filter.Status
            });
        }

        var transDto = new $.Transporter();
        $scope.SubTypes = StaicData.EWAY_TXN_SUB_TYPE;
        var ewayBillDto = new $.EwayBill();
        $scope.Transporters = [];
        $scope.TP = { Name: '', GST: '', Email: '', Phone: '' };
        $scope.filter = { From: '', To: '', LedgerId: 0, DocType: '', Status: '' };

        var token = $rootScope.getTokenInfo();

        if (token) {
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;
        }
        if (typeof rentacContractsDashApplyDefaultDates === 'function') {
            rentacContractsDashApplyDefaultDates(savedEwayBillsFilter, token, $scope.filter);
        } else {
            var toDate = new Date();
            var fromDate = new Date(toDate);
            fromDate.setMonth(fromDate.getMonth() - 1);
            $scope.filter.To = convertDate(toDate);
            $scope.filter.From = convertDate(fromDate);
            if (token && token.FinYearStart && typeof moment === 'function') {
                var fromParsed = moment($scope.filter.From, 'DD/MM/YYYY', true);
                var fyStart = moment(token.FinYearStart);
                if (fromParsed.isValid() && fyStart.isValid() && fromParsed.isBefore(fyStart, 'day')) {
                    $scope.filter.From = convertDate(fyStart.toDate());
                }
            }
        }


        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Accounts) {
                $scope.Accounts.push({ LedgerId: 0, Code: '', Name: 'All Clients' });
            }
            if (e.data != null && e.data.length > 0) {
                $scope.initialValue = e.data[0];
            }

        });


        $scope.find = function () {
            var filter = cloneObj($scope.filter);

            filter.From = formatdate(filter.From);
            filter.To = formatdate(filter.To);

            ewayBillDto.GetAll(function (e) {
                $scope.ewayBills = e.data.Data;
                persistEwayBillsFilter();
            }, filter);
        }

        $scope.GetSubTypeName = function (subTypeId) {
            var type = $scope.SubTypes.find(o => o.Id == subTypeId);
            if (type) {
                return type.Name;
            }

        }
        $scope.GetTransModeName = function (tptId) {
            var type = StaicData.EWAY_TRANSPORT_MODELS.find(o => o.Id == tptId);
            if (type) {
                return type.Name;
            }

        }
        $scope.find();


        $scope.PushToPortal = function (item) {

            ewayBillDto.PushToPortal(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                else {
                    alert('E-way bill created on portal.');
                }
            }, item);
        };

        $scope.Print = function (item) {

            $scope.printPdf();
        }


        $scope.printPdf = function () {
            var item = $scope.SelectedBill;
            var strInput = "ewaybillpreview," + item.EwayBillId
            var encrypedText = $crypto.encrypt(strInput);

            var econded = btoa(encrypedText);
            var report = new $.Reports();
            report.downloadReport(function (e) {

                FileSaver.saveAs(e.data, 'text.pdf');
            }, econded);

        }


        $scope.SelectedBill = null;
        $scope.EwayBillPreview = null
        $scope.showPeview = function (item) {
            $scope.EwayBillPrevie = null
            $scope.SelectedBill = item;
            $('#ewayDialog').modal('show');
            var strInput = "ewaybillpreview," + item.EwayBillId
            var encrypedText = $crypto.encrypt(strInput);

            var econded = btoa(encrypedText);
            var report = new $.Reports();
            report.previewReport(function (e) {
                $scope.EwayBillPreview = 1;
                $('#rpt').html(e.data);
            }, econded);

        }

        window.setTimeout(function (e) {
            FormsValidation.init('frmewaybill_addvehhicle');
            FormsValidation.init('frmcancelewaybill');


        }, 100);

        $scope.EwayBill = { VehicleNo: '', VehicleType: 'R', EwayBillNo: 0, EwayBillId: 0, TransporterGST: '' };
        $scope.addVehicle = function () {

            var m = $('#frmewaybill_addvehhicle').valid();

            if (!m) {
                return;
            }
            var model = $scope.EwayBill;
            model.EwayBillNo = $scope.SelectedBill.EwayBillNo;
            model.EwayBillId = $scope.SelectedBill.EwayBillId;



            var ewayBillObj = new $.EwayBill();
            ewayBillObj.UpdateVehicle(function (e) {
                if (e.data.Code == 500) {
                    alert(e.data.Message);
                    return;
                }
                alert('Vehicle information updated successfully.');
                $scope.showPeview($scope.SelectedBill);
            }, model)
        }

        $scope.showChangeTPForm = function () {
            transDto.GetAll(function (e) {
                $scope.Transporters = e.data;

            });
            $('#changeTransporter').modal('show');
        }
        $scope.ChangeTransporter = function () {

            var ewayBillObj = new $.EwayBill();
            var model = $scope.EwayBill;
            model.EwayBillNo = $scope.SelectedBill.EwayBillNo;
            model.EwayBillId = $scope.SelectedBill.EwayBillId;
            ewayBillObj.UpdateTransporter(function (e) {
                if (e.data.Code == 500) {
                    alert(e.data.Message);
                    return;
                }
                alert('Transporter information updated successfully.');
                $scope.showPeview($scope.SelectedBill);
            }, model)
        }
        $scope.CancelBill = function () {

            var cnf = confirm('Are you sure you want to cancel the e-way bill');
            if (!cnf) {
                return;
            }
            var m = $('#frmcancelewaybill').valid();

            if (!m) {
                return;
            }
            var ewayBillObj = new $.EwayBill();
            var model = $scope.EwayBill;
            model.EwayBillNo = $scope.SelectedBill.EwayBillNo;
            model.EwayBillId = $scope.SelectedBill.EwayBillId;
            ewayBillObj.CancelBill(function (e) {
                if (e.data.Code == 500) {
                    alert(e.data.Message);
                    return;
                }
                alert('E-way bill cancelled successfully.');
                $scope.showPeview($scope.SelectedBill);
            }, model)
        }
        $scope.portalBills = [];
        $scope.fetchFromPortal = function () {

            if ($scope.filter.CreatedOn == '' || $scope.filter.DocType == '0') {
                alert('Please selec the filter');
                return;
            }
            var ewayBillObj = new $.EwayBill();
            var model = cloneObj($scope.filter);
            model.CreatedOn = formatdate(model.To);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);
            ewayBillObj.FetchFromPortalByDate(function (e) {
                if (e.data.Code == 200) {
                    $scope.portalBills = e.data.Data;
                    persistEwayBillsFilter();
                    $('#dlgPortalBills').modal('show');
                }
                else {
                    alert(e.data.Message);
                }
            }, model);
        };

        $scope.mapLocally = function (item) {

            var ewayBillObj = new $.EwayBill();
            ewayBillObj.MapEwayBill(function (e) {

                if (e.data.Code == 200) {
                    if (e.data.Data != true) {
                        alert(e.data.Message);
                        return;
                    }
                    alert('Bill updated successfully.');
                    $('#dlgPortalBills').modal('hide');
                }
                else {
                    alert(e.data.Message);
                }
            }, item);
        };

        $scope.edit = function (item) {
            persistEwayBillsFilter();
            var k = item.EwayBillId + ',' + item.DocType + ',' + item.InvoiceId;
            var ec = $crypto.encrypt(k);
            $state.go('editEwayBill', { key: ec });
        };
    }]);
