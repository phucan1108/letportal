import { State, Action, Selector, StateContext } from '@ngxs/store';
// import { FetchDynamicFormByIdAction, CreateDynamicFormAction, UpdateDynamicFormInfoAction, UpdateDynamicFormBuilderAction, UpdateDynamicFormOptionsAction, UpdateDynamicFormActionCommandsAction, UpdateDynamicFormEventsAction, UpdateDynamicFormRouteAction, GenerateDynamicFormBuilderAction } from './dynamicform.actions';
import * as PageActions from './pagebuilder.actions'
import { Guid } from 'guid-typescript';
import { tap } from 'rxjs/operators';
import * as _ from 'lodash'
import { ShellConfigProvider } from 'app/core/shell/shellconfig.provider';
import { Page, PagesClient, StandardComponent } from 'services/portal.service';
import { patch, append } from '@ngxs/store/operators';
import { state } from '@angular/animations';
import { Injectable } from '@angular/core';

export interface PageBuilderStateModel {
  processPage: Page,
  standards: StandardComponent[],
  filterState: PageActions.PageBuilderAction,
  availableEvents: Array<string>,
  availableFunctionEvents: Array<string>,
  availableShells: Array<string>,
  availableBoundDatas: Array<string>,
  availableTriggerEventsList: Array<string>,
  isFormBuilderValid: boolean
}

@State<PageBuilderStateModel>({
  name: 'pagebuilder',
  defaults: {
    processPage: {
      id: Guid.create().toString(),
      displayName: '',
      name: '',
      appId: '',
      shellOptions: [],
      claims: [],
      urlPath: '',
      builder: {
        sections: []
      },
      commands: [],
      pageDatasources: [],
      events: []
    },
    standards: [],
    filterState: null,
    availableEvents: [],
    availableFunctionEvents: [],
    availableShells: [],
    availableBoundDatas: [],
    availableTriggerEventsList: [],
    isFormBuilderValid: false
  }
})
@Injectable()
export class PageBuilderState {

  constructor(
    private pagesClient: PagesClient,
    private shellConfig: ShellConfigProvider
  ) { }

  @Selector()
  public static getState(state: PageBuilderStateModel) {
    return state;
  }

  @Action(PageActions.InitCreatePageBuilderAction)
  public initCreate(ctx: StateContext<PageBuilderStateModel>, { }: PageActions.InitCreatePageBuilderAction) {
    const state = ctx.getState()
    return ctx.setState({
      ...state,
      filterState: PageActions.InitCreatePageBuilderAction
    })
  }

  @Action(PageActions.InitEditPageBuilderAction)
  public initEdit(ctx: StateContext<PageBuilderStateModel>, { page }: PageActions.InitEditPageBuilderAction) {
    const state = ctx.getState()
    return ctx.setState({
      ...state,
      processPage: page,
      filterState: PageActions.InitEditPageBuilderAction
    })
  }

  @Action(PageActions.FetchPageByIdAction)
  public add(ctx: StateContext<PageBuilderStateModel>, { id }: PageActions.FetchPageByIdAction) {
    const state = ctx.getState()
    return this.pagesClient.getOne(id).pipe(
      tap((result: Page) => {
        ctx.setState({
          ...state,
          processPage: result,
          filterState: PageActions.FetchPageByIdAction
        })
      })
    )
  }

  @Action(PageActions.CreatePageAction)
  public create(ctx: StateContext<PageBuilderStateModel>, { }: PageActions.CreatePageAction) {
    const state = ctx.getState()
    return this.pagesClient.create(state.processPage).pipe(
      tap((result: string) => {
        ctx.setState({
          ...state,
          processPage: {
            ...state.processPage,
            id: result
          },
          filterState: PageActions.CreatePageAction
        })
      })
    )
  }

  @Action(PageActions.EditPageAction)
  public editForm(ctx: StateContext<PageBuilderStateModel>, { }: PageActions.EditPageAction) {
    const state = ctx.getState()
    return this.pagesClient.update(state.processPage.id, state.processPage).pipe(
      tap(() => {
        ctx.setState({
          ...state,
          filterState: PageActions.EditPageAction
        })
      })
    )
  }

