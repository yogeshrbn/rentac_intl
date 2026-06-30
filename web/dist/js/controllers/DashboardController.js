

app.controller("DashboardController", ['$scope', '$routeParams', '$rootScope', '$cacheFactory', '$http', 'StorageService', 'LoginService', 'AuthenticationService', 'LedgerFactory',
    function ($scope, $routeParams, $rootScope, $cacheFactory, $http, storageService, loginService, authService, LedgerFactory) {
        var ctx = document.getElementById("myChart").getContext('2d');
        var _billingChartCTX = document.getElementById("_billingChart").getContext('2d');

        //var ctx1 = document.getElementById("myChart1").getContext('2d');
        var issueChartCanvas = document.getElementById("topRatedItems").getContext('2d');
        var _billingChartDataObj = {
            datasets: [
                //{
                //    label: 'Billing',
                //    data: [],
                //    backgroundColor: [
                //        'rgba(54, 99, 132, 1)',
                //        'rgba(54, 162, 235, 1)',
                //        'rgba(255, 206, 86, 1)',
                //        'rgba(255, 105, 86, 1)',
                //        'rgba(100, 205, 86, 16)'
                //    ],
                //    borderColor: [
                //        'rgba(54, 99, 132, 1)',
                //        'rgba(54, 162, 235, 1)',
                //        'rgba(255, 206, 86, 1)',
                //        'rgba(255, 105, 86, 1)',
                //        'rgba(100, 205, 86, 16)'
                //    ],
                //    borderWidth: 1
                //},
                {
                    label: 'Rent',
                    data: [],
                   
                    borderColor: [
                        'rgba(255,99,132,1)',
                       
                    ],
                   // borderWidth: 1
                }
            ],

            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: [

            ]
        };


        var newCustomerData = {
            labels: [],
            datasets: [{
                label: 'Customers',
                data: [],

                backgroundColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderColor: [
                    'rgba(255,99,132,1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
        };
        var _billingChart = new Chart(_billingChartCTX, {
            type: 'line',
            data: _billingChartDataObj,
            options: {
                scales: {
                    yAxes: [
                        {
                            ticks: {
                                callback: function (label, index, labels) {
                                     
                                    if (label <= 1) {
                                        return parseInt(label) + 'K';
                                    } else {
                                        return parseInt(label) / 1000 + 'K';
                                    }
                                },
                                beginAtZero: true
                            }
                        }
                    ],
                    //xAxes: [{
                    //    scaleLabel: {
                    //        display: true,
                    //        labelString: 'Months'
                    //    },
                    //    //barPercentage: 0.4, // Controls bar width
                    //    //categoryPercentage: 0.8 // Controls space between groups
                    //}]

                },
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Chart.js Grouped Bar Chart'
                    }
                }
            }
        });
        var myChart = new Chart(ctx, {
            type: 'bar',
            data: newCustomerData,
            options: {
                scales: {
                    yAxes: [
                        {
                            ticks: {
                                beginAtZero: true
                            }
                        }
                    ]
                }

            }
        });
        var data = {
            datasets: [{
                data: [],
                backgroundColor: [
                    'rgba(54, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(255, 105, 86, 1)',
                    'rgba(100, 205, 86, 16)'
                ],
            }],

            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: [

            ]
        };

        var issueChart = new Chart(issueChartCanvas, {
            type: 'pie',

            data: data,
            options: {

                legend: {
                    display: true,

                    position: 'top',
                    labels: {
                        font: {
                            size: 10 // Set your desired font size
                        },
                        maxWidth: 5,
                        fontColor: 'rgb(0,0, 255, 1)'
                    }
                }
            }
        });


        // $scope.overDueSummary = [];
        $scope.dBFilter = { dateFrom: new Date(), dateTo: new Date() };
        $scope.overdueSummary = function (number) {

            var report = new $.Reports({});
            var filter = { DueFromDays: number };
            report.OverDueAmountSummary(function (o) {
                $scope.overDueSummary = o.data;
            }, filter);
        };
        $scope.newCustomers = function () {

            var report = new $.Reports({});
            var filter = Object.assign({}, $scope.dBFilter);

            filter.dateFrom = new Date(filter.dateFrom).toLocaleDateString("en-GB");
            filter.dateTo = new Date(filter.dateTo).toLocaleDateString("en-GB");
            report.neCustomers(function (o) {
                $scope.newCustomerData = o.data;
                newCustomerData.labels = o.data.map(o => o.MonthYear);
                newCustomerData.datasets[0].data = o.data.map(o => o.Customers);
                myChart.update();
            }, filter);
        };

        $scope.clientDashboardStats = function () {
            // var f = storageService.getSelectedFinYear();

            var report = new $.Reports({});

            var filter = Object.assign({}, $scope.dBFilter);

            filter.dateFrom = new Date(filter.dateFrom).toLocaleDateString("en-GB");
            filter.dateTo = new Date(filter.dateTo).toLocaleDateString("en-GB");
            report.clientDashboardStats(function (o) {

                $scope.summary = o.data;
                $scope.summary.stats.Sales = $scope.summary.stats.Sales / 1000 + 'K';
                data.datasets[0].data = o.data.topItemsOnRent.map(o => o.Qty);
                data.labels = o.data.topItemsOnRent.map(o => o.Product);



                issueChart.update();
                 
                _billingChartDataObj.labels = o.data.billingSummary.map(o => o.MonthYear);
                _billingChartDataObj.datasets[0].data = o.data.billingSummary.map(o => o.Rent);
                // _billingChartDataObj.datasets[1].data = o.data.billingSummary.map(o => o.Sales);

                //var _clonnedData = cloneObj(_dataObject);
                //_clonnedData.data = o.data.billingSummary.map(o=> o.Total);
                //_clonnedData.labels = o.data.billingSummary.map(o => o.MonthYear);
                //_billingChart.data = _clonnedData;
                _billingChart.update();

                //$('#stocksummary').puidatatable({
                //    columns: [{ field: 'Product', headerText: 'Item', }, { field: 'OnSite', headerText: 'On Site', },
                //    { field: 'StockInHand', headerText: 'On Floor', }],

                //    scrollable: true,
                //    editMode: 'cell',
                //    emptyMessage: 'No records found',
                //    // draggableColumns:true,
                //    scrollHeight: '80%',

                //    datasource: $scope.summary.stock
                //});
                // $scope.gridApi.setGridOption("rowData", $scope.summary.stock);

            }, filter);
        };

        $scope.login = function (finyearId) {
            debugger
            loginService.login('user', 'refresh_token', finyearId, loggedInInfo.UserId).then(function (response) {
                if (!response) {
                    init();
                }
            });
        };
        function init() {
            $scope.newCustomerData = null;
            $scope.stats = null;

            $scope.FinYear = storageService.getFinYear();
            setFilterAndFinYeaer();
            $scope.overdueSummary(0);
            $scope.newCustomers();
            $scope.clientDashboardStats();
        }
        init();
        function setFilterAndFinYeaer() {

            var f = storageService.getSelectedFinYear();
            $scope.dBFilter = { dateFrom: new Date(f.MinDate), dateTo: new Date(f.MaxDate) };
        }
        $scope.selectYear = function (f) {

            $scope.selectedYear = f.FinYear;
            $scope.login(f.FinYearId);
            storageService.selectFinYear(f.FinYear);
            //$scope.dBFilter = { dateFrom: new Date(f.MinDate), dateTo: new Date(f.MaxDate) };
            setFilterAndFinYeaer();
        }

        var loggedInInfo = authService.getTokenInfo();
        if (loggedInInfo) {
            $scope.selectedYear = new Date(loggedInInfo.FinYearStart).getFullYear() + '-' + new Date(loggedInInfo.FinYearEnd).getFullYear()
        }
        LedgerFactory.GetAllParties(function (e) {
            storageService.putCache('parties', JSON.stringify(e.data));

        });


        const myTheme = agGrid.themeQuartz.withParams({
            borderColor: "#00000054",
            headerHeight: "30px",
            headerTextColor: "#000",
            fontSize: 13,
            headerFontSize: 13,
            wrapperBorderRadius: 0,
            headerColumnBorder: true,
            rowHeight: 30,
            headerBackgroundColor: "#d7d7d7",
            headerCellMovingBackgroundColor: "rgb(80, 40, 140)",
            headerCellHoverBackgroundColor: "#98CCF8",
        });
        var gridApi;

        //Grid Options: Contains all of the grid configurations
        //$scope.gridOptions = {
        //    theme: myTheme,

        //   // loading: false,
        //    rowData: null,
        //    domLayout: 'normal',

        //    // Columns to be displayed (Should match rowData properties)
        //  alwaysShowHorizontalScroll: false,
        //    columnDefs: [
        //        { field: "Product", headerName: 'Product',width:310 },
        //        { field: "OnSite", headerName: 'OnSite', width: 90 },
        //        { field: "StockInHand", headerName: 'In Hand', width: 90 },
        //    ],

        //    defaultColDef: {

        //        autoHeaderHeight: true,
        //        resizable: false,


        //    },
        //};
        //// Create Grid: Create new grid within the #myGrid div, using the Grid Options object
        //$scope.gridApi = agGrid.createGrid(document.querySelector("#stocksummary"), $scope.gridOptions);

    }]);