import { Page, PageControl, ShellOption } from 'services/portal.service';
import { AuthUser } from '../security/auth.model';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import * as _ from 'lodash';

export interface PageResponse{
    page: Page,
    allowAccess: boolean
}

export interface RenderingPageSectionState{
    sectionName: string,
    sectionClass: string,
    state: RenderingSectionState
}

export enum RenderingSectionState{
    Init = 0,
    Rendering = 1,
    Complete = 2
}

export interface PageRenderedControl<T> extends PageControl{
    defaultOptions: T
}

export interface PageControlEvent {
    name: string
    sectionName: string
    controlName: string
    triggeredByEvent: string
    data: any
}

export interface PageLoadedDatasource {
    name: string,
    data: any    
}

export interface PageSectionBoundData{
    name: string,
    data: any,
    isKeptDataName: boolean
}

export interface TriggeredControlEvent{
    controlFullName: string,
    eventType: string,
    fullEventType: string,
    data: any
}

export interface DefaultControlOptions {
    label: string
    placeholder: string
    disabled: string
    hidden: string
    bindname: string
    // For Textarea only
    textarearows: number
    multiple: boolean
    // For Uploader only
    allowfileurl: boolean
    saveonchange: boolean
    // For checkbox/slider only
    allowZero: boolean   
    allowYesNo: boolean
    
    checkDisabled: boolean
    checkedHidden: boolean
}

export interface MapDataControl{
    controlFullName: string,
    sectionMapName: string,
    bindName: string
}

export interface PageShellData{
    user: AuthUser,
    claims: any,
    options: any,
    data: any,
    configs: any,
    appsettings: any,
    queryparams: any
}

export class ControlOptions{
    label: string
    placeholder: string
    hidden: boolean
    disabled: boolean
    bindname: string

    public static getDefaultControlOptions(control: PageControl){
        let labelOpt = ControlOptions.LabelOption
        labelOpt.value = control.name
        let placeholderOpt = ControlOptions.PlaceholderOption
        let disabledOpt = ControlOptions.DisabledOption
        let hiddenOpt = ControlOptions.HiddenOption
        hiddenOpt.value = (control.name === 'id' || control.name === '_id' || (control.name.toLowerCase().indexOf('id') > -1)) ? 'true' : 'false'
        let bindnameOpt = ControlOptions.BindnameOption
        bindnameOpt.value = control.name
        return [
            labelOpt,
            placeholderOpt,
            disabledOpt,
            hiddenOpt,
            bindnameOpt
        ]
    }

    public static LabelOption: ExtendedShellOption = {
        id: '',
        description: 'Label will be displayed when it isn\'t empty',
        key: 'label',
        value: '',
        allowDelete: false
    }

    public static PlaceholderOption: ExtendedShellOption = {
        id: '',
        description: 'Placeholder will be displayed when it isn\'t empty',
        key: 'placeholder',
        value: '',
        allowDelete: false
    }

    public static DisabledOption: ExtendedShellOption = {
        id: '',
        description: 'Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false',
        key: 'disabled',
        value: 'false',
        allowDelete: false
    }

    public static HiddenOption: ExtendedShellOption = {
        id: '',
        description: 'Hidden is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: true',
        key: 'hidden',
        value: 'false',
        allowDelete: false
    }

    public static BindnameOption: ExtendedShellOption = {
        id: '',
        description: 'Bind Name is a name which helps to map the data in or out',
        key: 'bindname',
        value: '',
        allowDelete: false
    }

    public static MultipleOptions: ExtendedShellOption = {
        id: '',
        description: 'Multiple options can be selected. Default: false',
        key: 'multiple',
        value: 'false',
        allowDelete: false
    }

    public static TextareaRowsOption: ExtendedShellOption = {
        id: '',
        description: 'Rows attribute of textarea. Default: 6',
        key: 'textarearows',
        value: '6',
        allowDelete: false
    }

    public static UploaderAllowFileUrl: ExtendedShellOption = {
        id: '',
        description: 'Allow a uploader set downloadable url back to a field after saving instead of file id. Default: false',
        key: 'allowfileurl',
        value: 'false',
        allowDelete: false
    }

    public static UploaderSaveOnChange: ExtendedShellOption = {
        id: '',
        description: 'Allow a uploader upload a file after user changes. Default: false',
        key: 'saveonchange',
        value: 'false',
        allowDelete: false
    }
}