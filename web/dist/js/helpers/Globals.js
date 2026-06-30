var API_URL = 'http://localhost:51921/api/';
var AUTH_SERVER = 'https://localhost:44311/';
var REPORT_SERVER = 'https://localhost:44358/';

var SERVER_URL = 'http://localhost:51921/api/';
var SERVER_RPT_URL = 'http://localhost:51921/';
var IMAGE_LOCATION = 'http://localhost:51921/';
//var API_URL = 'http://rentac.rbntechnologies.com/api/api/';
//var SERVER_URL = 'http://rentac.rbntechnologies.com/api/';
var STORAGE_LOC = 'https://rentacstorage.blob.core.windows.net/rentacdev/';


var ROOT_SOPE = null;
var HTTP_SERVICE = null;
var CACHE_FACTORY = null;
var STOREID = 1;
var PAGE_SIZE = 20;
var SLIDING_TIMER = 1800; // 30 minutes
var SESSION_DURATION = 1800;//30 minutes
var MinDate = '';
var MaxDate = '';
var razorKey = 'rzp_test_cTIM9RQXJYmtm1';
var DECIMAL_PLACES = 2;
function round(num) {
    const tenths = Math.floor(num * 10) % 10;

    if (tenths >= 5) {
        return Math.ceil(num);
    } else {
        return Math.floor(num);
    }
}

function EnableToolbar(id) {
    if (id == 0) {
        $('.toolbar').attr('visible', 'hidden');
    }
    else {
        $('.toolbar').attr('visible', 'visible');
    }
}

function validatePrefix(str) {
    var regex = /^[a-zA-Z0-9/-]+$/;
    return regex.test(str);
}
function htmlEncode(str) {
    if (str == null || str == undefined) {
        return "";
    }
    var div = document.createElement('div');
    str = str.replace(/\n/g, '');
    div.innerText = str; // Sets the plain text, automatically encoding special HTML characters

    return div.innerHTML; // Returns the HTML-encoded string
}
function showMessage(text) {
    alert(text);
    //var div = '<div id="msg" class="message msgDiv"><center id="infoMsg" class="alert alert-success"></center></div>';
    //$('.widget').append(div);
    //$('#infoMsg').text(text);
    //$('#infoMsg').hide();
    //$('#infoMsg').slideDown(300, function () {
    //    window.setTimeout(
    //    $('.msgDiv').slideUp(4000, function () {

    //        $('.msgDiv').remove();
    //    }), 1000);
    //});
}
function getLastDayOfMonth(date) {
    return new Date(date.getFullYear(), date.getMonth() + 1, 0);
}

function isEmpty(element) {
    if (element.length == 0) {
        return false;
    }
    return element.val().length == 0;

}
function convertDate(inputFormat, checkMaxDate = true) {

    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(inputFormat);
    if (d.getFullYear() == 1) {
        return "";
    }
    // to control the max date. The max date must not exceed the fin year end date.
    var diff = dateDiff(MaxDate, d);
    var diff1 = dateDiff(MinDate, d);

    if (diff > 0 && checkMaxDate == true) {
        d = new Date(MaxDate);
    }

    return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
}
function formatForPicker(inputFormat) {
    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(inputFormat);

    d = convertDate(d, false).split('/');
    return d[2] + '-' + d[1] + '-' + d[0] + "T00:00:00";


}
defaultLocal = 'hi-IN';
//function formatdate(date, local) {

//    const parseDMY = s => {
//        let [d, m, y] = s.split(/\D/);
//        return new Date(y, m - 1, d);
//    };
//    return parseDMY(date).toLocaleDateString();
//}

function formatdate(date, local) {

    var dates = date.split('/');
    return dates[1] + '/' + dates[0] + '/' + dates[2];
}


//this is only to convert dd-mm-yyyy to javascript date format.
//used on the dashboard.
function stringToDate(strDate) {
    return new Date(strDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
}
function dateDiff(start, end) {
    // end - start returns difference in milliseconds 
    start = new Date(start);
    end = new Date(end);
    var diff = new Date(end - start);
    // get days
    var days = diff / 1000 / 60 / 60 / 24;
    return days;
}
function trimAll(strText) {

    return strText.replace(/\s/g, '');

}
var Enums = {
    BANKS: 18,
    PURCHASE_ACCOUNT: 23,
    SUNDRY_DEBTORS: 17,
    SUNDRY_CREDITORS: 10,
    SALE_ACCOUNT: 22
}
var DEBTORS_AND_CREDITORS = Enums.SUNDRY_CREDITORS + ',' + Enums.SUNDRY_DEBTORS;
var session_timer = null;
function StartSessionSlider() {
    SLIDING_TIMER = SESSION_DURATION;
    window.clearInterval(session_timer);
    session_timer = window.setInterval(function () {
        if (window.location.hash != '#/login') {
            SLIDING_TIMER--;
            // console.log(SLIDING_TIMER);
            saveToSession();
            if (SLIDING_TIMER < 0) {
                window.clearInterval(session_timer);
                sessionStorage.clear();
                //   window.location = '/pages/index.html#/login';
            }
        }
    }, 1000);

}
function saveToSession() {
    sessionStorage.setItem("session_timer", session_timer);
    sessionStorage.setItem("SLIDING_TIMER", SLIDING_TIMER);

}
function getSessionTimer() {
    SLIDING_TIMER = sessionStorage.getItem("SLIDING_TIMER");
    session_timer = sessionStorage.getItem("session_timer");
}
function resetSlider() {


    //  getSessionTimer();
    // if (session_timer == null) {

    //  StartSessionSlider();
    // }
}
function prepend(value, array) {
    var newArray = array.slice();
    newArray.unshift(value);
    return newArray;
}
function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}
const customMimeToExtension = {
    'text/plain': 'txt',
    'image/png': 'png',
    'image/jpeg': 'jpeg',
    'image/jpg': 'jpg',
    'application/pdf': 'pdf',

    'application/json': 'json'
    // ... add more as needed
};
function isValidDate(dateObject) {
    // First, check if it's actually a Date object
    if (Object.prototype.toString.call(dateObject) === "[object Date]") {
        // Then, check if its time value is not NaN
        return !isNaN(dateObject.getTime());
    }
    return false; // Not a Date object
}
function addDays(date, days) {
    const result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}
