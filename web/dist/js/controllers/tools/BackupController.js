app.controller('BackupController', function ($scope, BackupService, FileSaver) {
    $scope.backups = [];

    $scope.list = function () {
        BackupService.list().then((o) => {
            if (o.data.Code == 200) {
                $scope.backups = o.data.Data;
            }
            else {
                alert(o.data.Message);
            }
        });
    }
    $scope.backupNow = function () {
        BackupService.backupdb().then((o) => {
             
            var data = o.data;
            if (o.data.Code == 200) {

                alert('backup request submitted successfully');
            }
            else {
                alert(data.Message);
            }
            $scope.list();
        });
    }
    $scope.downloadBackup = function (log) {
         
        BackupService.downloadBackup(log).then((o) => {
            debugger
            var blob = new Blob([o.data], { type: 'application/msaccess' });
            FileSaver.saveAs(blob, log.BackupFileName);
        });

    }
    $scope.list();
});