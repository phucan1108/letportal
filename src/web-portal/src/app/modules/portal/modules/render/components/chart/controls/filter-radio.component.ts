import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
    selector: 'filter-radio',
    templateUrl: 'filter-radio.component.html'
})

export class FilterRadioComponent implements OnInit {
    @Input()
    filter: ExtendedChartFilter

    @Input()
    formGroup: FormGroup

    @Output()
    changed = new EventEmitter<any>()

    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit() { }
}