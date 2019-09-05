import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule, MatFormFieldModule, MatDatepickerModule, MatAutocompleteModule, MatListModule, MatSliderModule, MatCardModule, MatSelectModule, MatButtonModule, MatIconModule, MatNativeDateModule, MatSlideToggleModule, MatCheckboxModule, MatMenuModule, MatTabsModule, MatTooltipModule, MatSidenavModule, MatProgressBarModule, MatProgressSpinnerModule, MatSnackBarModule, MatTableModule, MatGridListModule, MatToolbarModule, MatExpansionModule, MatDividerModule, MatSortModule, MatStepperModule, MatChipsModule, MatPaginatorModule, MatDialogModule, MatRadioModule, MatTreeModule, MatIconRegistry, MAT_DIALOG_DATA } from '@angular/material';
import { NgJsonEditorModule } from 'ang-jsoneditor';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MomentumTableModule } from 'momentum-table';
import { ScrollDispatchModule } from '@angular/cdk/scrolling';
import { MatMomentDateModule, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { QuillModule } from 'ngx-quill';
import { CommandModalComponent } from './dynamic-list/builder/components/commands/command-dialog.component';
import { DynamicListDataSourceComponent } from './dynamic-list/builder/components/dynamic-list.datasource.component';
import { DatasourceDialogComponent } from './shared/datasource/datasource.component';
import { BuilderDnDComponent } from './pages/builder/components/page-builders/builder-dnd.component';
import { TextboxComponent } from './pages/controls/textbox.component';
import { SectionDialogComponent } from './pages/builder/components/page-builders/section-dialog.component';
import { ControlsGridComponent } from './standard/controls/controls-grid.component';
import { ControlDialogComponent } from './standard/controls/control-dialog.component';
import { PageButtonGridComponent } from './pages/builder/components/page-buttons/page-button-grid.component';
import { PageButtonDialogComponent } from './pages/builder/components/page-buttons/page-button-dialog.component';
import { PageEventDialogComponent } from './pages/builder/components/page-events/page-event-dialog.component';
import { PageEventGridComponent } from './pages/builder/components/page-events/page-event-grid.component';
import { SuggestionHintComponent } from './shared/suggestion-hint/suggestion-hint.component';
import { PageShellOptionDialogComponent } from './pages/builder/components/page-shells/page-shell-option-dialog.component';
import { GeneralControlComponent } from './pages/controls/general-control.component';
import { MenuDialogComponent } from './menu/components/menu-dialog.component';
import { ClaimDialogComponent } from './shared/claims/claim-dialog.component';
import { PageInfoComponent } from './shared/pages/page-info.component';
import { ShellOptionComponent } from './shared/shelloptions/shelloption.component';
import { CommandGridComponent } from './dynamic-list/builder/components/commands/command-grid.component';
import { DatabaseOptionComponent } from './shared/databasepopulation/database-opt.component';
import { FilterOptionComponent } from './dynamic-list/builder/components/filters/filter-option.component';
import { ColumnGridComponent } from './dynamic-list/builder/components/columns/column-grid.component';
import { ParamsDialogComponent } from './shared/databasepopulation/params-dialog.component';
import { DatasourceOptionsDialogComponent } from './shared/datasourceopts/datasourceopts.component';
import { ControlEventsDialogComponent } from './standard/controls/control-events.dialog.component';
import { PageDatasourceDialogComponent } from './pages/builder/components/page-datasources/page-datasource.dialog.component';
import { CommandOptionsComponent } from './shared/button-options/commandoptions.component';
import { ShellOptionDialogComponent } from './shared/shelloptions/shelloption.dialog.component';
import { PageButtonRouteDialogComponent } from './pages/builder/components/page-buttons/page-button-route.component';
import { ButtonOptionsComponent } from './shared/button-options/buttonoptions.component';
import { PageButtonOptionsDialogComponent } from './pages/builder/components/page-buttons/page-button-options.component';
import { AsyncValidatorDialogComponent } from './standard/controls/control-async-validator.dialog.component';
import { PortalPageComponent } from './portal-page.component';
import { PageBuilderPageComponent } from './pages/builder/page-builder.page';
import { DynamicListBuilderComponent } from './dynamic-list/builder/dynamic-list-builder.component';
import { MenuComponent } from './menu/menu.component';
import { ClaimTableComponent } from './shared/claims/claim-table.component';
import { MenuProfilesComponent } from './menu-profiles/menu-profiles.component';
import { RoleClaimsComponent } from './role-claims/role-claims.component';
import { StandardPageComponent } from './standard/standard.component';
import { ListDatasourceComponent } from './dynamic-list/builder/components/datasource/list-datasource.component';
import { StandardPopulationComponent } from './standard/standard-population/standard-population.component';
import { StandardControlsListComponent } from './pages/builder/components/page-builders/standard-controls-list.component';
import { HttpOptionsComponent } from './shared/httpoptions/httpoptions.component';
import { PageDatasourceGridComponent } from './pages/builder/components/page-datasources/page-datasource.grid.component';
import { SectionTemplate } from './pages/render/components/section-template.directive';
import { PageRouteComponent } from './pages/builder/components/page-buttons/page-route.component';
import { DatabaseOptionsComponent } from './shared/databaseoptions/databaseoptions.component';
import { RouterModule } from '@angular/router';
import { DynamicListRouterResolver } from './dynamic-list/resolver/dynamic-list.router.resolve';
import { DynamicListBuilderResolver } from './dynamic-list/resolver/dynimic-list.builder.resolve';
import { PageBuilderResolver } from './pages/resolver/page.builder.resolve';
import { MenuRouterResolver } from './menu/resolve/menu.router.resolve';
import { MenuProfilesRouterResolver } from './menu-profiles/resolve/menu-profiles.router.resolve';
import { RoleClaimsRouterResolver } from './role-claims/resolve/role-claims.router.resolve';
import { StandardResolver } from './standard/resolver/standard.resolve';

@NgModule({
    declarations: [
        PortalPageComponent,
		PageBuilderPageComponent,
		DynamicListBuilderComponent,
		CommandModalComponent,
		DynamicListDataSourceComponent,
		DatasourceDialogComponent,
		BuilderDnDComponent,
		TextboxComponent,
		SectionDialogComponent,
		ControlsGridComponent,
		ControlDialogComponent,
		PageButtonGridComponent,
		PageButtonDialogComponent,
		PageEventDialogComponent,
		PageEventGridComponent,
		SuggestionHintComponent,
		PageShellOptionDialogComponent,
		GeneralControlComponent,
		MenuComponent,
		MenuDialogComponent,
		ClaimTableComponent,
		ClaimDialogComponent,
		MenuProfilesComponent,
		PageInfoComponent,
		ShellOptionComponent,
		CommandGridComponent,
		DatabaseOptionComponent,
		FilterOptionComponent,
		ColumnGridComponent,
		ParamsDialogComponent,
		RoleClaimsComponent,
		StandardPageComponent,
		ListDatasourceComponent,
		StandardPopulationComponent,
		DatasourceOptionsDialogComponent,
		StandardControlsListComponent,
		ControlEventsDialogComponent,
		HttpOptionsComponent,
		PageDatasourceGridComponent,
		PageDatasourceDialogComponent,
		SectionTemplate,
		CommandOptionsComponent,
		ShellOptionDialogComponent,
		PageButtonRouteDialogComponent,
		ButtonOptionsComponent,
		PageButtonOptionsDialogComponent,
		PageRouteComponent,
		AsyncValidatorDialogComponent,
		DatabaseOptionsComponent
    ],
    imports: [
        CommonModule,
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
        NgJsonEditorModule,
        DragDropModule,
        MomentumTableModule,
        ScrollDispatchModule,
        MatMomentDateModule,
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
        })],
    entryComponents: [
        CommandModalComponent,
		DynamicListDataSourceComponent,
		DatasourceDialogComponent,
		BuilderDnDComponent,
		TextboxComponent,
		SectionDialogComponent,
		ControlsGridComponent,
		ControlDialogComponent,
		PageButtonGridComponent,
		PageButtonDialogComponent,
		PageEventDialogComponent,
		PageEventGridComponent,
		SuggestionHintComponent,
        PageShellOptionDialogComponent,        
		GeneralControlComponent,
		MenuDialogComponent,
		ClaimDialogComponent,
		PageInfoComponent,
		ShellOptionComponent,
		CommandGridComponent,
		DatabaseOptionComponent,
		FilterOptionComponent,
		ColumnGridComponent,
		ParamsDialogComponent,
		DatasourceOptionsDialogComponent,
		ControlEventsDialogComponent,
		PageDatasourceDialogComponent,
		CommandOptionsComponent,
		ShellOptionDialogComponent,
		PageButtonRouteDialogComponent,
		ButtonOptionsComponent,
		PageButtonOptionsDialogComponent,
		AsyncValidatorDialogComponent
    ],
    exports: [RouterModule],
    providers: [
        MatIconRegistry,
		DynamicListRouterResolver,
		DynamicListBuilderResolver,
		PageBuilderResolver,
		MenuRouterResolver,
		MenuProfilesRouterResolver,
		RoleClaimsRouterResolver,
		StandardResolver,
		{ provide: MAT_DIALOG_DATA, useValue: [] },
		{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } }		
    ],
})
export class PortalBuilderModule { }