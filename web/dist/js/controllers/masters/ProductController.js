app.controller('ProductController', function ($scope, $location, $stateParams, $state, $http, $timeout, AuthenticationService,
    FileUploadService, ModalFactory, ProductTaxClassicationService, TaxService) {
    var pId = $stateParams.pId == undefined ? 0 : $stateParams.pId;
    var PRODUCT_LIST_FILTER_KEY = 'rentacProductListFilter_v1';

    function persistProductListFilter() {
        if (pId != 0) {
            return;
        }
        try {
            sessionStorage.setItem(PRODUCT_LIST_FILTER_KEY, JSON.stringify({
                Search: angular.copy($scope.Search),
                CurrentPage: $scope.CurrentPage
            }));
        } catch (ex) { }
    }

    function persistProductListWithScrollTarget(productId) {
        if (pId != 0) {
            return;
        }
        try {
            sessionStorage.setItem(PRODUCT_LIST_FILTER_KEY, JSON.stringify({
                Search: angular.copy($scope.Search),
                CurrentPage: $scope.CurrentPage,
                pendingScrollToProductId: productId
            }));
        } catch (ex) { }
    }

    function restoreProductListFilter() {
        try {
            var raw = sessionStorage.getItem(PRODUCT_LIST_FILTER_KEY);
            return raw ? JSON.parse(raw) : null;
        } catch (ex) {
            return null;
        }
    }

    function afterProductListLoaded() {
        if (pId != 0) {
            return;
        }
        var pendingId = null;
        try {
            var st = restoreProductListFilter();
            if (st && st.pendingScrollToProductId) {
                pendingId = st.pendingScrollToProductId;
            }
        } catch (ex) { }
        persistProductListFilter();
        if (pendingId) {
            $timeout(function () {
                var el = document.getElementById('product-row-' + pendingId);
                if (el && typeof el.scrollIntoView === 'function') {
                    el.scrollIntoView({ block: 'center', behavior: 'smooth' });
                }
            }, 150);
        }
    }

    var companyDTO = new $.Company({ CompanyId: 0 });
    var Product = new $.Product({ StoreId: STOREID, ProductId: pId, WeightRate: 0 });
    var ProductRate = new $.ProductRate({ ProductId: pId });
    var objUOM = new $.UOM({ UOMId: 0 });
    var tax = new $.Tax({ ItemValue: pId, ItemId: 1 });
    var loginInfo = AuthenticationService.getTokenInfo();
    $scope.Search = { Name: '' };
    var savedProductFilter = (pId == 0) ? restoreProductListFilter() : null;
    $scope.CurrentPage = 1;
    if (savedProductFilter && savedProductFilter.CurrentPage > 0) {
        $scope.CurrentPage = savedProductFilter.CurrentPage;
    }
    if (savedProductFilter && savedProductFilter.Search) {
        angular.extend($scope.Search, savedProductFilter.Search);
    }
    var accGroup = new $.AccountGroup({});
    //  debugger
    var category = new $.ProductCategory({ StoreId: STOREID });
    // var salts = new $.Salt({});
    FormsValidation.init();

    $scope.Product = Product;
    $scope.Rate = ProductRate;

    $scope.Categories = StaicData.ProductCategories;
    $scope.UOM = StaicData.UOM;
    $scope.TAXES = [];

    function loadTaxCategories() {
        TaxService.getTaxCategories().then(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.TAXES = (e.data.Data || []).map(function (c) {
                return {
                    TaxId: c.TaxCategoryId,
                    TaxName: c.TaxName,
                    CGST: c.CGST,
                    SGST: c.SGST,
                    IGST: c.IGST
                };
            });
        });
    }
    loadTaxCategories();
    //category.GetAll(function (e) {
    //    $scope.Categories = e.data;
    //});
    function getTaxes() {
        tax.GetApplicableTaxes(function (e) {
            $scope.Taxes = e.data;
        });
    }
    function BindList() {
        Product.GetAll(function (e) {
            $scope.Products = e.data;
            afterProductListLoaded();
        });
    }
    function getProductGroups() {
        Product.ListItemGroup(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.GroupList = e.data.Data;
        });
    }
    getProductGroups();
    $scope.onProductPageChange = function () {
        if (pId == 0) {
            persistProductListFilter();
        }
    };
    //objUOM.GetAllUOM(function (e) {
    //    $scope.UOM = e.data;
    //});
   
    function GetRates() {
        ProductRate.GetAllRates(function (e) {

            $scope.Rates = e.data;
        });
    }

    //companyDTO.GetAll(function (e) {
    //    $scope.Companies = e.data;
    //});
    //var salt = new $.Salt({ StoreId: STOREID });
    //salt.GetAll(function (e) {
    //    $scope.Salts = e.data;
    //});
    accGroup.GetAccountsByGroup(function (e) {

        $scope.SundryCreditors = e.data;

    }, { AccountGroupId: Enums.SALE_ACCOUNT });
    accGroup.GetAccountsByGroup(function (e) {

        $scope.SundryDebtors = e.data;

    }, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    $scope.Save = function () {

        var m = $('#form-product').valid();
        if (m) {

            Product.Add($scope.Product, function (e) {
                if (e.data.Code == 200) {
                    showMessage(MessageClass.SAVED);
                    $location.url('/products');
                } else {
                    showMessage(e.data.Message);
                }

            });
        }
    }

    $scope.Delete = function (productId) {

        //$scope.Message = 'Are you sure to delete this BOM?';
        //ModalFactory.Confirm($scope);




        var deleteController = function ($scope) {

            $scope.Message = 'Are you sure to delete this product record?';
            $scope.OkButtonClick = function () {

                var pd = new $.Product();
                pd.DeleteProduct(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    ModalFactory.Dialog.hide();
                    BindList();
                    // $('#previewDialog').modal('hide');
                }, { ProductId: productId });
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        }
        ModalFactory.Confirm(deleteController, $scope, $('body'));
    }

    $scope.RowSelected = function (index) {
    }
    //wathes the chagnes in the UOM and fills the sizes from the database.
    $scope.$watch('Product.UOM', function (newValue, oldValue, scope) {
        loadUOMSizes(newValue);
    });
    //loads the sizes
    function loadUOMSizes(uomId) {
        var uom = new $.UOM({ UOMId: uomId });
        //uom.GetUOMSize(function (e) {
        //    $scope.UOMSize = e.data;
        //});
    }
    $scope.AddRate = function () {

        //this.Rate.ProductId = ProductRate.ProductId;
        //  $scope.Rate.EffectiveDate = $filter('date')($scope.Rate.EffectiveDate, 'MM/dd/yyyy');
        ProductRate.AddRate($scope.Rate, function (e) {
            GetRates();
        });
    }
    $scope.SaveTaxes = function () {
        var data = JSON.stringify($scope.Taxes);
        tax.SaveTax(function (e) {
            showMessage(MessageClass.SAVED);
        }, data);

    }
    $scope.ActivateRate = function (activate, rentId) {
        $scope.Rate.RentRateId = rentId;
        $scope.Rate.Active = activate;
        ProductRate.ActivateRate($scope.Rate, function (e) {
            GetRates();
        });
    }


    //---Sizelist
    $scope.Product.ProductSize = [];
    //  $scope.NewSize = { ProductId: Product.ProductId, Size: '', ProductSizeId: 0 };
    //var sizeObj = { ProductId: Product.ProductId, Size: '', ProductSizeId: 0 };

    function loadProductSizes() {
        Product.GetSizeList(function (e) {
            $scope.Product.ProductSize = e.data;
        });

    }
    $scope.AddSize = function () {

        if ($scope.Product.ProductSize == undefined) {
            $scope.Product.ProductSize = [];
        }
        $scope.Product.ProductSize.push({ ProductId: Product.ProductId, Size: $scope.NewSize, ProductSizeId: 0, OpeningBalance: parseFloat($scope.OpeningBalance), Code: $scope.Code });
        $scope.NewSize = null;
        $scope.OpeningBalance = 0;
    }
    $scope.find = function (page) {
        if (pId != 0) {
            return;
        }
        if (page == null || page === undefined) {
            $scope.CurrentPage = 1;
        } else {
            $scope.CurrentPage = page;
        }
        Product.Find(function (e) {
            if (e.status == 200) {
                $scope.Products = e.data.items;
                afterProductListLoaded();
            }
            else {
                alert(e.message);
            }
        }, $scope.Search);
    }

    $scope.goNewProduct = function ($event) {
        if ($event) {
            $event.preventDefault();
        }
        persistProductListFilter();
        $state.go('addproduct');
    }

    $scope.goEditProduct = function ($event, productId) {
        if ($event) {
            $event.preventDefault();
        }
        persistProductListWithScrollTarget(productId);
        $state.go('addproduct/:pId', { pId: productId });
    }
    $scope.selectedFiles = [];
    if (pId == 0) {
        if (savedProductFilter) {
            $scope.find($scope.CurrentPage > 0 ? $scope.CurrentPage : 1);
        } else {
            BindList();
        }
    }
    else {
        //Fills the UOM data. It fills in case of adding new or editting existing product
        Product.GetInfo(function (e) {

            $scope.Product = e.data;
            if ($scope.Product.Image1) {
                $scope.selectedFiles.push($scope.Product.Image1);
            }
            ProductTaxClassicationService.setTaxes('productsctrl', e.data.TaxClassiFications);
            GetRates();
            getTaxes();
            loadProductSizes();
        });
    }
    $scope.onFileSelected = function (files) {
        $scope.Product.Image1 = null;
        debugger
        if (files) {
            if (files.length > 0) {
                FileUploadService.upload(files[0]).then(function (e) {
                    if (e.data.Code == 200) {
                        $scope.Product.Image1 = e.data.Data;
                    }
                });
            }
        }
    }
    $scope.onFileDeleted = function (file) {
        $scope.Product.Image1 = null;
    }
    $scope.getProductImage = function (item) {

        return FileUploadService.getDocs(item.CompanyId, item.Image1);
    }
    //-------------------

    //--- config
    $scope.Config = {
        productcodegenmode: 'manual'
    };
    var config = new $.Config();
    config.GetByCategory(function (e) {
        var res = e.data;
        if (res && res.Data) {
            var productcodegenmode = res.Data.find(o => o.SubCategory == 'masters' &&
                o.Category == 'general' && o.Key == 'productcodegenmode');
            if (productcodegenmode) {
                $scope.Config.productcodegenmode = productcodegenmode.Value;
            }
        }
    }, 'general');

    $scope.appliedTaxes = ProductTaxClassicationService.getTaxes();

    // Listen for tax updates
    $scope.$on('taxesUpdated', function (event, data) {
         
        if (data.args == 'taxesctrl') {
            $scope.Product.TaxClassiFications = data.taxes;
        }
    });

    //--end config
});
app.controller('ProductTaxClassificationController', function ($scope, $http, ProductTaxClassicationService, TaxService) {

    $scope.productId = 0;   // set from parent scope
    $scope.companyId = 0;   // set from session/login

    $scope.taxNatures = [
        { value: 'Goods', text: 'Goods' },
        { value: 'Service', text: 'Service' }
    ];

    $scope.taxCategories = [];

    TaxService.getCategoryOptions().then(function (options) {
        $scope.taxCategories = options;
    });

    // Master Model
    $scope.taxClassifications = [
        createRow('Sale', 'Goods'),
        createRow('Delivery', 'Goods'),
        createRow('Rental', 'Service'),
        createRow('Contract', 'Service'),
        createRow('Job Work', 'Service'),
        createRow('Purchase', 'Goods')
    ];

    function createRow(transactionType, nature) {
        return {
            transactionType: transactionType,
            nature: nature,
            hsnCode: '',
            sacCode: '',
            taxCategoryId: 0,
            isReverseCharge: false,
            isExempt: false,
            isNilRated: false,
            isZeroRated: false
        };
    }

    // 🔥 Smart GST Logic (HSN/SAC Toggle)
    $scope.onNatureChange = function (row) {
        if (row.nature === 'Goods') {
            row.sacCode = ''; // Clear SAC if Goods
        }
        else if (row.nature === 'Service') {
            row.hsnCode = ''; // Clear HSN if Service
        }
    };

    // Disable logic for UI
    $scope.isHSNDisabled = function (row) {
        return row.taxNature === 'Service';
    };

    $scope.isSACDisabled = function (row) {
        return row.taxNature === 'Goods';
    };

    // 🔒 GST Validation (Audit Safe)
    $scope.validateRows = function () {
        for (var i = 0; i < $scope.taxClassifications.length; i++) {
            var row = $scope.taxClassifications[i];

            if (row.taxNature === 'Goods' && !row.hsnCode) {
                alert(row.supplyType + ' requires HSN Code for Goods.');
                return false;
            }

            if (row.taxNature === 'Service' && !row.sacCode) {
                alert(row.supplyType + ' requires SAC Code for Service.');
                return false;
            }
        }
        return true;
    };

    // Save API (Bulk Save for ERP)
    $scope.saveTaxClassification = function () {

        if (!$scope.validateRows()) {
            return;
        }

        var payload = {
            companyId: $scope.companyId,
            productId: $scope.productId,
            classifications: $scope.taxClassifications
        };

        $http.post('/api/ProductTaxClassification/SaveBulk', payload)
            .then(function (response) {
                alert('Tax classification saved successfully');
            }, function (error) {
                console.error(error);
                alert('Error while saving tax classification');
            });
    };

    $scope.$watch('taxClassifications', function (n, o) {
        ProductTaxClassicationService.setTaxes('taxesctrl', $scope.taxClassifications);
    }, true);

    $scope.$on('taxesUpdated', function (event, data) {
        debugger
        if (data.args == 'productsctrl') {
            var taxes = data.taxes;
            // Update taxClassifications with matching tax data
            if (taxes && Array.isArray(taxes)) {
                $scope.taxClassifications = $scope.taxClassifications.map(function (item) {
                    // Find matching tax by transaction type and nature
                    var matchingTax = taxes.find(function (tax) {
                        return tax.TransactionType === item.transactionType 
                            
                    });

                    if (matchingTax) {
                        // Update with matching tax data
                        return {
                            transactionType: item.transactionType,
                            nature: matchingTax.Nature || item.nature,
                            hsnCode: matchingTax.HsnCode || item.hsnCode,
                            sacCode: matchingTax.SacCode || item.sacCode,
                            taxCategoryId: matchingTax.TaxCategoryId || item.taxCategoryId,
                            isReverseCharge: matchingTax.IsReverseCharge !== undefined ? matchingTax.IsReverseCharge : item.isReverseCharge,
                            isExempt: matchingTax.IsExempt !== undefined ? matchingTax.IsExempt : item.isExempt,
                            isNilRated: matchingTax.IsNilRated !== undefined ? matchingTax.IsNilRated : item.isNilRated,
                            isZeroRated: matchingTax.IsZeroRated !== undefined ? matchingTax.IsZeroRated : item.isZeroRated
                        };
                    }

                    // If no match found, keep original item
                    return item;
                });
            }
        }
    }, true);

});


