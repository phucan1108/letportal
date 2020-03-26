import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import StringUtils from 'app/core/utils/string-util';

@Component({
    selector: 'filter-checkbox',
    templateUrl: 'filter-checkbox.component.html'
})

export class FilterCheckboxComponent implements OnInit {

    @Input()
    filter: ExtendedChartFilter

    @Input()
    formGroup: FormGroup

    @Output()
    changed = new EventEmitter<any>()
    controlName = ''

    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit() {
        if(this.filter.name.indexOf('.') > 0){
            this.controlName = StringUtils.replaceAllOccurences(this.filter.name, '.','')
        }
        else{
            this.controlName = this.filter.name
        }
    }
}