import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { PageButton, ActionType, Route } from 'services/portal.service';
import { MatTable, MatDialog } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
import * as _ from 'lodash';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { ArrayUtils } from 'app/core/utils/array-util';
import { Guid } from 'guid-typescript';
import { PageButtonDialogComponent } from './page-button-dialog.component';
import { Store, Actions } from '@ngxs/store';
import { filter, tap } from 'rxjs/operators';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { InitEditPageBuilderAction, GeneratePageActionCommandsAction, NextToWorkflowAction, UpdatePageActionCommandsAction, UpdateAvailableEvents, GatherAllChanges, NextToDatasourceAction } from 'stores/pages/pagebuilder.actions';
import { ObjectUtils } from 'app/core/utils/object-util';
import { PageButtonRouteDialogComponent } from './page-button-route.component';
import { PageButtonOptionsDialogComponent } from './page-button-options.component';
import { NGXLogger } from 'ngx-logger';

@Component({
    selector: 'let-page-button-grid',
    templateUrl: './page-button-grid.component.html'
})
export class PageButtonGridComponent implements OnInit {
    @ViewChild('table', { static: true }) table: MatTable<PageButton>;
    currentActionCommands: Array<PageButton> = []

    selection = new SelectionModel<PageButton>(true, []);
    displayedListColumns = ['name', 'confirmation', 'action', 'actions'];

    constructor(
        private shortcutUtil: ShortcutUtil,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
        private store: Store,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.store.select(state => state.pagebuilder)
            .pipe(
                filter(result => result.filterState && (
                    result.filterState === InitEditPageBuilderAction
                    || result.filterState === GeneratePageActionCommandsAction
                    || result.filterState === NextToDatasourceAction
                    || result.filterState === NextToWorkflowAction
                    || result.filterState === GatherAllChanges)),
                tap(result => {
                    switch (result.filterState) {
                        case InitEditPageBuilderAction:
                            this.currentActionCommands = []
                            const commandsTempEdit = result.processPage.commands as Array<PageButton>
                            _.forEach(commandsTempEdit, action => {
                                this.currentActionCommands.push({
                                    ...action
                                })
                            })
                            break
                        case GeneratePageActionCommandsAction:
                            this.currentActionCommands = []
                            const commandsTemp = result.processPage.commands as Array<PageButton>
                            _.forEach(commandsTemp, action => {
                                this.currentActionCommands.push({
                                    ...action
                                })
                            })
                            break
                        case NextToDatasourceAction:
                            this.store.dispatch([
                                new UpdatePageActionCommandsAction(ObjectUtils.clone(this.currentActionCommands)),
                                new UpdateAvailableEvents(this.generateAvailableEvents())
                            ])
                            break
                        case NextToWorkflowAction:
                            this.store.dispatch([
                                new UpdatePageActionCommandsAction(ObjectUtils.clone(this.currentActionCommands)),
                                new UpdateAvailableEvents(this.generateAvailableEvents())
                            ])
                            break
                        case GatherAllChanges:
                            if (this.currentActionCommands) {
                                this.store.dispatch(new UpdatePageActionCommandsAction(ObjectUtils.clone(this.currentActionCommands)))
                            }
                            break
                    }
                })
            ).subscribe()
    }

    addNewCommand() {
        let actionCommand: PageButton = {
            id: Guid.create().toString(),
            name: 'New Command',
            icon: 'edit',
            color: 'primary',
            allowHidden: 'false',
            isRequiredValidation: true,
            buttonOptions: {
                actionCommandOptions: {
                    actionType: ActionType.ExecuteDatabase,
                    databaseOptions: {
                        databaseConnectionId: '',
                        entityName: '',
                        query: ''
                    },
                    notificationOptions: {
                        completeMessage: 'Completed!',
                        failedMessage: 'Oops! Something went wrong, please try again!'
                    },
                    httpServiceOptions: {
                        httpMethod: 'Get',
                        httpServiceUrl: '',
                        httpSuccessCode: '200',
                        jsonBody: '',
                        outputProjection: ''
                    },
                    workflowOptions: {
                        mapWorkflowInputs: [],
                        workflowId: ''
                    },
                    isEnable: false
                },
                confirmationOptions: {
                    isEnable: false,
                    confirmationText: 'Are you sure to proceed it?'
                },
                routeOptions: {
                    isEnable: false,
                    routes: []
                }
            }
        }

        const dialogRef = this.dialog.open(PageButtonDialogComponent, {
            data: {
                command: actionCommand
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.currentActionCommands.push(result)
                this.refreshControlTable()
            }
        })
    }

