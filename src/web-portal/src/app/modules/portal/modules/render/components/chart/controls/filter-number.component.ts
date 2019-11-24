import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Observable } from 'rxjs';

@Component({
    selector: 'filter-number',
    templateUrl: 'filter-number.component.html'
})

export class FilterNumberComponent implements OnInit {
    @Input()
    filter: ExtendedChartFilter
    
    @Input()
    formGroup: FormGroup

    @Output()
    changed = new EventEmitter<any>()

    optionsList: Observable<any>

    constructor(
        private fb: FormBuilder
    ) { }

    ngOnInit() { 
        this.optionsList = this.filter.datasource
    }
}