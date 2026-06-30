app.controller('AccountsDashboardController', ['$scope', '$http', 'AuthenticationService', function ($scope, $http, authService) {
    var ageingChartInstance = null;
    function defaultWidgets() {
        return [
            { key: 'rentalRevenue', title: 'Rental Revenue', value: '₹ 0.00 L', icon: 'fa-line-chart', panelClass: 'ops-widget-productivity', hint: 'Current month' },
            { key: 'contractRevenue', title: 'Contract Revenue', value: '₹ 0.00 L', icon: 'fa-file-text-o', panelClass: 'ops-widget-inventory', hint: 'Current month' },
            { key: 'receivables', title: 'Receivables', value: '₹ 0.00 L', icon: 'fa-money', panelClass: 'ops-widget-active', hint: 'Outstanding' },
            { key: 'payables', title: 'Payables', value: '₹ 0.00 L', icon: 'fa-credit-card', panelClass: 'ops-widget-delayed', hint: 'Outstanding' }
        ];
    }
    function defaultAgeingWidgets() {
        return [
            { key: 'bucket0To30', title: '0–30 days', value: '₹ 0.00 L', icon: 'fa-calendar', panelClass: 'ops-widget-active' },
            { key: 'bucket31To60', title: '31–60 days', value: '₹ 0.00 L', icon: 'fa-calendar-o', panelClass: 'ops-widget-inventory' },
            { key: 'bucket61To90', title: '61–90 days', value: '₹ 0.00 L', icon: 'fa-clock-o', panelClass: 'ops-widget-delayed' },
            { key: 'bucket90Plus', title: '90+ days', value: '₹ 0.00 L', icon: 'fa-exclamation-circle', panelClass: 'ops-widget-productivity' }
        ];
    }

    $scope.revenueWidgets = defaultWidgets();
    $scope.ageingWidgets = defaultAgeingWidgets();
    $scope.siteOutstandingRows = [];
    $scope.accountsError = null;
    $scope.accountsLoading = false;

    function formatCurrencyInLakh(v) {
        var n = Number(v || 0);
        if (isNaN(n)) n = 0;
        var lakh = n / 100000;
        return '₹ ' + lakh.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' L';
    }
    function formatCurrency(v) {
        var n = Number(v || 0);
        if (isNaN(n)) n = 0;
        return '₹ ' + n.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }
    function formatCurrencyInThousand(v) {
        var n = Number(v || 0);
        if (isNaN(n)) n = 0;
        var thousand = n / 1000;
        return '₹ ' + thousand.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' K';
    }
    $scope.formatCurrencyInLakh = formatCurrencyInLakh;
    $scope.formatCurrency = formatCurrency;
    $scope.formatCurrencyInThousand = formatCurrencyInThousand;

    function getCurrentMonthBounds() {
        var now = new Date();
        var from = new Date(now.getFullYear(), now.getMonth(), 1);
        var to = new Date(now.getFullYear(), now.getMonth() + 1, 0);
        return { from: from, to: to };
    }

    function applyKpis(d) {
        if (!d) return;
        var rental = d.RentalRevenue != null ? d.RentalRevenue : d.rentalRevenue;
        var contract = d.ContractRevenue != null ? d.ContractRevenue : d.contractRevenue;
        var receivables = d.Receivables != null ? d.Receivables : d.receivables;
        var payables = d.Payables != null ? d.Payables : d.payables;
        $scope.revenueWidgets.forEach(function (w) {
            if (w.key === 'rentalRevenue') w.value = formatCurrencyInLakh(rental);
            if (w.key === 'contractRevenue') w.value = formatCurrencyInLakh(contract);
            if (w.key === 'receivables') w.value = formatCurrencyInLakh(receivables);
            if (w.key === 'payables') w.value = formatCurrencyInLakh(payables);
        });
    }
    function applyAgeing(d) {
        if (!d) return;
        var map = {
            bucket0To30: d.Bucket0To30 != null ? d.Bucket0To30 : d.bucket0To30,
            bucket31To60: d.Bucket31To60 != null ? d.Bucket31To60 : d.bucket31To60,
            bucket61To90: d.Bucket61To90 != null ? d.Bucket61To90 : d.bucket61To90,
            bucket90Plus: d.Bucket90Plus != null ? d.Bucket90Plus : d.bucket90Plus
        };
        $scope.ageingWidgets.forEach(function (w) {
            w.value = formatCurrencyInLakh(map[w.key]);
        });
        renderAgeingPieChart(map);
    }

    function renderAgeingPieChart(map) {
        if (!window.Chart) return;
        var canvas = document.getElementById('accountsAgeingPieChart');
        if (!canvas) return;

        if (ageingChartInstance) {
            ageingChartInstance.destroy();
            ageingChartInstance = null;
        }

        var values = [
            Number(map.bucket0To30 || 0),
            Number(map.bucket31To60 || 0),
            Number(map.bucket61To90 || 0),
            Number(map.bucket90Plus || 0)
        ];
        var total = values[0] + values[1] + values[2] + values[3];

        ageingChartInstance = new Chart(canvas, {
            type: 'bar',
            data: {
                labels: ['0-30 days', '31-60 days', '61-90 days', '90+ days'],
                datasets: [{
                    label: 'Amount',
                    data: values,
                    backgroundColor: ['#4caf50', '#03a9f4', '#ff9800', '#f44336'],
                    borderColor: '#ffffff',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                legend: { display: false },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            callback: function (value) {
                                var k = Number(value || 0) / 1000;
                                return '₹ ' + k.toLocaleString('en-IN', { maximumFractionDigits: 2 }) + ' K';
                            }
                        }
                    }]
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem, data) {
                            var i = tooltipItem.index;
                            var v = Number(data.datasets[0].data[i] || 0);
                            var k = v / 1000;
                            var p = total > 0 ? ((v * 100) / total).toFixed(1) : '0.0';
                            return data.labels[i] + ': ₹ ' + k.toLocaleString('en-IN', { maximumFractionDigits: 2 }) + ' K (' + p + '%)';
                        }
                    }
                }
            }
        });
    }

    $scope.reloadAccountsKpis = function () {
        $scope.accountsLoading = true;
        $scope.accountsError = null;
        var bounds = getCurrentMonthBounds();
        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('AccountsDashboard/RevenueKpis?from=' + bounds.from.toISOString() + '&to=' + bounds.to.toISOString());
        $http(req).then(function (res) {
            applyKpis(res.data);
        }, function () {
            $scope.accountsError = 'Could not load revenue KPIs. Ensure database migration ran (p_accounts_revenueKpis).';
            $scope.revenueWidgets = defaultWidgets();
        }).finally(function () {
            $scope.accountsLoading = false;
        });
    };
    $scope.reloadAgeingSummary = function () {
        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('AccountsDashboard/AgeingSummary');
        $http(req).then(function (res) {
            applyAgeing(res.data);
        }, function () {
            $scope.ageingWidgets = defaultAgeingWidgets();
            renderAgeingPieChart({
                bucket0To30: 0,
                bucket31To60: 0,
                bucket61To90: 0,
                bucket90Plus: 0
            });
        });
    };
    $scope.reloadSiteOutstanding = function () {
        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('AccountsDashboard/SiteWiseOutstanding');
        $http(req).then(function (res) {
            $scope.siteOutstandingRows = Array.isArray(res.data) ? res.data : [];
        }, function () {
            $scope.siteOutstandingRows = [];
        });
    };

    $scope.reloadAccountsKpis();
    $scope.reloadAgeingSummary();
    $scope.reloadSiteOutstanding();
}]);
