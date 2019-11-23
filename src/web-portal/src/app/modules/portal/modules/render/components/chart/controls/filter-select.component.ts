import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Observable } from 'rxjs';

@Component({
    selector: 'filter-select',
    templateUrl: 'filter-select.component.html'
})

export class FilterSelectComponent implements OnInit {
    @Input()
    filter: ExtendedChartFilter

    @Output()
    changed = new EventEmitter<any>()

    formGroup: FormGroup
    optionsList: Observable<any>
    
    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit() { 
        this.optionsList = this.filter.datasource
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