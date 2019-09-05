import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PortalPageComponent } from './portal-page.component';
import { DynamicListBuilderComponent } from './dynamic-list/builder/dynamic-list-builder.component';
import { DynamicListBuilderResolver } from './dynamic-list/resolver/dynimic-list.builder.resolve';
import { PageBuilderPageComponent } from './pages/builder/page-builder.page';
import { PageBuilderResolver } from './pages/resolver/page.builder.resolve';
import { PageRenderComponent } from './pages/render/page-render.page';
import { PageRenderResolver } from './pages/resolver/page.render.resolve';
import { MenuComponent } from './menu/menu.component';
import { MenuRouterResolver } from './menu/resolve/menu.router.resolve';
import { MenuProfilesComponent } from './menu-profiles/menu-profiles.component';
import { MenuProfilesRouterResolver } from './menu-profiles/resolve/menu-profiles.router.resolve';
import { RoleClaimsComponent } from './role-claims/role-claims.component';
import { RoleClaimsRouterResolver } from './role-claims/resolve/role-claims.router.resolve';
import { StandardPageComponent } from './standard/standard.component';
import { StandardResolver } from './standard/resolver/standard.resolve';

const routes: Routes = [
	{
		path: '',
		component: PortalPageComponent,
		children: [
			{
				path: 'dynamic-list/builder',
				redirectTo: 'dynamic-list/builder/'
			},
			{
				path: 'dynamic-list/builder/:dynamicId',
				component: DynamicListBuilderComponent,
				resolve: {
					dynamicList: DynamicListBuilderResolver
				}
			},
			{
				path: 'standard/builder',
				redirectTo: 'standard/builder/'
			},
			{
				path: 'standard/builder/:standardId',
				component: StandardPageComponent,
				resolve: {
					standard: StandardResolver
				}
			},
			{
				path: 'page/builder',
				redirectTo: 'page/builder/'
			},
			{
				path: 'page/builder/:pageId',
				component: PageBuilderPageComponent,
				resolve: {
					page: PageBuilderResolver
				}
			},
			{
				path: 'page/:pageName',
				component: PageRenderComponent,
				resolve: {
					page: PageRenderResolver
				}
			},
			{
				path: 'form/:dynamicFormName',
				component: PageRenderComponent,
				resolve: {
					dynamicForm: PageRenderResolver
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
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PortalRoutingModule {}
