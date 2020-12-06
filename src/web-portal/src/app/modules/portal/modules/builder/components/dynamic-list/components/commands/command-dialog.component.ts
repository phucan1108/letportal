import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NGXLogger } from 'ngx-logger';
import { StaticResources } from 'portal/resources/static-resources';
import { ActionCommandRenderOptions } from 'portal/shared/button-options/actioncommandrenderoptions';
import { CommandOptionsComponent } from 'portal/shared/button-options/commandoptions.component';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { ActionCommandOptions, ActionType, CommandButtonInList, CommandPositionType } from 'services/portal.service';

@Component({
    selector: 'let-command-dialog',
    templateUrl: './command-dialog.component.html'
})
export class CommandModalComponent implements OnInit {
    commandButtonForm: FormGroup;

    iconFilterOptions: Observable<string[]>;

    _icons = StaticResources.iconsList()

    _colors = [
        'primary', 'warn', 'basic', 'accent', 'disabled', 'link'
    ]

    _commandPositions = StaticResources.commandPositionTypes()

    _commandTypes = StaticResources.actionTypes()

    _httpMethods = ['GET', 'POST', 'PUT', 'DELETE']

    commandButton: CommandButtonInList
    isInList = true;

    isEditMode = false;
    isJsonParseValid = true

    actionCommandOptions: ActionCommandOptions
    options: ActionCommandRenderOptions = {
        modes: ['database', 'http', 'workflow', 'redirect']
    }
    isSmallDevice = false
    @ViewChild('actionOptions', { static: true }) actionOptions: CommandOptionsComponent

    constructor(
        public dialogRef: MatDialogRef<CommandModalComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private breakpointObserver: BreakpointObserver,
        private logger: NGXLogger
    ) {
        this.breakpointObserver.observe([
            Breakpoints.HandsetPortrait,
            Breakpoints.HandsetLandscape
        ]).subscribe(result => {
            if (result.matches) {
                this.isSmallDevice = true
                this.logger.debug('Small device on command dialog', this.isSmallDevice)
            }
            else{
                this.isSmallDevice = false
                this.logger.debug('Small device on command dialog', this.isSmallDevice)
            }
        });
    }

    ngOnInit(): void {
        this.commandButton = this.data;
        this.logger.debug('Sent data command button', this.commandButton)
        this.isEditMode = this.commandButton.name ? true : false;
        if (this.isEditMode) {
            this.enableEditMode();
        }
        else{
            this.isInList = this.commandButton.commandPositionType === CommandPositionType.InList;
            this.actionCommandOptions = {
                actionType: ActionType.Redirect,
                redirectOptions: {
                    redirectUrl: '',
                    isSameDomain: true
                }
            }
        }
        this.createCommandButtonForm();
        this.onFormValueChanges();
    }

    private _filterIcon(choosingIconValue: string): Array<string> {
        const filterValue = choosingIconValue.toLowerCase()

        return this._icons.filter(op => op.toLowerCase().includes(filterValue))
    }

    createCommandButtonForm() {
        this.commandButtonForm = this.fb.group({
            name: [this.commandButton.name, Validators.required],
            displayName: [this.commandButton.displayName, Validators.required],
            icon: [this.commandButton.icon, Validators.required],
            color: [this.commandButton.color, Validators.required],
            commandPositionType: [this.commandButton.commandPositionType, Validators.required],
            allowRefreshList: [this.commandButton.allowRefreshList]
        })
    }

    enableEditMode() {
        this.isInList = this.commandButton.commandPositionType === CommandPositionType.InList;
        this.actionCommandOptions = this.commandButton.actionCommandOptions
    }

    onJsonEditorChange($event) {
        try {
            this.commandButtonForm.get('jsonBody').setValue(JSON.stringify($event))
            this.isJsonParseValid = true;
        }
        catch (err) {
            this.isJsonParseValid = false;
        }
    }

    onFormValueChanges() {
        this.commandButtonForm.get('displayName').valueChanges.subscribe(newValue => {
            const commandNameValue = (newValue as string).toLowerCase().replace(/\s/g, '')
            this.commandButtonForm.get('name').setValue(commandNameValue)
            this.cd.markForCheck()
        })

        this.iconFilterOptions = this.commandButtonForm.get('icon').valueChanges.pipe(
            startWith(''),
            map(value => this._filterIcon(value))
        )

        this.commandButtonForm.get('commandPositionType').valueChanges.subscribe(newValue => {
            this.isInList = newValue === CommandPositionType.InList
            this.cd.markForCheck()
        })
    }

    generateCommandButton(): CommandButtonInList {
        if(this.actionOptions.isValid()){
            const formValues = this.commandButtonForm.value
            return {
                id: this.commandButton.id,
                order: this.commandButton.order,
                name: formValues.name,
                displayName: formValues.displayName,
                icon: formValues.icon,
                color: formValues.color,
                commandPositionType: formValues.commandPositionType,
                //allowRefreshList: formValues.allowRefreshList,
                allowRefreshList: true, //New changes: always refresh
                actionCommandOptions: this.actionOptions.get()
            }
        }
    }

    onSubmittingCommand() {
        const actionCommand = this.generateCommandButton()
        this.logger.debug('Current action command button', actionCommand)
        this.dialogRef.close(actionCommand)
    }
}
