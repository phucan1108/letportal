import { Component, OnInit, Input } from '@angular/core';
import { PageControl, ControlType } from 'services/portal.service';
import { StaticResources } from 'portal/resources/static-resources';
 

@Component({
    selector: 'let-standardcontrols-list',
    templateUrl: './standard-controls-list.component.html'
})
export class StandardControlsListComponent implements OnInit {

    @Input()
    controls: PageControl[]

    @Input()
    sectionName: string

    displayedListColumns = ['controlName', 'controlType', 'controlEvents'];

    _controlTypes = StaticResources.controlTypes()

    constructor() { }

    ngOnInit(): void { }

    translateControlType(controlType: ControlType) {
        let controlText = ''
        this._controlTypes?.forEach(control => {
            if (control.value === controlType) {
                controlText = control.name
                return false;
            }
        })

        return controlText;
    }

    getAllControlEvents(control: PageControl){
        switch(control.type){
            default:
                return `${this.sectionName}_${control.name}_change`
        }
    }
}
