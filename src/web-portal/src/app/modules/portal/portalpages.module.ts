import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PagesRoutingModule } from './portal-pages-routing.module';
import { PagesWrapperComponent } from './portal-wrapper.component';
import { LayoutModule } from '@angular/cdk/layout'
import { MatToolbarModule } from '@angular/material/toolbar'
import { MatButtonModule } from '@angular/material/button'
import { MatSidenavModule } from '@angular/material/sidenav'
import { MatIconModule } from '@angular/material/icon'
import { MatListModule } from '@angular/material/list'
import { MatGridListModule } from '@angular/material/grid-list'
import { MatCardModule } from '@angular/material/card'
import { MatMenuModule } from '@angular/material/menu'
import { MatSnackBarModule } from '@angular/material/snack-bar'
import { MatDialogModule } from '@angular/material/dialog'
import { MatTreeModule } from '@angular/material/tree'
import { MatExpansionModule } from '@angular/material/expansion'
import { MatProgressBarModule } from '@angular/material/progress-bar'
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'
import { MatBadgeModule } from '@angular/material/badge'
import { MatTooltipModule } from '@angular/material/tooltip'

import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { AppDashboardComponent } from './components/app-dashboard/app-dashboard.component';
import { NavigationComponent } from './components/navigation/navigation.component';
import { ChatModule } from './modules/chat/chat.module';
import { CoreModule } from 'app/core/core.module';

@NgModule({
    declarations: [
        PagesWrapperComponent,
        NavigationComponent,
        AppDashboardComponent
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
        MatTooltipModule
    ],
    exports: [],
    providers: [
    ],
})
export class PortalPagesModule {}