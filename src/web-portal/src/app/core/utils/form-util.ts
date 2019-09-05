import { FormGroup, FormControl, ValidatorFn, AbstractControl } from '@angular/forms';
import * as _ from 'lodash';

export class FormUtil {
    public static triggerFormValidators(formGroup: FormGroup) {
        Object.keys(formGroup.controls).forEach(field => {
            const control = formGroup.get(field);
            if (control instanceof FormControl) {
                control.markAsTouched({ onlySelf: true });
            } else if (control instanceof FormGroup) {
                this.triggerFormValidators(control);
            }
        });
    }

    public static isExist(objs: any[], defaultObj: any): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } | null => {
            const found = _.find(objs, obj => obj == control.value)
            if(found){
                return found != defaultObj ? { 'isExist': { value: control.value }} : null
            }
            else{
                return null
            }
        };
    }
}