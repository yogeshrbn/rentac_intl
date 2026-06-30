angular.module('medRack').factory('LedgerFactory', [
    function () {
        return {
            GetAccountBalanceForBill: function (func, filter) {
                var ledger = new $.Ledger(filter);
                ledger.GetAccountBalanceForBill(func);
            },
            SearchClient: function (func, filter) {

                var ledger = new $.Ledger(filter);
                ledger.SearchClient(func, filter);
            },
            GetAccountLedger: function (filter, func) {

                var ledger = new $.Ledger(filter);
                ledger.GetAccountLedger(func, filter);
            },
            AddLedger: function (func, data) {
                var ledger = new $.Ledger(data);
                ledger.Add(func);
            },
            GetAllParties: function (func) {

                var ledger = new $.Ledger({});
                ledger.GetAllActive(func);
            },
            GetSiteTaxes: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.GetSiteTaxes(func, filter);
            },
            AddMasterSite: function (func, data, docFiles) {
                var ledger = new $.Ledger({});
                ledger.AddMasterSite(func, data, docFiles);
            },
            GetMasterSites: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.GetMasterSites(func, filter);
            },
            GetMasterSiteById: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.GetMasterSiteById(func, filter);
            },
            GetLastBill: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.GetLastBill(func, filter);
            },
            PartyStockBalance: function (filter, o) {
                var ledger = new $.Ledger({});
                ledger.StockBalance(o, filter);
            },
            PartyStockBalance_BySize: function (filter, o) {
                var ledger = new $.Ledger({});
                ledger.StockBalanceBySize(o, filter);
            },
            Remove: function (o, filter) {
                var ledger = new $.Ledger({});
                ledger.Remove(o, filter);
            },
            DueMatReminder: function (o, filter) {
                var ledger = new $.Ledger({});
                ledger.DueMatReminder(o, filter);
            },
            DueBillReminder: function (o, filter) {
                var ledger = new $.Ledger({});
                ledger.DueBillReminder(o, filter);
            },
            GetTransactionById: function (func, filter) {
                var ledgerTran = new $.LedgerTrasaction({});
                ledger.GetTransactionById(func, filter);
            },
            PartyWiseRates: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.PartyWiseRates(func, filter);
            },
            EstimatedRentPerDay: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.EstimatedRentPerDay(func, filter);
            },
            PrintTransaction: function (func, filter) {
                var ledgerTran = new $.LedgerTrasaction({});
                ledgerTran.PrintTransaction(func, filter);
            },
            GetDrCrNotes: function (func, filter) {
                var ledgerTran = new $.LedgerTrasaction({});
                ledgerTran.GetDrCrNotes(func, filter);
            },
            EmailDrCrNote: function (func, filter) {
                var ledgerTran = new $.LedgerTrasaction({});
                ledgerTran.EmailDrCrNote(func, filter);
            },
            GetPartyBalance: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.GetPartyBalance(func, filter);
            },
            GetUnbilledSites: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.GetUnbilledSites(func, filter);
            },
            GetAllClientSites: function (func, filter) {
                var ledger = new $.Ledger({});
                ledger.GetAllClientSites(func, filter);
            }
        }
    }]);
