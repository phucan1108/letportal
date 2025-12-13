import { Component, OnInit, ViewChild, ChangeDetectorRef, Inject } from '@angular/core';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'dynamic-list-datasource',
    templateUrl: './dynamic-list.datasource.component.html'
})
export class DynamicListDataSourceComponent implements OnInit {
    @ViewChild(JsonEditorComponent, { static: true }) editor: JsonEditorComponent;
    jsonOptions = new JsonEditorOptions();
    queryJsonData: any;

    dataSourceForm: FormGroup;

    constructor(public dialogRef: MatDialogRef<DynamicListDataSourceComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef) { }

    ngOnInit(): void {
        this.dataSourceForm = this.fb.group({
            jsonDataSource: ['', Validators.required]
        })

        this.jsonOptions.mode = 'code';
        this.queryJsonData = this.data ? JSON.parse(this.data) : {};
        // Hot fix for json editor
        this.jsonOptions.onChange = () => {
            try {
                this.dataSourceForm.get('jsonDataSource').setValue(JSON.stringify(this.editor.get()));
            }
            catch {

            }
        }
    }

    onSubmittingDataSource() {
        this.dialogRef.close(this.dataSourceForm.value.jsonDataSource)
    }
}
