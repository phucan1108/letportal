import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Observable, of } from 'rxjs';
import StringUtils from 'app/core/utils/string-util';

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
        this.optionsList = of(this.filter.datasource)
    }
}