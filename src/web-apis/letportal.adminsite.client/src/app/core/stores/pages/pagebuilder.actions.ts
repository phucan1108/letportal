import { Page, ShellOption, PageBuilder, PageButton, PortalClaim, PageEvent, StandardComponent, PageDatasource } from 'services/portal.service';
const PAGE_BUILDER_PAGE = '[Form Builder Page]'

export class PageBuilderAction {
  public static readonly type = '';
}

export class FetchPageByIdAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Fetch By Id`;
  constructor(public id: string) { }
}

export class InitCreatePageBuilderAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Init Create Page`;
  constructor(){}
}

export class InitEditPageBuilderAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Init Edit Page`
  constructor(public page: Page) { }
}

export class CreatePageAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Create New Page`
  constructor() { }
}

export class EditPageAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Edit Page`
  constructor() { }
}

export class UpdatePageInfoAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Page Info`
  constructor(
    public id: string, 
    public name: string, 
    public appId: string, 
    public displayName: string, 
    public urlPath: string, 
    public canSave: boolean) { }
}

export class UpdatePageOptionsAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Page Options`
  constructor(public options: ShellOption[]) { }
}

export class GeneratePageBuilderInfoAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Generate Page Builder Info`
  constructor(public pageBuilder: PageBuilder) { }
}

export class GeneratePageActionCommandsAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Generate Page Action Commands`
  constructor(public pageActions: PageButton[]) { }
}

export class GeneratePageEventsAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Generate Page Events`
  constructor(public pageEvents: PageEvent[]) { }
}


export class UpdatePageBuilderInfoAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Page Builder Info`
  constructor(public pageBuilder: PageBuilder) { }
}

export class UpdatePageActionCommandsAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Page Actions`
  constructor(public pageActions: PageButton[]) { }
}

export class UpdatePageEventsAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Page Events`
  constructor(public pageEvents: Array<PageEvent>) { }
}
export class NextToPageBuilderAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Next to page builder`
  constructor() { }
}

export class NextToDatasourceAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Next to Datasource`
  constructor() { }
}

export class NextToWorkflowAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Next to workflow`
  constructor() { }
}

export class NextToRouteAction implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Next to route`
  constructor() { }
}

export class UpdateAvailableEvents implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Available Events`
  constructor(public availableEvents: Array<string>) { }
}

export class UpdateAvailableTriggerEventsList implements PageBuilderAction{
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Available Trgger Events List`
  constructor(public availableTriggerEventsList: Array<string>) { }
}

export class UpdateAvailableBoundDatas implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Available Bound Datas`
  constructor(public availableBoundDatas: Array<string>) { }
}

export class UpdateAvailableShells implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Available Shells`
  constructor(public availableShells: Array<string>) { }
}

export class UpdateShellOptions implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Shell Options`
  constructor(public shellOptions: Array<ShellOption>){ }
}

export class UpdatePageClaims implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update Page Claims`
  constructor(public claims: Array<PortalClaim>){ }
}

export class UpdateStandardComponents implements PageBuilderAction {
  public static readonly type = `${PAGE_BUILDER_PAGE} Update standard components`
  constructor(public standardComponents: StandardComponent[]){ }
}

export class UpdatePageDatasources implements PageBuilderAction{
  public static readonly type = `${PAGE_BUILDER_PAGE} Update page datasources`
  constructor(public pageDatasources: PageDatasource[]){}
}

export class GatherAllChanges implements PageBuilderAction{
  public static readonly type = `${PAGE_BUILDER_PAGE} Gather all changes`
  constructor(){}
}

export type All =
  FetchPageByIdAction |
  InitCreatePageBuilderAction |
  InitEditPageBuilderAction |
  CreatePageAction |
  EditPageAction |
  UpdatePageActionCommandsAction |
  UpdatePageBuilderInfoAction |
  UpdatePageEventsAction |
  UpdatePageInfoAction |
  UpdatePageOptionsAction |
  GeneratePageBuilderInfoAction |
  GeneratePageActionCommandsAction |
  GeneratePageEventsAction |
  NextToPageBuilderAction |
  NextToDatasourceAction |
  NextToRouteAction |
  NextToWorkflowAction |
  UpdateAvailableEvents |
  UpdateAvailableBoundDatas |
  UpdateAvailableShells |
  UpdateAvailableTriggerEventsList |
  UpdatePageClaims |
  UpdateStandardComponents |
  UpdatePageDatasources |
  GatherAllChanges