app.controller('FileUploadController', function ($scope, $timeout) {
    var vm = this;
    
    // Initialize variables
    // $scope.selectedFiles = [];
    vm.uploadedFiles = [];
    vm.uploading = false;
    vm.uploadProgress = 0;
    vm.errorMessage = '';
    vm.successMessage = '';
    vm.isDragging = false;
    vm.allowedTypes = [];
    if (!$scope.selectedFiles)
        $scope.selectedFiles = [];

    // Trigger file input click
    vm.triggerFileInput = function () {
        document.getElementById('fileInputCtl').click();

    };

    // Handle file selection
    $scope.onFileSelect = function (evt) {
         
        vm.errorMessage = '';
        vm.successMessage = '';
        var files = evt.currentTarget.files;
        vm.allowedTypes = [
            'application/pdf',
            'image/jpeg',
            'image/jpg',
            'image/png',
        ];

        if ($scope.allowedTypes) {
            vm.allowedTypes = $scope.allowedTypes;
        }

        if (files && files.length > 0) {
            var fileArray = [];
            var errorMessages = [];

            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var fileName = file.name;

                // Allowed file types: PDF and common image formats


                // Validate file type
                if (vm.allowedTypes.indexOf(file.type) === -1) {
                    errorMessages.push(fileName + ': Only PDF and image files (JPG, PNG, GIF, BMP, WebP, SVG, TIFF) are allowed!');
                    continue;
                }

                // Validate file size (max 10MB)
                if (file.size > 10 * 1024 * 1024) {
                    errorMessages.push(fileName + ': File size must be less than 10MB!');
                    continue;
                }

                fileArray.push(file);
            }

            if (errorMessages.length > 0) {
                vm.errorMessage = errorMessages.join('<br>'); // Use <br> for HTML line breaks if displaying in HTML
            }

            if (fileArray.length > 0) {
                $scope.selectedFiles = fileArray;
                 
                if ($scope.onFileSelected) {
                    $scope.onFileSelected({ files: fileArray });
                }

                if (errorMessages.length === 0) {
                    vm.successMessage = 'File(s) selected successfully!';
                } else {
                    vm.successMessage = 'Valid files were selected. Some files were skipped due to errors.';
                }
            }

            // Clear the file input to allow re-selecting the same file
            evt.currentTarget.value = '';

            // Only apply if needed (check if digest is already in progress)
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        }
    };

    // Handle drop event
    vm.onDrop = function (files, event) {
        $scope.onFileSelect(files);
    };

    // Upload files
    vm.uploadFiles = function () {
        if ($scope.selectedFiles.length === 0) {
            vm.errorMessage = 'Please select at least one PDF file.';
            return;
        }

        vm.uploading = true;
        vm.uploadProgress = 0;
        vm.errorMessage = '';
        vm.successMessage = '';

        // Simulate file upload progress
        var progressInterval = setInterval(function () {
            vm.uploadProgress += 10;
            // $scope.$apply();

            if (vm.uploadProgress >= 100) {
                clearInterval(progressInterval);

                // Simulate API call completion
                $timeout(function () {
                    // Add files to uploaded list
                    angular.forEach($scope.selectedFiles, function (file) {
                        vm.uploadedFiles.push({
                            name: file.name,
                            size: file.size,
                            uploadDate: new Date(),
                            url: URL.createObjectURL(file) // For preview
                        });
                    });

                    vm.successMessage = 'Files uploaded successfully!';
                    $scope.selectedFiles = [];
                    vm.uploading = false;
                    vm.uploadProgress = 0;
                }, 500);
            }
        }, 200);

        // In a real application, you would use $http to upload to a server:
        /*
        var formData = new FormData();
        angular.forEach($scope.selectedFiles, function(file) {
            formData.append('pdfFiles', file);
        });
        
        $http.post('/api/upload-pdf', formData, {
            transformRequest: angular.identity,
            headers: {'Content-Type': undefined},
            uploadEventHandlers: {
                progress: function(e) {
                    if (e.lengthComputable) {
                        vm.uploadProgress = (e.loaded / e.total) * 100;
                        $scope.$apply();
                    }
                }
            }
        }).then(function(response) {
            vm.uploading = false;
            vm.successMessage = 'Files uploaded successfully!';
            $scope.selectedFiles = [];
        }).catch(function(error) {
            vm.uploading = false;
            vm.errorMessage = 'Upload failed: ' + error.data.message;
        });
        */
    };

    // Preview file
    vm.previewFile = function (file) {
        if (file.url) {
            window.open(file.url, '_blank');
        }
    };
    vm.deleteSelectedFile = function (file) {
        if ($scope.onFileDelete)
            $scope.onFileDelete({ file: file });
        var index = $scope.selectedFiles.indexOf(file);
        if (index !== -1) {
            $scope.selectedFiles.splice(index, 1);
            vm.successMessage = 'File deleted successfully!';

            // Clear success message after 3 seconds
            $timeout(function () {
                vm.successMessage = '';
            }, 3000);
        }
    };
    // Delete file
    vm.deleteFile = function (file) {
        if ($scope.onFileDelete)
            $scope.onFileDelete(file);

        var index = vm.uploadedFiles.indexOf(file);
        if (index !== -1) {
            vm.uploadedFiles.splice(index, 1);
            vm.successMessage = 'File deleted successfully!';

            // Clear success message after 3 seconds
            $timeout(function () {
                vm.successMessage = '';
            }, 3000);
        }
    };

    // Clear messages after timeout
    $scope.$watch('vm.errorMessage', function (newVal) {
        if (newVal) {
            $timeout(function () {
                vm.errorMessage = '';
            }, 5000);
        }
    });

    $scope.$watch('vm.successMessage', function (newVal) {
        if (newVal) {
            $timeout(function () {
                vm.successMessage = '';
            }, 5000);
        }
    });
});