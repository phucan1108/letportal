import { Component, OnInit, OnChanges, Input, EventEmitter, Output, HostListener, ElementRef } from '@angular/core';
import { MatFormFieldControl } from '@angular/material/form-field';
import { ControlValueAccessor } from '@angular/forms';
import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { MultipleDataSelection } from 'portal/modules/models/control.extended.model';

@Component({
    selector: 'let-mat-auto-multiple',
    templateUrl: './autocomplete-multiple.component.html',
    styleUrls: ['./autocomplete-multiple.component.scss'],
    providers: [{ provide: MatFormFieldControl, useExisting: AutocompleteMultipleComponent }]
})
export class AutocompleteMultipleComponent implements ControlValueAccessor, OnChanges {

    @Input()
    get dropdownList() {
        return this._dropdownList;
    }

    set dropdownList(val) {
        this._dropdownList = val;
        this.onDropdownSelected();
        this.dropdownListChange.emit(this._dropdownList);
    }

    constructor(private _elementRef: ElementRef<HTMLElement>) { }
    get selectedDpdwnInput() {
        return this._selectedDpdwnInput;
    }

    set selectedDpdwnInput(val) {
        this._selectedDpdwnInput = val;
        this.propagateChange(this._selectedDpdwnInput);
    }

    get empty() {
        return !this._selectedDpdwnInput;
    }

    get shouldLabelFloat() { return this.focused || !this.empty; }

    @Input()
    get placeholder(): string { return this._placeholder; }
    set placeholder(value: string) {
        this._placeholder = value;
    }

    @Input()
    get required(): boolean { return this._required; }
    set required(value: boolean) {
        this._required = coerceBooleanProperty(value);
    }

    @Input()
    get disabled(): boolean { return this._disabled; }
    set disabled(value: boolean) {
        this._disabled = coerceBooleanProperty(value);
    }

    // This counter is preventing double changes on Init
    counterChange = 0;
    focused = false;

    // Custom Params
    @Input('display-key') displayKey = '';
    @Input('info-key') infoKey = '';
    @Input() masterToggle = true;
    @Input() alignInfoRight = false;
    @Input() icon = '';
    @Input() hint = '';

    // Dropdown List 2-way binding
    _dropdownList: Array<MultipleDataSelection>;

    @Output() selectionChanged = new EventEmitter();

    @Output()
    dropdownListChange = new EventEmitter<Array<any>>();

    displayDropdown = false;

    @Input() _selectedDpdwnInput: any = '';
    private _placeholder: string;
    private _required = false;
    private _disabled = false;
    onChange = (_: any) => { };
    @HostListener('document:click', ['$event'])
    clickout(event: any) {
        if (!(this._elementRef && this._elementRef.nativeElement.contains(event.target))) {
            this.displayDropdown = false;
        }
    }

    ngOnChanges() {
        if (!this.dropdownList) {
            throw new TypeError('\'dropdownList\' is Required');
        } else if (!(this.dropdownList instanceof Array)) {
            throw new TypeError('\'dropdownList\' should be an Array of objects');
        } else if (!this.displayKey) {
            throw new TypeError('\'display-key\' is required');
        }
    }

    onDropdownSelected() {
        const selectedVals = this.dropdownList.filter(x => x.selected).map(x => x[this.displayKey]);
        if (selectedVals.length === 1) {
            this.writeValue(selectedVals[0]);
        } else if (selectedVals.length > 1) {
            this.writeValue(`${selectedVals[0]} (+${selectedVals.length - 1} Others)`);
        } else {
            this.writeValue();
        }

        this.counterChange++;
        if (this.counterChange > 2) {
            this.selectionChanged.emit(null);
        }
    }

    isAllselected() {
        return this.dropdownList.length > 0 && this.dropdownList.filter(x => x.selected).length === this.dropdownList.length;
    }

    isAtleastOneSelected() {
        return this.dropdownList.length > 0 &&
            this.dropdownList.filter(x => x.selected).length > 0
            && !this.isAllselected();
    }

    onCheckAllChanged() {
        if (this.isAllselected()) {
            this.dropdownList?.forEach(x => {
                x.selected = false;
            });
        } else {
            this.dropdownList?.forEach(x => {
                x.selected = true;
            });
        }
        this.onDropdownSelected();
    }

    showDropdown() {
        this.displayDropdown = true;
    }

    propagateChange = (_: any) => { };

    writeValue(value?: any) {
        this.selectedDpdwnInput = value;
    }

    registerOnChange(fn: any) {
        this.propagateChange = fn;
    }

    registerOnTouched() { }

    setDisabledState(isDisabled: boolean): void {
        this.disabled = isDisabled;
    }

}
