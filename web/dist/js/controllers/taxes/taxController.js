app.controller('TaxCategoryListController', ['$scope', '$rootScope', 'TaxService', 'ModalFactory',
    function ($scope, $rootScope, TaxService, ModalFactory) {

        $scope.categories = [];

        $scope.list = function () {
            TaxService.getTaxCategories().then(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.categories = e.data.Data || [];
            });
        };

        $scope.showAddForm = function () {
            ModalFactory.AddEditTaxCategory('AddEditTaxCategoryController', { TaxCategoryId: 0, TaxName: '' });
        };

        $scope.edit = function (item) {
            ModalFactory.AddEditTaxCategory('AddEditTaxCategoryController', angular.copy(item));
        };

        $scope.deleteCategory = function (item) {
            var deleteController = function (confirmScope) {
                confirmScope.Message = 'Are you sure you want to delete this tax category?';
                confirmScope.OkButtonClick = function () {
                    TaxService.deleteTaxCategory(item.TaxCategoryId).then(function (e) {
                        if (e.data.Code != 200) {
                            alert(e.data.Message);
                            return;
                        }
                        ModalFactory.Dialog.hide();
                        $rootScope.$emit('OnTaxCategorySaved');
                    });
                };
                confirmScope.closeDialog = function () {
                    ModalFactory.Dialog.hide();
                };
            };
            ModalFactory.Confirm(deleteController, $scope, $('body'));
        };

        var onSaved = $rootScope.$on('OnTaxCategorySaved', function () {
            $scope.list();
        });

        $scope.$on('$destroy', function () {
            onSaved();
        });

        $scope.list();

        $scope.availableTaxes = [{ Name: '333', Id: 3 }];
        $scope.selectedTaxes = [];
        $scope.fields = { text: 'Name', value: 'Id' };


    }]);

app.controller('AddEditTaxCategoryController', ['$scope', '$mdDialog', '$rootScope', 'TaxService', 'localData',
    function ($scope, $mdDialog, $rootScope, TaxService, localData) {

        $scope.Category = { TaxCategoryId: 0, TaxName: '' };
        $scope.availableTaxes = [];
        $scope.selectedTaxes = [];
        $scope.fields = { text: 'Name', value: 'Id' };

        function buildTaxLabel(tax) {
            var suffix = tax.RateType === 'Fixed' ? '' : '%';
            tax.displayLabel = tax.Name + ' (' + tax.Rate + suffix + ')';
            return tax;
        }

        function applySelectedTaxes(mappings) {
             
            if (!mappings) {
                return;
            }

            $scope.selectedTaxes = mappings.map(function (m) {
                return m.TaxId;
            });
        }

        function loadAvailableTaxes() {
            return TaxService.getTaxMasters().then(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.availableTaxes = (e.data.Data || []).map(buildTaxLabel);
            });
        }

        window.setTimeout(function () {
            FormsValidation.init('form-tax-category');
        }, 100);

        $scope.OkButtonClick = function () {
            save();
        };

        $scope.closeDialog = function () {
            $mdDialog.hide();
        };

        function save() {
            var valid = $('#form-tax-category').valid();
            if (!valid) {
                return;
            }

            var payload = {
                TaxCategoryId: $scope.Category.TaxCategoryId,
                TaxName: $scope.Category.TaxName,
                TaxIds: $scope.selectedTaxes || []
            };

            TaxService.saveTaxCategory(payload).then(function (e) {
                if (e.data.Code == 200) {
                    $rootScope.$emit('OnTaxCategorySaved');
                    $mdDialog.hide();
                } else {
                    alert(e.data.Message);
                }
            });
        }

        loadAvailableTaxes().then(function () {
            if (localData && localData.TaxCategoryId > 0) {
                TaxService.getTaxCategoryById(localData.TaxCategoryId).then(function (e) {
                    if (e.data.Code == 200) {
                        $scope.Category = e.data.Data;
                        applySelectedTaxes(e.data.Data.Mappings);
                    } else {
                        alert(e.data.Message);
                    }
                });
            } else if (localData) {
                $scope.Category = angular.copy(localData);
            }
        });
    }]);

