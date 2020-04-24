import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { ChatHeadComponent } from './components/chat-head/chat-head.component';
import { ChatSearchComponent } from './components/chat-search/chat-search.component';
import { ReactiveFormsModule } from '@angular/forms';
import { ChatWrapperComponent } from './components/chat-warpper/chat-wrapper.component';
import { ChatBoxComponent } from './components/chat-box/chat-box.component';
import { ChatBoxContentComponent } from './components/chat-box-content/chat-box-content.component';
import { EmojiPickerModule } from 'emoji-picker'
import { ChatBoxWrapperComponent } from './components/chat-box-wrapper/chat-box-wrapper.component';
import { NgxsModule } from '@ngxs/store';
import { NgxsStoreModule } from 'app/core/store.module';
import { VideoCallDialogComponent } from './components/video-call-dialog/video-call-dialog.component';
import { AvatarComponent } from './components/avatar/avatar.component';
import { VideoStreamDialogComponent } from './components/video-stream-dialog/video-stream-dialog.component';
import { CoreModule } from 'app/core/core.module';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTreeModule } from '@angular/material/tree';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
    declarations: [
        ChatHeadComponent,
        ChatSearchComponent,
        ChatWrapperComponent,
        ChatBoxContentComponent,
        ChatBoxComponent,
        ChatBoxWrapperComponent,
        VideoCallDialogComponent,
        AvatarComponent,
        VideoStreamDialogComponent
    ],
    entryComponents: [
        VideoCallDialogComponent
    ],
    imports: [ 
        CoreModule.forChild(),
        CommonModule,
        ReactiveFormsModule,
        EmojiPickerModule,
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
        ChatWrapperComponent,
        VideoStreamDialogComponent
    ],
    providers: [],
})
export class ChatModule {}