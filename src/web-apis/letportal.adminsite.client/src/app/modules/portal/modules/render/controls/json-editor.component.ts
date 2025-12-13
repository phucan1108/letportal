import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { JsonEditorOptions, JsonEditorComponent } from 'ang-jsoneditor';
import { FormGroup } from '@angular/forms';

@Component({
    selector: 'let-json-editor',
    templateUrl: './json-editor.component.html'
})
export class JsonEditorCustomComponent implements OnInit {
    @ViewChild(JsonEditorComponent, { static: true }) editor: JsonEditorComponent;
    jsonOptions = new JsonEditorOptions();
    queryJsonData: any = '';

    @Input()
    formGroup: FormGroup

    @Input()
    formControlKey: string

    @Input()
    passingValue: any

    @Output()
    onEditorChange: EventEmitter<any> = new EventEmitter()

    constructor() {
        this.jsonOptions.mode = 'code';
        try
        {
            this.queryJsonData = JSON.parse(this.passingValue);
        }
        catch
        {
            this.queryJsonData = this.passingValue
        }

        // Hot fix for json editor
        this.jsonOptions.onChange = () => {
            try {
                this.onJsonEditorChange(this.editor.get())
            }
            catch {

            }
        }

    }

    ngOnInit(): void {

        this.formGroup.get(this.formControlKey).valueChanges.subscribe(newValue => {
            this.editor.set(JSON.parse(newValue))
        })
    }

    onJsonEditorChange(value: any) {
        this.formGroup.get(this.formControlKey).setValue(JSON.stringify(value))
        // this.onEditorChange.emit(JSON.stringify(value))
    }
}
