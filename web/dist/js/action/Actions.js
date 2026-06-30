//---------COMPANY------------------
var ADDCOMPANY = "company/Save";
var GETALLCOMPANIES = "company/GetAll";
var GETCOMPANYDETILS = "company/GetDetails";
var ACTIVATE_DEACTIVATE_COMPANY = "company/ActivateDeActivate";
var SEARCH_COMPANY = "company/SearchCompany";

//END OF COMPANY
//---------Ledger------------------
var ADDCLIENT = "Ledger/Save";
var GETALLCLIENTS = "Ledger/GetAll";
var GETCLIENTDETAILS = "Ledger/GetDetails";
var ACTIVATE_DEACTIVATE_CLIENT = "Ledger/ActivateDeActivate";
var GET_ALL_ACTIVE_CIENTS = "Ledger/GetAllLedger?name=";
 
var GET_SITE_TAXES = "Ledger/GetSiteTaxes";
var ADD_LEDGER_MASTER_SITE = "Ledger/AddSite";
var GET_LEDGER_MASTER_SITES = "Ledger/GetSites";
var GET_LEDGER_MASTER_SITE_BY_ID = "Ledger/GetSiteById";
var GET_ALL_CLIENT_SITES = "Ledger/GetAllClientsSites";

//Transactions 
var CREATE_TRANSACTION = "Ledger/CreateTransactions";
var STAGE_QUICK_RECEIPT_IMAGE = "Ledger/StageQuickReceiptImage";
var CLEAR_QUICK_RECEIPT_DOCUMENT = "Ledger/ClearQuickReceiptDocument";
var RECEIPT_REGISTER = 'Ledger/ReceiptTransRegister';
var GET_CONTRACT_RECEIPT_PAYMENTS = 'Ledger/GetContractReceiptPayments';
var PARTY_STOCK_REGISTER = "Ledger/StockRegister"
var PARTY_OPENING_BALANCE = "Ledger/PartOpeningBalance"
var PARTY_STOCK_REGISTER_PRINT = "Ledger/PrintPartyStock"
var LEDGER_TRANSACTION_LOOKUP = "Ledger/GetLedgerTransactionLookup";
var LEDGER_TRAN_DATA_BY_LOOKUP = "Ledger/GetTransactionDetails";
var LEDGER_TRAN_DELETE = "Ledger/DeleteLedgerTransactions";
var BANK_TRAN_REGISTER = "Ledger/BankEntryRegister";
var BANK_TRAN_REGISTER_RPT = "Ledger/BankEntryRegister_RPT";
var GET_TRAN_BY_ID = "Ledger/PrintTransaction";
var PARTY_BALANCE_AMOUNT = "Ledger/GetPartyBalance";
var PARTY_STOCK_BALANCE = "Ledger/PartyStockBalance";
var UNBILLED_SITES = "Ledger/GetUnbilledSites";
var LEDGER_BALANCE_FORBILLING = "Ledger/GetAccountBalanceForBill";
var PARTY_STOCK_BALANCE_BY_SIZE = "Ledger/PartyStockBalance_BySize";
var LAST_BILL = "Ledger/GetLastBill";
var REMOVE_LEDGER = "Ledger/Remove";
var GET_CLIENT_JOBS = "Ledger/GetClientJobs";
var PARTY_STOCK_BALANCE_DASHBOARD = "Ledger/PartyStockBalance_Dashboard";
var PRINT_REPORT = "Ledger/PrintReport";
var SEARCH_CLIENT = "Ledger/SearchClient";
var ACCOUNT_LEDGER = "Ledger/AccountLedger";
var ACCOUNT_LEDGER_RPT = "Ledger/AccountLedger_rpt";
var GET_LEDGER_RUNNING_SITES = "WorkOrder/GetClientSites";
var PRINT_RECEIPTS = "Ledger/PrintReceipt";
var PARTY_STOCK_BALANCE_REPORT = "Ledger/PartyStockBalance_Report";

var GET_LEDGER_PRODUCT_RATES = "Ledger/GetProductRates";
var ADD_UPDATE_LEDGER_RATES = "Ledger/SaveProductRates";
var UPDATE_LEDGER_RATES = "Ledger/UpdateProductRates";
var GET_SITE_WISE_LEDGER = "Ledger/FixedSiteLedger";
var DUE_MAT_REMINDER = "Ledger/DueMaterialReminder";
var DUE_BILL_REMINDER = "Ledger/DueBillReminder";
var PARTY_WISE_RATES = "Ledger/GetPartyWiseRates";
var ESTIMATED_RENT_PER_DAY = "Ledger/EstimatedRentPerDay";
var PRINT_TRANSACTION = "Ledger/PrintTransaction";
var GET_DR_CR_NOTES = "Ledger/GetDrCrNotes";
var EMAIL_DR_CR_NOTE = "Ledger/EmailDrCrNote";
var CLIENT_WISE_ITEMS = "Ledger/ClientWiseItems";
var ITEM_WISE_CLIENTS = "Ledger/ItemWiseClients";
var CASHBOOK = "Ledger/Cashbook";
var STOCK_IN_HAND = "Inventory/StockInhand";
var STOCK_SUMMARY = "Inventory/StockSummary";

