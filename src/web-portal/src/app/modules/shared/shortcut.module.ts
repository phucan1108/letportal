import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShortcutUtil } from './components/shortcuts/shortcut-util';
import { ConfirmationDialogComponent } from './components/shortcuts/custom-action-dialog/custom-action-dialog.component';
import { MatProgressBarModule } from '@angular/material/progress-bar'
import { MatSnackBarModule } from '@angular/material/snack-bar'
import { MatDialogModule } from '@angular/material/dialog'
import { MatButtonModule } from '@angular/material/button'
import { MatIconModule } from '@angular/material/icon'
import { ActionNotificationComponent } from './components/shortcuts/action-natification/action-notification.component';
import { EventDialogComponent } from './components/shortcuts/event-dialog/event-dialog.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
    declarations: [
        ConfirmationDialogComponent,
        ActionNotificationComponent,
        EventDialogComponent
    ],
    imports: [
        CommonModule,
        MatProgressBarModule,
        MatSnackBarModule,
        MatDialogModule,
        MatButtonModule,
        MatIconModule,
        TranslateModule
    ],
    entryComponents: [
        ConfirmationDialogComponent,
        ActionNotificationComponent,
        EventDialogComponent
    ],
    exports: [
        ConfirmationDialogComponent,
        EventDialogComponent
    ],
    providers: [
        ShortcutUtil
    ],
})
export class SharedModule {}