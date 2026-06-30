

app.controller('TaskListController', function ($scope, $rootScope) {
     
    var date = new Date();
    $scope.Token = $rootScope.getTokenInfo();
    debugger
    $scope.TaskStatusList = StaicData.TASK_STATUSES;
    $scope.Filter = { StatusId: 0 };

    $scope.Task = {

        Task: '',
        DeilveryDate: convertDate(date),
        AssignedTo: 0,

    };
    $scope.findTasks = function () {
        var taskObj = new $.Task();
        taskObj.List(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Tasks = e.data.Data;
        }, $scope.Filter);
    }
    $scope.findTasks();

    $scope.Save = function () {

        var taskObj = new $.Task();
        var model = cloneObj($scope.Task);
        model.DeliveryDate = formatdate(model.DeliveryDate);
        taskObj.Save(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Task created successfully.');
            $('#modalNewTask').modal('hide');
        }, model);
    }
    function loadUsers() {
        var user = new $.User();
        user.GetAllUsers(function (e) {
            $scope.Users = e.data;
        });
    }
    loadUsers();

    $scope.edit = function (item) {
        $('#modalNewTask').modal('show');
        $scope.Task = cloneObj(item);
        $scope.Task.DeliveryDate = convertDate($scope.Task.DeliveryDate);
    }

    $scope.newTask = function () {
        
        $scope.Task.TaskId = 0;
        $scope.Task.Task = '';
        $scope.Task.AssignedTo = 0;
        $scope.Task.DeilveryDate = convertDate(date);
        $('#modalNewTask').modal('show');
    }

    $scope.reAssign = function () {
        var taskObj = new $.Task();
        var model = cloneObj($scope.Task);
        model.DeliveryDate = formatdate(model.DeliveryDate);
        taskObj.Assign(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Task assigned successfully.');
            $('#modalNewTask').modal('hide');
            $scope.findTasks();
        }, model);
    }
    $scope.complete = function () {
        var taskObj = new $.Task();
        var model = cloneObj($scope.Task);
      
        taskObj.UpdateStatus(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Task completed successfully.');

            $('#modalNewTask').modal('hide');
            $scope.findTasks();
        }, { TaskId: model.TaskId, StatusId: 5 });
    }
    $scope.delete = function () {
        var taskObj = new $.Task();
        var model = cloneObj($scope.Task);

        taskObj.UpdateStatus(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Task deleted successfully.');
            $('#modalNewTask').modal('hide');
            $scope.findTasks();
        }, { TaskId: model.TaskId, StatusId: 4 });
    }
});
