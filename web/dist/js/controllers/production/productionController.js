// Manufacturing Controller
app
    .controller('AddProductionController', ['$scope', 'ProductionService', function ($scope, ManufacturingService) {
        // Initialize form data
        $scope.formData = {
            LedgerId: 0,
            productId: null,
            quantity: 1,
            requiredItems: [],
            operations: [],
            plannedStartDate: null,
            actualStartDate: null,
            plannedEndDate: null,
            actualEndDate: null,
            salesOrder: null,
            bom: [],

        };
        $scope.newRequiredItem = { item: null, quantity: 0, bom: [] };
        $scope.newOperation = {};

        //$scope.dropdowns = {
        //    clients: null,
        //    manufacturableItems: [],
        //    salesOrders: [],
        //    boms: []
        //};
        $scope.clients = [];
        $scope.manufacturableItems = [];
        $scope.operations = [];

        //  $scope.itemToManufacture = {item:null,bom:null,quantity:1};
        // Load initial data
        $scope.loadInitialData = function () {
            // Load clients
            ManufacturingService.getClients().then(function (response) {

                $scope.clients = response.data;
            });

            // Load manufacturable items
            ManufacturingService.getManufacturableItems().then(function (response) {
                $scope.manufacturableItems = response.data;
            });

            ManufacturingService.getOperations().then(function (response) {
                $scope.operations = response.data.Data;
            });
            //// Load sales orders
            //ManufacturingService.getSalesOrders().then(function (response) {
            //    $scope.dropdowns.salesOrders = response.data;
            //});
        };

        // When item is selected, load BOM
        $scope.onItemSelected = function (value) {

            if (!value) {

                return;
            }
            $scope.formData.productId = value.originalObject.ProductId;
            //   $scope.itemToManufacture.item = value.originalObject;
            if ($scope.formData.productId) {
                ManufacturingService.getBOM({ productId: $scope.formData.productId }).then(function (response) {
                    var bom = response.data.Data.Details;
                    $scope.formData.bom = angular.copy(bom);
                    // Auto-populate required items from BOM
                    for (var i = 0; i < bom.length; i++) {
                        var newItem = bom[i];
                        var existingIndex = $scope.findItemIndex($scope.formData.requiredItems, newItem.ProductId);
                        if (existingIndex !== -1) {
                            // Item exists - update quantity
                            var currentQty = parseFloat($scope.formData.requiredItems[existingIndex].Quantity);
                            var newQty = currentQty + parseFloat(newItem.Quantity);


                            $scope.formData.requiredItems[existingIndex].Quantity = newQty;

                        } else {
                            // Item doesn't exist - add new
                            $scope.formData.requiredItems.push({
                                ProductId: newItem.ProductId,
                                Product: newItem.Product,
                                Quantity: parseFloat(newItem.Quantity)
                            });
                        }

                    }

                });
            }
        };

        $scope.findItemIndex = function (items, newProductId) {
            if (!items)
                return -1;

            if (!newProductId || !newProductId) return -1;

            for (var i = 0; i < items.length; i++) {
                if (items[i] && items[i].ProductId === newProductId) {
                    return i;
                }
            }
            return -1;
        };

        $scope.onRequiredItemSelected = function (value) {
            $scope.newRequiredItem.item = value.originalObject;
        }
        // Add required item row
        $scope.addRequiredItem = function () {
            if (!$scope.newRequiredItem.item || !$scope.newRequiredItem.Quantity || $scope.newRequiredItem.Quantity <= 0) {
                alert('Please select an item and enter a valid quantity');
                return;
            }

            var existingIndex = $scope.findItemIndex($scope.formData.requiredItems, $scope.newRequiredItem.item.ProductId);

            if (existingIndex !== -1) {
                // Item exists - update quantity
                var currentQty = parseFloat($scope.formData.requiredItems[existingIndex].Quantity);
                var newQty = currentQty + parseFloat($scope.newRequiredItem.Quantity);

                //if (confirm('Item "' + $scope.newRequiredItem.item.Product + '" already exists with quantity ' + currentQty +
                //    '. Update quantity to ' + newQty + '?')) {
                $scope.formData.requiredItems[existingIndex].Quantity = newQty;
                //  }
            } else {
                // Item doesn't exist - add new
                $scope.formData.requiredItems.push({
                    ProductId: $scope.newRequiredItem.item.ProductId,
                    Product: $scope.newRequiredItem.item.Product,
                    Quantity: parseFloat($scope.newRequiredItem.Quantity)
                });
            }

            // Reset new item form
            $scope.newRequiredItem = { item: null, quantity: 0 };
            $('#newRequiredItem_value').focus();
        };



        // Remove required item row
        $scope.removeRequiredItem = function (index) {
            $scope.formData.requiredItems.splice(index, 1);
        };

        // Add operation row
        $scope.addOperation = function () {

            var clone = cloneObj($scope.newOperation)
            var op = $scope.operations.find(o => o.OperationId == clone.OperationId);
            clone.Name = op.Name;
            var isExist = $scope.formData.operations.find(o => o.OperationId == clone.OperationId);
            if (!isExist)
                $scope.formData.operations.push(clone);
        };

        // Remove operation row
        $scope.removeOperation = function (index) {
            $scope.formData.operations.splice(index, 1);
        };

        // Calculate consumed quantity (example logic)
        $scope.calculateConsumedQty = function () {
            // Implement your business logic for consumed quantity calculation
            return $scope.formData.bom.reduce(function (total, item) {
                return total + (item.quantity || 0);
            }, 0);
        };

        // Calculate returned quantity (example logic)
        $scope.calculateReturnedQty = function () {
            // Implement your business logic for returned quantity calculation
            return 0; // Placeholder
        };

        // Save manufacturing order
        $scope.saveWorkOrder = function (_isValid) {
            debugger

            // var _isValid = $('#form-workorder').valid();
            if (!_isValid) {
                alert('Please fix the validation errors before saving.');
                return;
            }
            // Check if at least one required item is added
            if (!$scope.formData.bom || $scope.formData.bom.length === 0) {
                alert('At least one required item must be added');
                return;
            }

            // Validate that all required items have valid quantities
            var invalidItems = $scope.formData.bom.filter(function (item) {
                return !item.Quantity || item.Quantity <= 0;
            });

            if (invalidItems.length > 0) {
                alert('All required items must have a quantity greater than 0');
                return;
            }

            // Additional date validations
            if ($scope.formData.plannedStartDate && $scope.formData.plannedEndDate) {
                var startDate = new Date($scope.formData.plannedStartDate);
                var endDate = new Date($scope.formData.plannedEndDate);

                if (endDate < startDate) {
                    alert('Planned End Date cannot be before Planned Start Date');
                    return;
                }
            }
            var _model = cloneObj($scope.formData);
             
            _model.plannedStartDate = new Date(_model.plannedStartDate);
            if (_model.actualStartDate) {
                _model.actualStartDate = new Date(_model.actualStartDate);
            }
            _model.plannedEndDate = new Date(_model.plannedEndDate);
            if (_model.actualEndDate) {
                _model.actualEndDate = new Date(_model.actualEndDate);
            }

            ManufacturingService.saveManufacturingOrder($scope.formData).then(function (response) {
                // Success handling
                console.log('Order saved successfully', response);
                alert('Manufacturing order saved successfully!');
                $scope.resetForm();
            }).catch(function (error) {
                // Error handling
                console.error('Error saving order', error);
                alert('Error saving manufacturing order!');
            });
        };

        // Cancel/Reset form
        $scope.cancel = function () {
            $scope.resetForm();
        };

        // Reset form to initial state
        $scope.resetForm = function () {
            $scope.formData = {
                LedgerId: 0,
                productId: null,
                quantity: 1,
                requiredItems: [],
                operations: [],
                plannedStartDate: null,
                actualStartDate: null,
                plannedEndDate: null,
                estimatedDeliveryDate: null,
                salesOrder: null,
                bom: []
            };
            $scope.manufacturingForm.$setPristine();
            $scope.manufacturingForm.$setUntouched();
        };

        // Initialize controller
        $scope.loadInitialData();

    }]);