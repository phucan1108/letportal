import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtendedChartFilter } from 'portal/modules/models/chart.extended.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable, of } from 'rxjs';
import StringUtils from 'app/core/utils/string-util';

@Component({
    selector: 'filter-select',
    templateUrl: 'filter-select.component.html'
})

export class FilterSelectComponent implements OnInit {
    @Input()
    filter: ExtendedChartFilter

    @Input()
    formGroup: FormGroup

    @Output()
    changed = new EventEmitter<any>()
    controlName = ''
    optionsList: Observable<any>

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
        this.optionsList = this.filter.datasource
    }
}