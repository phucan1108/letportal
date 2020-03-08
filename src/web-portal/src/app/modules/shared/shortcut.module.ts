import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShortcutUtil } from './components/shortcuts/shortcut-util';
import { ConfirmationDialogComponent } from './components/shortcuts/custom-action-dialog/custom-action-dialog.component';
import { MatProgressBarModule, MatSnackBarModule, MatDialogModule, MatButtonModule, MatIconModule } from '@angular/material';
import { ActionNotificationComponent } from './components/shortcuts/action-natification/action-notification.component';

@NgModule({
    declarations: [
        ConfirmationDialogComponent,
        ActionNotificationComponent,
    ],
    imports: [ 
        CommonModule,
        MatProgressBarModule,
        MatSnackBarModule,        
        MatDialogModule,
        MatButtonModule,
        MatIconModule
    ],
    entryComponents: [
        ConfirmationDialogComponent,
        ActionNotificationComponent
    ],
    exports: [
        ConfirmationDialogComponent
    ],
    providers: [
        ShortcutUtil
    ],
})
export class SharedModule {}