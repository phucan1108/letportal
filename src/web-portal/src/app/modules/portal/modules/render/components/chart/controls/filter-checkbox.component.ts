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

    @Output()
    changed = new EventEmitter<any>()

    formGroup: FormGroup

    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit() {
        this.formGroup = this.fb.group({
            chartFilter: [false]
        })

        this.formGroup.get('chartFilter').valueChanges.subscribe(newVal => {
            this.changed.emit({
                filter: this.filter,
                value: newVal
            })
        })
    }
}