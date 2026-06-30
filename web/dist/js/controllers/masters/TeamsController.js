app.controller('TeamsController', function ($scope, ModalFactory) {
    $scope.Team = { Name: '', Code: '', TeamId: 0 };
    var employee = new $.Employee();
    $scope.TeamList = [];
    $scope.save = function () {
        var employees = $scope.selectedEmployees;
    
        if (employees == null || employees == undefined) {
            alert('Please select employees for this team');
            return;
        }
        if (employees.length == 0) {
            alert('Please select employees for this team');
            return;
        }
        $scope.Team.Employees = employees.map((o) => { return { EmployeeId: o.EmployeeId } });
        employee.SaveTeam(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.list();
            $('#addEditTeam').modal('hide');
            $scope.Group.ItemGroupId = 0;

        }, $scope.Team);
    }

    $scope.list = function () {
        employee.TeamList(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.TeamList = e.data.Data;
        });
    }
    $scope.list();
    $scope.edit = function (team) {
        $scope.byId(team.TeamId);
    }
    $scope.byId = function (teamId) {
        employee.TeamById(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Team = e.data.Data;
            $scope.selectedEmployees = $scope.Team.Employees;
            if ($scope.selectedEmployees) {
                $.each($scope.Employees, function (index, value) {
                    var emp = $scope.selectedEmployees.find(o => o.EmployeeId == value.EmployeeId);
                    if (emp) {
                        value.Selected = true;
                    }
                });
            }
            $('#addEditTeam').modal('show');
        }, { TeamId: teamId });
    }
    $scope.delete = function (team) {
        var deleteController = function ($scope) {

            $scope.Message = 'Are you sure to delete this team record?';
            $scope.OkButtonClick = function () {

                var pd = new $.Employee();
                pd.DeleteTeam(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    $('#addEditTeam').modal('hide');
                    $scope.list();
                }, { TeamId: team.TeamId });
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        }
        ModalFactory.Confirm(deleteController, $scope, $('body'));



    }


    employee.GetAll(function (e) {
        if (e.data.Code != 200) {
            alert(e.data.Message);
            return;
        }
        $scope.Employees = e.data.Data;

    });

});