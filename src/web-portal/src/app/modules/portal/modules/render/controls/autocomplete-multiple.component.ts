import { Component, OnInit, OnChanges, Input, EventEmitter, Output, HostListener, ElementRef } from '@angular/core';
import { MatFormFieldControl } from '@angular/material';
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

    // This counter is preventing double changes on Init
    counterChange = 0;
    focused = false;
    onChange = (_: any) => { };

    // Custom Params
    @Input('display-key') displayKey: string = '';
    @Input('info-key') infoKey: string = '';
    @Input() masterToggle: boolean = true;
    @Input() alignInfoRight: boolean = false;
    @Input() icon: string = '';
    @Input() hint: string = '';

    // Dropdown List 2-way binding
    _dropdownList: Array<MultipleDataSelection>;

    @Input()
    get dropdownList() {
        return this._dropdownList;
    }

    set dropdownList(val) {
        this._dropdownList = val;
        this.onDropdownSelected();
        this.dropdownListChange.emit(this._dropdownList);
    }

    @Output() selectionChanged = new EventEmitter();

    @Output()
    dropdownListChange = new EventEmitter<Array<any>>();

    displayDropdown = false;
    @HostListener('document:click', ['$event'])
    clickout(event: any) {
        if (!(this._elementRef && this._elementRef.nativeElement.contains(event.target))) {
            this.displayDropdown = false;
        }
    }

    constructor(private _elementRef: ElementRef<HTMLElement>) { }

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
            this.dropdownList.forEach(x => {
                x.selected = false;
            });
        } else {
            this.dropdownList.forEach(x => {
                x.selected = true;
            });
        }
        this.onDropdownSelected();
    }

    showDropdown() {
        this.displayDropdown = true;
    }

    propagateChange = (_: any) => { };

    @Input() _selectedDpdwnInput: any = '';
    get selectedDpdwnInput() {
        return this._selectedDpdwnInput;
    }

    set selectedDpdwnInput(val) {
        this._selectedDpdwnInput = val;
        this.propagateChange(this._selectedDpdwnInput);
    }

    writeValue(value?: any) {
        this.selectedDpdwnInput = value;
    }

    registerOnChange(fn: any) {
        this.propagateChange = fn;
    }

    registerOnTouched() { }

    get empty() {
        return !this._selectedDpdwnInput;
    }

    get shouldLabelFloat() { return this.focused || !this.empty; }

    @Input()
    get placeholder(): string { return this._placeholder; }
    set placeholder(value: string) {
        this._placeholder = value;
    }
    private _placeholder: string;

    @Input()
    get required(): boolean { return this._required; }
    set required(value: boolean) {
        this._required = coerceBooleanProperty(value);
    }
    private _required = false;

    @Input()
    get disabled(): boolean { return this._disabled; }
    set disabled(value: boolean) {
        this._disabled = coerceBooleanProperty(value);
    }
    private _disabled = false;

    setDisabledState(isDisabled: boolean): void {
        this.disabled = isDisabled;
    }

}
