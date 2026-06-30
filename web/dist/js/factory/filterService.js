app.service('FilterService', ['ModalFactory', function (modalService) {
    this.IssueItemFilter = function (o) {
        var template = 'templ/dialogs/filters/issueItem.html';
        modalService.ShowDialog(() => {


        }, template, []);
    }
}
]);