//END OF CLIENT
//----SALT----
var ADDSALT = "salt/Save";
var GETALLSALTS = "salt/GetAll";
var GETSALTINFO = "salt/GetInfo";
var CHANGESALTSTATUS = "salt/ChangeStatus";
var UPDATE_SALT_STATUS = "salt/ChangeStatus";
//----END OF SALT------

//---PRODUCT-------
var ADDPRODUCT = "product/Save_v1";
var GETALLPRODUCTS = "product/GetAll";
var GetAllUOM = "product/GetAllUOM";
var GetUOMSize = "product/GetUOMSize";
var ADDSIZE = "product/AddSize";
var GETALLSIZE = "product/GetAllSize";
var ACTIVATE_SIZE = "product/ActivateSize";
var PRODUCT_SIZE_LIST = "product/GetProductSizeList";
var PRODUCT_SIZE_LIST_BY_COMPANY = "product/GetProductSizeListByCompany";


//-----end of product
//PRODUCT CATEGORY

//-PRODUCT RATES
var PRODUCT_ADD_RATE = 'product/AddRate';
var PRODUCT_GET_RATES = 'product/GetRates';
var ACTIVATE_RATE = 'product/ActivateRate';
//END OF RATES

//---------WORK ORDER
var WORKORDER_GETALL = 'WorkOrder/GetAll';
var WORKORDER_GET_BY_COMPANY = 'WorkOrder/GetByCompany';
var WORKORDER_DETAIL = 'WorkOrder/GetById';
var WORKORDERITEMS_DETAIL = 'WorkOrder/GetItems';
var GET_SITES = "WorkOrder/GetSites";
var GET_SITE_ITEMS = "WorkOrder/GetSiteItems";
var ADD_SITE = "WorkOrder/AddSite";
var GET_SITETAXES = "WorkOrder/GetSiteTaxes";
var UPDATE_SITEINFO = "WorkOrder/UpdateSiteInfo";
var WORKORDER_UPDATE_STATUS = "WorkOrder/UpdateStatus";
var ITEM_ISSUED_REGISTER = "WorkOrder/ItemIssued";
var ITEM_RECEIVED_REGISTER = "WorkOrder/ItemReceived";
var ITEM_RECEIVED_REGISTER_RPT = "WorkOrder/ItemReceived_rpt";

var GET_NEXT_WORKORDER_NUMBER = "NextId/WorkOrderNumber";
var GET_NEXT_RENT_INVOICE_NUMBER = "NextId/RentInvoiceNumber";
var PRINT_ISSUED_LIST = "WorkOrder/PrintIssueReceipt";
var WORK_ORDRER_BALANCE = "WorkOrder/GetWorkOrderBalance";
var WORK_ORDRER_DUE_DATES_REMINDER = "WorkOrder/WorkOrderDueDateReminder";
var WORK_ORDER_OVERDUE_REMINDER = "WorkOrder/WorkOrderOverDuesReminder";
var ADD_SITE_CHALLAN = "challan/AddChallanToSite";
var GET_SITE_CHALLANS = "challan/GetSiteChallans";
var GET_OTHER_CHARGES = "WorkOrder/GetOtherCharges";
var GET_SITE_OTHER_CHARGES = "WorkOrder/LederSiteCharges";
var EMAIL_ISSUE_CHALLAN = "WorkOrder/EmailIssueChallan";
var SMS_ISSUE_CHALLAN = "WorkOrder/SmsIssueChallan";
var EMAIL_RECEIVE_RECEIPT = "WorkOrder/EmailReceivedReceipt";
var SMS_RECEIVE_RECEIPT = "WorkOrder/SMSReceivedReceipt";
var ADD_CHALLAN_DOCUMENT = "WorkOrder/AddChallanDocument";
var GET_CHALLAN_DOCUMENTS = "WorkOrder/GetChallanDocument";
var DEL_CHALLAN_DOCUMENT = "WorkOrder/DeleteChallanDocument";
var PENDING_CHALLAN_ACKNOWLEDGEMENTS = "WorkOrder/PendingChallanAcknowledgements";
var TRANSFER_MATERIAL = "WorkOrder/TransferMaterial";
var ADJUST_MATERIAL = "WorkOrder/AdjustMaterial";
var ADJUST_MATERIAL_LIST = "WorkOrder/MatAjustList";
var ADJUST_MATERILA_BY_ID = "WorkOrder/MatAjusDetailsById";
var MAT_TRANSFER_BY_ID = "WorkOrder/MatTransferDetailsById";
var DELETE_WorkOrder_ITEM = "Workorder/DeleteWorkOrderItem";
//--------END OF WORK ORDER

