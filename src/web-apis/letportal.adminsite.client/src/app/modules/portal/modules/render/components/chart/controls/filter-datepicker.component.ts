import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import StringUtils from 'app/core/utils/string-util';

@Component({
    selector: 'filter-datepicker',
    templateUrl: 'filter-datepicker.component.html'
})

export class FilterDatepickerComponent implements OnInit {
    @Input()
    filter: ExtendedChartFilter

    @Input()
    formGroup: FormGroup

    @Output()
    changed = new EventEmitter<any>()

    controlName = ''
    controlNameMin = ''
    controlNameMax = ''
    isMultiple = false

    minDate: Date
    maxDate: Date
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

        this.isMultiple = this.filter.isMultiple
        if(this.isMultiple){
            this.controlNameMin = this.controlName + '_min'
            this.controlNameMax = this.controlName + '_max'
        }

        this.minDate = this.filter.minDate
        this.maxDate = this.filter.maxDate
    }
}