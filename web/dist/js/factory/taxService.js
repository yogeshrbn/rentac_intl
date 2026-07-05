/**
* Tax Service - Handles all tax-related API calls
* This service manages CRUD operations for taxes, tax categories,
* and tax calculations
*/
angular.module('medRack')

    .factory('TaxService', ['$http', '$q', function ($http, $q) {
        var baseUrl = API_URL + '/Tax';
        var calculationUrl = API_URL + '/taxcalculation';
        var emptyGuid = '00000000-0000-0000-0000-000000000000';

        function isEmptyGuid(id) {
            return !id || id === emptyGuid;
        }

        function clearTaxCache() {
            cache.taxes = null;
            cache.activeTaxes = null;
            for (var key in cache) {
                if (key.indexOf('active_') === 0) {
                    cache[key] = null;
                }
            }
        }

        // Cache for frequently accessed data
        var cache = {
            taxes: null,
            categories: null,
            activeTaxes: null
        };

        return {
            // ============================================
            // TAX MASTER CRUD OPERATIONS
            // ============================================

            getTaxMasters: function (useCache) {
                if (useCache && cache.taxes) {
                    return $q.resolve({ data: { Code: 200, Data: cache.taxes } });
                }

                return $http.get(baseUrl + '/TaxMasters')
                    .then(function (response) {
                        if (response.data.Code === 200) {
                            cache.taxes = response.data.Data;
                        }
                        return response;
                    })
                    .catch(function (error) {
                        console.error('Error fetching taxes:', error);
                        return $q.reject(error);
                    });
            },

            getTaxMasterById: function (id) {
                return $http.post(baseUrl + '/TaxMasterById', { Id: id });
            },

            saveTaxMaster: function (taxData) {
                return $http.post(baseUrl + '/SaveTaxMaster', taxData)
                    .then(function (response) {
                        clearTaxCache();
                        return response;
                    })
                    .catch(function (error) {
                        console.error('Error saving tax:', error);
                        return $q.reject(error);
                    });
            },

            getTaxes: function (params, useCache) {
                return this.getTaxMasters(useCache);
            },

            getActiveTaxes: function (country) {
                return this.getTaxMasters(true).then(function (response) {
                    if (response.data.Code === 200 && response.data.Data) {
                        var taxes = response.data.Data.filter(function (tax) {
                            return tax.IsActive === true;
                        });
                        if (country) {
                            taxes = taxes.filter(function (tax) {
                                return tax.Country === country;
                            });
                        }
                        response.data.Data = taxes;
                    }
                    return response;
                });
            },

            getTax: function (id) {
                return this.getTaxMasterById(id);
            },

            createTax: function (taxData) {
                taxData.Id = emptyGuid;
                return this.saveTaxMaster(taxData);
            },

            updateTax: function (id, taxData) {
                taxData.Id = id;
                return this.saveTaxMaster(taxData);
            },

            // ============================================
            // TAX CATEGORY CRUD OPERATIONS
            // ============================================

            /**
             * Get all tax categories
             * @param {Object} params - Filter parameters
             * @param {boolean} params.isActive - Filter by active status
             * @param {string} params.search - Search by name or code
             * @param {boolean} params.useCache - Use cached data
             * @returns {Promise} - Array of category objects
             */
            getTaxCategories: function (params, useCache) {
                if (useCache && cache.categories) {
                    return $q.resolve({ data: { Code: 200, Data: cache.categories } });
                }

                return $http.get(baseUrl + '/TaxCategories', { params: params })
                    .then(function (response) {
                        if (response.data.Code === 200) {
                            cache.categories = response.data.Data;
                        }
                        return response;
                    })
                    .catch(function (error) {
                        console.error('Error fetching tax categories:', error);
                        return $q.reject(error);
                    });
            },

            getTaxCategoryById: function (taxCategoryId) {
                return $http.post(baseUrl + '/TaxCategoryById', { TaxCategoryId: taxCategoryId });
            },

            saveTaxCategory: function (categoryData) {
                return $http.post(baseUrl + '/SaveTaxCategory', categoryData)
                    .then(function (response) {
                        cache.categories = null;
                        return response;
                    })
                    .catch(function (error) {
                        console.error('Error saving tax category:', error);
                        return $q.reject(error);
                    });
            },

            deleteTaxCategory: function (taxCategoryId) {
                return $http.post(baseUrl + '/DeleteTaxCategory', { TaxCategoryId: taxCategoryId })
                    .then(function (response) {
                        cache.categories = null;
                        return response;
                    })
                    .catch(function (error) {
                        console.error('Error deleting tax category:', error);
                        return $q.reject(error);
                    });
            },

            getTaxCategory: function (id) {
                return this.getTaxCategoryById(id);
            },

            createTaxCategory: function (categoryData) {
                categoryData.TaxCategoryId = 0;
                return this.saveTaxCategory(categoryData);
            },

            updateTaxCategory: function (id, categoryData) {
                categoryData.TaxCategoryId = id;
                return this.saveTaxCategory(categoryData);
            },

            // ============================================
            // TAX CALCULATION
            // ============================================

            /**
             * Calculate tax for a transaction
             * @param {Object} calculationData - Tax calculation request
             * @param {string} calculationData.itemId - Item ID (optional)
             * @param {string} calculationData.itemType - 'Product' or 'Service'
             * @param {string} calculationData.itemCategory - Item category
             * @param {number} calculationData.amount - Amount to calculate tax on
             * @param {number} calculationData.quantity - Quantity
             * @param {string} calculationData.customerType - 'Business', 'Consumer', etc.
             * @param {string} calculationData.location - Customer location
             * @param {string} calculationData.orderDate - Order date
             * @param {Array} calculationData.selectedTaxIds - Manual tax selection (optional)
             * @returns {Promise} - Tax calculation result
             */
            calculateTax: function (calculationData) {
                return $http.post(calculationUrl + '/calculate', calculationData)
                    .then(function (response) {
                        return response;
                    })
                    .catch(function (error) {
                        console.error('Error calculating tax:', error);
                        return $q.reject(error);
                    });
            },

            // ============================================
            // UTILITY METHODS
            // ============================================

            /**
             * Clear all cached data
             */
            clearCache: function () {
                clearTaxCache();
                cache.categories = null;
            },

            /**
             * Get tax options for dropdowns
             * @param {string} country - Optional country filter
             * @returns {Promise} - Array of {id, name, rate, code} objects
             */
            getTaxOptions: function (country) {
                return this.getActiveTaxes(country)
                    .then(function (response) {
                        if (response.data.Code === 200 && response.data.Data) {
                            return response.data.Data.map(function (tax) {
                                return {
                                    id: tax.Id,
                                    name: tax.Name,
                                    code: tax.Code,
                                    rate: tax.Rate,
                                    label: tax.Name + ' (' + tax.Rate + (tax.RateType === 'Fixed' ? '' : '%') + ')',
                                    isCompound: tax.IsCompound,
                                    isDefault: tax.IsDefault
                                };
                            });
                        }
                        return [];
                    })
                    .catch(function () {
                        return [];
                    });
            },

            /**
             * Get category options for dropdowns
             * @param {boolean} isActive - Filter by active status
             * @returns {Promise} - Array of {id, name, code} objects
             */
            getCategoryOptions: function () {
                return this.getTaxCategories(null, true)
                    .then(function (response) {
                        if (response.data.Code === 200 && response.data.Data) {
                            return response.data.Data.map(function (category) {
                                return {
                                    id: category.TaxCategoryId,
                                    name: category.TaxName,
                                    label: category.TaxName
                                };
                            });
                        }
                        return [];
                    })
                    .catch(function () {
                        return [];
                    });
            },

            /**
             * Get default tax for a country
             * @param {string} country - Country code
             * @returns {Promise} - Default tax object or null
             */
            getDefaultTax: function (country) {
                return this.getActiveTaxes(country)
                    .then(function (response) {
                        if (response.data.Code === 200 && response.data.Data) {
                            var defaultTax = response.data.Data.find(function (tax) {
                                return tax.IsDefault === true;
                            });
                            return defaultTax || null;
                        }
                        return null;
                    })
                    .catch(function () {
                        return null;
                    });
            }
        };
    }]);