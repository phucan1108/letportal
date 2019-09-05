import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PagesRoutingModule } from './pages-routing.module';
import { PagesComponent } from './pages.component';
import { NavigationComponent } from './navigation/navigation.component';
import { LayoutModule } from '@angular/cdk/layout';
import { MatToolbarModule, MatButtonModule, MatSidenavModule, MatIconModule, MatListModule, MatGridListModule, MatCardModule, MatMenuModule, MatSnackBarModule, MatDialogModule, MatTreeModule, MatExpansionModule, MatProgressBarModule, MatProgressSpinnerModule } from '@angular/material';
import { PortalModule } from './portal/portal.module';
import { AppDashboardComponent } from './app-dashboard/app-dashboard.component';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
@NgModule({
    declarations: [
        PagesComponent,
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
export class PagesModule {}