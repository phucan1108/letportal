import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
    selector: 'filter-datepicker',
    templateUrl: 'filter-datepicker.component.html'
})

export class FilterDatepickerComponent implements OnInit {
    @Input()
    filter: ExtendedChartFilter

    @Output()
    changed = new EventEmitter<any>()

    formGroup: FormGroup

    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit() {
        this.formGroup = this.fb.group({
            minDate: [null],
            maxDate: [null]
        })

        this.formGroup.get('minDate').valueChanges.subscribe(newVal => {
            this.changed.emit({
                filter: this.filter,
                minDate: newVal,
                maxDate: this.formGroup.get('maxDate').value
            })
        })
     }
}