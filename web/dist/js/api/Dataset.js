(function ($) {



    $.Company = function (options) {
        var defaults = {
            CompanyId: 0,
            Name: null,
            Address1: null,
            Address2: null,
            Email: null,
            Phone1: null,
            Phone2: null,
            City: null,
            State: null,
            Contact: null,
            Web: null,
            ZipCode: null,
            IsActive: null,
            TIN: null,
            TAN: null,
            PAN: null,
            SignAuthority: null,
            GSTNo: null,
            ReportHeader: null,
            StateId: 0,
            Logo: null,
            DefaultWarehouseId: 0,
            WarehouseIds: ''

        };

        this.Props = $.extend(defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
    };
    $.Company.prototype = {

        Add: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(ADDCOMPANY);
        },
        GetAll: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(GETALLCOMPANIES, 'GET');
        },
        GetBranches: function (o, companyId) {
            var url = 'company/Branches?companyId=' + companyId;
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(url, 'GET');
        },

        GetDetails: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(GETCOMPANYDETILS, 'POST');
        },
        GetTaxPayerDetails: function (o, gstNo) {
            var url = 'company/GetTaxPayerDetails?gstNo=' + gstNo;
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(url, 'GET');
        },
        UpdateGSTDetails: function (o, data) {
            var url = 'company/UpdateGSTDetails';
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(url, 'POST');
        },
        UpdateEInvoicing: function (o, data) {
            var url = 'company/UpdateEInvoicing';
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(url, 'POST');
        },
        SaveAndValidateIRPUser: function (o, data) {
            var url = 'settings/UpdateIRPCredentials';
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(url, 'POST');
        },
        UpdateEwayCreds: function (o, data) {
            var url = 'settings/UpdateEwayBillCreds';
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(url, 'POST');
        },
        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        },

        ActivateDeActivate: function (o) {
            this.BuildRequest(o);
            this.HTTP.Exec(ACTIVATE_DEACTIVATE_COMPANY, 'POST');
        },
        SearchCompany: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(SEARCH_COMPANY);
        },

    };

    //----Client
    $.Ledger = function (options) {
        var defaults = {
            LedgerId: 0,
            Name: null,
            TradeName: null,
            UseTradeNameForBilling: 0,
            Code: null,
            Address1: null,
            Address2: null,
            Email: null,
            Phone1: null,
            Phone2: null,
            City: null,
            State: null,
            Contact: null,
            Web: null,
            ZipCode: null,
            IsActive: null,
            TIN: null,
            TAN: null,
            DefaultRate: 0,
            SysDefined: 0,
            AccountGroup: null,
            GSTNo: null, OpeningBal: null, TransType: null,
            AadharCard: null, ServiceTaxNumber: null,
            PAN: null, ContactPersonName: null,
            ContactPersonDesignation: null,
            ContactPersonOffPhone: null, ContactPersonMobile: null,
            IsActive: 1,
            Rates: []
        };

        this.Props = $.extend(defaults, options);
        this.LEDGER_TRANACTIONS = 'ledger/LedgerTransactions';
        this.ALL_ACCOUNT_IN_GROUP_TRANSACTIONS = 'ledger/LedgerAccGroupTransactions';
        this.ALL_LEGER_BY_GROUPS = 'ledger/GetAllByGroups';
        this.CLOSE_SITE = 'ledger/CloseSite';
        this.COPY_RPODUCT_RATES = 'ledger/CopyRates';
        this.GET_AVANCE_RECEIPTS = 'ledger/getAdvanceReceipts';


        //  this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
    };
    $.Ledger.prototype = {
        getAdvanceReceipts: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.GET_AVANCE_RECEIPTS);
        },
        CopyProductRates: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.COPY_RPODUCT_RATES);
        },
        CloseSite: function (o, data) {

            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.CLOSE_SITE);
        },
        GetAllByGroups: function (o, data) {

            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            var url = this.ALL_LEGER_BY_GROUPS + '?groups=' + data;
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(url, 'GET');
        },
        GetAccountBalanceForBill: function (o) {

            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            var url = LEDGER_BALANCE_FORBILLING + '?ledgerId=' + this.Props.LedgerId;
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(url, 'GET');
        },
        AllClientsWithSites: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            var url = 'ledger/AllClientsWithSites';
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(url, 'GET');
        },
        Add: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(JSON.stringify(this));
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ADDCLIENT, null, 'string', null);
        },
        SearchClient: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(SEARCH_CLIENT);
        },
        GetAll: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GETALLCLIENTS, 'GET');
        },
        GetAllActive: function (o) {

            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {

                o.call(null, e);
            };
            http.Exec(GET_ALL_ACTIVE_CIENTS, 'GET');
        },
        GetDetails: function (o) {

            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GETCLIENTDETAILS, 'POST');
        },
        Remove: function (o, filter) {

            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(REMOVE_LEDGER, 'POST');
        },

        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        },

        ActivateDeActivate: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ACTIVATE_DEACTIVATE_CLIENT, 'POST');
        },
        PartyStockRegister: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_STOCK_REGISTER);
        },
        PartyOpeningBalance: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_OPENING_BALANCE);
        },
        PartyStockRegisterPrint: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_STOCK_REGISTER_PRINT);
        },
        GetTranDetails: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(LEDGER_TRAN_DATA_BY_LOOKUP);
        },
        BankEntryRegister: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(BANK_TRAN_REGISTER);
        },
        BankEntryRegister_rpt: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(BANK_TRAN_REGISTER_RPT);
        },
        StockBalance: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_STOCK_BALANCE);
        },
        StockBalanceBySize: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_STOCK_BALANCE_BY_SIZE);
        },
        Print_StockBalance_report: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_STOCK_BALANCE_REPORT);
        },
        Print_StockBalance_Dashboard: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_STOCK_BALANCE_DASHBOARD);
        },
        PrintReport: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(this, e);
            };
            http.Exec(PRINT_REPORT);
        },
        GetAccountLedger: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ACCOUNT_LEDGER);
        },
        GetLedgerTransactions: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.LEDGER_TRANACTIONS);
        },
        LedgerAccGroupTransactions: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.ALL_ACCOUNT_IN_GROUP_TRANSACTIONS);
        },
        GetAccountLedger_rpt: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ACCOUNT_LEDGER_RPT);
        },
        GetClientWiseItems: function (o, filter, promiseOnly = false) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            http.init(filter);
            http.Success = function (e) {
                if (o)
                    o.call(null, e);
            };
            return http.Exec(CLIENT_WISE_ITEMS);
        },
        GetItemWiseClients: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ITEM_WISE_CLIENTS);
        },
        DeleteTransaction: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(LEDGER_TRAN_DELETE);
        },
        GetSites: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            //  GetClientSites
            http.Exec(GET_LEDGER_RUNNING_SITES);
        },
        GetProductRates: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GET_LEDGER_PRODUCT_RATES);
        },
        AddProductRates: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            var s = JSON.stringify(this);
            http.init(s);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ADD_UPDATE_LEDGER_RATES, null, 'string', null);
        },
        UpdateProductRates: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            // var s = JSON.stringify(this);
            http.init(JSON.stringify(data));
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ADD_UPDATE_LEDGER_RATES, null, 'string', null);
        },
        SiteWiseLedger: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GET_SITE_WISE_LEDGER);
        },
        GetSiteTaxes: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GET_SITE_TAXES);
        },
        GetMasterSites: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GET_LEDGER_MASTER_SITES);
        },
        GetMasterSiteById: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GET_LEDGER_MASTER_SITE_BY_ID);
        },
        AddMasterSite: function (o, data, docFiles) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            if (docFiles && docFiles.length) {
                var hasFile = false;
                for (var j = 0; j < docFiles.length; j++) {
                    if (docFiles[j]) {
                        hasFile = true;
                        break;
                    }
                }
                if (hasFile) {
                    http.init(JSON.stringify(data));
                    http.Exec(ADD_LEDGER_MASTER_SITE, null, 'form', docFiles);
                } else {
                    http.Exec(ADD_LEDGER_MASTER_SITE);
                }
            } else {
                http.Exec(ADD_LEDGER_MASTER_SITE);
            }
        },
        GetLastBill: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(LAST_BILL);
        },
        DueMatReminder: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(DUE_MAT_REMINDER);
        },
        DueBillReminder: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(DUE_BILL_REMINDER);
        },
        PartyWiseRates: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_WISE_RATES);
        },
        EstimatedRentPerDay: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ESTIMATED_RENT_PER_DAY);
        },
        GetPartyBalance: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PARTY_BALANCE_AMOUNT);
        },
        GetUnbilledSites: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(UNBILLED_SITES);
        },
        GetCashBook: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(CASHBOOK);
        },
        GetStockInhand: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(STOCK_IN_HAND);
        },
        GetStockSummary: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(STOCK_SUMMARY);
        },
        GetAllClientSites: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GET_ALL_CLIENT_SITES);
        },
        GetClientJobs: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GET_CLIENT_JOBS);
        }

    };

    $.LedgerTrasaction = function (options) {
        var defaults = {
            DrLedgerId: 0,
            CrLedgerId: 0,
            TransactionAmount: 0,
            TransactionDate: '',
            Description: '',
            TransactionMode: 0,
            TransactionType: 0,
            LedgerName: '',
            Discount: 0,
            NetAmount: 0,
            EntryType: 0,
            Narration: '',
            TranRefNumber: '',
            WorkOrderId: 0,
            ContractId: 0,
            LedgerSiteId: 0,
            LedgerId: 0
        };
        var Props = $.extend(defaults, options);
        this.CrLedgerId = Props.CrLedgerId;
        this.DrLedgerId = Props.DrLedgerId;
        this.TransactionAmount = Props.TransactionAmount;
        this.TransactionDate = Props.TransactionDate;
        this.Description = Props.Description;
        this.TransactionMode = Props.TransactionMode;
        this.TransactionType = Props.TransactionType;
        this.LedgerName = Props.LedgerName;
        this.Discount = Props.Discount;
        this.NetAmount = Props.NetAmount;
        this.EntryType = Props.EntryType;
        this.Narration = Props.Narration;
        this.TranRefNumber = Props.TranRefNumber;
        this.WorkOrderId = Props.WorkOrderId;
        this.ContractId = Props.ContractId;
        this.LedgerSiteId = Props.LedgerSiteId;
        this.LedgerId = Props.LedgerId;
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        this.TRANSACTION_BYID_FOR_EDIT = "ledger/GetTransaction";
        this.LedgerTransactionsAll = 'ledger/LedgerTransactionsAll';
    };
    $.LedgerTrasaction.prototype = {
        CreateTransaction: function (o, data) {
            if (data) {
                this.HTTP.init(data);
            }
            else
                this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(CREATE_TRANSACTION);
        },
        GetLookup: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(LEDGER_TRANSACTION_LOOKUP);
        },
        GetReceiptRegister: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(RECEIPT_REGISTER);
        },
        StageQuickReceiptImage: function (o, files) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init('{}');
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(STAGE_QUICK_RECEIPT_IMAGE, 'POST', 'string', files);
        },
        ClearQuickReceiptDocument: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(CLEAR_QUICK_RECEIPT_DOCUMENT);
        },
        GetContractReceiptPayments: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GET_CONTRACT_RECEIPT_PAYMENTS);
        },
        PrintReceipts: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            //  GetClientSites
            this.HTTP.Exec(PRINT_RECEIPTS);
        },
        PrintTransaction: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            //  GetClientSites
            this.HTTP.Exec(PRINT_TRANSACTION);
        },
        GetDrCrNotes: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            //  GetClientSites
            this.HTTP.Exec(GET_DR_CR_NOTES);
        },
        GetAllTransactions: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            //  GetClientSites
            this.HTTP.Exec(this.LedgerTransactionsAll);
        },

        EmailDrCrNote: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            //  GetClientSites
            this.HTTP.Exec(EMAIL_DR_CR_NOTE);
        },
        GetTransactionById: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            //  GetClientSites
            this.HTTP.Exec(GET_TRAN_BY_ID);
        },
        GetTransactionByForEditId: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            //  GetClientSites
            this.HTTP.Exec(this.TRANSACTION_BYID_FOR_EDIT);
        }
    };
    //---------Salt
    $.Salt = function (options) {
        var defaults = {
            SaltId: 0,
            Name: null,
            Dosage: null,
            DrugInstructions: null,
            Indications: null,
            SideEffects: null,
            Precautions: null,
            Note: null,
            Narcotic: false,
            SCH_H: false,
            SCH_H1: false,
            CreatedBy: null,
            StoreId: 0,
            Status: 1
        };

        this.Props = $.extend(defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
    };
    $.Salt.prototype = {
        GetAll: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(GETALLSALTS);
        },
        Add: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(ADDSALT);
        },
        GetDetails: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(GETSALTINFO, 'POST');
        },
        UpdateStatus: function (o) {
            this.BuildRequest(o);
            this.HTTP.Exec(UPDATE_SALT_STATUS, 'POST');
        },

        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        }
    };
    //----end of salt dataset

    //---------------product dataset start
    $.Product = function (options) {
        var defaults = {
            ProductId: 0,
            Name: '',
            Code: '',
            UOM: '',
            UOM2: null,
            ApplyUnit2Rate: false,
            Size: '',
            CompanyId: 1,
            ProductType: 1,
            Status: 1,
            LocalTax: '',
            VatRate: 0.000,
            CST: 1,
            CSTRate: 0.000,
            ExiseRate: 0.000,
            PurchaseRate: 0.000,
            MRP: 0.000,
            Category: 0,
            Description: '',
            CompanyName: '',
            Rate: 0.00,
            LossRate: 0.00,
            OpeningBalance: 0,
            TaxCategoryId: 0,
            SaleAccount: 0,
            PurchaseAccount: 0,
            PurchaseUnitId: null,
            SalePrice: 0,
            CostPrice: 0,
            RentRate: 0,
            BrekageRate: 0,
            WeightRate: 0


        }
        //this.Props= $.extend( defaults, options);
        $.extend(this, defaults, options);
        //this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        this.GET_DETAIL = "product/GetDetail";
        this.FIND_RPODUCT = "product/Search";
        this.DELETE_PRODUCT = "product/DeleteProduct";

        this.SAVE_BOM = "product/SaveBom";
        this.BOM_LIST = "product/BOMList";
        this.BOM_BY_ID = "product/BOMDetails";
        this.BOM_BY_PRODUCT_ID = "product/BOMByProductId";
        this.DELETE_BOM = "product/DeleteBom";

        this.ITEM_GROUP_SAVE = "product/SaveItemGroup";
        this.ITEM_GROUP_LIST = "product/ListItemGroup";
        this.ITEM_GROUP_BYID = "product/ItemGroupById";
        this.ITEM_GROUP_DEL = "product/DeleteItemGroup";


    };

    $.Product.prototype = {
        DelteItemGroup: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.ITEM_GROUP_DEL);
        },
        SaveItemGroup: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.ITEM_GROUP_SAVE);
        },
        ListItemGroup: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });


            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.ITEM_GROUP_LIST);
        },
        ItemGroupById: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.ITEM_GROUP_BYID);
        },
        DeleteProduct: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.DELETE_PRODUCT);
        },
        DeleteBOM: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.DELETE_BOM);
        },
        BOMByProductId: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.BOM_BY_PRODUCT_ID);
        },
        BOMDetails: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.BOM_BY_ID);
        },
        SaveBOM: function (o, data) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.SAVE_BOM);
        },
        BOMList: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            //http.init(data);

            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.BOM_LIST);
        },
        Find: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            //this.BuildRequest(o);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.FIND_RPODUCT);
        },
        GetAll: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            //this.BuildRequest(o);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GETALLPRODUCTS);
        },
        Add: function (obj, o) {
            debugger
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(obj);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(ADDPRODUCT, null, null);
        },
        GetInfo: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(this.GET_DETAIL);
        },

        GetUOMSize: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(GetUOMSize);
        },
        GetSizeList: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PRODUCT_SIZE_LIST);
        },
        GetSizeListByCompany: function (o) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(this);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(PRODUCT_SIZE_LIST_BY_COMPANY);
        }

        //BuildRequest: function (o) {
        //    this.HTTP.init(this);
        //    this.HTTP.Success = function (e) {
        //        o.call(null, e);
        //    };
        //}
    };

    //-------- end of product


    $.ProductCategory = function (options) {
        var defaults = {
            Name: null,
            MinMargin: 0.000,
            Status: 1,
            StoreId: STOREID
        };
        this.Props = $.extend(defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

        this.ADD = "productcategory/Save";
        this.GETALL = "productcategory/GetAll";
        this.GET_DETAIL = "productcategory/GetDetail";
        this.CHANGE_STATUS = "productcategory/ChangeStatus";



    };
    $.ProductCategory.prototype = {
        GetAll: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(this.GETALL);
        },
        Add: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.ADD);
        },
        ChangeStatus: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.CHANGE_STATUS);
        },
        GetInfo: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GET_DETAIL);
        },
        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        }
    };

    //----end productCategory----

    //--AccountGroups

    $.AccountGroup = function (options) {
        var defaults = {
            Name: null,
            ParentGroup: null,
            StoreId: STOREID,
            IsActive: null,
            GroupCode: null,
            AccountGroupId: 0,
            PrimaryGroup: null,
            Editable: null
        };
        this.Props = $.extend(defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

        this.GETALL = "Account/GetAllGroups";
        this.ADD = "Account/SaveGroup";
        this.GETINFO = "Account/GetGroupInfo";
        this.UPDATESTATUS = "Account/UpdateGroupStatus";
        this.BANK_ENTRY_TYPES = "Account/BankEntryTypes";
        this.GET_BANKS = "Account/GetBanks";
        this.GET_PURCHASE = "Account/PurchaseAccounts";
        this.GET_ACCOUNT_BY_GROUP = "Account/GetAccountsByGroup";
        this.GET_BANK_ENTRIES = "Account/GetBankEntries";
    };
    $.AccountGroup.prototype = {
        GetAll: function (o) {
            this.BuildRequest(o);
            this.HTTP.Exec(this.GETALL);
        },
        Add: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.ADD);
        },
        GetInfo: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GETINFO);
        },
        UpdateStatus: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.UPDATESTATUS);
        },
        GetBankEntryTypes: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.BANK_ENTRY_TYPES);
        },
        GetBanks: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GET_BANKS);
        },
        GetAccountsByGroup: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GET_ACCOUNT_BY_GROUP);
        },
        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        },
        GetBankEntries: function (o, filter) {
            this.HTTP.init(filter);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GET_BANK_ENTRIES);
        }
    };
    //end AccountGroups
    //Ledger
    /*
    $.Ledger = function (options) {
        var defaults = {
            Name: null,
            Code: null,
            StoreId: STOREID,
            IsActive: null,
            Email: null,
            ZipCode: 0,
            Group: null,
            Website: null,
            ContactPersonName: null,
            ContactLastName: null,
            ContactPersonDesignation: null,
            OffPhone: null,
            ContactPhoneResi: null,
            Fax: null,
            DLNo: null,
            DLExpDate: null,
            FinYear: null,
            OpeningBalance: null,
            PAN: null,
            STNo: null,
            STExpDate: null,
            Address1: null,
            Address2: null,
            StateId: null,
            CityId: null,
            ContactPersonPhoneOff: null,
            ContactPersonMobile: null,
            LedgerId: 0,
            IsActive: 0

        };

        this.Props = $.extend(defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

        this.GETALL = "Account/GetAllLedger";
        this.ADD = "Account/SaveLedger";
        this.GETINFO = "Account/GetLedgerInfo";
        this.UPD_ADDRESS = "Account/SaveAddress";
        this.CHANGESTATUS = "Account/ChangeLedgerStatus";
        this.FINDLEDGER = "Account/FindLedger";
    };
    $.Ledger.prototype = {
        GetAll: function (o) {
            this.BuildRequest(o);
            this.HTTP.Exec(this.GETALL);
        },
        Add: function (o) {
            this.HTTP.init(this);
            debugger

            var d = JSON.stringify(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.ADD);
        },
        SaveAddress: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.UPD_ADDRESS);
        },
        GetInfo: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GETINFO);
        },
        ChangeStatus: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.CHANGESTATUS);
        },
        Find: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.FINDLEDGER);
        },
        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        }
    };
    */
    //end of ledger
    //start of StateAndCity
    $.StateCity = function (options) {
        var defaults = {
            StateId: null,
            StateName: null,
            CityId: null,
            CityName: null
        };
        this.Props = $.extend(defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        this.GET_ALL_STATES = "StateCity/GetAllStates";
        this.GET_ALL_CTIES = "StateCity/GetCities";
    };
    $.StateCity.prototype = {
        GetStates: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GET_ALL_STATES);
        },
        GetCities: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GET_ALL_CTIES);
        },
        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        }
    };

    //end AccountGroups
    //Ledger
    $.WorkOrder = function (options) {
        var defaults = {
            Name: null,
            StoreId: STOREID,
            WorkOrderId: 0,
            WorkOrderDate: null,
            Number: null,
            Items: null,
            Taxes: null,
            SubTotal: null,
            Total: null,
            Tax: null,
            Site: null,
            SitePic: null,
            LedgerId: null,
            CompanyId: null,
            ClientAmount: 0,
            JobNumber: null,
            Client: null,
            Closed: 0,
            InvoiceNumber: null,
            SiteId: 0,
            Details: null,
            InvoiceDate: null,
            ChallanHeaderType: 0,
            Edit: false


        };
        var Props = $.extend(defaults, options);
        this.StoreId = Props.StoreId;
        this.WorkOrderId = Props.WorkOrderId;
        this.WorkOrderDate = Props.WorkOrderDate;
        this.Number = Props.Number;
        this.Items = Props.Items;
        this.Taxes = Props.Taxes;
        this.Name = Props.Name;
        this.SubTotal = Props.SubTotal;
        this.Total = Props.Total;
        this.Tax = Props.Tax;
        this.Site = Props.Site;
        this.SitePic = Props.SitePic;
        this.LedgerId = Props.LedgerId;
        this.CompanyId = Props.CompanyId;
        this.ClientAmount = Props.ClientAmount;
        this.Closed = Props.Closed
        this.InvoiceNumber = Props.InvoiceNumber
        this.SiteId = Props.SiteId
        this.Details = Props.Details
        this.InvoiceDate = Props.InvoiceDate;
        this.ChallanHeaderType = Props.ChallanHeaderType;
        this.Edit = Props.Edit;
        var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        this.SaveWorkOrder = "WorkOrder/SaveWorkOrder";
        this.ADD_MAT_LOSS = "workorder/SaveMatLoss";
        this.MAT_LOSS_ALL = "workorder/MatLossList";
        this.MAT_LOSS_BY_ID = "workorder/MatLossById";
        this.MAT_LOSS_DEL = "workorder/DeleteMatLossId";
        this.ADD_PRODUCTION_WO = "workorder/AddProductionWO";
        this.WORK_ORDERS_LIST = "workorder/WorkOrders";
        this.JOB_WORKORDER_BYID = "workorder/JobWoById";
        this.DELETE_CHALLAN = "workorder/DeleteChallan";
        this.GET_LAST_CHALLAN_NUMBER = "workorder/GetLastChallanNo";
        this.GET_NEXT_CHALLAN_NUMBER_PREVIEW = "workorder/GetNextChallanNumberPreview";
        this.GET_LAST_RUTRN_CHALLAN_NUMBER = "workorder/GetLastReturnChallanNo";
        this.GET_NEXT_RECEIVING_CHALLAN_PREVIEW = "workorder/GetNextReceivingChallanNumberPreview";
        this.MAT_TRANSFER_BY_ID = "workorder/MatTransferDetailsById";

    };
    $.WorkOrder.prototype = {
        GetLastReturnChallanNo: function (o, data, promiseOnly = false) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.GET_LAST_RUTRN_CHALLAN_NUMBER);
        },
        GetNextReceivingChallanNumberPreview: function (o, data, promiseOnly = false) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.GET_NEXT_RECEIVING_CHALLAN_PREVIEW);
        },
        LastChallanNumber: function (o, data, promiseOnly = false) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.GET_LAST_CHALLAN_NUMBER);
        },
        NextChallanNumberPreview: function (o, data, promiseOnly = false) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.GET_NEXT_CHALLAN_NUMBER_PREVIEW);
        },
        DeleteChallan: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {

                o.call(null, e);
            };

            HTTP.Exec(this.DELETE_CHALLAN);


        },
        WorkOrders: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {

                o.call(null, e);
            };

            HTTP.Exec(this.WORK_ORDERS_LIST);


        },
        AddProductionWo: function (o, data, files) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {

                o.call(null, e);
            };

            HTTP.Exec(this.ADD_PRODUCTION_WO, null, null, files);


        },
        Add: function (o, files, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            if (data) {
                HTTP.init(JSON.stringify(data));
            }
            else
                HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {

                o.call(null, e);
            };

            //if (this.WorkOrderId > 0 && this.Edit == true) {
            //    HTTP.Exec(ADD_SITE, null, 'string', files);
            //}
            //else {
            HTTP.Exec(this.SaveWorkOrder, null, 'string', files);
            //}


        },
        Update: function (o, files) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {

                o.call(null, e);
            };

            HTTP.Exec(this.SaveWorkOrder, null, 'string', files);


        },
        GetAll: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(WORKORDER_GETALL, null);

        },
        AddMatLoss: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_MAT_LOSS);

        },
        MatLossList: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.MAT_LOSS_ALL);

        },
        MatLossById: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.MAT_LOSS_BY_ID);

        },
        DeleteMatLoss: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.MAT_LOSS_DEL);
        },
        //GetWorkOrdersByCompany: function (o) {
        //    var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        //    HTTP.init(this);
        //    HTTP.Success = function (e) {
        //        o.call(null, e);
        //    };
        //    HTTP.Exec(WORKORDER_GET_BY_COMPANY, null);

        //},
        GetWorkOrdersByCompany: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(WORKORDER_GET_BY_COMPANY);
        },
        GetDetail: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(WORKORDER_DETAIL, null);

        },
        JobWoById: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.JOB_WORKORDER_BYID, null);

        },
        GetItems: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(WORKORDERITEMS_DETAIL, null);

        },
        GetSites: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_SITES);
        },
        AddSite: function (o, files) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {

                o.call(null, e);
            };

            HTTP.Exec(ADD_SITE, null, 'string', files);

        },
        AddInvoice: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(ADD_INVOICE, null, 'string');
        },
        ItemsIssued: function (d, o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(d);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(ITEM_ISSUED_REGISTER);
        },
        ItemsReceived: function (d, o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(d);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(ITEM_RECEIVED_REGISTER);
        },
        ItemsReceived_Print: function (d, o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(d);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(ITEM_RECEIVED_REGISTER_RPT);
        },
        PrintIssuedReceipt: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(PRINT_ISSUED_LIST);
        },
        GetWorkOrderBalance: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(WORK_ORDRER_BALANCE);
        },
        WorkOrderDueDateReminder: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(WORK_ORDRER_DUE_DATES_REMINDER);
        }, WorkOrderOverDuesReminder: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(WORK_ORDER_OVERDUE_REMINDER);
        },
        AddChallanToSite: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(ADD_SITE_CHALLAN);
        },
        GetSiteChallans: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_SITE_CHALLANS);
        },
        GetOtherCharges: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_OTHER_CHARGES);
        },
        GeSiteOtherBalanceCharges: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_SITE_OTHER_CHARGES);
        },
        EmailIssueChallan: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(EMAIL_ISSUE_CHALLAN);
        },
        SmsIssueChallan: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(SMS_ISSUE_CHALLAN);
        },
        EmailReceivedReceipt: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(EMAIL_RECEIVE_RECEIPT);
        },
        SMSReceivedReceipt: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(SMS_RECEIVE_RECEIPT);
        },
        AddChallanDocument: function (data, files, o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(JSON.stringify(data));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            //HTTP.Exec(ADD_CHALLAN_DOCUMENT);
            HTTP.Exec(ADD_CHALLAN_DOCUMENT, 'POST', 'string', files);
        },
        GetChallanDocuments: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_CHALLAN_DOCUMENTS);
        },
        DeleteChallanDocument: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(DEL_CHALLAN_DOCUMENT);
        },
        PendingChallanAcknowldgements: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(PENDING_CHALLAN_ACKNOWLEDGEMENTS);
        },
        TransferMaterial: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(data));
            HTTP.Success = function (e) {
                o.call(null, e);
            };

            HTTP.Exec(TRANSFER_MATERIAL, null, 'string', null);

        },
        MatTransferById: function (o, data, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(data);
            HTTP.Success = function (e) {
                if (o)
                    o.call(null, e);
            };
            return HTTP.Exec(MAT_TRANSFER_BY_ID);
        },
        AdjustMaterial: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(data));
            HTTP.Success = function (e) {
                o.call(null, e);
            };

            HTTP.Exec(ADJUST_MATERIAL, null, 'string', null);

        },
        AdjustMaterialList: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(ADJUST_MATERIAL_LIST);

        },
        AdjustMaterialById: function (o, data, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(data);
            HTTP.Success = function (e) {
                if (o)
                    o.call(null, e);
            };
            return HTTP.Exec(ADJUST_MATERILA_BY_ID);

        },
        DeleteWorkOrderItem: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(DELETE_WorkOrder_ITEM);
        },
        UpdateStatus: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(WORKORDER_UPDATE_STATUS);
        }

    };
    //-------
    $.WorkOrderItem = function (options) {
        var defaults = {
            Item: new $.Product(),
            ProductId: 0,
            PurchaseRate: null,
            Amount: 0.000,
            PurchaseQty: null,
            ExcessQty: 0,
            ShortQty: 0,
            Scrap: 0,
            Breakage: 0,
            Quantity: 0.00,
            SiteId: null,
            Rate: 0,
            DamageComponent: '',
            BreakageRate: 0,
            SentQty: 0,
            RecQty: 0,
            Product: null,
            ProductSizeId: 0,

            Size: ''
            , GRNItemId: 0
        }
        var Props = $.extend(defaults, options);
        this.Item = Props.Item.Props;
        this.PurchaseRate = Props.PurchaseRate;
        this.ExcessQty = Props.ExcessQty;
        this.Breakage = Props.Breakage;
        this.PurchaseQty = Props.PurchaseQty;
        this.SiteId = Props.SiteId;
        this.Rate = Props.Rate;
        this.DamageComponent = Props.DamageComponent;
        this.BreakageRate = Props.BreakageRate;
        this.ProductSizeId = Props.ProductSizeId;
        this.Size = Props.Size;
        this.ProductId = Props.ProductId;
        this.Product = Props.Product;
        this.GRNItemId = Props.GRNItemId;
        this.SentQty = Props.SentQty;
        this.Scrap = Props.Scrap;
        this.ShortQty = Props.ShortQty;

        this.Amount = Props.PurchaseRate * Props.PurchaseQty
    };

    //---------------WorkOrder Site
    $.Site = function (options) {
        var defaults = {
            SiteId: 0,
            WorkOrderId: '',
            JobNumber: null,
            ChallanNumber: null,
            Site: null,
            SiteEng: null,
            StartDate: null,
            Duration: 0,
            ShaftSize: null,
            ShaftHeight: null,
            Doc1: '',
            Doc2: '',
            Doc3: '',
            SubTotal: 0,
            TaxAmount: 0,
            Total: 0,
            Freight: 0,
            FreightTax: 0,
            CreatedBy: 0,
            CreatedDate: null,
            Closed: 0,
            PaymentClosed: 0,
            ClosedDate: null,
            PaymentClosedDate: null
            , LedgerId: 0,
            State: null,
            Weight: 0,
            ApproximateValue: 0,
            LRNumber: null,
            CRNumber: null,
            GRNumber: null

        }
        var Props = $.extend(defaults, options);
        this.SiteId = Props.SiteId;
        this.WorkOrderId = Props.WorkOrderId;
        this.JobNumber = Props.JobNumber;
        this.ChallanNumber = Props.ChallanNumber;
        this.ShaftSize = Props.ShaftSize;
        this.ShaftHeight = Props.ShaftHeight;
        this.SiteEng = Props.SiteEng;
        this.Doc1 = Props.Doc1;
        this.Doc2 = Props.Doc2;
        this.Doc3 = Props.Doc3;
        this.SubTotal = Props.SubTotal;
        this.TaxAmount = Props.TaxAmount;
        this.Total = Props.Total;
        this.Freight = Props.Freight;
        this.CreatedBy = Props.CreatedBy;
        this.CreatedDate = Props.CreatedDate;
        this.StartDate = Props.StartDate;
        this.Duration = Props.Duration;
        this.LedgerId = Props.LedgerId
        this.FreightTax = Props.FreightTax;
        this.State = Props.State;
        $.extend(this, defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
    };

    $.Site.prototype = {

        Add: function (obj, o) {
            this.HTTP.init(obj);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(ADDSITEINFO);
        },
        GetSiteInfo: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GETSITEINFO);
        },
        Update: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(UPDATE_SITEINFO);
        },
        GetItems: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_SITE_ITEMS, null);

        },
        GetTaxes: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_SITETAXES, null);

        },
        GetAllSiteNames: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(GET_ALL_SITENAMES, 'GET');
        },
        GetJobNumbers: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(GET_JOBNUMBERS, null);
        },
        CloseSite: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(CLOSE_SITE, null);
        },
        CloseSitePayment: function (o) {

            this.BuildRequest(o);
            this.HTTP.Exec(CLOSE_SITE_PAYMENT, null);
        },
        GetSiteGRN: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_SITEGRN, null);
        },
        GetSiteJobs: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_SITE_JOBS, null);
        },

        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        }
    };

    //-------------Tax--------------------
    $.Tax = function (options) {
        var defaults = {
            TaxId: 0,
            Name: null,
            Rate: 0.00,
            Amount: 0.00,
            TaxAmount: 0.00,
            Appicable: true,
            ItemId: 0.00,
            ItemValue: 0.00
        };
        var Props = $.extend(defaults, options);
        this.TaxId = Props.TaxId;
        this.Rate = Props.Rate;
        this.Name = Props.Name;
        this.Amount = Props.Amount;
        this.ItemId = Props.ItemId;
        this.ItemValue = Props.ItemValue;
        this.TaxAmount = Props.TaxAmount;
        this.Appicable = Props.Appicable;

        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        this.GETTAXES = "Tax/GetChallanTaxes";
    };
    $.Tax.prototype = {
        GetTaxes: function (_data, o) {
            this.HTTP.cache = true;
            this.HTTP.init(_data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(this.GETTAXES);
        },
        GetAllTaxes: function (o) {
            this.HTTP.cache = true;
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GET_ALL_PRODUCTS_TAXES);
        },
        GetApplicableTaxes: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GET_APPLICABLE_TAXES);
        },
        SaveTax: function (o, obj) {
            this.HTTP.init(obj);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };

            this.HTTP.Exec(SAVE_APPLICABLE_TAXES, null, 'string', null);
        },
        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        }
    };
    //----end of the Tax
    //UOM

    //---------------product dataset start
    $.UOM = function (options) {
        var defaults = {
            UOMId: 0,
            UOM: '',
            Size: '',
            UOMSizeId: 0,
            Active: 0

        }
        //  this.Props = $.extend(defaults, options);
        $.extend(this, defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
    };
    $.UOM.prototype = {
        GetUOMSize: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GetUOMSize);
        },
        Add: function (obj, o) {
            this.HTTP.init(obj);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(ADDSIZE);
        },
        ActivateSize: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {


                o.call(null, e);
            };
            this.HTTP.Exec(ACTIVATE_SIZE);
        },
        GetAllSize: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GETALLSIZE);
        },
        GetAllUOM: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GetAllUOM);
        },
        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        }
    };

    //END OF UOM
    //---------------Product Rate dataset start
    $.ProductRate = function (options) {
        var defaults = {
            ProductId: 0,
            RentRateId: 0,
            Rate: 0.00,
            EffectiveDate: null,
            Active: 1,
            UOMName: ''

        }
        //  this.Props = $.extend(defaults, options);
        $.extend(this, defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
    };
    $.ProductRate.prototype = {
        AddRate: function (obj, o) {
            this.HTTP.init(obj);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(PRODUCT_ADD_RATE);
        },
        GetAllRates: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(PRODUCT_GET_RATES);
        },
        ActivateRate: function (obj, o) {

            this.HTTP.init(obj);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(ACTIVATE_RATE);
        },

        BuildRequest: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
        }
    };
    //END OF ProductRate

    $.PayReminder = function (options) {
        var defaults = {
            PayReminderId: 0,
            SiteId: 0,
            Days: 0,
            Amount: 0,
            CreatedBy: 0,
            CreatedDate: null
        }
        var Props = $.extend(this, defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        this.SiteId = Props.SiteId;
        this.Days = Props.Days;
        this.Amount = Props.Amount;
        this.CreatedBy = Props.CreatedBy;
        this.CreatedDate = Props.CreatedDate;
        this.PayReminderId = Props.PayReminderId;

    };
    $.PayReminder.prototype = {
        Add: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(ADD_PAYREMINDER);
        },
        GetAll: function (o) {

            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GETALL_PAYREMINDER);
        },

        Delete: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(DELETE_PAYREMINDER);
        }
    };
    //--end of payment reminder

    //--reports
    $.Reports = function (options) {
        var defaults = {
            PayReminderId: 0,
            SiteId: 0,
            Days: 0,
            Amount: 0,
            Site: null,
            DaysLeft: 0,
            StartDate: null,
            JobNumber: null,
            AmountReceived: 0,
            GRN: null,
            ReceivingDate: null,
            ReceivingProductId: 0,
            ReceivingQty: 0,
            SentQty: 0,
            WorkOrderId: 0,
            SentProductId: 0,
            SentDate: null,
            Product: null,
            FromDate: null,
            ToDate: null,
            Closed: 0,
            LedgerId: 0

        }
        var Props = $.extend(this, defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        this.SiteId = Props.SiteId;
        this.Days = Props.Days;
        this.Amount = Props.Amount;
        this.Site = Props.Site;
        this.DaysLeft = Props.DaysLeft;
        this.PayReminderId = Props.PayReminderId;
        this.AmountReceived = Props.AmountReceived;
        this.GRN = Props.GRN;
        this.ReceivingProductId = Props.ReceivingProductId;
        this.ReceivingDate = Props.ReceivingDate;
        this.ReceivingQty = Props.ReceivingQty;
        this.SentQty = Props.SentQty;
        this.WorkOrderId = Props.WorkOrderId;
        this.SentProductId = Props.SentProductId;
        this.SentDate = Props.SentDate;
        this.Product = Props.Product;
        this.FromDate = Props.FromDate;
        this.ToDate = Props.ToDate;
        this.Closed = Props.Closed;

        OVERDUE_AMOUNT_SUMMARY = 'report/BillOverDueSummary';
        NEW_CUSTOMERS = 'report/NewCustomers';
        CLIENT_DASHBOARD_STATS = 'report/TotalStats';
        GSTR1_REPORT = 'report/Gstr1';
        GSTR3B_REPORT = 'report/TotalStats';
        GSTR_SALES_TAX_REPORT = 'report/GstSalesTaxReport';
        GSTR_PURCHASE_TAX_REPORT = 'report/GstPurchaseTaxReport';
        TRIAL_BALANCE = "report/TrialBalance";
        PROFIT_AND_LOSS_STATEMENT = "report/PnlStatement";
        BALANCE_SHEET = "report/BalanceSheet";
        PARTY_REGISTER = "api/reports/PartyRegister";
        PARTY_DELIVRY_CHALLANS = "api/reports/DeliveryChallans";
        PARTY_RETURN_CHALLANS = "api/reports/ReturnChallans";
        PARTY_BREAKAGE_REPORT = "api/reports/BreakageReport";
        PARTY_LOST_REPORT = "api/reports/LostReport";
        PARTY_EXCESS_REPORT = "api/reports/ExcessReport";
        VEHICLE_TRAVEL_SUMMARY = "api/reports/VehicleTravelReport";
        TRANSPORTER_SUMMARY = "api/reports/TransporterReport";

    };
    $.Reports.prototype = {
        TransporterReport: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print) {
                this.HTTP.DownloadReport(TRANSPORTER_SUMMARY, 'POST', null, null, 'download');
            } else {
                this.HTTP.DownloadReport(TRANSPORTER_SUMMARY, 'POST');
            }
        },
        vehicleTravelSummary: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print) {
                this.HTTP.DownloadReport(VEHICLE_TRAVEL_SUMMARY, 'POST', null, null, 'download');
            } else {
                this.HTTP.DownloadReport(VEHICLE_TRAVEL_SUMMARY, 'POST');
            }
        },
        deliveryChallans: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print) {
                this.HTTP.DownloadReport(PARTY_DELIVRY_CHALLANS, 'POST', null, null, 'download');
            } else {
                this.HTTP.DownloadReport(PARTY_DELIVRY_CHALLANS, 'POST');
            }
        },
        returnChallans: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print) {
                this.HTTP.DownloadReport(PARTY_RETURN_CHALLANS, 'POST', null, null, 'download');
            }
            else
                this.HTTP.DownloadReport(PARTY_RETURN_CHALLANS, 'POST');

        },
        partyRegister: function (o, data) {
            this.HTTP.init(data);
            debugger
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print) {
                this.HTTP.DownloadReport(PARTY_REGISTER, 'POST', null, null, 'download');
            }
            else
                this.HTTP.DownloadReport(PARTY_REGISTER, 'POST');

        },
        breakageReport: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print) {
                this.HTTP.DownloadReport(PARTY_BREAKAGE_REPORT, 'POST', null, null, 'download');
            } else {
                this.HTTP.DownloadReport(PARTY_BREAKAGE_REPORT, 'POST');
            }
        },
        lostReport: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print) {
                this.HTTP.DownloadReport(PARTY_LOST_REPORT, 'POST', null, null, 'download');
            } else {
                this.HTTP.DownloadReport(PARTY_LOST_REPORT, 'POST');
            }
        },
        excessReport: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print) {
                this.HTTP.DownloadReport(PARTY_EXCESS_REPORT, 'POST', null, null, 'download');
            } else {
                this.HTTP.DownloadReport(PARTY_EXCESS_REPORT, 'POST');
            }
        },
        balanceSheet: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(BALANCE_SHEET);
        },
        pnlStatement: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(PROFIT_AND_LOSS_STATEMENT);
        },
        trialBalance: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(TRIAL_BALANCE);
        },
        gstSalesTaxReport: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GSTR_SALES_TAX_REPORT);
        },
        gstPurchaseTaxReport: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GSTR_PURCHASE_TAX_REPORT);
        },
        gstr1: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GSTR1_REPORT);
        },
        neCustomers: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(NEW_CUSTOMERS);
        },
        clientDashboardStats: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(CLIENT_DASHBOARD_STATS);
        },
        PendingPayment: function (o) {

            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(PENDING_PAYMENT);
        },
        PaymentReceived: function (o) {

            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(PAYMENT_RECEVIED);
        },
        SiteWiseInventory: function (o) {

            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(SITE_WISE_INVENTORY);
        },
        SiteWiseInventorySummary: function (o) {

            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(SITE_WISE_INVENTORY_SUMMARY);
        },
        ClosedSites: function (o) {


            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(CLOSED_JOBNUMBERS);
        },
        ClosedSitesDialog: function (o) {

            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };

            this.HTTP.Exec(CLOSED_SITES);

        },
        OpenedJobNumbers: function (o) {

            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };

            this.HTTP.Exec(OPEN_JOBNUMBERS);

        },
        DownloadReport: function (o) {

            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(DOWNLOAD_INVENTORY_REPORT);
        },
        DashboardSummary: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(DASHBOARD_SUMMARY, 'GET');
        },
        SitePaymentSummary: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(SITE_PAYMENT_SUMMARY);
        },
        DailyInventoryTransactions: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(DAILY_INOUT_TRANSACTIONS);
        },
        OverDueAmountSummary: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(OVERDUE_AMOUNT_SUMMARY);
        },

        downloadReport: function (o, data) {
            // this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('Home/Print/' + data, 'GET', null, null, 'download');

        },
        downloadReportFromHtml: function (o, actionName, data) {
            // this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('Home/' + actionName + '/' + data, 'GET', null, null, 'download');

        },
        /** POST JSON body to Home/{actionName} — use for large payloads (avoids maxUrlLength). */
        downloadReportFromHtmlPost: function (o, actionName, dataObj) {
            this.HTTP.init(dataObj);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('Home/' + actionName, 'POST', null, null, 'download');
        },
        previewReport: function (o, data) {
            // this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('Home/Preview/' + data, 'GET');

        },
        previewReportFromHtml: function (o, actionName, data) {
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('Home/' + actionName + '/' + data, 'GET');
        },
        printPartyStockRegister: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('InventoryReports/Print', 'POST', null, null, 'download');

        },
        /** Party stock register — challan-wise grid as PDF (uses clientwiseitembalancebychallan.xslt on server). */
        printPartyStockChallanWisePdf: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('api/InventoryReports/PartyStockChallanWisePdf', 'POST', null, null, 'download');
        },
        ClientWiseItemBalance: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print == true) {
                this.HTTP.DownloadReport('api/InventoryReports/ClientWiseItemBalance', 'POST', null, null, 'download');
            }
            else {
                this.HTTP.DownloadReport('api/InventoryReports/ClientWiseItemBalance', 'POST');
            }
        },
        ItemWiseClientBalance: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print == true) {
                this.HTTP.DownloadReport('api/InventoryReports/ItemWiseClientBalance', 'POST', null, null, 'download');
            }
            else {
                this.HTTP.DownloadReport('api/InventoryReports/ItemWiseClientBalance', 'POST');
            }
        },
        billPaymentSummary: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print)
                this.HTTP.DownloadReport('api/Reports/billPaymentSummary', 'POST', null, null, 'download');
            else
                this.HTTP.DownloadReport('api/Reports/billPaymentSummary', 'POST');

        },
        amountReceiveable: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print)
                this.HTTP.DownloadReport('api/Reports/amountReceiveable', 'POST', null, null, 'download');
            else
                this.HTTP.DownloadReport('api/Reports/amountReceiveable', 'POST');

        },
        partyPayments: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            if (data.Print)
                this.HTTP.DownloadReport('api/Reports/PartyPayments', 'POST', null, null, 'download');
            else
                this.HTTP.DownloadReport('api/Reports/PartyPayments', 'POST');
        },
        issuedChallansRegisterReport: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('api/Reports/IssuedChallansRegisterReport', 'POST', null, null, 'download');
        },
        receivedChallansRegisterReport: function (o, data) {
            this.HTTP.init(data);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.DownloadReport('api/Reports/ReceivedChallansRegisterReport', 'POST', null, null, 'download');
        },
    };

    //--end of reports

    //--------Journal

    $.Journal = function (options) {
        var defaults = {
            SiteId: 0,
            ChallanNumber: null,
            InvoiceNumber: null,
            ChequeNumber: null,
            TransactionId: null,
            Bank: null,
            Remarks: null,
            Amount: 0.00,
            EntryDate: null,
            FromDate: null,
            ToDate: null,
            Site: null,
            JobNumber: null
        }

        var props = $.extend(this, defaults, options);
        this.HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

        this.SiteId = props.SiteId;
        this.ChallanNumber = props.ChallanNumber;
        this.InvoiceNumber = props.InvoiceNumber;
        this.ChequeNumber = props.ChequeNumber;
        this.Bank = props.Bank;
        this.Remarks = props.Remarks;
        this.Amount = props.Amount;
        this.EntryDate = props.EntryDate;
        this.FromDate = props.FromDate;
        this.ToDate = props.ToDate;
        this.Site = props.Site;
        this.JobNumber = props.JobNumber;
        this.TransactionId = props.TransactionId;



    };
    $.Journal.prototype = {
        CreateEntry: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(CREATE_ENTRY);
        },
        GetJournals: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GET_SITE_JOURNAL);
        },
        PaymentReceivedReport: function (o) {
            this.HTTP.init(this);
            this.HTTP.Success = function (e) {
                o.call(null, e);
            };
            this.HTTP.Exec(GET_JOURNAL);
        }

    };

    //---end of journal
    //--start of grn
    $.GRN = function (options) {
        var defaults = {
            SiteId: 0,
            JobNumber: null,
            GRN: null,
            ReceivingDate: null,
            Items: null,
            SiteName: null,
            Receiver: null,
            Sender: null,
            LedgerId: 0,
            GRNId: 0,
            ApproximateValue: 0,
            Weight: 0,
            LRNumber: null,
            CRNumber: null,
            GRNumber: null,
            PONumber: null
        };

        var props = $.extend(this, defaults, options);
        this.SiteId = props.SiteId;
        this.JobNumber = props.JobNumber;
        this.GRN = props.GRN;
        this.ReceivingDate = props.ReceivingDate;
        this.Items = props.Items;
        this.SiteName = props.SiteName;
        this.Receiver = props.Receiver;
        this.Sender = props.Sender;
        this.GRNId = props.GRNId;

        this.GET_OTHER_CHARGES = 'Inventory/GetOtherCharges';
        this.DELETE_CHALLAN = 'Inventory/DeleteChallan';
        this.SAVE_GRN = 'Inventory/Save';
        this.INWARD_CONFIRM = GRN_INWARD_CONFIRM;

    };
    $.GRN.prototype = {
        DeleteChallan: function (o, obj) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(obj);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.DELETE_CHALLAN);
        },
        InwardConfirm: function (o, obj) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(obj);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.INWARD_CONFIRM);
        },
        GetOtherCharges: function (o, obj) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(obj);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_OTHER_CHARGES);
        },
        Add: function (o, data, files) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE_GRN);
        },
        GetById: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_GRN_BYID);
        },
        GetItemsByGRNId: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_GRNITEMS_BYID);
        }
    };

    $.Invoice = function (options) {
    };
    $.Invoice.prototype = {
        Add: function (o, obj) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(obj);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(ADD_INVOICE, null, 'string');
        },
        GetList: function (obj, o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(obj);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_INVOICE_LIST);
        }
    };
    $.Billing = function (options) {
        var defaults = {
            From: null,
            To: null,
            WorkOrderNumber: null,
            BillNumber: null,
            LedgerId: 0,
            Total: 0,
            SubTotal: 0,
            TaxAmount: 0,
            Freight: 0,
            Discount: 0,
            SiteId: 0,
            BillCopyType: 0,
            ChargeReturnDay: 0,
            InvoiceType: 0,
            IsCashBill: false,
            FilterChallansByPO: false
        };
        var props = $.extend(this, defaults, options);
        this.From = props.From;
        this.To = props.To;
        this.WorkOrderNumber = props.WorkOrderNumber;
        this.BillNumber = props.BillNumber;
        this.LedgerId = props.LedgerId;
        this.Total = props.Total;
        this.SubTotal = props.SubTotal;
        this.TaxAmount = props.TaxAmount;
        this.Freight = props.Freight;
        this.SiteId = props.SiteId;
        this.BillCopyType = props.BillCopyType;
        this.Discount = props.Discount;
        this.PRINT_CONTRACT_BILL = 'billing/PrintContractBill';
        this.SEND_FOR_APPROVAL = 'billing/SendForApproval';
        this.APPROVE_BILL = 'billing/ApproveBill';

        this.GET_UNPAID_INVOICES = "billing/UnpaidInvoices";
        this.SETTLE_BILL = "billing/SettleBill";
        this.GET_LOSSITEMS_TO_BILL = "billing/GetLostItemsToBill";
        this.GET_BREAKAGE_FOR_BILL = "billing/GetBreakageItemsToBill";

    };
    $.Billing.prototype = {

        GetLossItemsToBill: function (o, data, promiseOnly = false) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });

            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.GET_LOSSITEMS_TO_BILL);
        },
        SettleBill: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(data);


            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SETTLE_BILL);
        },
        GetUnpaidInvoices: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.init(data);


            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_UNPAID_INVOICES);
        },
        GenerateBill: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            if (data) {
                HTTP.init(data);
            }
            else
                HTTP.init(this);

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GEN_BILL);
        }

        ,
        SaveBill: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            // HTTP.init(this);
            if (data) {
                HTTP.init(JSON.stringify(data));
            }
            else
                HTTP.init(JSON.stringify(this));

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(SAVE_BILL, null, 'string', null);
            //HTTP.Exec(SAVE_BILL);
        },
        SaveBreakageBill: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            // HTTP.init(this);
            HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(SAVE_BREAKAGE_BILL, null, 'string', null);
        },
        GetBillList: function (o, data) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            if (data) {
                HTTP.init(data);
            } else
                HTTP.init(this);

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_BILLS);
        },
        GetBillItems: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_BILLITEMS);
        },
        GetBillLossItems: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_LOSS_ITEMS);
        },
        GetBreakageItems: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_BREAKAGE_ITEMS);
        },
        PrintBill: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(PRINT_BILL);
        },
        PrintContractBill: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PRINT_CONTRACT_BILL);
        },
        GetBreakageForBill: function (o, data, promiseOnly = false) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            if (data) {
                HTTP.init(data);
            }
            else
                HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.GET_BREAKAGE_FOR_BILL);
        },
        BillingItemsTax: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(BILL_ITEMS_TAX);
        },
        CancelBill: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(BILL_CANCEL);
        },
        SendForapproval: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SEND_FOR_APPROVAL);
        },
        Approve: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.APPROVE_BILL);
        },
        MarkSettle: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(BILL_SETTLE);
        },
        DueBills: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(DUE_BILLS);
        },
        PrintDueBills: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(PRINT_DUE_BILLS);
        },
        PrintDueBillsPdf: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(PRINT_DUE_BILLS_PDF, 'POST', null, null, 'download');
        },
        PushToIRP: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec('Billing/PushToIRP');
        },
        GetBillInfo: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec('Billing/GetBillInfo');
        },
        ById: function (o, filter, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(filter)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec('Billing/ById');
        },

    };
    $.RecordSet = function (options) {
        var defaults = {
            Index: 0,
            Filter: [],
            LookupData: []
        };

        var props = $.extend(this, defaults, options);
        this.Index = props.Index;
        this.Filter = props.Filter;
        this.LookupData = props.LookupData;
        this.Total = props.LookupData.length - 1;
    };
    $.RecordSet.prototype = {
        MoveNext: function () {
            if (this.Index < this.LookupData.length - 1) {
                this.Index++;
            }
            return this.CurrentFilter();
        },
        MoveLast: function () {
            this.Index = this.LookupData.length - 1;
            return this.CurrentFilter();
        },
        MovePrev: function () {
            if (this.Index > 0) {
                this.Index--;
            }
            return this.CurrentFilter();
        },
        MoveFirst: function () {
            this.Index = 0;
            return this.CurrentFilter();
        },
        CurrentFilter: function () {
            return this.LookupData[this.Index];
        }
    };

    //----settings
    $.Settings = function (options) {
        var defaults = {
            UserId: 0,
            DefaultCompanyId: 0
        };
        var props = $.extend(this, defaults, options);
        this.UserId = props.UserId;
        this.DefaultCompanyId = props.DefaultCompanyId;
        this.CREATE_WHATSAPP = 'settings/CreateWhatsappApp';
        this.SET_CALLBACK = 'settings/ActivateForCallback';
        this.LIST_ALL_WHATSAPPS = 'settings/ListWhatsApps';
        this.GENERATE_SIGNUPLINK = 'settings/GenerateSignupLink';
        this.REFRESH_APP_DETAILS = 'settings/RefreshAppDetails';

    };
    $.Settings.prototype = {
        RefreshDetails: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.REFRESH_APP_DETAILS);
        },
        GenerateSignupLink: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GENERATE_SIGNUPLINK);
        },
        ListAllWhatsApps: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.LIST_ALL_WHATSAPPS);
        },
        ActivateForCallback: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SET_CALLBACK);
        },
        CreateWhatsApp: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.CREATE_WHATSAPP);
        },
        SetDefaultCompany: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(SET_USER_DEFAULT_COMPANY);
        }
    };
    //----settings
    //----Config
    $.Config = function (options) {
        var defaults = {
            UserId: 0,
            DefaultCompanyId: 0
        };
        var props = $.extend(this, defaults, options);
        this.GET_FIN_YEAR = 'Login/GetFinYearList';
        this.ADD_BILL_CONFIG = 'Config/SetBillNo';
        this.GET_BILLING_CONFIG = 'Config/GetBillingConfig';
        this.SAVE_CONFIG = 'Config/SaveConfig';
        this.GET_ALL_CONFIG = 'Config/GetAllConfig';
        this.GET_BY_CATEGORY = 'Config/GetConfig';

    };
    $.Config.prototype = {
        GetAllConfig: function (o, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(this);
            HTTP.Success = function (e) {
                if (o)
                    o.call(null, e);
            };
            return HTTP.Exec(this.GET_ALL_CONFIG, 'GET');
        },
        GetByCategory: function (o, category) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_BY_CATEGORY + '?category=' + category, 'GET');
        },
        GetBySubCategory: function (o, category, subCategory) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_BY_CATEGORY + '/' + category + '/' + subCategory, 'GET');
        },
        GetFinYearList: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_FIN_YEAR);
        },
        AddBillingSetting: function (data, o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_BILL_CONFIG);
        },
        GetBillingConfig: function (o, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.GET_BILLING_CONFIG);
        },
        SaveConfig: function (data, o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE_CONFIG);
        }

    };
    //----config
    $.User = function (options) {
        var defaults = {
            UserId: 0,
            DefaultCompanyId: 0,
            RbnClientId: 0,
            FullName: '',
            UserName: '',
            Email: '',
            Phone: '',
            Password: '',
            CompanyName: '',
            RoleId: 0,
            AllowSwitchCompany: false
        };
        var props = $.extend(this, defaults, options);
        this.UserId = props.UserId;
        this.DefaultCompanyId = props.DefaultCompanyId;
        this.RbnClientId = props.RbnClientId;

        this.FullName = props.FullName;
        this.UserName = props.UserName;
        this.Email = props.Email;
        this.Phone = props.Phone;
        this.Password = props.Password;
        this.CompanyName = props.CompanyName;
        this.AllowSwitchCompany = props.AllowSwitchCompany;
        this.RoleId = props.RoleId;

        GET_RBNCLIENT_INFO = "client/getInfo";
        UPD_RBNCLIENT_INFO = "client/UpdateInfo";
        UPD_RBNCLIENT_TAXINFO = "client/UpdateTaxInfo";
        CHANGE_PROFILE_PIC = "User/ChangeProfilePicture";
        GET_LICENSE_DETAILS = "User/GetLicenseDetails";
        CHANGE_PASSWORD = "User/changePassword";
        SEND_FORGOTPWD_LINK = "ForgotPassword/SendVerificationLink";
        VALIDATE_FORGOTPWD_LINK = "ForgotPassword/ValidateLinkEmail";
        RESET_PASSWORD = "ForgotPassword/UpdatePassword";
    };
    $.User.prototype = {
        resetPasswordByGuId: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(RESET_PASSWORD);
        },
        sendForgotPwdLink: function (o, email) {
            debugger
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            var url = SEND_FORGOTPWD_LINK + '?email=' + email
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(url, 'GET');
        },
        validateForgotPwdLink: function (o, guId) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            var url = VALIDATE_FORGOTPWD_LINK + '?guId=' + guId
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(url, 'GET');
        },
        SetDefaultCompany: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(SET_USER_DEFAULT_COMPANY);
        },
        GetAllUsers: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_ALL_USERS);
        },
        CreateUser: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(USER_CREATE);
        },

        ResetPassword: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(USER_RESET_PASSWORD);
        },
        ActivateDeActivate: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(UPDATE_USER_STATUS);
        },
        GetRouteAccess: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_ROUTE_ACCESS);
        },
        GetRbnClientInfo: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_RBNCLIENT_INFO, 'GET');
        },
        UpdateRbnClientInfo: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(UPD_RBNCLIENT_INFO);
        },
        changeProfilePic: function (o, files) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(CHANGE_PROFILE_PIC, null, 'string', files);
        },
        getLicenseDetails: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_LICENSE_DETAILS);
        },
        changePassword: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(CHANGE_PASSWORD);
        },
    };

    $.Address = function (options) {
        var defaults = {
            AddressId: 0,
            AddressTypeId: 0,
            AddressHolderId: 0,
            RoleId: 0,
            Address1: null,
            Address2: null,
            Phone1: null,
            Phone2: null,
            City: null,
            ZipCode: null,
            State: null,
            Email: null,
            Fax: null,
            Web: null,
            StateId: 0
        };
        var props = $.extend(this, defaults, options);
        this.AddressId = props.AddressId;
        this.AddressTypeId = props.AddressTypeId;
        this.AddressHolderId = props.AddressHolderId;
        this.RoleId = props.RoleId;
        this.Address1 = props.Address1;
        this.Address2 = props.Address2;
        this.Phone1 = props.Phone1;
        this.Phone2 = props.Phone2;
        this.City = props.City;
        this.State = props.State;
        this.Fax = props.Fax;
        this.Email = props.Email;
        this.Web = props.Web;
        this.ZipCode = props.ZipCode;
        this.StateId = props.StateId;


    }

    $.NextId = function () {
    };
    $.NextId.prototype = {
        GetWorkOrderNumber: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_NEXT_WORKORDER_NUMBER);
        },
        GetRentInvoiceNumber: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data || {});
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(GET_NEXT_RENT_INVOICE_NUMBER);
        }
    };
    //This is inventory transaction like purchase sale etc.
    $.Transaction = function (options) {
        var defaults = {
            PurchaseId: 0,
            SalesId: 0,
            Rate: 0,
            SalesDate: null,
            PurchaseDate: null,
            BillNumber: null,
            Items: null,
            Taxes: null,
            SubTotal: 0,
            Freight: 0,
            FreightIn: 0,
            FreightTax: 0,
            Total: 0,
            TaxAmount: 0,
            LedgerId: 0,
            CompanyId: 0,
            PurchaseAccountId: 0,
            SalesAccountId: 0,
            Details: null,
            WorkOrderId: 0,
            InvoiceType: 0,
            LedgerSiteId: 0,
            PurchaseType: 0,
            VendorCreditNoteDate: null,
            VendorCreditNoteNumber: null,
            PartyType: 1,
            UnregisteredPartyName: '',
            UnregisteredPartyAddress: '',
            UnregisteredPartyPhone: '',
            GstRate: 0,
            IGST: false,
            CGST: false,
            SGST: false,
            WarehouseId: 0
        };


        var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        var Props = $.extend(defaults, options);
        this.PurchaseId = Props.PurchaseId;
        this.PurchaseDate = Props.PurchaseDate;
        this.SalesId = Props.SalesId;
        this.SalesDate = Props.SalesDate;
        this.SalesAccountId = Props.SalesAccountId
        this.BillNumber = Props.BillNumber

        this.Items = Props.Items;
        this.Taxes = Props.Taxes;
        this.SubTotal = Props.SubTotal;
        this.TaxAmount = Props.TaxAmount;
        this.Total = Props.Total;
        this.LedgerId = Props.LedgerId;
        this.CompanyId = Props.CompanyId;
        this.PurchaseAccountId = Props.PurchaseAccountId;
        this.Details = Props.Details;
        this.Freight = Props.Freight;
        this.FreightIn = Props.FreightIn;
        this.FreightTax = Props.FreightTax;
        this.InvoiceType = Props.InvoiceType;
        this.Rate = Props.Rate;
        this.VendorCreditNoteNo = Props.VendorCreditNoteNo;
        this.VendorCreditNoteNumber = Props.VendorCreditNoteNumber;
        this.PurchaseType = Props.PurchaseType;
        this.PartyType = Props.PartyType;
        this.UnregisteredPartyName = Props.UnregisteredPartyName;
        this.UnregisteredPartyAddress = Props.UnregisteredPartyAddress;
        this.UnregisteredPartyPhone = Props.UnregisteredPartyPhone;
        this.GstRate = Props.GstRate;
        this.IGST = Props.IGST;
        this.CGST = Props.CGST;
        this.SGST = Props.SGST;
        this.WarehouseId = Props.WarehouseId;

        this.ADD_PURCHASE = "Purchase/Save";
        this.PURCHASE_REGISTER = "Purchase/GetPurhaseRegister";
        this.PURCHASE_ITEMS_LIST = "Purchase/GetPurchaseItems";
        this.PURCHASE_PRINT = "Purchase/PrintReceipt";
        this.PURCHASE_ITEMS_TAX = "Purchase/PurchaseItemsTax";
        this.PURCHASE_BY_ID = "Purchase/GetById";
        this.GET_UNPAID_PURCHASE_BiLLS = "purchase/UnpaidBills";
        this.DELETE_PURCHASE = "purchase/delete";


        //--sales
        this.ADD_SALES = "Sales/Save";
        this.SALES_REGISTER = "Sales/GetSalesRegister";
        this.SALES_ITEMS_LIST = "Sales/GetSalesItems";
        this.SALES_PRINT = "Sales/PrintReceipt";
        this.SALES_ITEMS_TAX = "Sales/SalesItemsTax";
        this.SAVE_QUOTATION = "Sales/SaveQuotation";
        this.GET_QUOTATION_BYID = "Sales/QuotationById";

        this.QUOTATION_LIST = "Sales/QuotationsList";
        this.PRINT_QUOTATION = "Sales/PrintQuotation";
        this.DELETE_QUOTATION = "Sales/UpdatedQuotationStatus";
        this.LINK_QUOTATION_LEDGER = "Sales/LinkQuotationToLedger";



    };
    $.Transaction.prototype = {
        UpdatedQuotationStatus: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.DELETE_QUOTATION);
        },
        DeletePurchase: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.DELETE_PURCHASE);
        },
        SaveQuotation: function (o, data, files) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            if (data) {
                //  HTTP.init(JSON.stringify(data));
                HTTP.init(data);
            }
            else
                HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE_QUOTATION, null, null, files);
        },
        GetQutotationById: function (o, quoteId) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            var url = this.GET_QUOTATION_BYID + '?quoteId=' + quoteId
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(url, 'GET');
        },
        LinkQuotationToLedger: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.LINK_QUOTATION_LEDGER);
        },
        QuotationsList: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.QUOTATION_LIST);
        },
        PrintQuotation: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PRINT_QUOTATION);
        },
        SavePurchase: function (o, data, files) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(data));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_PURCHASE, null, 'string', files);
        },
        GetUnpaidPurchaseBills: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_UNPAID_PURCHASE_BiLLS);
        },
        PurchaseRegister: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PURCHASE_REGISTER);
        },
        GetPurchaseById: function (o, pId) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            var url = this.PURCHASE_BY_ID + '?purchaseId=' + pId;
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(url, 'GET');
        },
        PurchaseItemsList: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PURCHASE_ITEMS_LIST);
        },
        PrintPurchaseReceipt: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PURCHASE_PRINT);

        },
        PurchaseItemsTax: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PURCHASE_ITEMS_TAX);
        },
        //sales
        SaveSales: function (o, data, files) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            if (data) {
                HTTP.init(JSON.stringify(data));
            }
            else
                HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_SALES, null, 'string', files);
        },
        SalesRegister: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SALES_REGISTER);
        },
        SalesItemsList: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SALES_ITEMS_LIST);
        },
        PrintSalesReceipt: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SALES_PRINT);
        },
        SalesItemsTax: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this)
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SALES_ITEMS_TAX);
        }

        //-- end of sales
    };
    //-------
    $.TransItem = function (options) {
        var defaults = {

            PurchaseRate: 0,
            Amount: 0.000,
            PurchaseQty: 0,
            Rate: 0,
            ExcessQty: 0,
            Breakage: 0,
            Quantity: 0.00,
            Unit1Quantity: 0.000,
            Product: null,
            ProductId: 0,
            BreakageRate: 0,
            SentQty: 0,
            RecQty: 0,
            TaxCategoryId: 0
        }
        var Props = $.extend(defaults, options);

        this.PurchaseRate = Props.PurchaseRate;
        this.ExcessQty = Props.ExcessQty;
        this.Breakage = Props.Breakage;
        this.PurchaseQty = Props.PurchaseQty;
        this.Unit1Quantity = Props.Unit1Quantity;
        this.SiteId = Props.SiteId;
        this.TaxCategoryId = Props.TaxCategoryId;

        //   this.Rate = Props.Rate;
        this.BreakageRate = Props.BreakageRate;


        this.Amount = Props.PurchaseRate * Props.PurchaseQty
    };

    //--end of GRN

    //----challan

    $.Challan = function (options) {
        var defaults = {
            LedgerId: 0,
            Date: null,
            ChallanNumber: null,
            Site: null,
            Vehicle: null,
            Driver: null,
            ChallanType: null,
            Freight: 0,
            SubTotal: 0,
            Total: 0,
            CompanyId: 0,
            WorkOrderId: 0,
            State: null,
            ChallanHeaderType: 0


        };
        var props = $.extend(defaults, options);
        this.LedgerId = props.LedgerId;
        this.Date = props.Date;
        this.ChallanNumber = props.ChallanNumber;
        this.Site = props.Site;
        this.Vehicle = props.Vehicle;
        this.Driver = props.Driver;
        this.ChallanType = props.ChallanType;
        this.Freight = props.Freight;
        this.SubTotal = props.SubTotal;
        this.Total = props.Total;
        this.CompanyId = props.CompanyId;
        this.WorkOrderId = props.WorkOrderId;
        this.Total = props.Total;
        this.State = props.State;
        this.ChallanHeaderType = props.ChallanHeaderType;
        var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
        this.ADD = "challan/SaveChallan";
        this.GETLIST = "challan/GetChallanList";
        this.PRINT_DELIVERY_CHALLAN = "WorkOrder/PrintDeliveryChallanReceipt";

    }
    $.Challan.prototype = {

        Add: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(JSON.stringify(this));
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD, null, 'string', null);
        },
        GetList: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GETLIST);
        },
        PrintDeliveryChallanReceipt: function (o) {

            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PRINT_DELIVERY_CHALLAN);
        }

    };

    $.Inventory = function (options) {
        this.ITEM_STOCK = 'Inventory/ItemStock';
        this.POST_STOCK_TXN = 'Inventory/PostStockTxn';
        this.STOCK_ADJ_LIST = 'Inventory/StockAdjustmentList';
        this.STOCK_ADJ_DETAILS = 'Inventory/StockAdjustmentDetails';
        this.STOCK_ADJ_DEL = 'Inventory/DeleteStockAdjustment';

        var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
    }
    $.Inventory.prototype = {
        ItemStock: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ITEM_STOCK);
        },
        PostStockTxn: function (data, o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.POST_STOCK_TXN);
        },
        StockAdjustmentList: function (filter, o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.STOCK_ADJ_LIST);
        },
        StockAdjustmentDetails: function (filter, o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.STOCK_ADJ_DETAILS);
        },
        DeleteStockAdjustment: function (filter, o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.STOCK_ADJ_DEL);
        },
        GetStockInhand: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(STOCK_IN_HAND);
        },
        GetStockSummary: function (o, filter) {
            var http = new $.ApiCaller({ http: HTTP_SERVICE });
            http.init(filter);
            http.Success = function (e) {
                o.call(null, e);
            };
            http.Exec(STOCK_SUMMARY);
        }
    };

    $.Lookup = function (options) {

        this.GET_OTHER_CHARGES = "Lookup/GetOtherCharges";
        this.GETALL_SYS_ROLES = "Lookup/GetAllSystemRoles";
        this.GETALL_EMP_ROLES = "Lookup/GetAllEmployeeRoles";
    };
    $.Lookup.prototype = {
        GetAllEmployeeRoles: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GETALL_EMP_ROLES);
        },
        GetAllSystemRoles: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GETALL_SYS_ROLES);
        },

        GetOtherCharges: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_OTHER_CHARGES);
        }
    };
    $.Employee = function (options) {
        this.ADD = "Employee/Add";
        this.GET_ALL = "Employee/GetAll";
        this.GET_BY_ID = "Employee/GetById";

        this.TEAM_SAVE = "Employee/SaveTeam";
        this.TEAM_LIST = "Employee/TeamList";
        this.TEAM_DELETE = "Employee/DeleteTeam";
        this.TEAM_BYID = "Employee/TeamById";


    };
    $.Employee.prototype = {
        SaveTeam: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.TEAM_SAVE);
        },
        TeamList: function (o, data, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(data);
            HTTP.Success = function (e) {
                if (o)
                    o.call(null, e);
            };
            return HTTP.Exec(this.TEAM_LIST, 'GET');
        },
        DeleteTeam: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.TEAM_DELETE);
        },
        TeamById: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.TEAM_BYID);
        },
        Add: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD);
        },
        GetAll: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_ALL);
        },
        GetById: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_BY_ID);
        }
    };
    $.Vehicle = function (options) {
        this.ADD = "Vehicle/Add";
        this.GET_ALL = "Vehicle/GetAll";
        this.GET_BY_ID = "Vehicle/GetById";

    };
    $.Vehicle.prototype = {
        Add: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD);
        },
        GetAll: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_ALL);
        },
        GetById: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_BY_ID);
        }
    };
    $.UserRole = function (options) {
        this.ADD = "UserRole/AddRoleFunction";
        this.GET_ALL = "UserRole/GetRoleFunctions";


    };
    $.UserRole.prototype = {
        AddRoleFunction: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD);
        },
        GetRoleFunctions: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_ALL);
        }
    };
    //--- end of challan
    $.VoucherNotes = function (options) {
        this.ADD = "VoucherNotes/Add";
        this.GET_ALL = "VoucherNotes/GetAll";
    }
    $.VoucherNotes.prototype = {
        Add: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD);
        },
        GetAll: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_ALL);
        }
    };

    $.Subscription = function () {
        this.CREATE_ORDER = "payment/createPackagePurchaseOrder";
        this.UPDATE_ORDER = "payment/updateOrder";
        this.GET_BILLS = "subscription/GetBills";
        this.PRINT_BILL = "subscription/printFile";
        this.REGISTER_CLIENT = "subscription/RegisterClient";

    };
    $.Subscription.prototype = {

        createOrder: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.CREATE_ORDER);
        },
        updateOrder: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.UPDATE_ORDER);
        },
        getBills: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_BILLS);
        },
        printbill: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            //HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            var url = this.PRINT_BILL + '?paymentId=' + data;
            HTTP.Exec(url, 'GET', null, null, 'download');
        },
        registerClient: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.REGISTER_CLIENT);
        },
    };
    $.Transporter = function () {
        this.SAVE = "transporter/Save";

        this.GETALL = "transporter/GetAll";


    };
    $.Transporter.prototype = {

        Save: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE);
        },
        GetAll: function (o, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });

            HTTP.Success = function (e) {
                if (o)
                    o.call(null, e);
            };
            return HTTP.Exec(this.GETALL, 'GET');
        },


    };
    $.EwayBill = function () {
        this.SAVE = "EwayBill/Save";

        this.GETALL = "EwayBill/GetAll";
        this.PUS_TO_PORTAL = "EwayBill/PushToPortal";
        this.PRINT = "EwayBill/PrintBill";
        this.GETINFO = "EwayBill/GetInfo";
        this.UPDATE_VEHICLE = "EwayBill/UpdateVehicle";
        this.UPDATE_TRANSPORTER = "EwayBill/UpdateTransporter";
        this.CANCEL_EWAYBILL = "EwayBill/CancelEwayBill";
        this.FETCH_BY_DATE_FROM_PORTAL = "EwayBill/FetchEwayBillsByDateFromPortal";
        this.MAP_EWAYBILL = "EwayBill/MapEwayBill";
        this.UPDATE_EWAYBILL_INFO = "EwayBill/UpdateEwayBillInfo";
        this.GET_PARTY_INFO = "EwayBill/getPartyInfo";


    };
    $.EwayBill.prototype = {
        getPartyInfo: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_PARTY_INFO);
        },
        MapEwayBill: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.MAP_EWAYBILL);
        },
        UpdateEwayBillInfo: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.UPDATE_EWAYBILL_INFO);
        },
        Save: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE);
        },
        UpdateVehicle: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.UPDATE_VEHICLE);
        },
        FetchFromPortalByDate: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.FETCH_BY_DATE_FROM_PORTAL);
        },
        CancelBill: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.CANCEL_EWAYBILL);
        },
        UpdateTransporter: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.UPDATE_TRANSPORTER);
        },
        GetAll: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GETALL);
        },
        PushToPortal: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PUS_TO_PORTAL);
        },
        Print: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.PRINT);
        },
        GetInfo: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GETINFO);
        },
    };
    $.Contract = function () {
        this.SAVE = "contract/Save";
        this.GETBYID = "contract/GetById";
        this.UPDATE_STATUS = 'contract/UpdateStatus';
        this.FILTER = "contract/Filter";
        this.GENERATE_BILL = "contract/GenerateBill";
        this.CONTRACT_INVENTORY = "contract/ContractInventory";
        this.CONTRACT_BILLS = "contract/GetBills";
        this.EXTEND_CONTRACT = "contract/Extend";
        this.ACTIVITY_TRACKER = "contract/ActivityTracker";
        this.EMPLOYEE_DPR = "contract/EmployeeDPR";
        this.GET_CONTRACT_DOCUMENTS = "contract/GetContractDocuments";
        this.ContractDelChallanItems_url = "contract/ContractDelChallanItems";
        this.ContractRetChallanItems_url = "contract/ContractRetChallanItems";

    };
    $.Contract.prototype = {
        ContractDelChallanItems: function (o, model, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.ContractDelChallanItems_url);
        },
        ContractRetChallanItems: function (o, model, promiseOnly = false) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE, promiseOnly: promiseOnly });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            return HTTP.Exec(this.ContractRetChallanItems_url);
        },
        EmployeeDPR: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.EMPLOYEE_DPR);
        },
        ActivityTracker: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ACTIVITY_TRACKER);
        },
        Extend: function (o, model) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(model);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.EXTEND_CONTRACT);
        },
        GetBills: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.CONTRACT_BILLS);
        },
        ContractInventory: function (o, filter) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(filter);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.CONTRACT_INVENTORY);
        },
        Save: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE);
        },
        Filter: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.FILTER);
        },
        GetById: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GETBYID);
        },
        GetContractDocuments: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GET_CONTRACT_DOCUMENTS);
        },
        UpdateStatus: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.UPDATE_STATUS);
        },
        GenerateBill: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.GENERATE_BILL);
        },
    };
    $.WorkStation = function () {
        this.ADD_TYPE = "Workstation/SaveWorkStationType";
        this.TYPE_LIST = "Workstation/GetWorkStationTypeList";
        this.TYPE_DETAILS = "workstation/GetWorkSationTypeDetails";

        this.ADD_WORKSTATION = "Workstation/SaveWorkStation";
        this.WORKSTATION_LIST = "Workstation/GetWorkStationList";
        this.WORKSTATION_DETAILS = "workstation/GetWorkSationDetails";
        //operations
        this.ADD_OPERATION = "Workstation/SaveOperation";
        this.OPERATION_LIST = "Workstation/GetOperations";
        this.OPERATION_DETAILS = "Workstation/GetOperation";


    };
    $.WorkStation.prototype = {

        SaveType: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_TYPE);
        },
        TypeList: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.TYPE_LIST);
        },
        TypeDetails: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.TYPE_DETAILS);
        },
        //Workstation
        SaveWorkStation: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_WORKSTATION);
        },
        WorkStationList: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.WORKSTATION_LIST);
        },
        WorkStationDetails: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.WORKSTATION_DETAILS);
        },
        //Operation
        SaveOperation: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_OPERATION);
        },
        OperationsList: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.OPERATION_LIST);
        },
        OperationDetails: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.OPERATION_DETAILS);
        }
    };
    $.Operation = function () {

        //operations
        this.ADD_OPERATION = "Workstation/SaveOperation";
        this.OPERATION_LIST = "Workstation/GetOperations";
        this.OPERATION_DETAILS = "Workstation/GetOperation";
        //its controller is inside workstation.js

    };
    $.Operation.prototype = {


        //Operation
        SaveOperation: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_OPERATION);
        },
        OperationsList: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.OPERATION_LIST);
        },
        OperationDetails: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.OPERATION_DETAILS);
        }
    };
    $.Template = function () {

        this.GETBY_GROUP = "Template/byGroup";

    };
    $.Template.prototype = {

        /* GetByGroup: function (o, groupName) {
             var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
 
             HTTP.Success = function (e) {
                 o.call(null, e);
             };
             HTTP.Exec(this.GETBY_GROUP + '/' + groupName, 'GET');
         }
         */

    };
    $.JobCard = function () {

        //JobCards
        this.ADD_JOB_CARD = "jobcard/Save";
        this.JOBCARD_LIST = "jobcard/GetList";
        this.JobCardDelChallanItems_action = "jobcard/JobCardDelChallanItems";
        this.JobCardRetChallanItems_action = "jobcard/JobCardRetChallanItems";
        this.UPDATE_JOBCARD_STATUS = "jobcard/UpdateStatus";

    };
    $.JobCard.prototype = {
        UpdateStatus: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.UPDATE_JOBCARD_STATUS);
        },

        //JobCard
        JobCardRetChallanItems: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.JobCardRetChallanItems_action);
        },
        JobCardDelChallanItems: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.JobCardDelChallanItems_action);
        },
        Save: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ADD_JOB_CARD);
        },
        GetList: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });

            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.JOBCARD_LIST);
        },

    };

    $.Notification = function () {
        //JobCards
        this.SEND_EMAIL = "Email/SendEmail";
        this.NOTIFY_NOTIFICATION = "api/Notify/Notify";
        this.GET_MY_ALERTS = "api/Notify/GetMyAlerts";
    };
    $.Notification.prototype = {
        GetAlerts: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.DownloadReport(this.GET_MY_ALERTS);
        },
        Notify: function (o, data) {
            debugger
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.DownloadReport(this.NOTIFY_NOTIFICATION);
        },
        Add: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.DownloadReport(this.SEND_EMAIL);
        },


    };

    $.Task = function () {

        //JobCards

        this.SAVE_TASK = "Task/Save";
        this.LIST_TASKS = "Task/ListTasks";
        this.ASSIGN_TASK = "Task/AssignTask";
        this.UPDATE_TASK_STATUS = "Task/UpdateStatus";


    };
    $.Task.prototype = {
        Assign: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ASSIGN_TASK);
        },
        UpdateStatus: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.UPDATE_TASK_STATUS);
        },
        List: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.LIST_TASKS);
        },

        Save: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE_TASK);
        },


    };
    $.Zone = function () {

        //JobCards

        this.SAVE = "zone/Save";
        this.ZONES_LIST = "zone/ZoneList";
        this.ZONE_BY_ID = "zone/ZoneById";
        this.DELETE_ZONE = "zone/DeleteZone";


    };
    $.Zone.prototype = {
        save: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE);
        },
        zoneList: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ZONES_LIST);
        },
        zoneById: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.ZONE_BY_ID);
        },
        deleteZone: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.DELETE_ZONE);
        },
    };

    $.RoleCosting = function () {
        this.LIST = "RoleCosting/List";
        this.BY_ID = "RoleCosting/ById";
        this.SAVE = "RoleCosting/Save";
        this.DELETE = "RoleCosting/Delete";
    };
    $.RoleCosting.prototype = {
        list: function (o) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(this);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.LIST, 'GET');
        },
        byId: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.BY_ID);
        },
        save: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.SAVE);
        },
        deleteItem: function (o, data) {
            var HTTP = new $.ApiCaller({ http: HTTP_SERVICE });
            HTTP.init(data);
            HTTP.Success = function (e) {
                o.call(null, e);
            };
            HTTP.Exec(this.DELETE);
        }
    };
}(jQuery));