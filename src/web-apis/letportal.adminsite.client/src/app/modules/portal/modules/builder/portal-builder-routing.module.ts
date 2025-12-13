import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppInstallationPage } from './pages/app-installation/app-installation.page';
import { AppPackagePage } from './pages/app-package/app-package.page';
import { BackupBuilderPage } from './pages/backup/backup-builder.page';
import { BackupRestorePage } from './pages/backup/backup-restore.page';
import { BackupUploadpage } from './pages/backup/backup-upload.page';
import { ChartBuilderPage } from './pages/chart/chart-builder.page';
import { CompositeBuilderPage } from './pages/composite-builder/composite-builder.page';
import { DynamicListBuilderPage } from './pages/dynamic-list/dynamic-list-builder.page';
import { LocalizationPage } from './pages/localization/localization.page';
import { MenuProfilesPage } from './pages/menu-profiles/menu-profiles.page';
import { MenuPage } from './pages/menu/menu.page';
import { PageBuilderPage } from './pages/page-builder/page-builder.page';
import { RoleClaimsPage } from './pages/role-claims/role-claims.page';
import { StandardPagePage } from './pages/standard/standard.page';
import { PortalBuilderPageComponent } from './portal-builder.component';
import { AppPackageResolve } from './resolve/app-package.resolve';
import { BackupResolve } from './resolve/backup.resolve';
import { ChartBuilderResolve } from './resolve/chart.builder.resolve';
import { CompositeControlResolve } from './resolve/composite-control.resolve';
import { DynamicListBuilderResolve } from './resolve/dynamic-list.builder.resolve';
import { LocaleResolve } from './resolve/locale.resolve';
import { MenuProfilesResolve } from './resolve/menu-profiles.resolve';
import { MenuResolve } from './resolve/menu.resolve';
import { PageBuilderResolve } from './resolve/page.builder.resolve';
import { RoleClaimsResolve } from './resolve/role-claims.resolve';
import { StandardResolve } from './resolve/standard.resolve';

const routes: Routes = [
    {
        path: '',
        component: PortalBuilderPageComponent,
        children: [
            {
				path: 'dynamic-list',
				redirectTo: 'dynamic-list/'
			},
			{
				path: 'dynamic-list/:dynamicId',
				component: DynamicListBuilderPage,
				resolve: {
					dynamicList: DynamicListBuilderResolve
				}
            },
            {
				path: 'standard',
				redirectTo: 'standard/'
			},
			{
				path: 'standard/:standardId',
				component: StandardPagePage,
				resolve: {
					standard: StandardResolve
				}
			},
			{
				path: 'page',
				redirectTo: 'page/'
			},
			{
				path: 'page/:pageId',
				component: PageBuilderPage,
				resolve: {
					page: PageBuilderResolve
				}
            },
            {
				path: 'menus/:appId',
				component: MenuPage,
				resolve: {
					app: MenuResolve
				}
			},
			{
				path: 'menu-profiles/:appId',
				component: MenuProfilesPage,
				resolve: {
					app: MenuProfilesResolve
				}
			},
			{
				path: 'roles/:roleName/claims',
				component: RoleClaimsPage,
				resolve: {
					roleClaims: RoleClaimsResolve
				}
			},
			{
				path: 'chart',
				redirectTo: 'chart/'
			},
			{
				path: 'chart/:chartid',
				component: ChartBuilderPage,
				resolve: {
					chart: ChartBuilderResolve
				}
			},
			{
				path: 'backup',
				component: BackupBuilderPage
			},
			{
				path: 'backup/upload',
				component: BackupUploadpage
			},
			{
				path: 'backup/restore/:backupid',
				component: BackupRestorePage,
				resolve: {
					backup: BackupResolve
				}
			},
			{
				path: 'localization/:appId',
				component: LocalizationPage
			},
			{
				path: 'localization/:appId/:localeId',
				component: LocalizationPage,
				resolve: {
					localization: LocaleResolve
				}
			},
			{
				path: 'app-package/:appId',
				component: AppPackagePage,
				resolve: {
					previewApp: AppPackageResolve
				}
			},
			{
				path: 'app-installation',
				component: AppInstallationPage
			},
			{
				path: 'composite-control-builder',
				component: CompositeBuilderPage
			},
			{
				path: 'composite-control-builder/:id',
				component: CompositeBuilderPage,
				resolve: {
					control: CompositeControlResolve
				}
			}
        ]
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PortalBuilderRoutingModule {}