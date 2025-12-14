import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InstallationPage } from './pages/installation/installation.page';

const routes: Routes = [
  {
    path: '',
    component: InstallationPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InstallationRoutingModule { }
