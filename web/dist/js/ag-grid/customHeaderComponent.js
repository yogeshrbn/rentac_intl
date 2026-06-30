class StockRegisterHeaderComponent {
    params;
    eGui;
    eFilterMenu;
    eText;
    eChallans;
    filterMenuPresent;
    onMenuClickListener;

    init(params) {
        this.params = params;
        console.log('CustomHeader.init() -> ' + this.params.column.getId());
        this.eGui = document.createElement('div');
        debugger
        var challans = '';
        for (var i = 0; i < this.params.column.colDef.challans.length; i++) {
            challans = challans + '<span data-ref="eChallans" style="display:block">' + this.params.column.colDef.challans[i] + '</span>'
        }
        console.log(challans);
        this.eGui.innerHTML =

            '<div style="display: flex;flex-direction:column;align-items:center">' +
            '<span data-ref="eFilterMenu" class="ag-icon ag-icon-menu" style="margin-right: 4px;"></span>' +
            challans +
            '<span data-ref="eText" style="display:block"></span>' +

            '</div>';

        this.eFilterMenu = this.eGui.querySelector('[data-ref="eFilterMenu"]');
        this.eText = this.eGui.querySelector('[data-ref="eText"]');
        this.eChallans = this.eGui.querySelector('[data-ref="eChallans"]');
        this.filterMenuPresent = this.params.enableFilterButton;

        if (this.filterMenuPresent) {
            this.onMenuClickListener = this.onMenuClick.bind(this);
            this.eFilterMenu.addEventListener('click', this.onMenuClickListener);
        } else {
            this.eFilterMenu.parentNode.removeChild(this.eFilterMenu);
        }


        this.updateHeaderNameText();
    }

    getGui() {
        return this.eGui;
    }

    refresh(params) {
        this.params = params;

        const res = this.params.enableFilterButton === this.filterMenuPresent;

        console.log('CustomHeader.refresh() -> ' + this.params.column.getId() + ' returning ' + res);

        this.updateHeaderNameText();

        return res;
    }

    updateHeaderNameText() {
        this.eText.textContent = this.params.displayName;
    }

    destroy() {
        console.log('CustomHeader.destroy() -> ' + this.params.column.getId());
        if (this.onMenuClickListener) {
            this.eFilterMenu.removeEventListener('click', this.onMenuClickListener);
        }
    }
}
class CustomButtonComponent {
    eGui;
    eButton;
    eventListener;
  
    init(params) {

        this.eGui = document.createElement('div');
        const eButton = document.createElement('button');
        eButton.className = 'btn btn-sm btn-primary';
        //const company = params.label || 'Actions';
        eButton.textContent = params.label || 'Actions';
        this.ctrParams = params;
        
        this.eventListener = params.onClick;

        eButton.addEventListener('click',
            () => {
                this.eventListener(this.ctrParams);
            }
        );
        if (params.onLoad instanceof Function) {
            params.onLoad(eButton, this.ctrParams);
        }
        this.eGui.appendChild(eButton);
    }

    getGui() {
        return this.eGui;
    }

    refresh() {
        return true;
    }

    destroy() {
        if (this.eButton) {
            this.eButton.removeEventListener('click', this.eventListener);
        }
    }
}