import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotificationRouterPage } from './notification-router.component';
import { NotificationBoxPage } from './pages/notification-box-page/notification-box-page.component';

const routes: Routes = [
    { 
        path: '', 
        component: NotificationRouterPage,
        children:[
            {
                path: 'box',
                component: NotificationBoxPage
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class NotificationRoutingModule {}
