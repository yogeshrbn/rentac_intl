app.controller('CEODashboardController', ['$scope', '$timeout', '$http', 'AuthenticationService', function ($scope, $timeout, $http, authService) {
    $scope.kpiBars = [
        { label: 'Total Active Sites', value: 42, display: '42' },
        { label: 'Total Stock Utilisation %', value: 73, display: '73%' },
        { label: 'Revenue MTD', value: 126, display: 'Rs 1.26 Cr' },
        { label: 'Gross Margin %', value: 31.4, display: '31.4%' },
        { label: 'Outstanding > 60 Days', value: 48.2, display: 'Rs 48.2 L' },
        { label: 'Cash in Bank', value: 22.7, display: 'Rs 22.7 L' },
        { label: 'Order Book', value: 380, display: 'Rs 3.8 Cr' },
        { label: 'Idle Stock %', value: 18, display: '18%' },
        { label: 'Labour Productivity', value: 86, display: '86%' },
        { label: 'Net Profit YTD', value: 210, display: 'Rs 2.1 Cr' }
    ];
    $scope.earlyWarningAlerts = [
        { title: 'Project margin below 20%', value: '4 projects', numericValue: 4, level: 'danger' },
        { title: 'Customer overdue > 60 days', value: '12 customers', numericValue: 12, level: 'danger' },
        { title: 'Idle stock > 20%', value: '23.4%', numericValue: 23.4, level: 'warning' },
        { title: 'Labour cost > 35%', value: '37.8%', numericValue: 37.8, level: 'warning' },
        { title: 'Damage % above threshold', value: '2 sites', numericValue: 2, level: 'danger' }
    ];
    $scope.rentalVsContractAnalytics = [
        { title: 'EBITDA % rental', value: '24.8%' },
        { title: 'EBITDA % contract', value: '18.9%' },
        { title: 'Capital Return Ratio', value: '1.62x' }
    ];
    $scope.forecastingAnalytics = [
        { title: 'Next 3 Months Revenue', value: 4.9, display: 'Rs 4.90 Cr' },
        { title: 'Expected Collection', value: 3.6, display: 'Rs 3.60 Cr' },
        { title: 'Material Requirement Forecast', value: 1.4, display: 'Rs 1.40 Cr' }
    ];
    $scope.forecastingMonths = [
        { month: 'Month 1', revenue: 0, collection: 0, material: 0 },
        { month: 'Month 2', revenue: 0, collection: 0, material: 0 },
        { month: 'Month 3', revenue: 0, collection: 0, material: 0 }
    ];

    // Kept as a separate widget section (table) while still showing summary above.
    // Also add the missing "Top 5 Risk Projects" widget content below.
    $scope.topRiskProjects = [
        { name: 'Site A - Metro Extension', risk: 'High', action: 'Material return pending' },
        { name: 'Site B - Industrial Shed', risk: 'High', action: 'Payment overdue 75d' },
        { name: 'Site C - Highway Package', risk: 'Medium', action: 'Idle stock rising' },
        { name: 'Site D - Township Phase 2', risk: 'Medium', action: 'Margin erosion' },
        { name: 'Site E - Bridge Works', risk: 'Medium', action: 'Labour productivity dip' }
    ];

    $scope.pipeline = [
        { stage: 'Prospecting', value: '₹ 1.8 Cr', weighted: '₹ 0.36 Cr' },
        { stage: 'Qualified', value: '₹ 1.4 Cr', weighted: '₹ 0.70 Cr' },
        { stage: 'Proposal', value: '₹ 1.2 Cr', weighted: '₹ 0.84 Cr' },
        { stage: 'Negotiation', value: '₹ 1.0 Cr', weighted: '₹ 0.80 Cr' }
    ];
    $scope.pipelineTotal = '₹ 5.4 Cr';

    function renderKpiChart() {
        var chartElement = document.getElementById('ceoKpiHorizontalChart');
        if (!chartElement || typeof Chart === 'undefined') {
            return;
        }

        if ($scope.kpiChart) {
            $scope.kpiChart.destroy();
        }

        var labels = $scope.kpiBars.map(function (item) { return item.label; });
        var values = $scope.kpiBars.map(function (item) { return item.value; });
        var displayMap = $scope.kpiBars.map(function (item) { return item.display; });

        $scope.kpiChart = new Chart(chartElement, {
            type: 'horizontalBar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'KPI',
                    data: values,
                    backgroundColor: '#39a3f4',
                    borderColor: '#2e8dd7',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                legend: {
                    display: false
                },
                animation: {
                    duration: 1,
                    onComplete: function () {
                        var chart = this.chart;
                        var ctx = chart.ctx;
                        ctx.save();
                        ctx.fillStyle = '#333';
                        ctx.font = '12px Arial';
                        ctx.textAlign = 'left';
                        ctx.textBaseline = 'middle';

                        var datasetMeta = this.getDatasetMeta(0);
                        datasetMeta.data.forEach(function (bar, index) {
                            var label = displayMap[index] || values[index];
                            ctx.fillText(label, bar._model.x + 8, bar._model.y);
                        });

                        ctx.restore();
                    }
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem) {
                            return displayMap[tooltipItem.index] || tooltipItem.xLabel;
                        }
                    }
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            }
        });
    }

    function renderForecastingChart() {
        var chartElement = document.getElementById('ceoForecastingBarChart');
        if (!chartElement || typeof Chart === 'undefined') {
            return;
        }

        if ($scope.forecastingChart) {
            $scope.forecastingChart.destroy();
        }

        var labels = $scope.forecastingMonths.map(function (item) { return item.month; });
        var revenue = $scope.forecastingMonths.map(function (item) { return Number(item.revenue || 0); });
        var collection = $scope.forecastingMonths.map(function (item) { return Number(item.collection || 0); });
        var material = $scope.forecastingMonths.map(function (item) { return Number(item.material || 0); });

        $scope.forecastingChart = new Chart(chartElement, {
            type: 'horizontalBar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Revenue Forecast',
                        data: revenue,
                        backgroundColor: ['#2e8dd7', '#f39c12', '#7cc2f7'],
                        borderColor: ['#2e8dd7', '#f39c12', '#7cc2f7'],
                        borderWidth: 1
                    },
                    {
                        label: 'Expected Collection',
                        data: collection,
                        backgroundColor: ['#2e8dd7', '#f39c12', '#7cc2f7'],
                        borderColor: ['#2e8dd7', '#f39c12', '#7cc2f7'],
                        borderWidth: 1
                    },
                    {
                        label: 'Material Requirement Forecast',
                        data: material,
                        backgroundColor: ['#2e8dd7', '#f39c12', '#7cc2f7'],
                        borderColor: ['#2e8dd7', '#f39c12', '#7cc2f7'],
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                legend: { display: true },
                animation: {
                    duration: 1,
                    onComplete: function () {
                        var chart = this.chart;
                        var ctx = chart.ctx;
                        ctx.save();
                        ctx.fillStyle = '#333';
                        ctx.font = '11px Arial';
                        ctx.textAlign = 'left';
                        ctx.textBaseline = 'middle';

                        this.data.datasets.forEach(function (ds, dsIndex) {
                            var meta = chart.getDatasetMeta(dsIndex);
                            meta.data.forEach(function (bar, index) {
                                var val = Number(ds.data[index] || 0).toFixed(2);
                                ctx.fillText('Rs ' + val + ' Cr', bar._model.x + 6, bar._model.y);
                            });
                        });

                        ctx.restore();
                    }
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem) {
                            return tooltipItem.datasetLabel + ': Rs ' + Number(tooltipItem.xLabel || 0).toFixed(2) + ' Cr';
                        }
                    }
                },
                scales: {
                    xAxes: [{
                        ticks: { beginAtZero: true }
                    }]
                }
            }
        });
    }


    function buildThreeMonthForecastSeries() {
        var totalRevenue = Number(($scope.forecastingAnalytics[0] && $scope.forecastingAnalytics[0].value) || 0);
        var totalCollection = Number(($scope.forecastingAnalytics[1] && $scope.forecastingAnalytics[1].value) || 0);
        var totalMaterial = Number(($scope.forecastingAnalytics[2] && $scope.forecastingAnalytics[2].value) || 0);

        // Distribution can be replaced with DB month-wise data when available.
        var monthFactors = [0.30, 0.33, 0.37];
        $scope.forecastingMonths = monthFactors.map(function (factor, idx) {
            return {
                month: 'Month ' + (idx + 1),
                revenue: Number((totalRevenue * factor).toFixed(2)),
                collection: Number((totalCollection * factor).toFixed(2)),
                material: Number((totalMaterial * factor).toFixed(2))
            };
        });
    }

    $timeout(renderKpiChart, 0);
    buildThreeMonthForecastSeries();
    $timeout(renderForecastingChart, 0);

    function loadRentalVsContractComparison() {
        var token = authService.getTokenInfo();
        if (!token || !token.FinYearStart || !token.FinYearEnd) return;

        var from = new Date(token.FinYearStart);
        var to = new Date(token.FinYearEnd);

        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('CeoDashboard/RentalVsContractComparison?from=' + from.toISOString() + '&to=' + to.toISOString());

        $http(req).then(function (res) {
            var d = res.data;
            if (!d) return;

            $scope.rentalVsContractAnalytics = [
                { title: 'EBITDA % rental', value: (d.EbitdaRentalPercent || 0).toFixed(1) + '%' },
                { title: 'EBITDA % contract', value: (d.EbitdaContractPercent || 0).toFixed(1) + '%' },
                { title: 'Capital Return Ratio', value: (d.CapitalReturnRatio || 0).toFixed(2) + 'x' }
            ];
        }, function () {
            // keep static values if API/proc isn't available yet
        });
    }

    function loadForecastingAnalytics() {
        var token = authService.getTokenInfo();
        if (!token || !token.FinYearStart || !token.FinYearEnd) return;

        var from = new Date(token.FinYearStart);
        var to = new Date(token.FinYearEnd);

        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('CeoDashboard/ForecastingAnalytics?from=' + from.toISOString() + '&to=' + to.toISOString());

        $http(req).then(function (res) {
            var d = res.data;
            if (!d) return;

            $scope.forecastingAnalytics = [
                { title: 'Next 3 Months Revenue', value: Number(d.Next3MonthsRevenue || 0), display: 'Rs ' + Number(d.Next3MonthsRevenue || 0).toFixed(2) + ' Cr' },
                { title: 'Expected Collection', value: Number(d.ExpectedCollection || 0), display: 'Rs ' + Number(d.ExpectedCollection || 0).toFixed(2) + ' Cr' },
                { title: 'Material Requirement Forecast', value: Number(d.MaterialRequirementForecast || 0), display: 'Rs ' + Number(d.MaterialRequirementForecast || 0).toFixed(2) + ' Cr' }
            ];
            buildThreeMonthForecastSeries();
            $timeout(renderForecastingChart, 0);
        }, function () {
            // keep static values if API/proc isn't available yet
        });
    }

    function loadEarlyWarningAlerts() {
        var token = authService.getTokenInfo();
        if (!token || !token.FinYearStart || !token.FinYearEnd) return;

        var from = new Date(token.FinYearStart);
        var to = new Date(token.FinYearEnd);

        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('CeoDashboard/EarlyWarningAlerts?from=' + from.toISOString() + '&to=' + to.toISOString());

        $http(req).then(function (res) {
            var d = res.data;
            if (!d) return;

            $scope.earlyWarningAlerts = [
                { title: 'Project margin below 20%', value: (d.ProjectMarginBelow20 || 0) + ' projects', numericValue: Number(d.ProjectMarginBelow20 || 0), level: 'danger' },
                { title: 'Customer overdue > 60 days', value: (d.CustomerOverdue60Days || 0) + ' customers', numericValue: Number(d.CustomerOverdue60Days || 0), level: 'danger' },
                { title: 'Idle stock > 20%', value: (d.IdleStockPercent || 0).toFixed(1) + '%', numericValue: Number(d.IdleStockPercent || 0), level: 'warning' },
                { title: 'Labour cost > 35%', value: (d.LabourCostPercent || 0).toFixed(1) + '%', numericValue: Number(d.LabourCostPercent || 0), level: 'warning' },
                { title: 'Damage % above threshold', value: (d.DamageAboveThreshold || 0) + ' sites', numericValue: Number(d.DamageAboveThreshold || 0), level: 'danger' }
            ];
        }, function () {
            // keep static values if API/proc isn't available yet
        });
    }

    function loadTopRiskProjects() {
        var token = authService.getTokenInfo();
        if (!token || !token.FinYearStart || !token.FinYearEnd) return;

        var from = new Date(token.FinYearStart);
        var to = new Date(token.FinYearEnd);

        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('CeoDashboard/TopRiskProjects?from=' + from.toISOString() + '&to=' + to.toISOString());

        $http(req).then(function (res) {
            var data = res.data;
            if (!data || !data.length) return;

            $scope.topRiskProjects = data.map(function (p) {
                return {
                    name: p.Project,
                    risk: p.Risk || 'Medium',
                    action: p.Action || 'Review project risk'
                };
            });
        }, function () {
            // keep static values if API/proc isn't available yet
        });
    }

    function loadSalesPipeline() {
        var token = authService.getTokenInfo();
        if (!token || !token.FinYearStart || !token.FinYearEnd) return;

        var from = new Date(token.FinYearStart);
        var to = new Date(token.FinYearEnd);

        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('CeoDashboard/SalesPipeline?from=' + from.toISOString() + '&to=' + to.toISOString());

        $http(req).then(function (res) {
            var data = res.data;
            if (!data || !data.length) return;

            var total = 0;
            $scope.pipeline = data.map(function (p) {
                var value = Number(p.Value || 0);
                var weighted = Number(p.WeightedValue || 0);
                total += value;
                return {
                    stage: p.Stage,
                    value: 'Rs ' + value.toFixed(2) + ' Cr',
                    weighted: 'Rs ' + weighted.toFixed(2) + ' Cr'
                };
            });
            $scope.pipelineTotal = 'Rs ' + total.toFixed(2) + ' Cr';
        }, function () {
            // keep static values if API/proc isn't available yet
        });
    }

    loadRentalVsContractComparison();
    loadForecastingAnalytics();
    loadEarlyWarningAlerts();
    loadTopRiskProjects();
    loadSalesPipeline();
}]);

