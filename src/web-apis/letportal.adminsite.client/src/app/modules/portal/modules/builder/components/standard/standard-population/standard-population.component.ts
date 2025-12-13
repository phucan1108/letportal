import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { SharedDatabaseOptions, ExtractingSchemaQueryModel, ControlType, ValidatorType, DatasourceControlType, PageControlEvent, PageControl, EventActionType } from 'services/portal.service';
import { ExtendedPageControl } from 'app/core/models/extended.models';
import { Guid } from 'guid-typescript';
 
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-standard-population',
    templateUrl: './standard-population.component.html'
})
export class StandardPopulationComponent implements OnInit {
    heading = 'Controls Population'

    databaseOptions: SharedDatabaseOptions = {
        databaseConnectionId: '',
        entityName: '',
        query: ''
    }

    @Output()
    onPopulatedControls = new EventEmitter<any>();

    controls: ExtendedPageControl[] = []

    constructor(
        private translate: TranslateService
    ) { }

    ngOnInit(): void {
        this.translate.get('pageBuilder.standard.population.heading').subscribe(
            text => {
                this.heading = text
            }
        )
     }

    onSelectingEntity($event: ExtractingSchemaQueryModel) {
        this.populatingControls($event)
    }
    onPopulatingQuery($event: ExtractingSchemaQueryModel) {
        this.populatingControls($event)
    }
    onSelectingEntityName($event) {

    }
    onExtractingParam($event) {

    }

    populatingControls(schema: ExtractingSchemaQueryModel) {
        this.controls = []

        let controlCounter = 0;
        schema.columnFields?.forEach((field) => {
            const pageControl: ExtendedPageControl = {
                id: Guid.create().toString(),
                name: field.name === '_id' ? 'id' : field.name,
                type: ControlType.Textbox,
                order: controlCounter,
                datasourceOptions: {
                    type: DatasourceControlType.StaticResource,
                    datasourceStaticOptions: {
                        jsonResource: ''
                    },
                    databaseOptions: {
                        databaseConnectionId: '',
                        entityName: '',
                        query: ''
                    },
                    httpServiceOptions: {
                        httpMethod: '',
                        httpServiceUrl: '',
                        httpSuccessCode: '200',
                        outputProjection: '',
                        jsonBody: ''
                    },
                    triggeredEvents: ''
                },
                validators: [
                    {
                        validatorType: ValidatorType.Required,
                        isActive: true,
                        validatorMessage: 'Please fill out this field',
                        validatorOption: ''
                    }
                ],
                readOnlyMode: true,
                options: [],
                value: '',
                isActive: true
            }

            pageControl.pageControlEvents = this.generateEventsList(pageControl)
            const beautifulName = this.getBeautifulName(pageControl.name)
            const defaultOptions: ExtendedShellOption[] = [
                {
                    id: '',
                    description: 'Label will be displayed when it isn\'t empty',
                    key: 'label',
                    value: beautifulName,
                    allowDelete: false
                },
                {
                    id: '',
                    description: 'Tooltip will be displayed when it isn\'t empty',
                    key: 'placeholder',
                    value: this.generateTooltipByControlType(pageControl.type,beautifulName),
                    allowDelete: false
                },
                {
                    id: '',
                    description: 'Disabled is an expression without double curly braces. It is two-ways binding and can access params such as queryparams, options, etc. Default: false',
                    key: 'disabled',
                    value: 'false',
                    allowDelete: false
                },
                {
                    id: '',
                    description: 'Hidden is an expression without double curly braces. It is two-ways binding and can access params such as page.queryparams, page.options, section. Default: true',
                    key: 'hidden',
                    value: (pageControl.name === 'id' || pageControl.name === '_id' || (pageControl.name.toLowerCase().indexOf('id') > -1)) ? 'true' : 'false',
                    allowDelete: false
                },
                {
                    id: '',
                    description: 'Bind Name is a name which helps to map the data in or out',
                    key: 'bindname',
                    value: pageControl.name,
                    allowDelete: false
                },
                {
                    id: '',
                    description: 'Default value when no value is set',
                    key: 'defaultvalue',
                    value: '',
                    allowDelete: false
                },
                {
                    id: '',
                    description: 'Rendered is an expression without double curly braces. Used to indicate the control must be rendered. Default: true',
                    key: 'rendered',
                    value: 'true',
                    allowDelete: false
                }
            ]

            pageControl.options = defaultOptions

            if (field.name.indexOf('_') === 0) {
                pageControl.name = pageControl.name.substring(0);
                pageControl.isActive = false
            }

            switch (field.fieldType) {
                case 'string':
                    pageControl.type = ControlType.Textbox
                    break;
                case 'boolean':
                    pageControl.type = ControlType.Slide
                    break;
                case 'number':
                    pageControl.type = ControlType.Number
                    break
                case 'datetime':
                    pageControl.type = ControlType.DateTime
                    break
                case 'list':
                    pageControl.type = ControlType.Textarea
                    break
                case 'document':
                    pageControl.type = ControlType.Textarea
                    break
            }

            this.controls.push(pageControl)

            controlCounter++;
        })

        this.onPopulatedControls.emit(this.controls)
    }

    generateTooltipByControlType(controlType: ControlType, label: string) {
        switch (controlType) {
            case ControlType.Textbox:
                return 'Please enter ' + label
            case ControlType.Select:
            case ControlType.AutoComplete:
                return 'Please choose one option'
            case ControlType.Number:
                return 'Please enter ' + label
            case ControlType.Slide:
            case ControlType.Checkbox:
                return 'Turn On/Off'
            case ControlType.DateTime:
                return 'Please choose one date'
        }
    }

    generateEventsList(control: PageControl): PageControlEvent[] {
        switch (control.type) {
            case ControlType.Label:
                return []
            case ControlType.AutoComplete:
                return [
                    {
                        eventName: `${control.name}_change`, eventActionType: EventActionType.TriggerEvent, triggerEventOptions: { eventsList: [] },
                        eventHttpServiceOptions: {
                            httpServiceUrl: '',
                            httpMethod: 'Get',
                            boundData: [],
                            httpSuccessCode: '200',
                            jsonBody: '',
                            outputProjection: ''
                        },
                        eventDatabaseOptions: {
                            databaseConnectionId: '',
                            entityName: '',
                            query: '',
                            outputProjection: '',
                            boundData: []
                        }
                    }
                ]
            default:
                return [
                    {
                        eventName: `${control.name}_change`, eventActionType: EventActionType.TriggerEvent, triggerEventOptions: { eventsList: [] },
                        eventHttpServiceOptions: {
                            httpServiceUrl: '',
                            httpMethod: 'Get',
                            boundData: [],
                            httpSuccessCode: '200',
                            jsonBody: '',
                            outputProjection: ''
                        },
                        eventDatabaseOptions: {
                            databaseConnectionId: '',
                            entityName: '',
                            query: '',
                            outputProjection: '',
                            boundData: []
                        }
                    }
                ]
        }
    }

    private getBeautifulName(controlName: string){
        if(controlName === '_id')
            return 'id'
        const firstLetter = controlName[0].toUpperCase()
        const subStr = controlName.substr(1)
        const combine = firstLetter + subStr
        try{
            const splitted = combine.split(/(?=[A-Z])/);
            return splitted.join(' ').trim()
        }
        catch{
            return controlName
        }
    }
}
