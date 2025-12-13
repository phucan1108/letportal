import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { HttpServiceOptions } from 'services/portal.service';
import { StaticResources } from 'portal/resources/static-resources';
import { JsonEditorOptions, JsonEditorComponent } from 'ang-jsoneditor';
import { NGXLogger } from 'ngx-logger';

@Component({
    selector: 'let-httpoptions',
    templateUrl: './httpoptions.component.html'
})
export class HttpOptionsComponent implements OnInit {
    @ViewChild('jsonPayloadEditor', { static: true }) jsonPayloadEditor: JsonEditorComponent
    public editorOptions1: JsonEditorOptions = new JsonEditorOptions()

    jsonHttpBodyData: any = {}

    @Input()
    httpOptions: HttpServiceOptions

    @Output()
    changed = new EventEmitter<any>()

    @Output()
    checkValid = new EventEmitter<boolean>()

    httpOptionsForm: FormGroup
    _httpMethods = StaticResources.httpCallMethods()

    constructor(
        private fb: FormBuilder,
        private logger: NGXLogger
    ) {
        this.editorOptions1.mode = 'code'
        this.editorOptions1.onChange = () => {
            try
            {
                const jsonStr = JSON.stringify(this.jsonPayloadEditor.get())
                if(!!jsonStr && jsonStr !== '{}'){
                    this.httpOptionsForm.get('httpJsonPayload').setValue(jsonStr)
                }
            }
            catch{

            }
        }
    }

    ngOnInit(): void {
        if(this.httpOptions.jsonBody){
            this.jsonHttpBodyData = JSON.parse(this.httpOptions.jsonBody)
        }
        this.initHttpOptionsForm()
    }

    initHttpOptionsForm(){
        this.httpOptionsForm = this.fb.group({
            httpCallUrl: [this.httpOptions.httpServiceUrl, Validators.required],
            httpCallMethod: [this.httpOptions.httpMethod, Validators.required],
            httpJsonPayload: [this.httpOptions.jsonBody],
            httpOutputProjection: [this.httpOptions.outputProjection],
            httpSuccessCode: [this.httpOptions.httpSuccessCode, Validators.required]
        })

        this.httpOptionsForm.valueChanges.subscribe(newValue => {
            this.changed.emit(this.combineHttpServiceOptions())
            this.checkValid.emit(this.httpOptionsForm.valid)
        })
    }

    onJsonEditorHtmlAttributeChange($event){
        this.httpOptionsForm.get('httpJsonPayload').setValue($event)
    }

    combineHttpServiceOptions(): HttpServiceOptions{
        const formValues = this.httpOptionsForm.value
        return {
            httpServiceUrl: formValues.httpCallUrl,
            httpMethod: formValues.httpCallMethod,
            httpSuccessCode: formValues.httpSuccessCode,
            outputProjection: formValues.httpOutputProjection,
            jsonBody: formValues.httpJsonPayload
        }
    }
}
