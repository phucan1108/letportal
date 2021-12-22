import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { CoreModule } from 'app/core/core.module';
import { NotificationRouterPage } from './notification-router.component';
import { NotificationRoutingModule } from './notification-routing.module';
import { NotificationBoxPage } from './pages/notification-box-page/notification-box-page.component';

@NgModule({
    declarations: [
        NotificationRouterPage,
        NotificationBoxPage
    ],
    imports: [ 
        CoreModule.forChild(),
        CommonModule,
        ReactiveFormsModule,
        NotificationRoutingModule,  
        FormsModule,
		ReactiveFormsModule,
		MatInputModule,
		MatFormFieldModule,   
        MatCardModule,                   
        MatCheckboxModule, 
        MatBadgeModule,
        MatToolbarModule,
        MatIconModule,
        MatMenuModule,
        MatDividerModule,
        MatIconModule,
        MatTooltipModule,
        MatProgressSpinnerModule,
        MatDialogModule,
        MatButtonModule,
        MatGridListModule,
        MatFormFieldModule,
        MatInputModule,
        MatListModule,
        MatProgressBarModule,
        MatRadioModule,  
        MatButtonToggleModule,      
        ScrollingModule    
    ],
    exports: [
        RouterModule
    ],
    entryComponents:[
    ],
    providers: [],
})
export class NotificationModule {}