app.controller('UOMController', function ($scope, $routeParams, $http, $filter) {
    var pId = $routeParams.pId == undefined ? 0 : $routeParams.pId;
    var UOM = new $.UOM({ UOMId: 0 });
    $scope.Size = new $.UOM({ UOMId: 0 });
    BindList();
    function BindList() {
        UOM.GetAllSize(function (e) {
            $scope.Sizes = e.data;
        });
    }
    $scope.Activate = function (active, uomSizeId) {
        UOM.Active = active;
        UOM.UOMSizeId = uomSizeId;
        UOM.ActivateSize(function (e) {
            BindList();
        });

    }
    UOM.GetAllUOM(function (e) {
        $scope.UOM = e.data;
    });
    $scope.Save = function () {

        UOM.Add($scope.Size, function (e) {
            showMessage(MessageClass.SAVED);
        });

    }
});
app.controller('BOMListController', function ($scope, $state, $crypto) {
    $scope.BOM = [];
    $scope.GetList = function () {
        var product = new $.Product();
        product.BOMList(function (e) {

            $scope.BOM = e.data.Data;
        });
    }
    $scope.GetList();
    $scope.editBom = function (item) {
        debugger
        var enc = $crypto.encrypt(item.BomId);

        $state.go('bomedit', { key: enc });
    }
});
app.controller('BOMController', function ($scope, $state, $filter) {
    var item = {
        ProductId: 0,
        Product: '',
        Rate: 0,
        Quantity: 0,
        Amount: 0
    };
    $scope.BOM = { ProductId: 0, Quantity: 1 };
    $scope.bomItem = JSON.parse(JSON.stringify(item))
    $scope.RawMaterial = [];
    //    $scope.RawMaterial.push($scope.bomItem);
    $scope.onBOMItemSelected = function (value) {
        $scope.BOM.ProductId = value.ProductId;
    }
    $scope.onItemSelected = function (value) {

        $scope.bomItem.ProductId = value.ProductId;
        $scope.bomItem.Product = value.Name;
        $scope.bomItem.Rate = 0;
        $scope.bomItem.Unit = value.Unit;
        $scope.bomItem.Quantity = 1;
        $scope.bomItem.Amount = $scope.bomItem.Quantity * $scope.bomItem.Rate;
        $('#txtbomQty').focus();
    }

    $scope.add = function () {
        var item = $scope.bomItem;
        if (item.ProductId == 0 || item.Quantity == 0) {
            alert('Please enter all details');
            return;
        }
        $scope.RawMaterial.push($scope.bomItem);
        $scope.bomItem = JSON.parse(JSON.stringify(item))
        $('#addProductproductSearch').val('');
        $('#addProductproductSearch').focus();
    }
    $scope.delete = function (index) {
        //$scope.$apply(function (index) {
        $scope.RawMaterial.splice(index, 1);
        // });
    }
    $scope.$watch('RawMaterial', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            // Array has changed             
            console.log('Array changed from', oldValue, 'to', newValue);
        }
    }, true); // objectEquality is true


    $scope.save = function () {

        var product = new $.Product();
        var model = cloneObj($scope.BOM);
        model.details = $scope.RawMaterial;
        if (model.ProductId == 0 || model.Quantity <= 0) {
            alert('Please provide BOM details');
            return;
        }
        if (!model.details || (model.details && model.details.length <= 0)) {
            alert('Please provide BOM raw materials');
            return;
        }
        product.SaveBOM(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('BOM saved successfully.');
            $state.go('bom');
        }, model);

    }
});
app.controller('BOMEditController', function ($scope, $stateParams, $state, $crypto, ModalFactory) {

    var bomId = $crypto.decrypt($stateParams.key)

    var item = {
        ProductId: 0,
        Product: '',
        Rate: 0,
        Quantity: 0,
        Amount: 0
    };
    $scope.BOM = { ProductId: 0, Quantity: 1, BomId: bomId };
    $scope.bomItem = JSON.parse(JSON.stringify(item))
    $scope.RawMaterial = [];


    $scope.getBomDetails = function () {
        var product = new $.Product();
        product.BOMDetails(function (e) {

            if (e.data.Code != 200) {
                alert('Could not fetch details' + e.data.Message);
                return;
            }
            $scope.BOM = e.data.Data;
            $scope.RawMaterial = e.data.Data.Details;
        }, { BomId: bomId });
    }
    $scope.getBomDetails();
    $scope.onBOMItemSelected = function (value) {
        $scope.BOM.ProductId = value.ProductId;
    }
    $scope.onItemSelected = function (value) {

        $scope.bomItem.ProductId = value.ProductId;
        $scope.bomItem.Product = value.Name;
        $scope.bomItem.Rate = 0;
        $scope.bomItem.Unit = value.Unit;
        $scope.bomItem.Quantity = 1;
        $scope.bomItem.Amount = $scope.bomItem.Quantity * $scope.bomItem.Rate;
        $('#txtbomQty').focus();
    }

    $scope.add = function () {
        var item = $scope.bomItem;
        if (item.ProductId == 0 || item.Quantity == 0) {
            alert('Please enter all details');
            return;
        }
        $scope.RawMaterial.push($scope.bomItem);
        $scope.bomItem = JSON.parse(JSON.stringify(item))
        $('#addProductproductSearch').val('');
        $('#addProductproductSearch').focus();
    }
    $scope.delete = function (index) {
        //$scope.$apply(function (index) {
        $scope.RawMaterial.splice(index, 1);
        // });
    }
    $scope.OkButtonClick = function () {

    }


    $scope.OkButtonClick = function () {


    }
    $scope.deleteBOM = function () {
        debugger
        $scope.Message = 'Are you sure to delete this BOM?';
        // ModalFactory.Confirm($scope);

        var deleteController = function ($scope) {

            $scope.Message = 'Are you sure to delete this BOM record?';
            $scope.OkButtonClick = function () {

                var product = new $.Product();
                product.DeleteBOM(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    alert('BOM deleted successfully.');
                    $state.go('bom');
                }, { BOMId: $scope.BOM.BomId });
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        }
        ModalFactory.Confirm(deleteController, $scope, $('body'));
    }

    $scope.$watch('RawMaterial', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            // Array has changed             
            console.log('Array changed from', oldValue, 'to', newValue);
        }
    }, true); // objectEquality is true


    $scope.save = function () {

        var product = new $.Product();
        var model = cloneObj($scope.BOM);
        model.details = $scope.RawMaterial;

        if (model.ProductId == 0 || model.Quantity <= 0) {
            alert('Please provide BOM details');
            return;
        }
        if (!model.details || (model.details && model.details.length <= 0)) {
            alert('Please provide BOM raw materials');
            return;
        }

        product.SaveBOM(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('BOM saved successfully.');
            $state.go('bom');
        }, model);

    }
});