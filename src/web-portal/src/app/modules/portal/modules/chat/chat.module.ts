import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { MatProgressSpinnerModule, MatProgressBarModule, MatToolbarModule, MatButtonModule, MatSidenavModule, MatIconModule, MatListModule, MatGridListModule, MatCardModule, MatMenuModule, MatSnackBarModule, MatDialogModule, MatTreeModule, MatExpansionModule, MatBadgeModule, MatTooltipModule, MatFormFieldModule, MatInputModule } from '@angular/material';
import { ChatHeadComponent } from './components/chat-head/chat-head.component';
import { ChatSearchComponent } from './components/chat-search/chat-search.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
    declarations: [
        ChatHeadComponent,
        ChatSearchComponent
    ],
    imports: [ 
        CommonModule,
        ReactiveFormsModule,
        MatInputModule,
		MatFormFieldModule,
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
    exports: [
        ChatHeadComponent
    ],
    providers: [],
})
export class ChatModule {}