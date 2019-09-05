import { PageControl, PageSection, PageButton, PageControlValidator, StandardComponent, DynamicList, PageControlEvent } from 'services/portal.service';
import { PageRenderedControl, DefaultControlOptions } from './page.model';


export interface GroupControls {
    controlsList: Array<PageRenderedControl<DefaultControlOptions>>,
    numberOfColumns: number
    isLineBreaker: boolean
}

export interface ExtendedPageControl extends PageControl {
    value: any
    readOnlyMode: boolean
}

export interface ExtendedStandardComponent extends StandardComponent{
}

export interface ExtendedPageSection extends PageSection {
    relatedStandard: StandardComponent
    relatedDynamicList: DynamicList
    isLoaded: boolean
}

export interface ExtendedPageButton extends PageButton {
    isHidden: boolean
}

export interface DynamicFormEventData{
    eventKey: string,
    eventData: any,
    returnedValue: any
}

export interface ExtendedFormValidator extends PageControlValidator{
    displayName: string,
    validatorOptionPlaceholder: string,
    hideOption: boolean
}

export interface ExtendedControlValidator{
    validatorName: string,
    validatorErrorMessage: string
}

export class DynamicFormOptions{
    
}

export interface ExtendedPageControlEvent extends PageControlEvent {
    id: string
}