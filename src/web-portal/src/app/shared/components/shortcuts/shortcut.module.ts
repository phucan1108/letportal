import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShortcutUtil } from './shortcut-util';
import { CustomActionEntityDialogComponent } from './custom-action-dialog/custom-action-dialog.component';
import { MatProgressBarModule, MatSnackBarModule, MatDialogModule, MatButtonModule, MatIconModule } from '@angular/material';
import { ActionNotificationComponent } from './action-natification/action-notification.component';

@NgModule({
    declarations: [
        CustomActionEntityDialogComponent,
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
        CustomActionEntityDialogComponent,
        ActionNotificationComponent
    ],
    exports: [
        CustomActionEntityDialogComponent
    ],
    providers: [
        ShortcutUtil
    ],
})
export class ShortcutModule {}