import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PortalBuilderPageComponent } from './portal-builder.component';
import { DynamicListBuilderPage } from './pages/dynamic-list/dynamic-list-builder.page';
import { StandardPagePage } from './pages/standard/standard.page';
import { PageBuilderPage } from './pages/page-builder/page-builder.page';
import { PageBuilderResolve } from './resolve/page.builder.resolve';
import { MenuPage } from './pages/menu/menu.page';
import { MenuResolve } from './resolve/menu.resolve';
import { MenuProfilesPage } from './pages/menu-profiles/menu-profiles.page';
import { MenuProfilesResolve } from './resolve/menu-profiles.resolve';
import { RoleClaimsPage } from './pages/role-claims/role-claims.page';
import { RoleClaimsResolve } from './resolve/role-claims.resolve';
import { DynamicListBuilderResolve } from './resolve/dynamic-list.builder.resolve';
import { StandardResolve } from './resolve/standard.resolve';
import { ChartBuilderPage } from './pages/chart/chart-builder.page';
import { ChartBuilderResolve } from './resolve/chart.builder.resolve';
import { BackupBuilderPage } from './pages/backup/backup-builder.page';
import { BackupUploadpage } from './pages/backup/backup-upload.page';
import { BackupRestorePage } from './pages/backup/backup-restore.page';
import { BackupResolve } from './resolve/backup.resolve';

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
			}
        ]
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PortalBuilderRoutingModule {}