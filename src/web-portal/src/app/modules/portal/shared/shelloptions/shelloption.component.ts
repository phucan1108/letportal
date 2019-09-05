import { Component, OnInit, Input, ViewChild, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { MatDialog, MatTable } from '@angular/material';
import { ShortcutUtil } from 'app/shared/components/shortcuts/shortcut-util';
import { NGXLogger } from 'ngx-logger';
import { Guid } from 'guid-typescript';
import { Constants } from 'portal/resources/constants';
import { ArrayUtils } from 'app/core/utils/array-util';
import * as _ from 'lodash';
import { MessageType, ToastType } from 'app/shared/components/shortcuts/shortcut.models';
import { ExtendedShellOption } from './extened.shell.model';
import { ListOptions } from 'portal/dynamic-list/models/extended.model';
import { Observable, BehaviorSubject } from 'rxjs';
import { slideInRightOnEnterAnimation, slideOutRightOnLeaveAnimation, collapseAnimation, slideInRightAnimation, slideOutRightAnimation, fadeInRightAnimation, fadeOutRightAnimation } from 'angular-animations';
import { DataTable } from 'momentum-table';
import { ShellOptionDialogComponent } from './shelloption.dialog.component';

@Component({
    selector: 'let-shell-option',
    templateUrl: './shelloption.component.html',
    styleUrls: ['./shelloption.component.scss']
})
export class ShellOptionComponent implements OnInit, AfterViewInit {
    ngAfterViewInit(): void {
    }
    @ViewChild('table') table: MatTable<ExtendedShellOption>;

    @ViewChild('mtable')
    mtable: DataTable;

    displayedListColumns = ['hint', 'key', 'value', 'actions']

    @Input()
    shellOptions$: Observable<Array<ExtendedShellOption>>;

    shellOptions: Array<ExtendedShellOption> = [];
    newOptionKey: string;
    @Output()
    changed = new EventEmitter();

    constructor(
        public dialog: MatDialog,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        this.shellOptions$.subscribe(options => {
            this.logger.debug('Passing options', options)
            _.forEach(options, shell => {
                shell.id = Guid.create().toString()
            })
            this.shellOptions = options
        })
    }

    //#region Shell Options

    addShell() {
        const dialogRef = this.dialog.open(ShellOptionDialogComponent, {
            data: {
                shellOptions: this.shellOptions
            }
        });
        dialogRef.afterClosed().subscribe(res => {
            if (res) {
                this.shellOptions.push(res)
                this.changed.emit(this.shellOptions)
            }
        })
    }

    deleteShell(shell: ExtendedShellOption) {
        this.shellOptions = _.filter(this.shellOptions, (elem) => {
            return elem.id !== shell.id
        })
        this.shortcutUtil.notifyMessage("Delete option successfully!", ToastType.Success);
        this.changed.emit(this.shellOptions)
    }

    allowDeleteShell(shell: ExtendedShellOption) {
        return ListOptions.isAllowEdit(shell) && shell.allowDelete
    }

    //#endregion
    editComplete($event) {
        this.changed.emit(this.shellOptions)
    }
}
