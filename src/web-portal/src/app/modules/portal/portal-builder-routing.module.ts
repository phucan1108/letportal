import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PortalBuilderPageComponent } from './portal-builder.component';
import { DynamicListBuilderComponent } from './dynamic-list/builder/dynamic-list-builder.component';
import { DynamicListBuilderResolver } from './dynamic-list/resolver/dynimic-list.builder.resolve';
import { StandardPageComponent } from './standard/standard.component';
import { StandardResolver } from './standard/resolver/standard.resolve';
import { PageBuilderPageComponent } from './pages/builder/page-builder.page';
import { PageBuilderResolver } from './pages/resolver/page.builder.resolve';
import { MenuComponent } from './menu/menu.component';
import { MenuRouterResolver } from './menu/resolve/menu.router.resolve';
import { MenuProfilesComponent } from './menu-profiles/menu-profiles.component';
import { MenuProfilesRouterResolver } from './menu-profiles/resolve/menu-profiles.router.resolve';
import { RoleClaimsComponent } from './role-claims/role-claims.component';
import { RoleClaimsRouterResolver } from './role-claims/resolve/role-claims.router.resolve';

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
				component: DynamicListBuilderComponent,
				resolve: {
					dynamicList: DynamicListBuilderResolver
				}
            },
            {
				path: 'standard',
				redirectTo: 'standard/'
			},
			{
				path: 'standard/:standardId',
				component: StandardPageComponent,
				resolve: {
					standard: StandardResolver
				}
			},
			{
				path: 'page',
				redirectTo: 'page/'
			},
			{
				path: 'page/:pageId',
				component: PageBuilderPageComponent,
				resolve: {
					page: PageBuilderResolver
				}
            },
            {
				path: 'menus/:appId',
				component: MenuComponent,
				resolve: {
					app: MenuRouterResolver
				}
			},
			{
				path: 'menu-profiles/:appId',
				component: MenuProfilesComponent,
				resolve: {
					app: MenuProfilesRouterResolver
				}
			},
			{
				path: 'roles/:roleName/claims',
				component: RoleClaimsComponent,
				resolve: {
					roleClaims: RoleClaimsRouterResolver
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