import { Page, PageControl, ShellOption } from 'services/portal.service';
import { AuthUser } from '../security/auth.model';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import * as _ from 'lodash';
import { ObjectUtils } from '../utils/object-util';

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
    customErrorMessages: CustomValidatorMessage[]
}

export interface CustomValidatorMessage {
    errorName: string
    errorMessage: string
}

export interface PageControlActionEvent {
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

export interface PageSectionStandardArrayBoundData{
    name: string,
    data: any,
    isKeptDataName: boolean,
    allowUpdateParts: boolean
}

export interface OpenInsertDialogOnStandardArrayEvent{
    sectionName: string,
    data: any,
    identityKey: any,
    allowUpdateParts: boolean,
    sectionMap: MapDataControl[]
}

export interface AddOneItemOnStandardArrayEvent{
    sectionName: string,
    isKeptDataName: boolean,
    allowUpdateParts: boolean
}

export interface RemoveOneItemOnStandardArrayEvent{
    sectionName: string,
    isKeptDataName: boolean,
    identityKey: string,
    removeItemKey: string,
    allowUpdateParts: boolean
}

export interface UpdateOneItemOnStandardArrayEvent{
    sectionName: string,
    isKeptDataName: boolean,
    identityKey: string,
    allowUpdateParts: boolean
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
    queryparams: any,
    parent: any
}

export class ControlOptions{

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
        description: 'Allow an uploader set downloadable url back to a field after saving instead of file id. Default: false',
        key: 'allowfileurl',
        value: 'false',
        allowDelete: false
    }

    public static UploaderSaveOnChange: ExtendedShellOption = {
        id: '',
        description: 'Allow an uploader upload a file after user changes. Default: false',
        key: 'saveonchange',
        value: 'false',
        allowDelete: false
    }
    label: string
    placeholder: string
    hidden: boolean
    disabled: boolean
    bindname: string

    public static getDefaultControlOptions(control: PageControl){
        const labelOpt = ObjectUtils.clone(ControlOptions.LabelOption)
        labelOpt.value = control.name
        const placeholderOpt = ObjectUtils.clone(ControlOptions.PlaceholderOption)
        const disabledOpt = ObjectUtils.clone(ControlOptions.DisabledOption)
        const hiddenOpt = ObjectUtils.clone(ControlOptions.HiddenOption)
        hiddenOpt.value = (control.name === 'id' || control.name === '_id' || (control.name.toLowerCase().indexOf('id') > -1)) ? 'true' : 'false'
        const bindnameOpt = ObjectUtils.clone(ControlOptions.BindnameOption)
        bindnameOpt.value = control.name
        return [
            labelOpt,
            placeholderOpt,
            disabledOpt,
            hiddenOpt,
            bindnameOpt
        ]
    }
}