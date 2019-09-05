import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import {
	MatIconRegistry,
	MatIcon,
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
import { DividedColumnsSectionComponent } from './pages/render/components/divided-columns-section.component';
import { ScrollDispatchModule } from '@angular/cdk/scrolling';
import { PageRenderComponent } from './pages/render/page-render.page';
import { PageRenderResolver } from './pages/resolver/page.render.resolve';
import { ActionCommandsSectionComponent } from './pages/render/components/action-commands-section.component';
import { PortalRoutingModule } from './portal-routing.module';
import { MomentumTableModule } from 'momentum-table';
import { PageRenderSectionWrapperComponent } from './pages/render/components/page-render-section-wrapper.component';
import { PageRenderBuilderComponent } from './pages/render/components/page-render-builder.component';
import { PortalPageComponent } from './portal-page.component';
import { GeneralControlComponent } from './pages/controls/general-control.component';
import { FileUploaderComponent } from './pages/controls/file-uploader.component';
import { IconPickerComponent } from './pages/controls/icon-picker.component';
import { JsonEditorCustomComponent } from './pages/controls/json-editor.component';
import { DynamicListRenderComponent } from './dynamic-list/render/dynamic-list.render.component';
import { AdvancedFilterDialogComponent } from './dynamic-list/render/components/advancedfilter-dialog.component';
import { DynamicListDataDialogComponent } from './dynamic-list/render/components/dynamic-list-data-dialog.component';
import { DynamicListCommandComponent } from './dynamic-list/render/components/dynamic-list.command.component';
import { DynamicListFiltersComponent } from './dynamic-list/render/components/dynamic-list.filters.component';
import { DynamicListGridComponent } from './dynamic-list/render/components/dynamic-list.grid.component';
import { QuillModule } from 'ngx-quill';
import { NgJsonEditorModule } from 'ang-jsoneditor';

@NgModule({
	imports: [
		CommonModule,
		PortalRoutingModule,
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
		PageRenderResolver,
		{ provide: MAT_DIALOG_DATA, useValue: [] },
		{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } }
	],
	declarations: [
		ActionCommandsSectionComponent,
		DividedColumnsSectionComponent,
		PageRenderBuilderComponent,
		PageRenderSectionWrapperComponent,
		PageRenderComponent,
		PortalPageComponent,
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
export class PortalModule { }
