import { LayoutModule } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { NgModule, Provider } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTreeModule } from '@angular/material/tree';
import { TranslateModule } from '@ngx-translate/core';
import { CoreModule } from 'app/core/core.module';
import { InterceptorsProvider, PAGE_INTERCEPTORS } from 'app/core/interceptors/interceptor.provider';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { ALL_INTERCEPTORS } from '../customs/custom.config';
import { AppDashboardComponent } from './components/app-dashboard/app-dashboard.component';
import { NavigationComponent } from './components/navigation/navigation.component';
import { NotificationBoxComponent } from './components/notification/notification-box.component';
import { PagesRoutingModule } from './portal-pages-routing.module';
import { PagesWrapperComponent } from './portal-wrapper.component';


const mapToProvider = (interceptor: any): Provider => {
    return {
        provide: PAGE_INTERCEPTORS,
        useClass: interceptor,
        multi: true
    }
}
@NgModule({
    declarations: [
        PagesWrapperComponent,
        NavigationComponent,
        AppDashboardComponent,
        NotificationBoxComponent
    ],
    imports: [
        CoreModule.forChild(),
        CommonModule,
        PagesRoutingModule, 
        LayoutModule,
        MatProgressButtonsModule,
        MatProgressSpinnerModule,
        MatProgressBarModule,
        MatToolbarModule,
        MatButtonModule,
        MatSidenavModule,
        MatIconModule,
        MatListModule,
        MatGridListModule,
        MatCardModule,
        MatMenuModule,
        MatSnackBarModule,
        MatDialogModule,
        MatTreeModule,
        MatExpansionModule,
        MatBadgeModule,
        MatTooltipModule,
        TranslateModule
    ],
    exports: [],
    providers: [
        ...ALL_INTERCEPTORS,
        ...ALL_INTERCEPTORS.map(mapToProvider),
        {
            provide: InterceptorsProvider, useClass: InterceptorsProvider
        }
    ],
})
export class PortalPagesModule { }