// excelService.js
app.factory('ExcelService', ['$window', function ($window) {

    // Check if XLSX is available
    if (!XLSX) {
        console.error('XLSX library not loaded. Make sure to include it in your HTML.');
        return null;
    }

    var service = {

        /**
         * Export JSON data to Excel
         * @param {Array} data - Array of objects to export
         * @param {string} fileName - Name of the Excel file (without extension)
         * @param {Array} headers - Optional custom headers
         * @param {string} sheetName - Optional sheet name
         */
        exportToExcel: function (data, fileName, headers, sheetName) {
            debugger
            if (!data || !Array.isArray(data) || data.length === 0) {
                console.error('No data to export');
                return;
            }

            // Use custom headers or extract from first object
            var wsHeaders = headers || Object.keys(data[0]);

            // Prepare worksheet data
            var wsData = [];

            // Add headers
            wsData.push(wsHeaders);

            // Add data rows
            data.forEach(function (item) {
                var row = [];
                wsHeaders.forEach(function (header) {
                    // Handle nested properties
                    var value = getNestedValue(item, header);
                    row.push(value !== undefined ? value : '');
                });
                wsData.push(row);
            });

            // Create worksheet
            var ws =  XLSX.utils.aoa_to_sheet(wsData);

            // Create workbook
            var wb =  XLSX.utils.book_new();

            // Add worksheet to workbook
            XLSX.utils.book_append_sheet(wb, ws, sheetName || 'Sheet1');

            // Generate Excel file
            XLSX.writeFile(wb, (fileName || 'export') + '.xlsx');
        },

        /**
         * Export JSON data with custom column widths
         */
        exportToExcelWithFormatting: function (data, fileName, options) {
            options = options || {};
            var headers = options.headers || Object.keys(data[0]);
            var sheetName = options.sheetName || 'Sheet1';
            var columnWidths = options.columnWidths || [];

            // Prepare worksheet data
            var wsData = [headers];

            data.forEach(function (item) {
                var row = [];
                headers.forEach(function (header) {
                    var value = getNestedValue(item, header);
                    row.push(value !== undefined ? value : '');
                });
                wsData.push(row);
            });

            // Create worksheet
            var ws = XLSX.utils.aoa_to_sheet(wsData);

            // Set column widths if provided
            if (columnWidths.length > 0) {
                ws['!cols'] = columnWidths.map(function (width) {
                    return { width: width };
                });
            }

            // Create workbook
            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, sheetName);

            XLSX.writeFile(wb, (fileName || 'export') + '.xlsx');
        },

        /**
         * Export multiple sheets to Excel
         */
        exportMultipleSheets: function (sheets, fileName) {
            var wb = XLSX.utils.book_new();

            sheets.forEach(function (sheet, index) {
                var wsHeaders = sheet.headers || Object.keys(sheet.data[0]);
                var wsData = [wsHeaders];

                sheet.data.forEach(function (item) {
                    var row = [];
                    wsHeaders.forEach(function (header) {
                        var value = getNestedValue(item, header);
                        row.push(value !== undefined ? value : '');
                    });
                    wsData.push(row);
                });

                var ws = XLSX.utils.aoa_to_sheet(wsData);
                XLSX.utils.book_append_sheet(
                    wb,
                    ws,
                    sheet.name || ('Sheet' + (index + 1))
                );
            });

            XLSX.writeFile(wb, (fileName || 'export') + '.xlsx');
        },

        /**
         * Export specific columns only
         */
        exportSelectedColumns: function (data, columns, fileName) {
            if (!columns || columns.length === 0) {
                service.exportToExcel(data, fileName);
                return;
            }

            var wsData = [columns];

            data.forEach(function (item) {
                var row = [];
                columns.forEach(function (column) {
                    var value = getNestedValue(item, column);
                    row.push(value !== undefined ? value : '');
                });
                wsData.push(row);
            });

            var ws = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(ws, XLSX.utils.aoa_to_sheet(wsData), 'Sheet1');

            XLSX.writeFile(ws, (fileName || 'export') + '.xlsx');
        }
    };

    // Helper function to get nested property values
    function getNestedValue(obj, path) {
        return path.split('.').reduce(function (prev, curr) {
            return prev ? prev[curr] : undefined;
        }, obj);
    }

    return service;
}]);