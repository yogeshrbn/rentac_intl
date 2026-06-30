app.controller("PackageListController", ['$scope', '$state', '$rootScope', '$http', 'PaymentService',
    '$mdDialog', 'FileSaver', 'Blob', 'AuthenticationService',
    function ($scope, $state, $rootScope, $http, PaymentService, $mdDialog, FileSaver, Blob, authService) {

        var token = authService.getTokenInfo();


        $scope.makePayment = function (amount, packageId, _monthlyYearly) {

            var s = new $.Subscription();
            s.createOrder((o) => {

                if (o.status == 200) {
                    var model = {
                        amount: amount * 100,
                        key: o.data.Data.key,
                        order_id: o.data.Data.order_id,
                        handler: this.updateOrder,
                        customerName: token.FullName
                    };

                    PaymentService.makePayment(model);
                }
            }, { amount: amount, packageId: packageId, monthlyYearly: _monthlyYearly });

            $scope.updateOrder = function (res) {


                var model = {
                    order_id: '',
                    payment_id: '',
                    payment_signature: ''
                };
                if (res.error) {
                    model = {
                        order_id: res.error.metadata.order_id,
                        payment_id: res.error.metadata.payment_id,
                        payment_signature: '',
                        description: res.error.description,
                        reason: res.error.reason,
                        code: res.error.code,
                        source: res.error.source
                    };

                } else {
                    model = {
                        order_id: res.razorpay_order_id,
                        payment_id: res.razorpay_payment_id,
                        payment_signature: res.razorpay_signature
                    };
                }
                s.updateOrder(o => {
                    if (o.status == 200 && model.code) {
                        $state.go('paymentFailed', { payment_id: 0 });
                    } else if (o.status == 200) {
                        if (o.data.Data.lcd) {

                            $rootScope.$emit('licensePurchased', o.data.Data.lcd);
                        }
                        $state.go('paymentConfirmation', { payment_id: 0 });

                    }
                }, model);
            }
        };

        $scope.bills;
        $scope.dueAmount = 0;

        function updatePackageInfo(lcd) {
            authenticationService.setTokenInfo(userInfo);
        }


        $scope.print = function ($event) {

            var s = new $.Subscription();
            s.printbill((o) => {

                if (o.status == 200) {
                    // var data = new Blob([o], { type: 'application/pdf' });
                    FileSaver.saveAs(o.data, 'text.pdf');
                    //$scope.bills = o.data.Data;
                    //$scope.dueAmount = $scope.bills.currentBill.Balance + $scope.bills.prevBalance;
                }
            }, null);
        };
    }]);
app.controller("PackagePaymentConfirmationController", ['$scope', '$state', '$routeParams', '$http', 'PaymentService',
    '$mdDialog', 'FileSaver', 'Blob', function ($scope, $state, $routeParams, $http, PaymentService, $mdDialog, FileSaver, Blob) {



        $scope.makePayment = function (amount, packageId) {

            var s = new $.Subscription();
            s.createOrder((o) => {

                if (o.status == 200) {
                    var model = {
                        amount: amount * 100,
                        key: o.data.Data.key,
                        order_id: o.data.Data.order_id,
                        handler: this.updateOrder
                    };

                    PaymentService.makePayment(model);
                }
            }, { amount: amount, packageId: packageId, monthlyYearly: 'monthly' });

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



        $scope.print = function ($event) {

            var s = new $.Subscription();
            s.printbill((o) => {

                if (o.status == 200) {
                    // var data = new Blob([o], { type: 'application/pdf' });
                    FileSaver.saveAs(o.data, 'text.pdf');
                    //$scope.bills = o.data.Data;
                    //$scope.dueAmount = $scope.bills.currentBill.Balance + $scope.bills.prevBalance;
                }
            }, null);
        };

      

    }]);