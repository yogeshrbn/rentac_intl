app.controller('WorkOrderListController', function ($scope, $routeParams, $http, $mdDialog, $rootScope) {
    var worder = new $.WorkOrder({ WorkOrderId: 0 });
    worder.GetAll(function (e) {

        $scope.WorkOrders = e.data;
    });
    $scope.RowSelected = function (index) {

    };
    $rootScope.$on("LoadSites", function (evt, index) {
 
        var arrIndex= $scope.WorkOrders.indexOf($scope.WorkOrders.find(o=>o.WorkOrderId==index));
        $scope.LoadSites(arrIndex,true);
    });
    $scope.CloseSite = function(index, event){
        
        event.preventDefault();
        var siteInfo = this.site;
        var site = new $.Site({SiteId: siteInfo.SiteId});
         
        site.CloseSite(function(e){
            $('#tr' + siteInfo.SiteId).addClass('closedSite');
          //  refreshSites(index,true);
        });
    }
    $scope.CloseSitePayment = function(index){
        var siteInfo = this.site;
        event.preventDefault()
        var site = new $.Site({SiteId: siteInfo.SiteId});
        site.CloseSitePayment(function(e){
            $('#tr' + siteInfo.SiteId).addClass('paymentClosedSite');
        });
    }
    $scope.LoadSites = function (index,refresh) {
        refreshSites(index,refresh);
      
    }
    function refreshSites(index,refresh){
        var indx = index;
        var wOrder = new $.WorkOrder({ WorkOrderId: $scope.WorkOrders[indx].WorkOrderId });
        var workOrderId = wOrder.WorkOrderId;
        var rowToShow = $('tr[id=' + workOrderId + ']');
        var plusToggle = rowToShow.prev().find('i').not('.fa-edit');
        $('.childRows').not(rowToShow).hide();

   

        if ($scope.WorkOrders[indx].Sites.length == 0 || refresh) {
            wOrder.GetSites(function (e) {
                $scope.WorkOrders[indx].Sites = e.data;

            });
        }
        if(refresh!=true){
            plusToggle.toggleClass('fa-plus fa-minus');
            rowToShow.toggle(500);
        }
        $('.fa-minus').not(plusToggle).toggleClass('fa-plus fa-minus')


    }
    $scope.editSite = function (index) {
         
        var siteInfo = this.site;
        var div = '<div style="width:90%;height:70%"></div>';

        $(div).load('templ/editSite.html', function () {
            var html = $(this).html();
            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    siteInfo: siteInfo
                },
                template: html,
                parent: angular.element(document.body),
                controller: 'SiteController'
            });
        });
    }

    $scope.editWorOrder = function(index){
       
        var div = '<div style="width:90%;height:70%"></div>';
        var workOrder = $scope.WorkOrders[index];
        $(div).load('templ/editWorkOrder.html', function () {
            var html = $(this).html();
            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    workOrder: workOrder
                },
                template: html,
                parent: angular.element(document.body),
                controller: editController
            });
           
        });
    }
    function editController($scope, $routeParams, $http,workOrder){
        var client = new $.Client({});
        var company = new $.Company({});
        var wOrder = new $.WorkOrder({});
        wOrder.Number = workOrder.Number;
        wOrder.WorkOrderDate = workOrder.WorkOrderDate;
        wOrder.ClientId = workOrder.ClientId;
        wOrder.CompanyId = workOrder.CompanyId;
        wOrder.ClientAmount = workOrder.ClientAmount;
        wOrder.WorkOrderId = workOrder.WorkOrderId;
        
        wOrder.WorkOrderDate = new Date(workOrder.WorkOrderDate);
        $scope.workOrder = wOrder;
        $scope.closeDialog = function () {
            $mdDialog.hide();
        }
        $scope.updateWorkOrder=function(){
            var fileList = [];
            $scope.workOrder.Update(function(e){
                
                $mdDialog.hide();
            },fileList);
            
            
        }
        $scope.ClientList = [];
        $scope.ClientList.push(client);
        debugger
        client.GetAll(function(e){
            $scope.ClientList = e.data;
        });
        company.GetAll(function (e) {
            $scope.Companies = e.data;
        });
    }

});

