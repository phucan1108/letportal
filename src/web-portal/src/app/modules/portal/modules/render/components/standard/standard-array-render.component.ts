import { Component, OnInit, Input } from '@angular/core';
import { ExtendedPageSection, GroupControls } from 'app/core/models/extended.models';
import { NGXLogger } from 'ngx-logger';
import { FormBuilder } from '@angular/forms';
import { Store } from '@ngxs/store';
import { CustomHttpService } from 'services/customhttp.service';
import { PageService } from 'services/page.service';
import { PageStateModel } from 'stores/pages/page.state';
import { Observable, Subscription } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import { EndRenderingPageSectionsAction, GatherSectionValidations, SectionValidationStateAction, AddSectionBoundData, BeginBuildingBoundData, AddSectionBoundDataForStandardArray } from 'stores/pages/page.actions';
import { PageLoadedDatasource, PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import { StandardOptions } from 'portal/modules/models/standard.extended.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import { StandardArrayTableHeader } from './models/standard-array.models';
import { StandardComponent } from 'services/portal.service';
import { StandardSharedService } from './services/standard-shared.service';

@Component({
    selector: 'let-standard-array-render',
    templateUrl: './standard-array-render.component.html',
    styleUrls: ['./standard-array-render.component.scss']
})
export class StandardArrayRenderComponent implements OnInit {

    @Input()
    section: ExtendedPageSection

    controlsGroups: Array<GroupControls>
    controls: Array<PageRenderedControl<DefaultControlOptions>>
    
    standard: StandardComponent
    pageState$: Observable<PageStateModel>
    subscription: Subscription
    datasources: PageLoadedDatasource[]
    readyToRender = false
    standardArrayOptions: StandardOptions

    headers: StandardArrayTableHeader[] = []
    displayedColumns: string[] = []
    tableData: any = new Object()
    constructor(
        private logger: NGXLogger,
        private fb: FormBuilder,
        private store: Store,        
        private pageService: PageService,
        private standardSharedService: StandardSharedService
    ) { }

    ngOnInit(): void { 
        this.standardArrayOptions = StandardOptions.getStandardOptions(this.section.relatedArrayStandard.options)
        this.standard = this.section.relatedArrayStandard
        this.controls = this.standardSharedService
                .buildControlOptions(this.section.relatedArrayStandard.controls as PageRenderedControl<DefaultControlOptions>[]);
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState
                && (state.filterState === EndRenderingPageSectionsAction
                    || state.filterState === GatherSectionValidations)),
            tap(
                state => {
                    switch (state.filterState) {
                        case EndRenderingPageSectionsAction:
                            this.logger.debug('Rendered hit')
                            this.datasources = state.datasources
                            let boundData = this.standardSharedService
                                    .buildDataArray(this.section.sectionDatasource.datasourceBindName, this.datasources)
                            this.tableData = this.standardSharedService
                                    .buildDataArrayForTable(boundData, this.standard)                           

                            this.buildArrayTableHeaders()
                            this.readyToRender = true
                            this.store.dispatch(new AddSectionBoundDataForStandardArray({
                                name: this.section.name,
                                isKeptDataName: true,
                                data: this.tableData
                            }))
                            break
                        case GatherSectionValidations:
                            this.store.dispatch(new SectionValidationStateAction(this.section.name, true))
                            break
                    }
                }
            )
        ).subscribe()
    }

    buildArrayTableHeaders(){
        if(ObjectUtils.isNotNull(this.standardArrayOptions.namefield)){
            const arrayColumns = this.standardArrayOptions.namefield.split(';')
            arrayColumns.forEach(colName => {
                try
                {
                    const displayName = (this.controls).find(a => a.name === colName).defaultOptions.label
                    this.headers.push({
                        name: colName,
                        displayName: displayName
                    })
                    this.displayedColumns.push(colName)
                }
                catch(ex)
                {
                    this.logger.error('Error with name field ' + colName,ex)
                }               
            })

            if(this.standardArrayOptions.allowadjustment){
                this.displayedColumns.push('actions')
            }
        }
    }
}
