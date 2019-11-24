import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';

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

    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit() {
    }
}