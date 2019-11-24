import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

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

    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit() {
     }
}