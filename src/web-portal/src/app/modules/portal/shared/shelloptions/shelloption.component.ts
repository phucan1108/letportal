import { Component, OnInit, Input, ViewChild, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table'
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { NGXLogger } from 'ngx-logger';
import { Guid } from 'guid-typescript';
import * as _ from 'lodash';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { ExtendedShellOption } from './extened.shell.model';
import { Observable } from 'rxjs';
import { DataTable } from 'momentum-table';
import { ShellOptionDialogComponent } from './shelloption.dialog.component';
import { ListOptions } from 'portal/modules/models/dynamiclist.extended.model';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-shell-option',
    templateUrl: './shelloption.component.html',
    styleUrls: ['./shelloption.component.scss']
})
export class ShellOptionComponent implements OnInit, AfterViewInit {

    constructor(
        public dialog: MatDialog,
        private translate: TranslateService,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger
    ) { }
    @ViewChild('table', { static: false }) table: MatTable<ExtendedShellOption>;

    @ViewChild('mtable', { static: true })
    mtable: DataTable;

    displayedListColumns = ['hint', 'key', 'value', 'actions']

    @Input()
    shellOptions$: Observable<Array<ExtendedShellOption>>;

    shellOptions: Array<ExtendedShellOption> = [];
    newOptionKey: string;
    @Output()
    changed = new EventEmitter();
    ngAfterViewInit(): void {
    }

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
        this.shortcutUtil.toastMessage(this.translate.instant('common.deleteSuccessfully'), ToastType.Success);
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