angular.module('medRack').factory('WorkOrderFactory', ['$window', 'ModalFactory',
    function ($window, ModalFactory) {
        return {
            PrintItemReceivingSlip: function (data, o) {

                //var filter = { GRNId: grnId };
                var wOrder = new $.WorkOrder({});
                var postBody = data;
                //if (headerTypePlain === 'original' || headerTypePlain === 'duplicate') {
                //    postBody = { grnIds: data, headerType: headerTypePlain };
                //}
                wOrder.ItemsReceived_Print(postBody, function (e) {
                    ModalFactory.Dialog.hide();
                    var filePath = SERVER_RPT_URL + 'temp/' + e.data;
                    // $window.target = '_blank';
                    $window.open(filePath);
                    if (o) {
                        o.call();
                    }
                });
            },
            GetItemsByGrnId: function (grnId, o) {
                var grn = new $.GRN({ GRNId: grnId });
                grn.GetItemsByGRNId(o);
            },
            GetOtherCharges: function (wokOrderId, o) {
                var workOrder = new $.WorkOrder({ WorkOrderId: wokOrderId });
                workOrder.GetOtherCharges(o);
            },
            GeSiteOtherBalanceCharges: function (ledgerSiteId, o) {
                var workOrder = new $.WorkOrder();
                workOrder.GeSiteOtherBalanceCharges(o, { LedgerSiteId: ledgerSiteId });
            },
            EmailIssueChallan: function (workOrderId, o) {
                var workOrder = new $.WorkOrder();
                workOrder.EmailIssueChallan(o, { WorkOrderId: workOrderId });
            },
            SMSIssueChallan: function (workOrderId, o) {
                var workOrder = new $.WorkOrder();
                workOrder.SmsIssueChallan(o, { WorkOrderId: workOrderId });
            },
            EmailReceivedReceipt: function (grnId, o) {
                var workOrder = new $.WorkOrder();
                workOrder.EmailReceivedReceipt(o, { GRNId: grnId });
            },
            SMSReceivedReceipt: function (grnId, o) {
                var workOrder = new $.WorkOrder();
                workOrder.SMSReceivedReceipt(o, { GRNId: grnId });
            },
            AddChallanDocument: function (data, files, o) {
                var workOrder = new $.WorkOrder();
                workOrder.AddChallanDocument(data, files, o);
            },
            GetChallanDocuments: function (o, filter) {
                var workOrder = new $.WorkOrder();
                workOrder.GetChallanDocuments(o, filter);
            },
            DeleteChallanDocument: function (o, filter) {
                var workOrder = new $.WorkOrder();
                workOrder.DeleteChallanDocument(o, filter);
            },
            PendingChallanAcknowldgements: function (o, filter) {
                var workOrder = new $.WorkOrder();
                workOrder.PendingChallanAcknowldgements(o, filter);
            }

        }

    }]);
angular.module('medRack').factory('InventoryFactory', function () {
    return {
        //all items stock for a company.
        ItemStock: function (o) {
            var inventory = new $.Inventory({});
            inventory.ItemStock(o);
        },
        PartyStockBalance: function (filter, o) {
            var ledger = new $.Ledger({});
            ledger.StockBalance(o, filter);
        },
        PartyStockBalance_BySize: function (filter, o) {
            var ledger = new $.Ledger({});
            ledger.StockBalanceBySize(o, filter);
        },
        PostStockTxn: function (data, o) {
            var inventory = new $.Inventory({});
            inventory.PostStockTxn(data, o);
        },
        StockAdjustmentList: function (filter, o) {
            var inventory = new $.Inventory({});
            inventory.StockAdjustmentList(filter, o);
        },
        StockAdjustmentDetails: function (filter, o) {
            var inventory = new $.Inventory({});
            inventory.StockAdjustmentDetails(filter, o);
        },
        DeleteStockAdjustment: function (filter, o) {
            var inventory = new $.Inventory({});
            inventory.DeleteStockAdjustment(filter, o);
        },
        GetStockInhand: function (o, filter) {
            var inventory = new $.Inventory({});
            inventory.GetStockInhand(o, filter);
        },
        GetStockSummary: function (o, filter) {
            var inventory = new $.Inventory({});
            inventory.GetStockSummary(o, filter);
        }
    }
});
angular.module('medRack').factory('LookupService', function () {
    return {
        GetAllEmployeeRoles: function (o) {
            var lookup = new $.Lookup({});
            lookup.GetAllEmployeeRoles(o);
        },
        GetAllSystemRoles: function (o) {
            var lookup = new $.Lookup({});
            lookup.GetAllSystemRoles(o);
        },
        GetOtherCharges: function (o) {
            var lookup = new $.Lookup({});
            lookup.GetOtherCharges(o);
        }
    }
});
angular.module('medRack').factory('EmployeeService', function () {
    return {
        GetAllEmployees: function (o) {
            var employee = new $.Employee({});
            employee.GetAll(o);
        },
        Add: function (o, data) {
            var employee = new $.Employee({});
            employee.Add(o, data);
        },
        GetInfo: function (o, data) {
            var employee = new $.Employee({});
            employee.GetById(o, data);
        }
    }
});
//angular.module('medRack').factory('CompanyService', function () {
//    return {
//        SearchCompany: function (o, filter) {
//            var comp = new $.Company({});
//            comp.SearchCompany(o, filter);
//        },
//        GetAllVehicle: function (o) {
//            var vehicle = new $.Vehicle({});
//            vehicle.GetAll(o);
//        },
//        AddVehicle: function (o, data) {
//            var vehicle = new $.Vehicle({});
//            vehicle.Add(o, data);
//        },
//        GetVehicleInfo: function (o, data) {
//            var vehicle = new $.Vehicle({});
//            vehicle.GetById(o, data);
//        }

