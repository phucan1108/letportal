import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PortalPageComponent } from './portal-page.component';
import { PageRenderComponent } from './pages/render/page-render.page';
import { PageRenderResolver } from './pages/resolver/page.render.resolve';

const routes: Routes = [
	{
		path: '',
		component: PortalPageComponent,
		children: [			
			{
				path: 'page/:pageName',
				component: PageRenderComponent,
				resolve: {
					page: PageRenderResolver
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
