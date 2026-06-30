app.controller('OperationsDashboardController', ['$scope', '$http', 'AuthenticationService', function ($scope, $http, authService) {
    $scope.siteKpisLoading = false;
    $scope.siteKpisError = null;

    function defaultWidgets() {
        return [
            { key: 'active', title: 'Active Sites', value: '—', hint: 'Currently running sites', panelClass: 'ops-widget-active', icon: 'fa-play-circle' },
            { key: 'completed', title: 'Completed Sites', value: '—', hint: 'Work done/picked-up', panelClass: 'ops-widget-completed', icon: 'fa-check-circle' },
            { key: 'delayed', title: 'Delayed Sites', value: '—', hint: 'Installation/Dismantle is delayed', panelClass: 'ops-widget-delayed', icon: 'fa-exclamation-triangle' },
            { key: 'idle', title: 'Idle Sites', value: '—', hint: 'No activity peformed', panelClass: 'ops-widget-idle', icon: 'fa-pause-circle' }
        ];
    }

    function defaultInventoryWidgets() {
        return [
            { key: 'totalAvailableStockMt', title: 'Total Available Stock (MT)', value: '—', hint: '', panelClass: 'ops-widget-inventory', icon: 'fa-cubes' },
            { key: 'onRentMt', title: 'On Rent (MT)', value: '—', hint: '', panelClass: 'ops-widget-inventory', icon: 'fa-truck' },
            { key: 'onContractMt', title: 'On Contract (MT)', value: '—', hint: '', panelClass: 'ops-widget-inventory', icon: 'fa-file-text-o' },
            { key: 'inYardMt', title: 'In Yard (MT)', value: '—', hint: '', panelClass: 'ops-widget-inventory', icon: 'fa-building-o' },
            { key: 'underRepair', title: 'Under Repair', value: '—', hint: '', panelClass: 'ops-widget-inventory', icon: 'fa-medkit' },
            { key: 'scrapPercent', title: 'Scrap %', value: '—', hint: '', panelClass: 'ops-widget-inventory', icon: 'fa-recycle' }
        ];
    }

    function toIsoDate(dt) {
        return dt.getFullYear() + '-' + String(dt.getMonth() + 1).padStart(2, '0') + '-' + String(dt.getDate()).padStart(2, '0');
    }

    function parseDateInput(v) {
        if (!v) return null;
        if (v instanceof Date) return new Date(v.getFullYear(), v.getMonth(), v.getDate());
        if (typeof v === 'string') {
            var m = v.match(/^(\d{4})-(\d{2})-(\d{2})$/);
            if (m) return new Date(parseInt(m[1], 10), parseInt(m[2], 10) - 1, parseInt(m[3], 10));
        }
        var d = new Date(v);
        if (isNaN(d.getTime())) return null;
        return new Date(d.getFullYear(), d.getMonth(), d.getDate());
    }

    function getThisWeekBounds() {
        var now = new Date();
        var day = now.getDay() || 7;
        var monday = new Date(now);
        monday.setHours(0, 0, 0, 0);
        monday.setDate(now.getDate() - day + 1);
        var sunday = new Date(monday);
        sunday.setDate(monday.getDate() + 6);
        return { from: monday, to: sunday };
    }

    function getSelectedRangeBounds() {
        var from = parseDateInput($scope.chartRange.from);
        if (!from) return null;
        $scope.chartRange.from = from;
        var to = new Date(from);
        to.setDate(from.getDate() + 6);
        $scope.chartRange.to = to;
        return { from: from, to: to };
    }

    function getDayKeys(bounds) {
        var list = [];
        var cursor = new Date(bounds.from);
        while (cursor <= bounds.to) {
            list.push(toIsoDate(cursor));
            cursor.setDate(cursor.getDate() + 1);
        }
        return list;
    }

    function toShortLabel(isoDate) {
        var dt = parseDateInput(isoDate);
        return dt.toLocaleDateString('en-GB', { weekday: 'short', day: '2-digit', month: 'short' });
    }

    function applyKpis(d) {
        if (!d) return;
        var map = {
            active: d.ActiveSites != null ? d.ActiveSites : d.activeSites,
            completed: d.CompletedSites != null ? d.CompletedSites : d.completedSites,
            delayed: d.DelayedSites != null ? d.DelayedSites : d.delayedSites,
            idle: d.IdleSites != null ? d.IdleSites : d.idleSites,
            totalAvailableStockMt: d.TotalAvailableStockMt != null ? d.TotalAvailableStockMt : d.totalAvailableStockMt,
            onRentMt: d.OnRentMt != null ? d.OnRentMt : d.onRentMt,
            onContractMt: d.OnContractMt != null ? d.OnContractMt : d.onContractMt,
            inYardMt: d.InYardMt != null ? d.InYardMt : d.inYardMt,
            underRepair: d.UnderRepair != null ? d.UnderRepair : d.underRepair,
            scrapPercent: d.ScrapPercent != null ? d.ScrapPercent : d.scrapPercent
        };
        $scope.siteWidgets.forEach(function (w) {
            var v = map[w.key];
            w.value = (v === undefined || v === null) ? '0' : String(v);
        });
        $scope.inventoryWidgets.forEach(function (w) {
            var v = map[w.key];
            if (v === undefined || v === null) {
                w.value = (w.key === 'scrapPercent') ? '0.00%' : '0';
                return;
            }
            var n = Number(v);
            if (isNaN(n)) {
                w.value = String(v);
                return;
            }
            if (w.key === 'underRepair') {
                w.value = String(Math.round(n));
            } else if (w.key === 'scrapPercent') {
                w.value = n.toFixed(2) + '%';
            } else {
                w.value = n.toFixed(2);
            }
        });
    }

    function renderInstallationChart(rows) {
        var el = document.getElementById('operationsInstallationTeamChart');
        if (!el || typeof Chart === 'undefined') return;
        if ($scope.installationChart) $scope.installationChart.destroy();
        var labels = (rows || []).map(function (r) { return r.TeamName || r.teamName || ('Team ' + (r.TeamId || r.teamId || 0)); });
        var values = (rows || []).map(function (r) { return Number(r.ContractsCount != null ? r.ContractsCount : r.contractsCount || 0); });

        $scope.installationChart = new Chart(el, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{ label: 'Contracts', data: values, backgroundColor: '#5bc0de', borderWidth: 1 }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    yAxes: [{ ticks: { beginAtZero: true, precision: 0 } }],
                    xAxes: [{ barPercentage: 0.7, categoryPercentage: 0.8 }]
                }
            }
        });
    }

    function renderDismantlingChart(rows, bounds) {
        var el = document.getElementById('operationsDismantlingChart');
        if (!el || typeof Chart === 'undefined') return;
        if ($scope.dismantlingChart) $scope.dismantlingChart.destroy();

        var dayKeys = getDayKeys(bounds);
        var labels = dayKeys.map(toShortLabel);
        var points = {};
        (rows || []).forEach(function (r) {
            var day = toIsoDate(new Date(r.ActivityDate || r.activityDate));
            points[day] = Number(r.DismantlingCount != null ? r.DismantlingCount : r.dismantlingCount || 0);
        });

        $scope.dismantlingChart = new Chart(el, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{ label: 'Dismantling', data: dayKeys.map(function (d) { return Number(points[d] || 0); }), backgroundColor: '#f0ad4e', borderWidth: 1 }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    yAxes: [{ ticks: { beginAtZero: true, precision: 0 } }],
                    xAxes: [{ barPercentage: 0.7, categoryPercentage: 0.8 }]
                }
            }
        });
    }

    function loadWeeklyCharts() {
        var bounds = getSelectedRangeBounds();
        if (!bounds) return;
        $scope.weekRangeText = toIsoDate(bounds.from) + ' to ' + toIsoDate(bounds.to);
        var fromIso = toIsoDate(bounds.from);
        var toIso = toIsoDate(bounds.to);
        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });

        $http(apiService.prepareGet('OperationsDashboard/InstallationTeamDaily?from=' + fromIso + '&to=' + toIso)).then(function (res) {
            renderInstallationChart(Array.isArray(res.data) ? res.data : []);
        }, function () {
            renderInstallationChart([]);
        });

        $http(apiService.prepareGet('OperationsDashboard/DailyActivity?from=' + fromIso + '&to=' + toIso)).then(function (res) {
            renderDismantlingChart(Array.isArray(res.data) ? res.data : [], bounds);
        }, function () {
            renderDismantlingChart([], bounds);
        });
    }

    $scope.reloadSiteKpis = function () {
        $scope.siteKpisLoading = true;
        $scope.siteKpisError = null;
        var apiService = new $.ApiCaller({ http: $http, promiseOnly: true });
        var req = apiService.prepareGet('OperationsDashboard/SiteKpis');
        $http(req).then(function (res) {
            applyKpis(res.data);
        }, function () {
            $scope.siteKpisError = 'Could not load site KPIs. Ensure database migration ran (p_operations_siteKpis).';
            $scope.siteWidgets = defaultWidgets();
            $scope.inventoryWidgets = defaultInventoryWidgets();
        }).finally(function () {
            $scope.siteKpisLoading = false;
        });
    };

    $scope.applyWeekRange = function () {
        loadWeeklyCharts();
    };
    $scope.onWeekFromDateChange = function () {
        var from = parseDateInput($scope.chartRange.from);
        if (!from) return;
        var to = new Date(from);
        to.setDate(from.getDate() + 6);
        $scope.chartRange.to = toIsoDate(to);
    };
    $scope.moveWeek = function (delta) {
        var from = parseDateInput($scope.chartRange.from);
        if (!from) return;
        from.setDate(from.getDate() + (7 * delta));
        $scope.chartRange.from = toIsoDate(from);
        $scope.onWeekFromDateChange();
        $scope.applyWeekRange();
    };
    $scope.reloadAll = function () {
        $scope.reloadSiteKpis();
        loadWeeklyCharts();
    };

    $scope.siteWidgets = defaultWidgets();
    $scope.inventoryWidgets = defaultInventoryWidgets();
    var startRange = getThisWeekBounds();
    $scope.chartRange = { from: toIsoDate(startRange.from), to: toIsoDate(startRange.to) };
    $scope.onWeekFromDateChange();
    $scope.weekRangeText = '';

    $scope.reloadAll();
}]);
