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
import { EndRenderingPageSectionsAction, GatherSectionValidations, SectionValidationStateAction, AddSectionBoundData, BeginBuildingBoundData } from 'stores/pages/page.actions';
import { PageLoadedDatasource, PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import { StandardOptions } from 'portal/modules/models/standard.extended.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import { StandardArrayTableHeader } from './models/standard-array.models';
import { StandardComponent } from 'services/portal.service';
import { StandardSharedService } from './services/standard-shared.service';

@Component({
    selector: 'let-standard-array-render',
    templateUrl: './standard-array-render.component.html'
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

    displayedColumns: StandardArrayTableHeader[] = []
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
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState
                && (
                    state.filterState === BeginBuildingBoundData
                    || state.filterState === EndRenderingPageSectionsAction
                    || state.filterState === GatherSectionValidations)),
            tap(
                state => {
                    switch (state.filterState) {
                        case BeginBuildingBoundData:
                            this.store.dispatch(new AddSectionBoundData({
                                name: this.section.name,
                                isKeptDataName: true,
                                data: null
                            }, []))
                            break
                        case EndRenderingPageSectionsAction:
                            this.logger.debug('Rendered hit')
                            this.datasources = state.datasources
                            let boundData = this.standardSharedService.buildDataArray(this.section.sectionDatasource.datasourceBindName, this.datasources)
                            this.tableData = this.standardSharedService.buildDataArrayForTable(boundData, this.standard)
                            this.readyToRender = true
                            break
                        case GatherSectionValidations:
                            this.store.dispatch(new SectionValidationStateAction(this.section.name, true))
                            break
                    }
                }
            )
        ).subscribe()

        this.buildArrayTableHeaders()
    }

    buildArrayTableHeaders(){
        if(ObjectUtils.isNotNull(this.standardArrayOptions.namefield)){
            const arrayColumns = this.standardArrayOptions.namefield.split(';')
            arrayColumns.forEach(colName => {
                const displayName = (this.standard.controls as PageRenderedControl<DefaultControlOptions>[]).find(a => a.name === colName).defaultOptions.label
                this.displayedColumns.push({
                    name: colName,
                    displayName: displayName
                })
            })
        }
    }
}
