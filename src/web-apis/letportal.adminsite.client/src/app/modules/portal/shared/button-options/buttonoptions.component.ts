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

    actionCommandOptions: ActionCommandOptions
    options: ActionCommandRenderOptions = {
        modes: ['database', 'http', 'workflow']
    }

    @ViewChild('actionOptions', { static: true }) actionOptions: CommandOptionsComponent

    constructor(
        private logger: NGXLogger,
        private fb: FormBuilder) { }

    ngOnInit(): void {

        this.logger.debug('Input button options', this.buttonOptions)


        if (this.buttonOptions.actionCommandOptions) {
            this.actionCommandOptions = this.buttonOptions.actionCommandOptions
        }
        else {
            this.buttonOptions.actionCommandOptions = {
                actionType: ActionType.ExecuteDatabase,
                dbExecutionChains: {
                    steps:[]
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
    }

    valid(){
        return this.actionOptions.isValid()
    }

    get(): ButtonOptions{
        return {
            actionCommandOptions: this.actionOptions.get(),
            routeOptions: this.buttonOptions.routeOptions
        }
    }
}
