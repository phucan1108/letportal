import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PagesWrapperComponent } from './portal-wrapper.component';
import { CanActivePortal } from 'portal/router/canActivePortal';
import { AuthGuard } from 'app/core/security/authGuard';
import { AppDashboardComponent } from './components/app-dashboard/app-dashboard.component';

const routes: Routes = [
    {
        path: '',
        component: PagesWrapperComponent,
        children: [
            {
                path: 'dashboard',
                component: AppDashboardComponent,
                canActivate: [AuthGuard]
            },
            {
                path: 'builder',
                loadChildren: () => import('./modules/builder/portal-builder.module').then(m => m.PortalBuilderModule),
                canActivate: [AuthGuard, CanActivePortal]
            },
            {
                path: 'page',
                loadChildren: () => import('./modules/render/portal-render.module').then(m => m.PortalRenderModule),
                canActivate: [AuthGuard , CanActivePortal]
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: [
        CanActivePortal,
        AuthGuard
    ]
})
export class PagesRoutingModule { }
