import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ErrorComponent } from './modules/error/error.component';

const routes: Routes = [
  {
    path: '',
    loadChildren: './modules/login/login.module#LoginModule'
  },
  {
    path: 'portal',
    loadChildren: './modules/portal/portalpages.module#PortalPagesModule'
  },
  {
    path: '404',
    component: ErrorComponent
  },  
  {
    path: '**', 
    redirectTo: '/404'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {onSameUrlNavigation: 'reload'})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
