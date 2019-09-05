import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PagesComponent } from './pages.component';
import { PortalModule } from './portal/portal.module';
import { AppDashboardComponent } from './app-dashboard/app-dashboard.component';
import { CanActivePortal } from 'portal/router/canActivePortal';
import { AuthGuard } from 'app/core/security/authGuard';

const routes: Routes = [
    {
        path: '',
        component: PagesComponent,
        children: [
            {
                path: '',
                component: AppDashboardComponent,
                canActivate: [AuthGuard]
            },            
            {
                path: 'portal/builder',
                loadChildren: './portal/portal-builder.module#PortalBuilderModule',
                canActivate: [AuthGuard, CanActivePortal]
            },
            {
                path: 'portal',
                loadChildren: './portal/portal.module#PortalModule',                
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
