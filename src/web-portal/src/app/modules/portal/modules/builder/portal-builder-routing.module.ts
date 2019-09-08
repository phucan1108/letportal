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
			}
        ]
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PortalBuilderRoutingModule {}