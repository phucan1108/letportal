import { SelectionModel } from '@angular/cdk/collections';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { MessageType, ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { Guid } from 'guid-typescript';
import { NGXLogger } from 'ngx-logger';
import { Constants } from 'portal/resources/constants';
import { StaticResources } from 'portal/resources/static-resources';
import { BehaviorSubject } from 'rxjs';
import { ActionType, CommandButtonInList, CommandPositionType } from 'services/portal.service';
import { CommandModalComponent } from './command-dialog.component';
 

@Component({
    selector: 'let-command-grid',
    templateUrl: './command-grid.component.html'
})
export class CommandGridComponent implements OnInit {

    @Input()
    commandsInList: Array<CommandButtonInList>;

    @Output()
    commandsChanged = new EventEmitter<any>();
    commandsInList$: BehaviorSubject<Array<CommandButtonInList>> = new BehaviorSubject([]);
    private selectingCommand: CommandButtonInList;
    selection = new SelectionModel<CommandButtonInList>(true, []);
    displayedCommandsInListColumns = ['select', 'displayName', 'commandPositionType', 'icon', 'commandType', 'actions'];
    _commandTypes = StaticResources.actionTypes()
    isSmallDevice = false
    constructor(
        private translate: TranslateService,
        private dialog: MatDialog,
        private shortcutUtil: ShortcutUtil,
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
                this.logger.debug('Small device on command grid', this.isSmallDevice)
            }
            else{
                this.isSmallDevice = false
                this.logger.debug('Small device on command grid', this.isSmallDevice)
            }
        });
    }

    ngOnInit(): void {
        this.commandsInList$.next(this.commandsInList)
    }

    isAllSelected(): boolean {
        const numSelected = this.selection.selected.length;
        const numRows = this.commandsInList.length;
        return numSelected === numRows;
    }

    masterToggle() {
        if (this.selection.selected.length === this.commandsInList.length) {
            this.selection.clear();
        } else {
            this.commandsInList?.forEach(row => this.selection.select(row));
        }
    }

    deleteSelectedCommands() {
        const _title = 'Delete Commands'
        const _description = 'Are you sure to delete all selected commands?'
        const _waitDesciption = 'Waiting...'
        const dialogRef = this.shortcutUtil.confirmationDialog(_title, _description, _waitDesciption, MessageType.Delete);
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
                return;
            }
            for (let i = 0; i < this.selection.selected.length; i++) {
                this.commandsInList = this.commandsInList.filter((elem) => {
                    return elem.name !== this.selection.selected[i].name
                })
            }
            this.refreshCommandTable();
        });
    }

    addNewCommand() {
        this.selectingCommand = {
            id: Guid.create().toString(),
            order: this.commandsInList.length,
            name: '',
            icon: 'create',
            displayName: '',
            color: 'primary',
            actionCommandOptions: {
                actionType: ActionType.Redirect,
                dbExecutionChains: {
                    steps: []
                },
                httpServiceOptions: {
                    httpMethod: 'GET',
                    httpServiceUrl: '',
                    httpSuccessCode: '200',
                    outputProjection: '{}',
                    jsonBody: ''
                },
                redirectOptions: {
                    redirectUrl: '',
                    isSameDomain: true
                },
                workflowOptions: {
                    workflowId: ''
                },
                notificationOptions: {
                    completeMessage: Constants.COMPLETE_DEFAULT_MESSAGE,
                    failedMessage: Constants.FAILED_DEFAULT_MESSAGE
                },
            },
            commandPositionType: CommandPositionType.InList,
            allowRefreshList: false
        }
        const dialogRef = this.dialog.open(CommandModalComponent, { data: this.selectingCommand });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.commandsInList.push(result)
                this.refreshCommandTable();
            }
        })
    }

    editCommand(edittingCommand: CommandButtonInList) {
        this.logger.debug('Editing command', edittingCommand)
        const dialogRef = this.dialog.open(CommandModalComponent, { data: edittingCommand });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.logger.debug('responsed edit command', result)
                this.commandsInList = ArrayUtils.updateOneItem(this.commandsInList, result, com => com.id === result.id);
                this.logger.debug('After changed', this.commandsInList)
                this.refreshCommandTable();
            }
        })
    }

    deleteCommand(deletingCommand: CommandButtonInList) {
        this.commandsInList = this.commandsInList.filter((elem) => {
            return elem.name !== deletingCommand.name
        })
        this.refreshCommandTable();
        this.shortcutUtil.toastMessage(this.translate.instant('common.deleteSuccessfully'), ToastType.Success);
    }

    setCommands(commands: CommandButtonInList[]) {
        this.commandsInList = commands
    }

    refreshCommandTable() {
        this.selection.clear();
        this.commandsChanged.emit(this.commandsInList)
        this.commandsInList$.next(this.commandsInList);
        this.cd.markForCheck()
    }

    translateCommadnPositionType(commandPositionType: CommandPositionType) {
        switch (commandPositionType) {
            case CommandPositionType.InList:
                return 'In List'
            case CommandPositionType.OutList:
                return 'Outside'
        }
    }
    translateCommandType(commandType: ActionType) {
        const found = this._commandTypes.find(c => c.value === commandType)
        return found ? found.name : commandType.toString()
    }
}
