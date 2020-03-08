import { Page, PageButton } from 'services/portal.service';
import { PageControlActionEvent, PageLoadedDatasource, RenderingPageSectionState, PageSectionBoundData, MapDataControl } from 'app/core/models/page.model';

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
    constructor() { }
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
    OnDestroyingPage