    editCommand(command: PageButton) {
        const dialogRef = this.dialog.open(PageButtonDialogComponent, {
            data: {
                command: command
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            this.currentActionCommands = ArrayUtils.updateOneItem(this.currentActionCommands, result, (command: PageButton) => { return command.id === result.id })
            this.refreshControlTable()
        })
    }

    deleteCommand(command: PageButton) {
        this.currentActionCommands = _.filter(this.currentActionCommands, (elem) => {
            return elem.id !== command.id
        })
        this.shortcutUtil.toastMessage('Delete command successfully!', ToastType.Success);

        this.refreshControlTable()
    }

    editRoute(command: PageButton) {
        const dialogRef = this.dialog.open(PageButtonRouteDialogComponent, {
            data: {
                command: command
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            command.buttonOptions.routeOptions.routes = result.routes
            command.buttonOptions.routeOptions.isEnable = result.isEnable

            this.currentActionCommands = ArrayUtils.updateOneItem(this.currentActionCommands, command, (button: PageButton) => { return button.id === command.id })
            this.refreshControlTable()
        })
    }


    editOptions(command: PageButton) {
        const dialogRef = this.dialog.open(PageButtonOptionsDialogComponent, {
            data: {
                buttonOptions: command.buttonOptions
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            command.buttonOptions = result
            //this.currentActionCommands = ArrayUtils.updateOneItem(this.currentActionCommands, result, (command: PageButton) => { return command.id === result.id })
            this.logger.debug('Current buttons', this.currentActionCommands)
            this.refreshControlTable()
        })
    }

    /** SELECTION */
    isAllSelected(): boolean {
        const numSelected = this.selection.selected.length;
        const numRows = this.currentActionCommands.length;
        return numSelected === numRows;
    }

    masterToggle() {
        if (this.selection.selected.length === this.currentActionCommands.length) {
            this.selection.clear();
        } else {
            this.currentActionCommands.forEach(row => this.selection.select(row));
        }
    }

    deleteSelectedControls() {
        const _title = "Delete Action Commands"
        const _description = "Are you sure to delete all selected commands?"
        const _waitDesciption = "Waiting..."
        const dialogRef = this.shortcutUtil.confirmationDialog(_title, _description, _waitDesciption);
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
                return;
            }

            for (let i = 0; i < this.selection.selected.length; i++) {
                this.currentActionCommands = _.remove(this.currentActionCommands, (elem) => {
                    console.log(this.selection.selected[i].id)
                    return elem.id === this.selection.selected[i].id
                })
            }
            this.refreshControlTable();
        });
    }

    onListDrop($event: CdkDragDrop<PageButton>) {
        const prevIndex = this.currentActionCommands.findIndex((control) => control === $event.item.data)
        moveItemInArray(this.currentActionCommands, prevIndex, $event.currentIndex);
        this.currentActionCommands = ArrayUtils.swapTwoItems(this.currentActionCommands, $event.previousIndex, $event.currentIndex)
        this.refreshControlTable()
    }

    refreshControlTable() {
        this.selection.clear()
        this.table.renderRows()
    }

    generateAvailableEvents(): Array<string> {
        let events: Array<string> = []

        _.forEach(this.currentActionCommands, (actionCommand: PageButton) => {
            events.push(`${actionCommand.name}_click`.toLowerCase())
        })

        return events
    }
}
