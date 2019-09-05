import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { ShortcutUtil } from 'app/shared/components/shortcuts/shortcut-util';
import { MatDialog, MatTable } from '@angular/material';
import { Store } from '@ngxs/store';
import { PageDatasource, DatasourceControlType } from 'services/portal.service';
import { filter, tap } from 'rxjs/operators';
import { NextToWorkflowAction, NextToRouteAction, UpdatePageDatasources, NextToDatasourceAction, GatherAllChanges, InitEditPageBuilderAction } from 'stores/pages/pagebuilder.actions';
import { PageBuilderStateModel } from 'stores/pages/pagebuilder.state';
import { Guid } from 'guid-typescript';
import * as _ from 'lodash';
import { ToastType } from 'app/shared/components/shortcuts/shortcut.models';
import { NGXLogger } from 'ngx-logger';
import { DatasourceOptionsDialogComponent } from 'portal/shared/datasourceopts/datasourceopts.component';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-page-datasource-grid',
    templateUrl: './page-datasource.grid.component.html'
})
export class PageDatasourceGridComponent implements OnInit {
    @ViewChild('table') table: MatTable<PageDatasource>;

    displayedListColumns = ['datasourceName', 'triggerCondition', 'isActive', 'actions'];
    currentDatasources: Array<PageDatasource> = []

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
                filter(state => state.filterState && (
                    state.filterState === InitEditPageBuilderAction
                    || state.filterState === NextToWorkflowAction
                    || state.filterState === GatherAllChanges)),
                tap((state: PageBuilderStateModel) => {
                    switch (state.filterState) {
                        case InitEditPageBuilderAction:
                            _.forEach(state.processPage.pageDatasources, ds => {
                                this.currentDatasources.push({
                                    ...ds
                                })
                            })
                            break
                        case NextToWorkflowAction:
                        case GatherAllChanges:
                            this.store.dispatch(new UpdatePageDatasources(
                                ObjectUtils.clone(this.currentDatasources)
                            ))
                            break
                    }
                })
            ).subscribe()
    }

    addNewDatasource() {
        this.currentDatasources.push({
            id: Guid.create().toString(),
            name: 'data',
            triggerCondition: 'true',
            isActive: true,
            options: {
                type: DatasourceControlType.StaticResource,
                databaseOptions: {
                    databaseConnectionId: '',
                    entityName: '',
                    query: ''
                },
                datasourceStaticOptions: {
                    jsonResource: ''
                },
                httpServiceOptions: {
                    httpServiceUrl: '',
                    httpMethod: 'Get',
                    httpSuccessCode: '200',
                    jsonBody: '',
                    outputProjection: ''
                },
                triggeredEvents: ''
            }
        })
    }

    deleteDatasource(datasource: PageDatasource) {
        this.currentDatasources = _.filter(this.currentDatasources, (elem) => {
            return elem.id !== datasource.id
        })
        this.shortcutUtil.notifyMessage("Delete datasource successfully!", ToastType.Success)
    }

    editDatasourceOption(datasource: PageDatasource) {
        let newDatasource = datasource.options
        this.logger.debug('editting datasource', newDatasource)
        if (!newDatasource) {
            newDatasource = {
                type: DatasourceControlType.StaticResource
            }
        }
        let dialogRef = this.dialog.open(DatasourceOptionsDialogComponent, {
            disableClose: true,
            data: {
                datasourceOption: newDatasource
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                datasource.options = result
                dialogRef = null
            }
        })
    }
}
