import { Page, PageButton } from 'services/portal.service';
import * as PageActions from './page.actions'
import { PageControlActionEvent, PageLoadedDatasource, RenderingPageSectionState, PageSectionBoundData, MapDataControl } from 'app/core/models/page.model';
import { State, Selector, Action, StateContext } from '@ngxs/store';
import { patch, append, updateItem } from '@ngxs/store/operators';
import { ObjectUtils } from 'app/core/utils/object-util';
import * as _ from 'lodash';
import { ArrayUtils } from 'app/core/utils/array-util';

export interface SectionValidation {
    section: string
    isValid: boolean
}

export interface PageStateModel {
    page: Page
    data: any
    wholePageValid: boolean
    sectionValidations: SectionValidation[]
    sectionsMap: MapDataControl[]
    datasources: PageLoadedDatasource[]
    renderingSections: RenderingPageSectionState[]
    clickingButton: PageButton
    options: any
    queryparams: any
    eventsList: PageControlActionEvent[]
    lastEvent: PageControlActionEvent
    filterState: any
}

@State<PageStateModel>({
    name: 'page',
    defaults: {
        page: null,
        data: null,
        wholePageValid: false,
        sectionValidations: [],
        sectionsMap: [],
        datasources: [],
        renderingSections: [],
        clickingButton: null,
        options: null,
        queryparams: null,
        eventsList: [],
        lastEvent: null,
        filterState: null
    }
})
export class PageState {
    constructor(
    ) { }

    @Selector()
    public static getState(state: PageStateModel) {
        return state;
    }

    @Action(PageActions.UpdateDatasourceAction)
    public updateDatasource(ctx: StateContext<PageStateModel>, { datasource }: PageActions.UpdateDatasourceAction) {
        const state = ctx.getState()
        const cloneDatasources = ObjectUtils.clone(state.datasources)
        cloneDatasources.push(datasource)
        ctx.setState({
            ...state,
            datasources: cloneDatasources,
            filterState: PageActions.UpdateDatasourceAction
        })
    }