  @Action(PageActions.UpdatePageInfoAction)
  public updateFormInfo(ctx: StateContext<PageBuilderStateModel>, { id, name, appId, displayName, urlPath, canSave }: PageActions.UpdatePageInfoAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        id,
        name,
        displayName,
        urlPath,
        appId
      },
      filterState: PageActions.UpdatePageInfoAction
    })
  }

  @Action(PageActions.UpdatePageOptionsAction)
  public updateFormOptions(ctx: StateContext<PageBuilderStateModel>, { options }: PageActions.UpdatePageOptionsAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        shellOptions: options
      },
      filterState: PageActions.UpdatePageOptionsAction
    })
  }

  @Action(PageActions.GeneratePageBuilderInfoAction)
  public generateFormBuilder(ctx: StateContext<PageBuilderStateModel>, { pageBuilder }: PageActions.GeneratePageBuilderInfoAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        builder: pageBuilder
      },
      filterState: PageActions.GeneratePageBuilderInfoAction
    })
  }

  @Action(PageActions.GeneratePageActionCommandsAction)
  public generateActionCommands(ctx: StateContext<PageBuilderStateModel>, { pageActions }: PageActions.GeneratePageActionCommandsAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        commands: pageActions
      },
      filterState: PageActions.GeneratePageActionCommandsAction
    })
  }

  @Action(PageActions.GeneratePageEventsAction)
  public generateFormEvents(ctx: StateContext<PageBuilderStateModel>, { pageEvents }: PageActions.GeneratePageEventsAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        events: pageEvents
      },
      filterState: PageActions.GeneratePageActionCommandsAction
    })
  }

  @Action(PageActions.UpdatePageBuilderInfoAction)
  public updateFormBuilder(ctx: StateContext<PageBuilderStateModel>, { pageBuilder }: PageActions.UpdatePageBuilderInfoAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        builder: pageBuilder
      },
      filterState: PageActions.UpdatePageBuilderInfoAction
    })
  }

  @Action(PageActions.UpdatePageActionCommandsAction)
  public updateActionCommands(ctx: StateContext<PageBuilderStateModel>, { pageActions }: PageActions.UpdatePageActionCommandsAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        commands: pageActions
      },
      filterState: PageActions.UpdatePageActionCommandsAction
    })
  }

  @Action(PageActions.UpdatePageEventsAction)
  public updateFormEvents(ctx: StateContext<PageBuilderStateModel>, { pageEvents }: PageActions.UpdatePageEventsAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        events: pageEvents
      },
      filterState: PageActions.UpdatePageEventsAction
    })
  }
  @Action(PageActions.NextToPageBuilderAction)
  public nextToBuilder(ctx: StateContext<PageBuilderStateModel>, { }: PageActions.NextToPageBuilderAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      filterState: PageActions.NextToPageBuilderAction
    })
  }

  @Action(PageActions.NextToDatasourceAction)
  public nextToDatasource(ctx: StateContext<PageBuilderStateModel>, { }: PageActions.NextToDatasourceAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      filterState: PageActions.NextToDatasourceAction
    })
  }

  @Action(PageActions.NextToWorkflowAction)
  public nextToWorkflow(ctx: StateContext<PageBuilderStateModel>, { }: PageActions.NextToWorkflowAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      filterState: PageActions.NextToWorkflowAction
    })
  }

  @Action(PageActions.NextToRouteAction)
  public nextToRoute(ctx: StateContext<PageBuilderStateModel>, { }: PageActions.NextToRouteAction) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      filterState: PageActions.NextToRouteAction
    })
  }

  @Action(PageActions.UpdateAvailableEvents)
  public updateAvailableEvents(ctx: StateContext<PageBuilderStateModel>, { availableEvents }: PageActions.UpdateAvailableEvents) {
    const state = ctx.getState()
    const distinctEvents = 
    state.availableEvents.concat(availableEvents).filter((val, index, selfArray) => {
      return selfArray.indexOf(val) === index
    })
    ctx.setState({
      ...state,
      filterState: PageActions.UpdateAvailableEvents,
      availableEvents: distinctEvents
    })
  }

  @Action(PageActions.UpdateAvailableTriggerEventsList)
  public updateAvailableTriggerEventsList(ctx: StateContext<PageBuilderStateModel>, { availableTriggerEventsList }: PageActions.UpdateAvailableTriggerEventsList) {
    const state = ctx.getState()
    const distinctEvents = 
    state.availableTriggerEventsList.concat(availableTriggerEventsList).filter((val,index,selfArray) => {
      return selfArray.indexOf(val) === index
    })
    ctx.setState({
      ...state,
      filterState: PageActions.UpdateAvailableTriggerEventsList,
      availableTriggerEventsList: distinctEvents
    })
  }

  @Action(PageActions.UpdateAvailableBoundDatas)
  public updateAvailableBoundDatas(ctx: StateContext<PageBuilderStateModel>, { availableBoundDatas }: PageActions.UpdateAvailableBoundDatas) {
    const state = ctx.getState()
    const distinctEvents = 
    state.availableBoundDatas.concat(availableBoundDatas).filter((val, index, selfArray) =>{
      return selfArray.indexOf(val) === index
    })
    ctx.setState({
      ...state,
      filterState: PageActions.UpdateAvailableBoundDatas,
      availableBoundDatas
    })
  }

  @Action(PageActions.UpdateAvailableShells)
  public updateAvailableShells(ctx: StateContext<PageBuilderStateModel>, { availableShells }: PageActions.UpdateAvailableShells) {
    const state = ctx.getState()
    const distinctShells = 
    state.availableShells
      .concat(availableShells, this.shellConfig.getAllAvailableShells())
      .filter((val,index, selfArray) => {
        return selfArray.indexOf(val) === index
      })    
    ctx.setState({
      ...state,
      filterState: PageActions.UpdateAvailableShells,
      availableShells: distinctShells
    })
  }

  @Action(PageActions.UpdateShellOptions)
  public updateShellOptions(ctx: StateContext<PageBuilderStateModel>, { shellOptions }: PageActions.UpdateShellOptions) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        shellOptions
      },
      filterState: PageActions.UpdateShellOptions
    })
  }

  @Action(PageActions.UpdatePageClaims)
  public updateFormClaims(ctx: StateContext<PageBuilderStateModel>, { claims }: PageActions.UpdatePageClaims) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        claims
      },
      filterState: PageActions.UpdatePageClaims
    })
  }

  @Action(PageActions.UpdateStandardComponents)
  public updateStandardComponents(ctx: StateContext<PageBuilderStateModel>, { standardComponents }: PageActions.UpdateStandardComponents) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      standards: standardComponents,
      filterState: PageActions.UpdateStandardComponents
    })
  }

  @Action(PageActions.UpdatePageDatasources)
  public updatePageDatasources(ctx: StateContext<PageBuilderStateModel>, { pageDatasources }: PageActions.UpdatePageDatasources) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      processPage: {
        ...state.processPage,
        pageDatasources
      },
      filterState: PageActions.UpdatePageDatasources
    })
  }

  @Action(PageActions.GatherAllChanges)
  public gatherAllChanges(ctx: StateContext<PageBuilderStateModel>, { }: PageActions.GatherAllChanges) {
    const state = ctx.getState()
    ctx.setState({
      ...state,
      filterState: PageActions.GatherAllChanges
    })
  }
}