app.controller('TaxListController', ['$scope', '$rootScope', 'TaxService', 'ModalFactory',
    function ($scope, $rootScope, TaxService, ModalFactory) {

        var emptyGuid = '00000000-0000-0000-0000-000000000000';

        $scope.taxes = [];

        function newTaxDefaults() {
            return {
                Id: emptyGuid,
                Name: '',
                Code: '',
                Rate: 0,
                RateType: 'Percentage',
                IsActive: true,
                IsCompound: false,
                IsDefault: false,
                EffectiveFrom: new Date()
            };
        }

        $scope.list = function () {
            TaxService.getTaxMasters().then(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.taxes = e.data.Data || [];
            });
        };

        $scope.showAddForm = function () {
            ModalFactory.AddEditTax('AddEditTaxController', newTaxDefaults());
        };

        $scope.edit = function (item) {
            ModalFactory.AddEditTax('AddEditTaxController', angular.copy(item));
        };

        var onSaved = $rootScope.$on('OnTaxSaved', function () {
            $scope.list();
        });

        $scope.$on('$destroy', function () {
            onSaved();
        });

        $scope.list();
    }]);

app.controller('AddEditTaxController', ['$scope', '$mdDialog', '$rootScope', 'TaxService', 'localData',
    function ($scope, $mdDialog, $rootScope, TaxService, localData) {

        var emptyGuid = '00000000-0000-0000-0000-000000000000';

        function isEmptyGuid(id) {
            return !id || id === emptyGuid;
        }

        function toDateInputValue(value) {
            if (!value) {
                return null;
            }
            return new Date(value);
        }

        function prepareTaxForSave(tax) {
            var model = angular.copy(tax);
            if (model.EffectiveFrom) {
                model.EffectiveFrom = new Date(model.EffectiveFrom).toISOString();
            }
            if (model.EffectiveTo) {
                model.EffectiveTo = new Date(model.EffectiveTo).toISOString();
            } else {
                model.EffectiveTo = null;
            }
            if (isEmptyGuid(model.Id)) {
                model.Id = emptyGuid;
            }
            return model;
        }

        function bindTax(tax) {
            $scope.Tax = tax;
            $scope.Tax.EffectiveFrom = toDateInputValue(tax.EffectiveFrom);
            $scope.Tax.EffectiveTo = toDateInputValue(tax.EffectiveTo);
            $scope.isEdit = !isEmptyGuid(tax.Id);
        }

        $scope.Tax = {
            Id: emptyGuid,
            Name: '',
            Code: '',
            Rate: 0,
            RateType: 'Percentage',
            IsActive: true,
            IsCompound: false,
            IsDefault: false,
            EffectiveFrom: new Date()
        };
        $scope.isEdit = false;

        window.setTimeout(function () {
            FormsValidation.init('form-tax');
        }, 100);

        $scope.OkButtonClick = function () {
            save();
        };

        $scope.closeDialog = function () {
            $mdDialog.hide();
        };

        function save() {
            var valid = $('#form-tax').valid();
            if (!valid) {
                return;
            }

            TaxService.saveTaxMaster(prepareTaxForSave($scope.Tax)).then(function (e) {
                if (e.data.Code == 200) {
                    $rootScope.$emit('OnTaxSaved');
                    $mdDialog.hide();
                } else {
                    alert(e.data.Message);
                }
            });
        }

        if (localData && !isEmptyGuid(localData.Id)) {
            TaxService.getTaxMasterById(localData.Id).then(function (e) {
                if (e.data.Code == 200) {
                    bindTax(e.data.Data);
                } else {
                    alert(e.data.Message);
                }
            });
        } else if (localData) {
            bindTax(angular.copy(localData));
        }
    }]);
