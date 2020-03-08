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
	MAT_DIALOG_DATA,
	MAT_DATE_FORMATS
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
import { FileUploaderComponent } from './controls/file-uploader.component';
import { IconPickerComponent } from './controls/icon-picker.component';
import { JsonEditorCustomComponent } from './controls/json-editor.component';
import { DynamicListRenderComponent } from './components/dynamic-list/dynamic-list.render.component';
import { AdvancedFilterDialogComponent } from './components/dynamic-list/components/advancedfilter-dialog.component';
import { DynamicListDataDialogComponent } from './components/dynamic-list/components/dynamic-list-data-dialog.component';
import { DynamicListCommandComponent } from './components/dynamic-list/components/dynamic-list.command.component';
import { DynamicListFiltersComponent } from './components/dynamic-list/components/dynamic-list.filters.component';
import { DynamicListGridComponent } from './components/dynamic-list/components/dynamic-list.grid.component';
import { QuillModule } from 'ngx-quill';
import { NgJsonEditorModule } from 'ang-jsoneditor';
import { NgxChartsModule } from '@swimlane/ngx-charts'
import { ChartRenderComponent } from './components/chart/chart-render.component';
import { ChartFilterRenderComponent } from './components/chart/chart-filter-render.component';
import { FilterCheckboxComponent } from './components/chart/controls/filter-checkbox.component';
import { FilterDatepickerComponent } from './components/chart/controls/filter-datepicker.component';
import { FilterSelectComponent } from './components/chart/controls/filter-select.component';
import { FilterNumberComponent } from './components/chart/controls/filter-number.component';
import { AngularMarkdownEditorModule } from 'angular-markdown-editor'
import { FilterRadioComponent } from './components/chart/controls/filter-radio.component';
import { MarkdownModule } from 'ngx-markdown';
import { CoreModule } from 'app/core/core.module';
import { AutocompleteMultipleComponent } from './controls/autocomplete-multiple.component';
import { GeneralControlComponent } from './controls/general-control.component';

export const FULL_MONTH_FORMATS = {
	parse: {
	  dateInput: 'LL',
	},
	display: {
	  dateInput: 'LL',
	  monthYearLabel: 'MMM YYYY',
	  dateA11yLabel: 'LL',
	  monthYearA11yLabel: 'MMMM YYYY',
	},
  };
  

@NgModule({
	imports: [
		CoreModule.forChild(),
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
		}),
		NgxChartsModule,
		AngularMarkdownEditorModule.forRoot(),
		MarkdownModule.forRoot()
	],
	entryComponents: [
		DynamicListDataDialogComponent
	],
	exports: [RouterModule],
	providers: [
		MatIconRegistry,
		PageRenderResolve,
		{ provide: MAT_DIALOG_DATA, useValue: [] },
		{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } },
		{provide: MAT_DATE_FORMATS, useValue: FULL_MONTH_FORMATS}
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
		AutocompleteMultipleComponent,

		// Dynamic Lists Render,
		DynamicListRenderComponent,
		AdvancedFilterDialogComponent,
		DynamicListDataDialogComponent,
		DynamicListCommandComponent,
		DynamicListFiltersComponent,
		DynamicListGridComponent,

		// Chart Render
		ChartRenderComponent,
		ChartFilterRenderComponent,
		FilterCheckboxComponent,
		FilterDatepickerComponent,
		FilterSelectComponent,
		FilterNumberComponent,
		FilterRadioComponent
	]
})
export class PortalRenderModule { }