app.controller('WorkOrderController', function ($scope, $routeParams, $http, $location) {
    var wId = $routeParams.wId == undefined ? 0 : $routeParams.wId;
    var sId = $routeParams.sId == undefined ? 0 : $routeParams.sId;
    var worder = new $.WorkOrder({ WorkOrderId: wId });
    $scope.WorkOrderId = 0;
    $scope.WorkOrder = worder;
    $scope.WorkOrder.Items = initializeArray();
    $scope.SitePic = '';
    $scope.Doc1 = '';
    $scope.Doc2 = '';

    $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    GetTaxes();
    hideWorkOrderTab();

    $scope.Find = function(){
    
    }

    $scope.SubTotal = function (_total) {

        var subTotal = 0;

        for (var i = 0; i < Object.keys($scope.WorkOrder.Items).length; i++) {
            if ($scope.WorkOrder.Items[i].PurchaseQty != null) {
                subTotal += parseFloat($scope.WorkOrder.Items[i].PurchaseQty) * parseFloat($scope.WorkOrder.Items[i].Rate);
            }
        }
        $scope.WorkOrder.SubTotal = $scope.WorkOrder.SiteInfo.SubTotal = subTotal;
        $scope.WorkOrder.SubTotal1 =  $scope.WorkOrder.SubTotal+parseFloat($scope.WorkOrder.SiteInfo.Freight);
        var taxAmount = 0;
        if (_total == 1) {
            console.log($scope.WorkOrder.Taxes.length);
            if ($scope.WorkOrder.Taxes != null) {
                for (var i = 0; i < $scope.WorkOrder.Taxes.length; i++) {
                    if ($scope.WorkOrder.Taxes[i].Applicable) {
                        taxAmount += (  $scope.WorkOrder.SubTotal1 )* $scope.WorkOrder.Taxes[i].Rate / 100.00;
                    }
                }
            }
            $scope.WorkOrder.Total = $scope.WorkOrder.SiteInfo.Total =  $scope.WorkOrder.SubTotal1 + taxAmount ;
            return $scope.WorkOrder.Total;
        }
        else {
            return subTotal;
        }


    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {

            /// this.$parent.item.Props.Item.Props.MRP = selected.originalObject.MRP;
            var Product = this.$parent.item.Item = selected.originalObject;//this.$parent.item.Props.Item.Props;
            $scope.WorkOrder.Items[selectedWorkOrderItemIndex].Size = parseFloat(Product.Size);
            $scope.WorkOrder.Items[selectedWorkOrderItemIndex].Rate = parseFloat(Product.Rate);
        }
    };

    $scope.ClientSelect = function (obj) {
        if (obj != undefined) {
            $scope.WorkOrder.ClientId = obj.originalObject.ClientId;
        }
    };
    $scope.CompanySelection = function (obj) {
        if (obj != undefined) {
            $scope.WorkOrder.CompanyId = obj.originalObject.CompanyId;
        }
    };

    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        console.log(selected);
    };

    $scope.RowSelected = function (index) {

        if ($scope.WorkOrder.Items[index] != undefined) {
            //  $scope.$digest(function () {
            selectedWorkOrderItemIndex = index;
            //  $scope.Product = $scope.WorkOrder.Items[index].Item.Props;
            // });
        }
        LoadSites(index, 0);

    };

    $scope.GetSelected = function () {
        var v = $scope[selectedProject];
    };

    function GetTaxes() {
        var Tax = new $.Tax({ ItemId: 0 });

        Tax.GetTaxes($scope.WorkOrder, function (e) {

            $scope.WorkOrder.Taxes = e.data;
        });
    }

    function initializeArray() {
        var WorkOrders = [];
        for (var i = 0; i <= 20; i++) {
            WorkOrders.push(new $.WorkOrderItem({}));
        }
        return WorkOrders;
    }

    $scope.Save = function () {



        var res = $scope.WorkOrder.Items.filter(function (v) {
            if (v.Item != undefined) {
                return v.Item.ProductId > 0;
            }
            else
                return false;
        });


        var m = $('#form-workorder').valid();
        var site = $('#form-site').valid();

        if (site && sId == 0) {
            if ($scope.WorkOrder.SiteInfo.JobNumber == null || $scope.WorkOrder.SiteInfo.JobNumber == '') {
                alert('Job number is required');
                return;
            }
        }
        if (m && site) {
            EnableToolbar(0);
            //   var reader = new FileReader();

            var fileList = [];
            fileList = addFileToList(fileList, 'fileSitePic');
            fileList = addFileToList(fileList, 'Doc1');
            fileList = addFileToList(fileList, 'Doc2');
            fileList = addFileToList(fileList, 'Doc3');

            if (res.length < 1) {
                alert('Work order can not be empty.');
                return;
            }
            debugger
            addWorkOrder(fileList);


        }
    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    function addWorkOrder(fileList) {

        $scope.WorkOrder.Add(function (e) {
            if (e.statusText == 'OK') {
                $location.path('/woInfo').search({ wId: e.data.WorkOrderId });
                $scope.$apply();
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, fileList);
    }

    function hideWorkOrderTab() {
        $scope.HideWorkOrder = 0;
        if (wId > 0) {


            $('#tbWorkOrder').addClass('disabled');
            //$('#tbWorkOrder').next().addClass('active');
            $('#tbWorkOrder a').removeAttr('data-toggle');

            $('a[href="#siteInfo"]').tab('show');
        }
    }

   


});

app.controller('WorkOrderInfoController', function ($scope, $routeParams, $http, $location, $window, $mdDialog) {
    var wId = $routeParams.wId == undefined ? 0 : $routeParams.wId;
    var worder = new $.WorkOrder({ WorkOrderId: wId });
    var site = new $.Site({ SiteId: 0 });
    worder.GetDetail(function (e) {
        $scope.WorkOrder = e.data;
    });
   
    worder.GetItems(function (e) {
        $scope.Items = e.data;
    });
    worder.GetSites(function (e) {

        $scope.Sites = e.data;
       
    });

    $scope.AddSite = function () {
        $location.path('/workorder').search({ wId: wId });
        $scope.$apply();
    };
    $scope.AddSiteItems = function (sId) {
        $location.path('/workorder').search({ wId: wId, sId: sId });
        $scope.$apply();
    };
    $scope.RowSelected = function (index) {

    };
    $scope.LoadItems = function (index, evt) {
        site.SiteId = $scope.Sites[index].SiteId;
        
        var e = evt || window.event;

        if ($scope.Sites[index].Items == undefined || $scope.Sites[index].Items == null) {
            refreshPayments(site.SiteId);
            site.GetItems(function (e) {

                $scope.Sites[index].Items = e.data;
                //load the taxes
                loadTaxes(index);
                loadGRN(index);
            });

        }
        toggleIcon(e);
    };

    function loadTaxes(siteIndex) {
        site.SiteId = $scope.Sites[siteIndex].SiteId;
        site.GetTaxes(function (e) {

            $scope.Sites[siteIndex].Taxes = e.data;
        });
    }

    $scope.filterDates = function (item) {
        var isNew = indexedDates.indexOf(item.StartDate) == -1;
        if (isNew) {
            indexedDates.push(event.date);
        }
        return isNew;
    }
    function toggleIcon(evt) {

        var obj = evt.target;
        if ($(obj).hasClass('panel-title')) {
            obj = $(evt.target).parent();
        }

        $(obj).toggleClass('fa-plus fa-minus');

    }

    $scope.NewEntry=function(siteId){
         
        $scope.Pay = new $.Journal({SiteId:siteId});
     
        var div = '<div style="width:70%;height:60%"></div>';
        //var workOrder = $scope.WorkOrders[index];
        $(div).load('templ/newPayEntry.html', function () {
            var html = $(this).html();
            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                //locals: {
                //    workOrder: workOrder
                //},
                template: html,
                parent: angular.element(document.body)
                //   controller: editController
            });
           
        });
        $scope.closeDialog = function () {
            $mdDialog.hide();
        }
        $scope.createEntry=function(){
             
            $scope.Pay.EntryDate = new Date($scope.Pay.EntryDate).toLocaleDateString();
            $scope.Pay.CreateEntry(function(e){
                $mdDialog.hide();
                refreshPayments($scope.Pay.SiteId);
            });
            
            
        }
    }

    function refreshPayments(siteId){
        $scope.Pay = new $.Journal({SiteId:siteId});
        var _siteIndex =  $scope.Sites.indexOf($scope.Sites.find(o=>o.SiteId==siteId));
        $scope.Pay.GetJournals(function(e){
            debugger
            $scope.Sites[_siteIndex].Payments = e.data;
        });
    }
    function loadGRN(index) {
        site.JobNumber =   $scope.Sites[index].JobNumber
        
        site.GetSiteGRN(function(e){
             
            $scope.Sites[index].GRNItems=e.data;
        });
    }
});
app.controller('SettingsController', function ($scope, $routeParams, $http) {
    var sId = $routeParams.sId == undefined ? 0 : $routeParams.sId;
    var site= new $.Site({SiteId:sId});
    var payReminder = new $.PayReminder({SiteId: sId});
    $scope.payReminder  = payReminder;
    site.GetSiteInfo(function(e){
        debugger
        $scope.Site = e.data;
    });
    $scope.Add = function(){
         
        payReminder.Add(function(e){
            refresh();
        });
    }
    refresh();
    function refresh(){
        payReminder.GetAll(function(e){
            debugger
            $scope.Reminders = e.data;
        });
    }
    $scope.Delete= function(reminderId){
        // alert(reminderId);
        payReminder.PayReminderId = reminderId;
        payReminder.Delete(function(e){
            refresh();
        });}
});
app.controller('SiteController', function ($scope, $http, $mdDialog, siteInfo, $rootScope) {
    debugger
    //var site = siteInfo;
    $scope.SiteInfo = new $.Site({
        SiteId: siteInfo.SiteId, JobNumber: siteInfo.JobNumber, ChallanNumber: siteInfo.ChallanNumber
          , SiteEng: siteInfo.SiteEng, StartDate: siteInfo.StartDate, Duration: siteInfo.Duration,
        ShaftHeight: siteInfo.ShaftHeight, ShaftSize: siteInfo.ShaftSize, Site: siteInfo.Site
    });

    //    $scope.SiteInfo = site;
    $scope.SiteInfo.StartDate = new Date(siteInfo.StartDate);
    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
    $scope.update = function () {
        debugger
        $scope.SiteInfo.StartDate = new Date($scope.SiteInfo.StartDate).toLocaleDateString();
        $scope.SiteInfo.Update(function (e) {
            debugger    
            $mdDialog.hide();
            $rootScope.$emit("LoadSites", siteInfo.WorkOrderId);
        });
    }
});