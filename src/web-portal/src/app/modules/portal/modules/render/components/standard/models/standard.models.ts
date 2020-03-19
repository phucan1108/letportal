import { ValidatorFn } from '@angular/forms';
import { CustomValidatorMessage } from 'app/core/models/page.model';

export interface ExtendedFormControlValidators{
    controlName: string,
    validators: ValidatorFn[],
    customErrorMessages: CustomValidatorMessage[]
}