import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
 
import { StaticResources } from 'portal/resources/static-resources';

@Component({
    selector: 'let-icon-picker-shared',
    templateUrl: './icon-picker.component.html'
})
export class IconPickerSharedComponent implements OnInit {
    iconFilterOptions: Observable<string[]>;
    _icons = StaticResources.iconsList()

    @Input()
    form: FormGroup

    @Input()
    formControlKey: string

    @Input()
    tooltip: string

    constructor() { }

    ngOnInit(): void {
        this.iconFilterOptions = this.form.get(this.formControlKey).valueChanges.pipe(
            startWith(''),
            map(value => this._filterIcon(value))
        )
    }

    private _filterIcon(choosingIconValue: string): Array<string> {
        const filterValue = choosingIconValue.toLowerCase()

        return this._icons.filter(op => op.toLowerCase().includes(filterValue))
    }

    isInvalid(validatorName: string): boolean {
        return this.form.get(this.formControlKey).hasError(validatorName)
    }
}
