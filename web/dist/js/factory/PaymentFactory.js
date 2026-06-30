angular.module('medRack').factory('PaymentService', function () {
    return {
        makePayment: function (o) {
            var options = {
                "key": o.key, // Enter the Key ID generated from the Dashboard
                "amount": o.amount, // Amount is in currency subunits. Default currency is INR. Hence, 50000 refers to 50000 paise
                "currency": "INR",
                "name": "Rentac", //your business name
                "description": "Rentac Subscription",
                "image": "https://rentac.ind.in/img/256X256_checkoutLogo.png",
                "order_id": o.order_id, //This is a sample Order ID. Pass the `id` obtained in the response of Step 1
                // "callback_url": "https://eneqd3r9zrjok.x.pipedream.net/",
                handler: o.handler,
                "prefill": { //We recommend using the prefill parameter to auto-fill customer's contact information especially their phone number
                    "name": o.customerName,
                    //"email": "gaurav.kumar@example.com",
                    "contact": '0000000000' //Provide the customer's phone number for better conversion rates 
                },
                //"notes": {
                //    "address": "Razorpay Corporate Office"
                //},
                "theme": {
                    "color": "#3399cc"
                }
            };
            var rzp1 = new Razorpay(options);
            rzp1.open();
            rzp1.on('payment.failed', function (response) {
                  
                o.handler.call(null, response);
                //alert(response.error.code);
                //alert(response.error.description);
                //alert(response.error.source);
                //alert(response.error.step);
                //alert(response.error.reason);
                //alert(response.error.metadata.order_id);
                //alert(response.error.metadata.payment_id);
            });
        }


    }
});