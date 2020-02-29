import { DynamicListClient, StandardComponentClient, PagesClient, PageControlAsyncValidator, AsyncValidatorType, DatabasesClient, ChartsClient } from 'services/portal.service';
import { AsyncValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { Observable, timer, of } from 'rxjs';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { CustomHttpService } from 'services/customhttp.service';
import { PageService } from 'services/page.service';

export class PortalValidators {

    public static chartUniqueName(chartsClient: ChartsClient): AsyncValidatorFn {
        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            return timer(500).pipe(
                switchMap(() => {
                    return chartsClient.checkExist(control.value).pipe(
                        map(
                            exist => {
                                return exist ? { 'uniqueName': true } : null;
                            }
                        )
                    )
                })
            )
        };
    }

    public static dynamicListUniqueName(dynamicListClient: DynamicListClient): AsyncValidatorFn {
        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            return timer(500).pipe(
                switchMap(() => {
                    return dynamicListClient.checkExist(control.value).pipe(
                        map(
                            exist => {
                                return exist ? { 'uniqueName': true } : null;
                            }
                        )
                    )
                })
            )
        };
    }

    public static standardUniqueName(standardClient: StandardComponentClient): AsyncValidatorFn {
        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            return timer(500).pipe(
                switchMap(() => {
                    return standardClient.checkExist(control.value).pipe(
                        map(
                            exist => {
                                return exist ? { 'uniqueName': true } : null;
                            }
                        )
                    )
                })
            )
        };
    }

    public static pageUniqueName(pageClient: PagesClient): AsyncValidatorFn {
        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            return timer(500).pipe(
                switchMap(() => {
                    return pageClient.checkExist(control.value).pipe(
                        map(
                            exist => {
                                return exist ? { 'uniqueName': true } : null;
                            }
                        )
                    )
                })
            )
        };
    }

    public static addAsyncValidator(
            validator: PageControlAsyncValidator, 
            controlBindName: string,
            controlFullName: string, 
            sectionName: string,
            controlName: string,
            defaultValue: any,  
            pageService: PageService, 
            customHttpService: CustomHttpService): AsyncValidatorFn {
        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            if (control.value === defaultValue)
                return of(null)
            switch (validator.asyncValidatorOptions.validatorType) {
                case AsyncValidatorType.DatabaseValidator:
                    return timer(500)
                        .pipe(                            
                            switchMap(() => {
                                let mergingObject = new Object()
                                mergingObject[controlBindName] = control.value
                                const parameters = pageService.retrieveParameters(
                                    validator.asyncValidatorOptions.databaseOptions.query,
                                    mergingObject,
                                    true)                                
                                return pageService.executeAsyncValidator({
                                    sectionName: sectionName,
                                    controlName: controlName,
                                    asyncName: validator.validatorName,
                                    parameters: parameters
                                })
                                    .pipe(
                                        map(response => {
                                            let evaluated = Function('response', 'return ' + validator.asyncValidatorOptions.evaluatedExpression)
                                            const isValid = evaluated(response)
                                            if (isValid) {
                                                pageService.notifyTriggeringEvent(controlFullName + '_' + 'noAsyncError', validator.validatorName)
                                                return null
                                            }
                                            else {
                                                pageService.notifyTriggeringEvent(controlFullName + '_' + 'hasAsyncError', validator.validatorName)
                                                let invalid = new Object()
                                                invalid[validator.validatorName] = true
                                                return invalid
                                            }
                                        })
                                    )
                            })
                        )
                case AsyncValidatorType.HttpValidator:
                    return timer(500)
                        .pipe(
                            switchMap(() => {
                                let mergingObject = new Object()
                                mergingObject[controlBindName] = control.value
                                let url = pageService.translateData(validator.asyncValidatorOptions.httpServiceOptions.httpServiceUrl, mergingObject, true)
                                let body = pageService.translateData(validator.asyncValidatorOptions.httpServiceOptions.jsonBody, mergingObject, true)
                                return customHttpService.performHttp(
                                    url,
                                    validator.asyncValidatorOptions.httpServiceOptions.httpMethod,
                                    body,
                                    validator.asyncValidatorOptions.httpServiceOptions.httpSuccessCode,
                                    validator.asyncValidatorOptions.httpServiceOptions.outputProjection)
                                    .pipe(
                                        map(response => {
                                            let evaluated = Function('response', 'return ' + validator.asyncValidatorOptions.evaluatedExpression)
                                            const isValid = evaluated(response)
                                            if (isValid) {
                                                pageService.notifyTriggeringEvent(controlFullName + '_' + 'noAsyncError', validator.validatorName)
                                                return null
                                            }
                                            else {
                                                pageService.notifyTriggeringEvent(controlFullName + '_' + 'hasAsyncError', validator.validatorName)
                                                let valid = new Object()
                                                valid[validator.validatorName] = true
                                                return valid
                                            }
                                        })
                                    )
                            })
                        )
                default:
                    return of(null)
            }

        };
    }
}