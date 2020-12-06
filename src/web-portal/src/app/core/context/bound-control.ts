import { FormControl } from '@angular/forms';
import { BehaviorSubject, Observable } from 'rxjs';
import { ControlType } from 'services/portal.service';

export interface BoundControl {
    name: string
    type: ControlType
    value: any
    hide: boolean
    change(newValue: any): void
    reset(): void    
    clear(): void
    getForm(): FormControl
    setComponentRef(componentRef: any): void
    getComponentRef(): any
}

export interface DatasourceBoundControl {
    connect(): Observable<any>
    getDs(): any[]
    bound(data: any[]): void
    refetch: (componentRef: any) => void
}

export class SimpleBoundControl implements BoundControl {
    value: any;
    public hide: boolean
    private componentRef: any
    constructor(
        public name: string,
        public type: ControlType,
        private formControl: FormControl,
        private initValue: any = null,
        private allowYN: boolean = false,
        private allowZero: boolean = false) {
    }
    change(newValue: any): void{
        this.formControl.setValue(newValue)
    }
    getForm(): FormControl {
        return this.formControl
    }
    setComponentRef(componentRef: any): void{
        this.componentRef = componentRef
    }
    getComponentRef(): any{
        return this.componentRef
    }
    reset(): void{
        this.formControl.setValue(this.initValue)
    }
    clear(): void{
        if(this.type === ControlType.Checkbox
            || this.type === ControlType.Slide){
                this.formControl.setValue(this.allowZero ? 0 : this.allowYN ? 'N' : false)
            }
            else{
                this.formControl.setValue(null)
            }
    }
}

export class SelectBoundControl extends SimpleBoundControl implements DatasourceBoundControl {
    private datasource: BehaviorSubject<any> = new BehaviorSubject([])
    constructor(
        public name: string,
        public type: ControlType,
        formControl: FormControl) {
        super(name, type, formControl)
    }
    bound(data: any[]): void{
        this.datasource.next(data)
    }
    connect(): Observable<any>{
        return this.datasource
    }
    getDs(): any[]{
        return this.datasource.getValue()
    }
    refetch: (componentRef: any) => void
}