function addMonths(date, months) {
    const d = new Date(date);
    const years = Math.floor(months / 12);
    const remainingMonths = months % 12;

    // Add years first
    if (years !== 0) {
        d.setFullYear(d.getFullYear() + years);
    }

    // Add remaining months
    if (remainingMonths !== 0) {
        d.setMonth(d.getMonth() + remainingMonths);
    }

    return d;
}
function getExtensionFromMime(mimeType) {
    return customMimeToExtension[mimeType] || null; // Returns null if not found
}

function downloadReport(reportName, data) {

    var resultByte = base64ToArrayBuffer(data);
    var bytes = new Uint8Array(resultByte); // pass your byte response to this constructor
    var blob = new Blob([bytes], { type: "application/pdf" });// change resultByte to bytes
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = reportName;//"ClientItemwiseItemBalance.pdf";
    link.click();
}
function isBase64(strData) {
    if (strData.split(',')[0].indexOf('base64') >= 0) {
        return true;
    }
    return false;
}
function dataURItoBlob(dataURI) {
    // convert base64/URLEncoded data component to raw binary data held in a string
    var byteString;
    if (dataURI.split(',')[0].indexOf('base64') >= 0)
        byteString = atob(dataURI.split(',')[1]);
    else
        byteString = unescape(dataURI.split(',')[1]);

    // separate out the mime component
    var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

    // write the bytes of the string to a typed array
    var ia = new Uint8Array(byteString.length);
    for (var i = 0; i < byteString.length; i++) {
        ia[i] = byteString.charCodeAt(i);
    }

    return new Blob([ia], { type: mimeString });
}
async function downloadFile(url, fileSaver) {
    try {
        if (isBase64(url)) {
            debugger
            var blb = dataURItoBlob(url);
            var fileType = getExtensionFromMime(blb.type);
            fileSaver.saveAs(blb, 'file.' + fileType);
            return;
        }
        filename = url.split('/').pop();
        const response = await fetch(url, {
            headers: {
                Accept:
                    "application/json, text/plain,application/zip, image/png, image/jpeg, image/*",
            },
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const blob = await response.blob();
        //  const blobUrl = URL.createObjectURL(blob);
        fileSaver.saveAs(blob, filename);
        // URL.revokeObjectURL(blobUrl);
    } catch (err) {
        console.error("Error in fetching and downloading file:", err);
    }
}
function cloneObj(jsonObj) {
    // convert base64/URLEncoded data component to raw binary data held in a string
    return JSON.parse(JSON.stringify(jsonObj));
}

function uniqueItems(arr) {
    return arr.filter(function (itm, i, arr) {
        return i == arr.indexOf(itm);
    });

}
function uniqueObjects(arr, key) {
    return [...new Map(arr.map(item => [item[key], item])).values()];
}
function sum(arr) {
    return arr.reduce((partialSum, a) => partialSum + a, 0);
}
resetSlider();
var CHALLAN_TYPE = {
    LIFT_DELIVERY: 1,
    RENT_DELIVERY: 2,
    PURCHASE: 3,
    PURCHASE_RETURN: 4,
    SALE: 5,
    SALE_RETURN: 6,
    WORKORDER: 7

}

class StaicData {
    static SYS_ACCOUNT_GROUPS =
        {
            SUNDRY_CREDITORS: 10,
            SUNDRY_DEBTORS: 17
        };
    static ProductCategories = [
        {
            "CategoryId": 1012,
            "Name": "Rented",
            "MinMargin": 0.0,
            "Status": 1,
            "StoreId": 1
        },
        {
            "CategoryId": 1013,
            "Name": "Consumeables",
            "MinMargin": 0.0,
            "Status": 1,
            "StoreId": 1
        }
    ];
    static QUOTATIONT_TYPES = [
        {
            "TypeId": 15,
            "Name": "Rented",

        },
        {
            "TypeId": 16,
            "Name": "Contract",
        },
        {
            "TypeId": 17,
            "Name": "Sales",
        }
    ];
    static QUOTATIONT_STATUSES = [
        {
            "Id": 1,
            "Name": "Draft",
        },

        {
            "Id": 2,
            "Name": "Shared",
        },
        {
            "Id": 3,
            "Name": "Deleted",
        },
        {
            "Id": 4,
            "Name": "Approved",
        },
        {
            "Id": 5,
            "Name": "Rejected",
        },
        {
            "Id": 6,
            "Name": "Issued",
        },
        {
            "Id": 7,
            "Name": "Delayed To Receive",
        },
    ];

    static UOM = [{ "UOMId": 1, "UOM": "PC" }, { "UOMId": 2, "UOM": "KG" }, { "UOMId": 3, "UOM": "RMT" }, { "UOMId": 4, "UOM": "SET" }, { "UOMId": 5, "UOM": "NOS" }, { "UOMId": 6, "UOM": "SQFT" }
        , { "UOMId": 7, "UOM": "MONTH" }, { "UOMId": 8, "UOM": "CUFT" }, { "UOMId": 9, "UOM": "MTR" }, { "UOMId": 10, "UOM": "TON" }];
    static TAX_CATEGORY = [{ "TaxId": 1, "TaxName": "GST 0%", CGST: 0, SGST: 0, IGST: 0, IGSTAmount: 0, SGSTAmount: 0, CGSTAmount: 0 },
    { "TaxId": 2, "TaxName": "GST 5%", CGST: 2.5, SGST: 2.5, IGST: 5, IGSTAmount: 0, SGSTAmount: 0, CGSTAmount: 0 },
    { "TaxId": 3, "TaxName": "GST 12%", CGST: 6, SGST: 6, IGST: 12, IGSTAmount: 0, SGSTAmount: 0, CGSTAmount: 0 },
    { "TaxId": 4, "TaxName": "GST 18%", CGST: 9, SGST: 9, IGST: 18, IGSTAmount: 0, SGSTAmount: 0, CGSTAmount: 0 },
    { "TaxId": 5, "TaxName": "GST 28%", CGST: 14, SGST: 14, IGST: 28, IGSTAmount: 0, SGSTAmount: 0, CGSTAmount: 0 }
    ];

    static EWAY_TXN_SUB_TYPE = [{ Id: 1, Name: 'Supply' }, { Id: 2, Name: 'Import' }, { Id: 3, Name: 'Export' },
    { Id: 4, Name: 'Job Work' }, { Id: 5, Name: 'For Own Use' }, { Id: 6, Name: 'Job Work Returns' }, { Id: 7, Name: 'Sales Return' },
    { Id: 8, Name: 'Others' }];
    static EWAY_TXN_TYPES = [{ Id: 1, Name: 'Regular' }, { Id: 2, Name: 'Bill To - Ship To' }, { Id: 3, Name: 'Bill From - Dispatch From' },
    { Id: 4, Name: 'Combination of 2 and 3' }];
    static EWAY_TRANSPORT_MODELS = [{ Id: 1, Name: 'Road' }, { Id: 2, Name: 'Rail' }, { Id: 3, Name: 'Air' },
    { Id: 4, Name: 'Ship' }];
    static PAYMENT_MODES = [{ Id: 1, Name: 'Cash' }, { Id: 2, Name: 'Bank' }, { Id: 3, Name: 'Cheque' }, { Id: 4, Name: 'NEFT/RTGS' }, { Id: 5, Name: 'OTHERS' }, { Id: 6, Name: 'UPI' }];

    static CONTRACT_TYPES = [{ Id: 1, Name: 'Fixed' }, { Id: 2, Name: 'Running' }, { Id: 3, Name: 'Per Day' }];
    static CONTRACT_MEASUREMENTS = [{ Id: 1, Name: 'SQFT' }, { Id: 2, Name: 'SQMTR' }];
    static TASK_STATUSES = [{ Id: 1, Name: 'Assigned' }, { Id: 2, Name: 'Active' }, { Id: 3, Name: 'In-Progress' },
    { Id: 4, Name: 'Deleted' }, { Id: 5, Name: 'Completed' }];
    static CONTRACT_ACTIVITY_TYPES = [{ Id: 1, Name: 'Install' }, { Id: 2, Name: 'Dismantle' }, { Id: 4, Name: 'Re-Install' }
        , { Id: 3, Name: 'Pickup' }
        , { Id: 5, Name: 'Others' }];
}
var REGEX = {
    PAN: /([A-Z]){5}([0-9]){4}([A-Z]){1}$/,
    GST: /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$/,
    MobileIN: /^(?:(?:\+|0{0,2})91(\s*[\ -]\s*)?|[0]?)?[789]\d{9}|(\d[ -]?){10}\d$/,
    EMAIL: /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
    BILL_PREFIX: /^[a-zA-Z0-9/-]+$/
}