import { AddOneItemOnStandardArrayEvent, MapDataControl, OpenInsertDialogOnStandardArrayEvent, PageControlActionEvent, PageLoadedDatasource, PageSectionBoundData, PageSectionStandardArrayBoundData as PageSectionStandardArrayBoundDataEvent, RemoveOneItemOnStandardArrayEvent, RenderingPageSectionState, UpdateOneItemOnStandardArrayEvent, UpdateSectionBoundDataForTreeEvent, UpdateTreeDataEvent } from 'app/core/models/page.model';
import { Page, PageButton } from 'services/portal.service';

const PAGE_ACTION = '[Page]'

export class PageAction {
    public static readonly type = ''
}

export class InitPageInfo implements PageAction {
    public static readonly type = `${PAGE_ACTION} Init Page info`
    constructor(public page: Page) { }
}

export class LoadDatasource implements PageAction {
    public static readonly type = `${PAGE_ACTION} Load datasource`
    constructor() { }
}

export class LoadDatasourceComplete implements PageAction {
    public static readonly type = `${PAGE_ACTION} Load datasource Complete`
    constructor(public datasources: PageLoadedDatasource[]) { }
}

export class BeginBuildingBoundData implements PageAction {
    public static readonly type = `${PAGE_ACTION} Build Bound Data`
    constructor() { }
}

export class AddSectionBoundData implements PageAction {
    public static readonly type = `${PAGE_ACTION} Add Section Bound Data`
    constructor(public pageSectionBoundData: PageSectionBoundData, public sectionsMap: MapDataControl[]) { }
}

export class EndBuildingBoundDataComplete implements PageAction {
    public static readonly type = `${PAGE_ACTION} Build Bound Data Complete`
    constructor() { }
}

export class UpdateDatasourceAction implements PageAction {
    public static readonly type = `${PAGE_ACTION} Update datasource`
    constructor(public datasource: PageLoadedDatasource) { }
}

export class PageReadyAction implements PageAction {
    public static readonly type = `${PAGE_ACTION} Page ready`
    constructor(public options: any, public queryparams: any) { }
}

export class BeginRenderingPageSectionsAction implements PageAction {
    public static readonly type = `${PAGE_ACTION} Page sections begin rendering`
    constructor(public renderingSections: RenderingPageSectionState[]) { }
}

export class RenderingPageSectionAction implements PageAction {
    public static readonly type = `${PAGE_ACTION} Rendering Page Section`
    constructor(public renderingSection: RenderingPageSectionState) { }
}

export class RenderedPageSectionAction implements PageAction {
    public static readonly type = `${PAGE_ACTION} Rendered Page Section`
    constructor(public renderedSection: RenderingPageSectionState) { }
}

export class EndRenderingPageSectionsAction implements PageAction {
    public static readonly type = `${PAGE_ACTION} Page sections end rendering`
    constructor() { }
}

export class ChangeControlValueEvent implements PageAction {
    public static readonly type = `${PAGE_ACTION} Change control value`
    constructor(public event: PageControlActionEvent) { }
}

export class OnDestroyingPage implements PageAction {
    public static readonly type = `${PAGE_ACTION} Destroying page`
    constructor() { }
}

export class GatherSectionValidations implements PageAction {
    public static readonly type = `${PAGE_ACTION} Gather section validations`
    constructor(public specificSection?: string) { }
}

export class UserClicksOnButtonAction implements PageAction {
    public static readonly type = `${PAGE_ACTION} User clicks on button`
    constructor(public button: PageButton) { }
}

export class SectionValidationStateAction implements PageAction {
    public static readonly type = `${PAGE_ACTION} Section validation state`
    constructor(public section: string, public isValid: boolean) { }
}

export class CompleteGatherSectionValidations implements PageAction {
    public static readonly type = `${PAGE_ACTION} Complete gather section validations`
    constructor() { }
}

export class ClickControlEvent implements PageAction {
    public static readonly type = `${PAGE_ACTION} User clicks on control`
    constructor(public event: PageControlActionEvent) { }
}

// For Array Standards
export class AddSectionBoundDataForStandardArray implements PageAction {
    public static readonly type = `${PAGE_ACTION} Add section bound data on Standard Array`
    constructor(public event: PageSectionStandardArrayBoundDataEvent) { }
}

export class OpenInsertDialogForStandardArray implements PageAction {
    public static readonly type = `${PAGE_ACTION} Open dialog for inserting on Standard Array`
    constructor(public event: OpenInsertDialogOnStandardArrayEvent) { }
}

export class CloseDialogForStandardArray implements PageAction {
    public static readonly type = `${PAGE_ACTION} Close dialog for inserting on Standard Array`
    constructor() { }
}

export class InsertOneItemForStandardArray implements PageAction {
    public static readonly type = `${PAGE_ACTION} Add new item on Standard Array`
    constructor(public event: AddOneItemOnStandardArrayEvent) { }
}

export class RemoveOneItemForStandardArray implements PageAction {
    public static readonly type = `${PAGE_ACTION} remove item on Standard Array`
    constructor(public event: RemoveOneItemOnStandardArrayEvent) { }
}

export class UpdateOneItemForStandardArray implements PageAction {
    public static readonly type = `${PAGE_ACTION} update item on Standard Array`
    constructor(public event: UpdateOneItemOnStandardArrayEvent) { }
}

//#region Tree
export class UpdateTreeData implements PageAction {
    public static readonly type = `${PAGE_ACTION} update tree data`
    constructor(public event: UpdateTreeDataEvent) { }
}

export class AddSectionBoundDataForTree implements PageAction {
    public static readonly type = `${PAGE_ACTION} Add Section Bound Data for tree`
    constructor(public event: UpdateSectionBoundDataForTreeEvent) { }
}

export type All =
    PageAction |
    UpdateDatasourceAction |
    PageReadyAction |
    ChangeControlValueEvent |
    BeginRenderingPageSectionsAction |
    RenderingPageSectionAction |
    RenderedPageSectionAction |
    EndRenderingPageSectionsAction |
    AddSectionBoundData |
    GatherSectionValidations |
    SectionValidationStateAction |
    CompleteGatherSectionValidations |
    OnDestroyingPage |
    AddSectionBoundDataForStandardArray |
    InsertOneItemForStandardArray |
    RemoveOneItemForStandardArray |
    UpdateOneItemForStandardArray |
    OpenInsertDialogForStandardArray |
    CloseDialogForStandardArray