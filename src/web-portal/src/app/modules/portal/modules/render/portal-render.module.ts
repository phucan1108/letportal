import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import {
	MatIconRegistry,
	MatInputModule,
	MatDatepickerModule,
	MatFormFieldModule,
	MatAutocompleteModule,
	MatSliderModule,
	MatListModule,
	MatCardModule,
	MatSelectModule,
	MatButtonModule,
	MatIconModule,
	MatNativeDateModule,
	MatSlideToggleModule,
	MatCheckboxModule,
	MatMenuModule,
	MatTabsModule,
	MatTooltipModule,
	MatSidenavModule,
	MatProgressBarModule,
	MatProgressSpinnerModule,
	MatSnackBarModule,
	MatGridListModule,
	MatTableModule,
	MatExpansionModule,
	MatToolbarModule,
	MatSortModule,
	MatDividerModule,
	MatStepperModule,
	MatChipsModule,
	MatPaginatorModule,
	MatDialogModule,
	MatRadioModule,
	MatDialog,
	MatTreeModule,
	MAT_DIALOG_DATA
} from '@angular/material';
import { MatMomentDateModule, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from "@angular/material-moment-adapter";
import { DragDropModule } from '@angular/cdk/drag-drop'
import { DividedColumnsSectionComponent } from './components/standard/components/divided-columns-section.component';
import { ScrollDispatchModule } from '@angular/cdk/scrolling';
import { PageRenderPage } from './pages/page-render.page';
import { PageRenderResolve } from './resolve/page.render.resolve';
import { ActionCommandsSectionComponent } from './components/standard/components/action-commands-section.component';
import { PortalRenderRoutingModule } from './portal-render-routing.module';
import { MomentumTableModule } from 'momentum-table';
import { PageRenderSectionWrapperComponent } from './components/standard/components/page-render-section-wrapper.component';
import { PageRenderBuilderComponent } from './components/standard/components/page-render-builder.component';
import { PortalRenderPageComponent } from './portal-render-page.component';
import { GeneralControlComponent } from '../controls/general-control.component';
import { FileUploaderComponent } from '../controls/file-uploader.component';
import { IconPickerComponent } from '../controls/icon-picker.component';
import { JsonEditorCustomComponent } from '../controls/json-editor.component';
import { DynamicListRenderComponent } from './components/dynamic-list/dynamic-list.render.component';
import { AdvancedFilterDialogComponent } from './components/dynamic-list/components/advancedfilter-dialog.component';
import { DynamicListDataDialogComponent } from './components/dynamic-list/components/dynamic-list-data-dialog.component';
import { DynamicListCommandComponent } from './components/dynamic-list/components/dynamic-list.command.component';
import { DynamicListFiltersComponent } from './components/dynamic-list/components/dynamic-list.filters.component';
import { DynamicListGridComponent } from './components/dynamic-list/components/dynamic-list.grid.component';
import { QuillModule } from 'ngx-quill';
import { NgJsonEditorModule } from 'ang-jsoneditor';

@NgModule({
	imports: [
		CommonModule,
		PortalRenderRoutingModule,
		FormsModule,
		ReactiveFormsModule,
		MatInputModule,
		MatFormFieldModule,
		MatDatepickerModule,
		MatAutocompleteModule,
		MatListModule,
		MatSliderModule,
		MatCardModule,
		MatSelectModule,
		MatButtonModule,
		MatIconModule,
		MatNativeDateModule,
		MatSlideToggleModule,
		MatCheckboxModule,
		MatMenuModule,
		MatTabsModule,
		MatTooltipModule,
		MatSidenavModule,
		MatProgressBarModule,
		MatProgressSpinnerModule,
		MatSnackBarModule,
		MatTableModule,
		MatGridListModule,
		MatToolbarModule,
		MatExpansionModule,
		MatDividerModule,
		MatSortModule,
		MatStepperModule,
		MatChipsModule,
		MatPaginatorModule,
		MatDialogModule,
		MatRadioModule,
		MatTreeModule,
		DragDropModule,
		MomentumTableModule,
		ScrollDispatchModule,
		MatMomentDateModule,
		NgJsonEditorModule,
		QuillModule.forRoot({
            modules: {
                syntax: true,
                toolbar: [
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                    [{ 'script': 'sub' }, { 'script': 'super' }],
                    [{ 'indent': '-1' }, { 'indent': '+1' }],
                    ['link', 'image']
                ]
            }
        })
	],
	entryComponents: [
		DynamicListDataDialogComponent
	],
	exports: [RouterModule],
	providers: [
		MatIconRegistry,
		PageRenderResolve,
		{ provide: MAT_DIALOG_DATA, useValue: [] },
		{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } }
	],
	declarations: [
		// Render Page Components
		ActionCommandsSectionComponent,
		DividedColumnsSectionComponent,
		PageRenderBuilderComponent,
		PageRenderSectionWrapperComponent,
		PageRenderPage,
		PortalRenderPageComponent,
		// Controls
		GeneralControlComponent,
		FileUploaderComponent,
		IconPickerComponent,
		JsonEditorCustomComponent,

		// Dynamic Lists Render,
		DynamicListRenderComponent,
		AdvancedFilterDialogComponent,
		DynamicListDataDialogComponent,
		DynamicListCommandComponent,
		DynamicListFiltersComponent,
		DynamicListGridComponent
	]
})
export class PortalRenderModule { }
