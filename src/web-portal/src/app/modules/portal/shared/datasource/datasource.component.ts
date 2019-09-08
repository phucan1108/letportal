import { Component, OnInit, ViewChild, Inject, ChangeDetectorRef } from '@angular/core';
import { JsonEditorComponent, JsonEditorOptions } from 'ang-jsoneditor';
import { FormGroup, FormBuilder, Validators, NgForm } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DynamicListDataSourceComponent } from '../../modules/builder/components/dynamic-list/components/dynamic-list.datasource.component';
import { Datasource, DatasourceClient, DatasourceType, DatabaseConnection, DatabasesClient } from 'services/portal.service';
import { Observable, BehaviorSubject } from 'rxjs';

@Component({
    selector: 'datasource-dialog',
    templateUrl: './datasource.component.html'
})
export class DatasourceDialogComponent implements OnInit {
    @ViewChild('formDirective') ngForm: NgForm;
    @ViewChild(JsonEditorComponent) editor: JsonEditorComponent;
    jsonOptions = new JsonEditorOptions();
    queryJsonData: any;

    databaseConnections: Observable<Array<DatabaseConnection>>;
    allDatasources: Array<Datasource> = [];
    allDatasources$: BehaviorSubject<Array<Datasource>> = new BehaviorSubject([]);
    dataSourceForm: FormGroup;

    dataSource: Datasource;
    dataSourceId: string;

    isAddingNewDatasource = false;

    _datasourceTypes = [
        { name: "Static", value: DatasourceType.Static },
        { name: "Database", value: DatasourceType.Database }
    ]

    constructor(public dialogRef: MatDialogRef<any>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private datasourceClient: DatasourceClient,
        private databaseClient: DatabasesClient) { }

    ngOnInit(): void {
        this.dataSourceForm = this.fb.group({
            id: [''],
            name: [''],
            datasourceType: [DatasourceType.Static],
            canCache: [false],
            databaseId: ['', Validators.required],
            query: ['']
        })
        
        if (this.data) {
            this.dataSourceId = this.data;
            this.datasourceClient.get(this.dataSourceId).subscribe(rep => {
                this.dataSource = rep;
                this.createDatasourceForm();
            })
        }

        this.datasourceClient.getAll().subscribe(rep => {
            this.allDatasources = rep;
            this.allDatasources$.next(this.allDatasources);
        });

        this.databaseConnections = this.databaseClient.getAll();


        this.jsonOptions.mode = 'code';
        // Hot fix for json editor
        this.jsonOptions.onChange = () => {
            try {
                this.dataSourceForm.get('query').setValue(JSON.stringify(this.editor.get()));
            }
            catch {

            }
        }
    }

    createDatasourceForm() {
        if (this.dataSource) {
            this.dataSourceForm = this.fb.group({
                id: [this.dataSourceId],
                name: [this.dataSource.name],
                datasourceType: [this.dataSource.datasourceType],
                canCache: [this.dataSource.canCache],
                databaseId: [this.dataSource.databaseId, Validators.required],
                query: [this.dataSource.query]
            })

            if (this.dataSource.query) {
                this.queryJsonData = JSON.parse(this.dataSource.query);
            }
        }
        else {
            this.dataSourceForm = this.fb.group({
                id: [''],
                name: [''],
                datasourceType: [DatasourceType.Static],
                canCache: [false],
                databaseId: ['', Validators.required],
                query: ['']
            })
        }
    }

    onSubmittingDataSource() {
        if (this.isAddingNewDatasource) {
            const newDatasource = this.combineFormData();

            this.datasourceClient.create({
                datasource: newDatasource
            }).subscribe(rep => {
                this.dialogRef.close(rep.id)
            })
        }
        else {
            this.dialogRef.close(this.dataSourceForm.value.id);
        }
    }

    combineFormData(): Datasource {
        let formValues = this.dataSourceForm.value

        return {
            id: formValues.id,
            canCache: formValues.canCache,
            databaseId: formValues.databaseId,
            name: formValues.name,
            datasourceType: formValues.datasourceType,
            query: formValues.query
        }
    }

    onAddingNewDatasource() {
        this.isAddingNewDatasource = true;
        this.dataSource = null;        
        this.ngForm.resetForm()
        this.dataSourceForm.reset()
        this.createDatasourceForm();
    }

    onValueChanges() {
        this.dataSourceForm.get('id').valueChanges.subscribe(newValue => {
            this.datasourceClient.get(this.dataSourceId).subscribe(rep => {
                this.dataSource = rep;
                this.createDatasourceForm();
            })
        })
    }
}