//Site
var ADD_PAYREMINDER = "Site/AddPayReminder";
var DELETE_PAYREMINDER = "Site/DeletePayReminder";
var GETALL_PAYREMINDER = "Site/GetAllPayReminder";
var GETSITEINFO = "Site/GetSiteInfo";
var GET_ALL_SITENAMES = "Site/GetAllSiteNames";
var GET_SITEGRN = "Site/GetSiteGRN";
var GET_JOBNUMBERS = 'site/GetJobNumbers';
var CLOSE_SITE = 'site/CloseSite';
var CLOSE_SITE_PAYMENT = 'site/CloseSitePayment';
var GET_SITE_JOBS = 'site/GetSiteJobs';

//-End of Site

//--reports
var PENDING_PAYMENT = "Report/PendingPayments";
var PAYMENT_RECEVIED = "Report/PaymentReceived";
var SITE_WISE_INVENTORY = "Report/SiteWiseInventory";
var SITE_WISE_INVENTORY_SUMMARY = "Report/SiteWiseInventorySummary";
var DOWNLOAD_INVENTORY_REPORT = "Report/DownloadInventory";
var CLOSED_JOBNUMBERS = "Report/ClosedSites";
var DASHBOARD_SUMMARY = "Report/DashboardSummary";
var CLOSED_SITES = "Site/GetClosedSites"
var OPEN_JOBNUMBERS = "Site/GetOpenJobNumbers"
var SITE_PAYMENT_SUMMARY = "Site/SitePaymentSummary"
var DAILY_INOUT_TRANSACTIONS = "Report/DailyInOutTransactions"

//--end of reports
//--Journals
var CREATE_ENTRY = "Journal/CreateEntry";
var GET_SITE_JOURNAL = "Journal/GetSiteJournals";
var GET_JOURNAL = "Journal/GetJournal";
//-- end of journals
//-- GRN
var ADD_GRN = "Inventory/AddGRN";
var GET_GRNITEMS_BYID = "Inventory/GetItemsByGrnId";
var GET_GRN_BYID = "Inventory/GrnById";
var GRN_INWARD_CONFIRM = "Inventory/InwardConfirm";

//--end of GRN
//END OF PRODUCT CATEGORY
//-------INVOICE ----------
var ADD_INVOICE = "Invoice/Add";
var GET_INVOICE_LIST = "Invoice/GetList";

//---INVOICE

//--Billing 
var GEN_BILL = "Billing/GenerateBill";
var SAVE_BILL = "Billing/SaveBill";
var GET_BILLS = "Billing/GetBillList";
var GET_BILLITEMS = "Billing/GetBillItems";
var PRINT_BILL = "Billing/PrintBill";
var GET_ALL_USERS = "User/GetAllUsers";
var USER_CREATE = "User/CreateUser";
var GET_ROUTE_ACCESS = "user/GetRouteAccess";
var UPDATE_USER_STATUS = "User/UpdateStatus";
var USER_RESET_PASSWORD = "User/ResetPassword";
var GET_BREAKAGE_FOR_BILL = "Billing/GetBreakageForBill";
var SAVE_BREAKAGE_BILL = "Billing/SaveBreakageBill";
var BILL_ITEMS_TAX = "Billing/BillingItemsTax";
var BILL_CANCEL = "Billing/CancelBill";
var BILL_SETTLE = "Billing/MarkSettle";
var DUE_BILLS = "Billing/DueBills";
var PRINT_DUE_BILLS = "Billing/PrintDueBills";
var PRINT_DUE_BILLS_PDF = "Billing/PrintDueBillsPdf";
var GET_LOSS_ITEMS = "Billing/GetBillLossItems";
var GET_BREAKAGE_ITEMS = "Billing/GetBreakageItems";
var GET_BILL_DETAILS = "Billing/GetBillDetails";
//end of BIlling

//--settings

var SET_USER_DEFAULT_COMPANY = "Settings/SetDefaultCompany";
//end of Settings

//TAX

var GET_APPLICABLE_TAXES = "Tax/GetApplicableTaxes";
var SAVE_APPLICABLE_TAXES = "Tax/SaveTax";
var GET_ALL_PRODUCTS_TAXES = "Tax/GetAllTaxes";
//end of taxes

class Cacheable {

    static ActiveParties = [GET_ALL_ACTIVE_CIENTS];
}