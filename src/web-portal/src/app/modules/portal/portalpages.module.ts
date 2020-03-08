import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PagesRoutingModule } from './portal-pages-routing.module';
import { PagesWrapperComponent } from './portal-wrapper.component';
import { LayoutModule } from '@angular/cdk/layout';
import { MatToolbarModule, MatButtonModule, MatSidenavModule, MatIconModule, MatListModule, MatGridListModule, MatCardModule, MatMenuModule, MatSnackBarModule, MatDialogModule, MatTreeModule, MatExpansionModule, MatProgressBarModule, MatProgressSpinnerModule } from '@angular/material';

import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { AppDashboardComponent } from './components/app-dashboard/app-dashboard.component';
import { NavigationComponent } from './components/navigation/navigation.component';
@NgModule({
    declarations: [
        PagesWrapperComponent,
        NavigationComponent,
        AppDashboardComponent
    ],
    imports: [ 
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
        MatExpansionModule
    ],
    exports: [],
    providers: [
    ],
})
export class PortalPagesModule {}