import { FormGroup } from '@angular/forms';
import { SectionContructionType } from 'services/portal.service';
import { BoundControl } from './bound-control';

export interface BoundSection{
    name: string
    type: SectionContructionType   
    hide: boolean
    valid(): boolean
}

export interface FormGroupBoundSection{    
    get(controlName: string): BoundControl 
    getFormGroup(): FormGroup
    reset: () => void
    load(controls: BoundControl[]): void
    loadFormGroup(formGroup: FormGroup): void
    getAll(): BoundControl[]
}

export interface OpenableBoundSection{
    opened: boolean    
    setOpenedSection(openingSection: FormGroupBoundSection): void
    getOpenedSection(): FormGroupBoundSection
    close: () => void
    submit: () => void  
}

export class SimpleBoundSection implements BoundSection{
    hide: boolean;
    constructor(public name: string, public type: SectionContructionType){

    }    
    valid(): boolean {
        return true
    }
}

export class StandardBoundSection implements BoundSection, FormGroupBoundSection{
    public readonly type: SectionContructionType = SectionContructionType.Standard
    public hide: boolean = false 
    constructor(
        public name: string,
        private controls: BoundControl[],
        private formGroup: FormGroup){
    }

    load(controls: BoundControl[]){
        this.controls = controls
    }

    loadFormGroup(formGroup: FormGroup){
        this.formGroup = formGroup
    }

    getAll(){
        return this.controls
    }

    reset: () => void;

    get(controlName: string): BoundControl {
        return this.controls.find(a => a.name === controlName)
    }
    
    getFormGroup(): FormGroup {

        return this.formGroup
    }
    valid(): boolean {
        return this.formGroup.valid
    }
}

export class ArrayBoundSection implements BoundSection, OpenableBoundSection{
    public readonly type: SectionContructionType = SectionContructionType.Array
    public hide: boolean = false   
    public opened: boolean    
    private currentOpen: FormGroupBoundSection 
    constructor(public name: string){
    }    
    close: () => void;
    submit: () => void;
    setOpenedSection(openingSection: FormGroupBoundSection): void {
        this.currentOpen = openingSection
    }
    getOpenedSection(): FormGroupBoundSection {
        return this.currentOpen
    }
    valid(): boolean {
        return true
    }
}

export class TreeBoundSection implements BoundSection, OpenableBoundSection{
    public readonly type: SectionContructionType = SectionContructionType.Tree
    public hide: boolean = false    
    public opened: boolean    
    private currentOpen: FormGroupBoundSection  
    constructor(public name: string){
    }  
    close: () => void;
    submit: () => void;
    valid(): boolean {
        return true
    }
    setOpenedSection(openingSection: FormGroupBoundSection): void {
        this.currentOpen = openingSection
    }
    getOpenedSection(): FormGroupBoundSection {
        return this.currentOpen
    }
}