//    }
//});
angular.module('medRack').factory('ReportService', ['$window', '$http', 'FileSaver', function ($window, $http, FileSaver) {
    var reportURL = REPORT_SERVER;
    return {
        printBills: function (data) {
            var econded = btoa(data);
            return $http({
                method: 'GET',
                responseType: 'blob',
                url: reportURL + 'Home/Print/' + econded
            });

        },
        balanceSheet: function (o, filter) {
            var report = new $.Reports({});

            report.balanceSheet(o, filter);
        },
        pnlStatement: function (o, filter) {
            var report = new $.Reports({});

            report.pnlStatement(o, filter);
        },
        trialBalance: function (o, filter) {
            var report = new $.Reports({});

            report.trialBalance(o, filter);
        },
        //due bills of the company as of current date 
        DueBills: function (o, filter) {
            var billing = new $.Billing({});
            billing.DueBills(o, filter);
        },
        PrintDueBills: function (o, filter) {
            var billing = new $.Billing({});
            billing.PrintDueBills(o, filter);
        },
        PrintDueBillsPdf: function (o, filter) {
            var billing = new $.Billing({});
            billing.PrintDueBillsPdf(o, filter);
        },
        Print: function (file) {

            var filePath = SERVER_RPT_URL + 'temp/' + file;
            // $window.target = '_blank';
            $window.open(filePath);
        },
        //gstr1: function (o, filter) {
        //    var report = new $.Reports({});

        //    report.gstr1(o, filter);
        //},
        gstSalesTaxReport: function (o, filter) {
            var report = new $.Reports({});

            report.gstSalesTaxReport(o, filter);
        },
        gstPurchaseTaxReport: function (o, filter) {
            var report = new $.Reports({});

            report.gstPurchaseTaxReport(o, filter);
        },
        loadPreviewFromReportServer: function (o, encrypedText, headerTypePlain) {
            var econded = btoa(encrypedText);
            if (headerTypePlain === 'original' || headerTypePlain === 'duplicate') {
                econded = econded + '?headerType=' + encodeURIComponent(headerTypePlain);
            }
            var report = new $.Reports();
            report.previewReport(o, econded);
        },
        printFromReportServer: function (encrypedText, fileName = null) {
            var econded = btoa(encrypedText);
            
            var report = new $.Reports();
            report.downloadReport(function (e) {
                var pdfFileName = 'text.pdf';
                if (fileName != null) {
                    pdfFileName = fileName;
                }
                FileSaver.saveAs(e.data, pdfFileName);
            }, econded);
        },
        gstr1: function (_data) {
            debugger
            if (_data.Print == true) {
                return $http({
                    method: 'POST',
                    data: _data,
                    responseType: 'blob',
                    url: reportURL + 'api/reports/Gstr1'
                }, _data);
            }
            else {
                return $http.post(reportURL + 'api/reports/Gstr1', _data);
            }
        }
    }
}]);
angular.module('medRack').factory('UserRoleService', function () {
    return {
        //due bills of the company as of current date 
        AddRoleFunction: function (o, data) {
            var userRole = new $.UserRole({});
            userRole.AddRoleFunction(o, data);
        },
        GetRoleFunctions: function (o, data) {
            var userRole = new $.UserRole({});
            userRole.GetRoleFunctions(o, data);
        }
    }
});
angular.module('medRack').factory('UserService', function () {
    return {
        //due bills of the company as of current date 
        getLicenseInfo: function (o) {
            var userRole = new $.UserRole({});
            userRole.AddRoleFunction(o, data);
        },
        GetRoleFunctions: function (o, data) {
            var userRole = new $.UserRole({});
            userRole.GetRoleFunctions(o, data);
        },
        ChangePassword: function (o, data) {
            var userRole = new $.User({});
            userRole.changePassword(o, data);
        },
        sendForgotPwdLink: function (o, email) {
            var userRole = new $.User({});
            userRole.sendForgotPwdLink(o, email);
        },
        validateForgotPwdLink: function (o, guid) {
            var userRole = new $.User({});
            userRole.validateForgotPwdLink(o, guid);
        },
        resetPasswordByGuId: function (o, model) {
            var userRole = new $.User({});
            userRole.resetPasswordByGuId(o, model);
        }
    }
});