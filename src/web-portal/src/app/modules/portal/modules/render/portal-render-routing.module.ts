import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PortalRenderPageComponent } from './portal-render-page.component';
import { PageRenderPage } from './pages/page-render.page';
import { PageRenderResolve } from './resolve/page.render.resolve';

const routes: Routes = [
	{
		path: ':pageName',
		component: PageRenderPage,
		resolve: {
			page: PageRenderResolve
		}
	}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PortalRenderRoutingModule {}
