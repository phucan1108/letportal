import { Chart, DynamicList, PageButton, PageControl, PageControlEvent, PageControlValidator, PageSection, StandardComponent } from 'services/portal.service';
import { DefaultControlOptions, PageRenderedControl } from './page.model';


export interface GroupControls {
    controlsList: Array<PageRenderedControl<DefaultControlOptions>>,
    numberOfColumns: number
    isLineBreaker: boolean
    isCompositeGroup: boolean
    compositeGroupRef: string
    compositeGroupLabel?: string | undefined
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
    relatedChart: Chart
    relatedArrayStandard: StandardComponent
    relatedTreeStandard: StandardComponent
    relatedButtons: ExtendedPageButton[]
    isLoaded: boolean
    isBroken: boolean // Use for deleted component
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