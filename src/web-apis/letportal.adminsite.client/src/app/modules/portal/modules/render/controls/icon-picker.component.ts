import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
 
import { ExtendedControlValidator } from 'app/core/models/extended.models';
import { StaticResources } from 'portal/resources/static-resources';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-icon-picker',
    templateUrl: './icon-picker.component.html'
})
export class IconPickerComponent implements OnInit {
    iconFilterOptions: Observable<string[]>;
    _icons = StaticResources.iconsList()

    @Input()
    form: FormGroup

    @Input()
    formControlKey: string

    @Input()
    tooltip: string

    @Input()
    control: PageRenderedControl<DefaultControlOptions>

    @Input()
    validators: Array<ExtendedControlValidator> = []

    isInPage = false
    constructor() { }

    ngOnInit(): void {
        this.iconFilterOptions = this.form.get(this.formControlKey).valueChanges.pipe(
            startWith(''),
            map(value => this._filterIcon(value))
        )

        this.isInPage = ObjectUtils.isNotNull(this.control)
    }

    private _filterIcon(choosingIconValue: string): Array<string> {
        const filterValue = choosingIconValue.toLowerCase()

        return this._icons.filter(op => op.toLowerCase().includes(filterValue))
    }

    isInvalid(validatorName: string): boolean {
        return this.form.get(this.control ? this.control.name : this.formControlKey).hasError(validatorName)
    }

    getErrorMessage(validatorName: string) {
        if(ObjectUtils.isNotNull(this.validators)){
            return this.validators.find(validator => validator.validatorName === validatorName).validatorErrorMessage
        }
        else{
            return ''
        }
    }
}
