
app.controller("ExportController", ['$scope', '$window', '$compile', 'ModalFactory', '$attrs',
    function ($scope, $window, $compile, ModalFactory, $attrs) {

        $scope.export = function (_type) {

            if (_type == 'xlsx') {
                $scope.exportToExcel();
                return;
            }
            var x = $scope;
            var div = document.createElement('div');
            var template = 'templ/exports/' + $scope.url;
            var fileName = $scope.url.split('.html')[0];
            $(div).load(template + '?d=' + new Date().getTime(), function () {

                const { jsPDF } = window.jspdf;
                var html = $(this).html();
                var compiled = $('<div/>').append($compile(html)($scope));

                setTimeout((o) => {
                    $scope.$apply();
                    html = compiled.html();
                    let pdf = new jsPDF({
                        orientation: 'p',
                        unit: 'pt',
                        format: 'letter',
                       
                        compress: true
                    });
                  
                    pdf.html(html,
                        {
                            width: 580,
                            windowWidth: 580,
                            margin: 15,
                            callback: function (pdf) { pdf.save(fileName) }
                        });
                }, 100);


            });

        }
        $scope.exportToExcel = function () {
            
            const data = $scope.data;// JSON.parse($scope.gstrData);
           
            const columns = getColumns(data);
            columns.splice(columns.length - 1, 1);
            const worksheet = XLSX.utils.json_to_sheet(data, { header: columns });
            const workbook = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(workbook, worksheet, 'sheet-1');
            XLSX.writeFile(workbook, 'excelExport.xlsx');
        }
        function getColumns(data) {
            const columns = [];
            data.forEach(row => {
                Object.keys(row).forEach(col => {
                    if (!columns.includes(col)) {
                        columns.push(col);
                    }
                });
            });
            return columns;
        }

    }]);
