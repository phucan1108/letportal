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
                loadChildren: './modules/builder/portal-builder.module#PortalBuilderModule',
                canActivate: [AuthGuard, CanActivePortal]
            },
            {
                path: 'page',
                loadChildren: './modules/render/portal-render.module#PortalRenderModule',                
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