    @Action(PageActions.LoadDatasource)
    public loadDatasource(ctx: StateContext<PageStateModel>, { }: PageActions.LoadDatasource) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            filterState: PageActions.LoadDatasource
        })
    }

    @Action(PageActions.LoadDatasourceComplete)
    public loadDatasourceComplete(ctx: StateContext<PageStateModel>, { datasources }: PageActions.LoadDatasourceComplete) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            datasources,
            filterState: PageActions.LoadDatasourceComplete
        })
    }

    @Action(PageActions.BeginBuildingBoundData)
    public buildData(ctx: StateContext<PageStateModel>, { }: PageActions.BeginBuildingBoundData) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            filterState: PageActions.BeginBuildingBoundData
        })
    }

    @Action(PageActions.AddSectionBoundData)
    public addSectionData(ctx: StateContext<PageStateModel>, { pageSectionBoundData, sectionsMap }: PageActions.AddSectionBoundData) {
        const state = ctx.getState()
        let tempData = state.data ? ObjectUtils.clone(state.data) : new Object()

        if (pageSectionBoundData.data) {
            if (pageSectionBoundData.isKeptDataName) {
                _.forEach(Object.keys(pageSectionBoundData.data), key => {
                    tempData[key] = pageSectionBoundData.data[key]
                })
            }
            else {
                // Keep a mapping data with section name. Ex: data.appinfo.id
                tempData = pageSectionBoundData.data
            }
        }
        if (sectionsMap && sectionsMap.length > 0) {

            let cloneSectionsMap = ObjectUtils.clone(state.sectionsMap)
            cloneSectionsMap = ArrayUtils.appendItemsDistinct(cloneSectionsMap, sectionsMap, 'controlFullName')
            ctx.setState({
                ...state,
                sectionsMap: cloneSectionsMap,
                data: tempData,
                filterState: PageActions.AddSectionBoundData
            })
        }
        else {
            ctx.setState({
                ...state,
                data: tempData,
                filterState: PageActions.AddSectionBoundData
            })
        }
    }
    @Action(PageActions.AddSectionBoundDataForStandardArray)
    public addSectionDataForStandardArray(ctx: StateContext<PageStateModel>, { event }: PageActions.AddSectionBoundDataForStandardArray) {
        const state = ctx.getState()
        let tempData = state.data ? ObjectUtils.clone(state.data) : new Object()

        tempData[event.name] = {
            fresh: event.data,
            inserts: [],
            removes: [],
            updates: []
        }
        ctx.setState({
            ...state,
            data: tempData,
            filterState: PageActions.AddSectionBoundDataForStandardArray
        })
    }

    @Action(PageActions.EndBuildingBoundDataComplete)
    public buildDataComplete(ctx: StateContext<PageStateModel>, { }: PageActions.EndBuildingBoundDataComplete) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            filterState: PageActions.EndBuildingBoundDataComplete
        })
    }

    @Action(PageActions.PageReadyAction)
    public pageReady(ctx: StateContext<PageStateModel>, { queryparams, options }: PageActions.PageReadyAction) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            options,
            queryparams,
            filterState: PageActions.PageReadyAction
        })
    }

    @Action(PageActions.BeginRenderingPageSectionsAction)
    public beginRenderingSections(ctx: StateContext<PageStateModel>, { renderingSections }: PageActions.BeginRenderingPageSectionsAction) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            renderingSections,
            filterState: PageActions.BeginRenderingPageSectionsAction
        })
    }

    @Action(PageActions.RenderingPageSectionAction)
    public renderingPageSection(ctx: StateContext<PageStateModel>, { renderingSection }: PageActions.RenderingPageSectionAction) {
        const state = ctx.getState()
        const cloneRenderingSections: RenderingPageSectionState[] = ObjectUtils.clone(state.renderingSections)
        _.forEach(cloneRenderingSections, s => {
            if (s.sectionName === renderingSection.sectionName) {
                s.state = renderingSection.state
            }
        })
        ctx.setState({
            ...state,
            renderingSections: cloneRenderingSections,
            filterState: PageActions.RenderingPageSectionAction
        })
    }

    @Action(PageActions.RenderedPageSectionAction)
    public renderedPageSection(ctx: StateContext<PageStateModel>, { renderedSection }: PageActions.RenderedPageSectionAction) {
        const state = ctx.getState()
        const cloneRenderingSections: RenderingPageSectionState[] = ObjectUtils.clone(state.renderingSections)
        _.forEach(cloneRenderingSections, s => {
            if (s.sectionName === renderedSection.sectionName) {
                s.state = renderedSection.state
                s.sectionClass = renderedSection.sectionClass
            }
        })

        const cloneSectionValidations: SectionValidation[] = ObjectUtils.clone(state.sectionValidations)
        cloneSectionValidations.push({
            section: renderedSection.sectionName,
            isValid: false
        })

        ctx.setState({
            ...state,
            renderingSections: cloneRenderingSections,
            sectionValidations: cloneSectionValidations,
            filterState: PageActions.RenderedPageSectionAction
        })
    }

    @Action(PageActions.EndRenderingPageSectionsAction)
    public endRenderingPageSections(ctx: StateContext<PageStateModel>, { }: PageActions.EndRenderingPageSectionsAction) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            filterState: PageActions.EndRenderingPageSectionsAction
        })
    }

    @Action(PageActions.OnDestroyingPage)
    public destroyingPage(ctx: StateContext<PageStateModel>, { }: PageActions.OnDestroyingPage) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            filterState: PageActions.OnDestroyingPage
        })
    }

    @Action(PageActions.ChangeControlValueEvent)
    public changeControlValue(ctx: StateContext<PageStateModel>, { event }: PageActions.ChangeControlValueEvent) {

        const state = ctx.getState()

        const foundMap = _.find(state.sectionsMap, map => map.controlFullName === (event.sectionName + '_' + event.controlName))
        if (foundMap && foundMap.bindName) {
            const data = ObjectUtils.clone(state.data)
            if (foundMap.sectionMapName) {
                data[foundMap.sectionMapName][foundMap.bindName] = event.data
            }
            else {
                data[foundMap.bindName] = event.data
            }
            const cloneEventsList = ObjectUtils.clone(state.eventsList)
            cloneEventsList.push(event)
            ctx.setState({
                ...state,
                eventsList: cloneEventsList,
                data,
                lastEvent: event,
                filterState: PageActions.ChangeControlValueEvent
            })
        }
        else {
            const cloneEventsList = ObjectUtils.clone(state.eventsList)
            cloneEventsList.push(event)
            ctx.setState({
                ...state,
                eventsList: cloneEventsList,
                lastEvent: event,
                filterState: PageActions.ChangeControlValueEvent
            })
        }
    }

    @Action(PageActions.ClickControlEvent)
    public clickControlEvent(ctx: StateContext<PageStateModel>, { event }: PageActions.ClickControlEvent) {
        const state = ctx.getState()
        const cloneEventsList = ObjectUtils.clone(state.eventsList)
        cloneEventsList.push(event)
        ctx.setState({
            ...state,
            eventsList: cloneEventsList,
            lastEvent: event,
            filterState: PageActions.ClickControlEvent
        })
    }

    @Action(PageActions.UserClicksOnButtonAction)
    public userClickOnButton(ctx: StateContext<PageStateModel>, { button }: PageActions.UserClicksOnButtonAction) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            clickingButton: button,
            filterState: PageActions.UserClicksOnButtonAction
        })
    }

    @Action(PageActions.GatherSectionValidations)
    public gatherSectionValidations(ctx: StateContext<PageStateModel>, { }: PageActions.GatherSectionValidations) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            filterState: PageActions.GatherSectionValidations
        })
    }

    @Action(PageActions.SectionValidationStateAction)
    public sectionValidationState(ctx: StateContext<PageStateModel>, { section, isValid }: PageActions.SectionValidationStateAction) {
        const state = ctx.getState()
        const clone = ObjectUtils.clone(state.sectionValidations)
        _.forEach(clone, val => {
            if (val.section === section) {
                val.isValid = isValid
            }
        })
        ctx.setState({
            ...state,
            sectionValidations: clone,
            filterState: PageActions.SectionValidationStateAction
        })
    }

    @Action(PageActions.CompleteGatherSectionValidations)
    public completeSectionValidationState(ctx: StateContext<PageStateModel>, { }: PageActions.CompleteGatherSectionValidations) {
        const state = ctx.getState()
        let isValid = true
        _.forEach(state.sectionValidations, val => {
            if (!val.isValid) {
                isValid = false
                return false
            }
        })

        ctx.setState({
            ...state,
            wholePageValid: isValid,
            filterState: PageActions.CompleteGatherSectionValidations
        })
    }
}