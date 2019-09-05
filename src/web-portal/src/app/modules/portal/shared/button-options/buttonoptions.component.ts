import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ButtonOptions, ConfirmationOptions, ActionCommandOptions, ActionType } from 'services/portal.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActionCommandRenderOptions } from './actioncommandrenderoptions';
import { CommandOptionsComponent } from './commandoptions.component';
import { NGXLogger } from 'ngx-logger';

@Component({
    selector: 'let-buttonoptions',
    templateUrl: './buttonoptions.component.html'
})
export class ButtonOptionsComponent implements OnInit {

    @Input()
    buttonOptions: ButtonOptions

    confirmationOptions: ConfirmationOptions
    confirmationFormGroup: FormGroup

    actionCommandOptions: ActionCommandOptions
    options: ActionCommandRenderOptions = {
        modes: ['database', 'http', 'workflow']
    }

    @ViewChild('actionOptions') actionOptions: CommandOptionsComponent

    constructor(
        private logger: NGXLogger,
        private fb: FormBuilder) { }

    ngOnInit(): void {
        
        this.logger.debug('Input button options', this.buttonOptions)

        if (this.buttonOptions.confirmationOptions) {
            this.confirmationOptions = this.buttonOptions.confirmationOptions
        }
        else {
            this.buttonOptions.confirmationOptions = {
                isEnable: false,
                confirmationText: 'Are you sure to proceed it?'
            }
            this.confirmationOptions = this.buttonOptions.confirmationOptions
        }

        if (this.buttonOptions.actionCommandOptions) {
            this.actionCommandOptions = this.buttonOptions.actionCommandOptions
        }
        else {
            this.buttonOptions.actionCommandOptions = {
                actionType: ActionType.ExecuteDatabase,
                databaseOptions: {
                    databaseConnectionId: '',
                    entityName: '',
                    query: ''
                },
                isEnable: false,
                notificationOptions: {
                    completeMessage: '',
                    failedMessage: ''
                }
            }
            this.actionCommandOptions = this.buttonOptions.actionCommandOptions
        }

        if (this.buttonOptions.routeOptions) {
            
        }
        else {
            this.buttonOptions.routeOptions = {
                isEnable: false,
                routes: []
            }
        }        

        this.initConfirmationFormGroup()
    }

    initConfirmationFormGroup() {
        this.confirmationFormGroup = this.fb.group({
            isEnable: [this.confirmationOptions.isEnable],
            confirmationText: [this.confirmationOptions.confirmationText, [Validators.required, Validators.maxLength(250)]]
        })
    }

    valid(){
        return this.confirmationFormGroup.valid && this.actionOptions.isValid()
    }

    get(): ButtonOptions{
        let confirmationValues = this.confirmationFormGroup.value

        return {
            confirmationOptions: {
                isEnable: confirmationValues.isEnable,
                confirmationText: confirmationValues.confirmationText 
            },
            actionCommandOptions: this.actionOptions.get(),
            routeOptions: this.buttonOptions.routeOptions
        }
    }
}
