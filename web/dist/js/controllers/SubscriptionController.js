app.controller("SubscriptionController", ['$scope', '$rootScope', '$routeParams', '$http', 'PaymentService',
    '$mdDialog', 'FileSaver', 'Blob', function ($scope, $rootScope, $routeParams, $http, PaymentService, $mdDialog, FileSaver, Blob) {
        /*
      
        $scope.data = {
            datasets: [{
                data: [],
                backgroundColor: [

                    'rgba(255, 206, 86, 1)',
                    'rgba(54, 99, 132, 1)',
                    'rgba(255, 105, 86, 1)'
                ],
            }],

            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: [
                '0-100',
                '101-500',
                '501 Above',

            ]
        };
        var ctxCurrentUsageChart = document.getElementById("chartCurrentUsage");//.getContext('2d');
        var myCurrentUsageChart = new Chart(ctxCurrentUsageChart, {
            type: 'pie',
            data: $scope.data,
            options: {
               title: { display: true, text: 'Custom Chart Title' }, 
                legend: { display: true, position: 'left', align: 'end', fullWidth: false },
                config: { circumference: 100 },
                layout: {
                    padding: {
                        left: 0,
                        right: 0,
                        top: 0,
                        bottom: 0
                    }
                }
            }
            //layout: {
            //    padding: {
            //        left: 0, right: 0, top: 0, bottom: 0
            //    }
            //},
            //options: {
            //    scales: {
            //        yAxes: [{
            //            ticks: {
            //                beginAtZero: true
            //            }
            //        }]
            //    }
            //}
        });
        */
        $scope.makePayment = function () {

            var s = new $.Subscription();
            s.createOrder((o) => {

                if (o.status == 200) {
                    var model = {
                        amount: $scope.dueAmount * 100,
                        key: o.data.Data.key,
                        order_id: o.data.Data.order_id,
                        handler: this.updateOrder
                    };

                    PaymentService.makePayment(model);
                }
            }, { amount: $scope.dueAmount });

            $scope.updateOrder = function (res) {

                if (res.code) {
                    alert('payment failed' + res.description);
                    return;
                }
                var model = {
                    order_id: res.razorpay_order_id,
                    payment_id: res.razorpay_payment_id,
                    payment_signature: res.razorpay_signature
                };
                s.updateOrder(o => {

                    if (o.status == 200) {
                        alert('Thank you for making payment');
                    }
                }, model);
            }
        };

        $scope.bills;
        $scope.dueAmount = 0;
        $scope.Filter = { fromDate: '', endDate: '' };
        var date = new Date();
        var token = $rootScope.getTokenInfo();
        $scope.Filter.endDate = convertDate(date);
        if (token) {
            $scope.Filter.fromDate = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = date;//
        }
        $scope.getBills = function () {
            var model = { fromDate: formatdate($scope.Filter.fromDate), endDate: formatdate($scope.Filter.endDate) };
            var s = new $.Subscription();
            s.getBills((o) => {

                if (o.status == 200) {

                    $scope.bills = o.data.Data;
                    //$scope.dueAmount = $scope.bills.prevBalance;
                    //if ($scope.bills.currentBill != null) {
                    //    $scope.dueAmount += $scope.bills.currentBill.Balance
                    //}
                    //$scope.data.datasets[0].data = [];
                    //for (var i = 0; i < $scope.bills.currentBill.Details.length; i++) {
                    //    $scope.data.datasets[0].data.push($scope.bills.currentBill.Details[i].Quantity);
                    //}
                    //myCurrentUsageChart.update();
                }
            }, model);
        }
        $scope.getBills();



        $scope.viewMoreBills = function ($event) {
            $event.preventDefault();
            var div = '<div style="width:50%;height:10%"></div>';

            $(div).load('templ/dialogs/clientinvoicesdialog.html?d=' + new Date().getTime(), function () {
                var html = $(this).html();
                //$scope.Message = message;
                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    /* height: '200px',*/
                    template: html,
                    parent: angular.element(document.body),
                    controller: function ($scope, $mdDialog) {
                        $scope.OkButtonClick = function () {
                            $mdDialog.hide();
                            // okButton($scope.HeaderType);
                        }
                        $scope.closeDialog = function () {

                            //$mdDialog.hide();
                            //if (cancelButton) {
                            //    cancelButton(null);
                            //}
                        }
                    }
                });
            });
        };


        $scope.print = function ($event) {

            var s = new $.Subscription();
            s.printbill((o) => {

                if (o.status == 200) {
                    // var data = new Blob([o], { type: 'application/pdf' });
                    FileSaver.saveAs(o.data, 'text.pdf');
                    //$scope.bills = o.data.Data;
                    //$scope.dueAmount = $scope.bills.currentBill.Balance + $scope.bills.prevBalance;
                }
            }, $event.BillingId);
        };
    }]);