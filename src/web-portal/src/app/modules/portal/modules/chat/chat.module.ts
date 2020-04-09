import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { MatProgressSpinnerModule, MatProgressBarModule, MatToolbarModule, MatButtonModule, MatSidenavModule, MatIconModule, MatListModule, MatGridListModule, MatCardModule, MatMenuModule, MatSnackBarModule, MatDialogModule, MatTreeModule, MatExpansionModule, MatBadgeModule, MatTooltipModule, MatFormFieldModule, MatInputModule } from '@angular/material';
import { ChatHeadComponent } from './components/chat-head/chat-head.component';
import { ChatSearchComponent } from './components/chat-search/chat-search.component';
import { ReactiveFormsModule } from '@angular/forms';
import { ChatWrapperComponent } from './components/chat-warpper/chat-wrapper.component';
import { ChatBoxComponent } from './components/chat-box/chat-box.component';
import { ChatBoxContentComponent } from './components/chat-box-content/chat-box-content.component';
import { NgxEmojiPickerModule } from 'ngx-emoji-picker';
import { ChatBoxWrapperComponent } from './components/chat-box-wrapper/chat-box-wrapper.component';
import { NgxsModule } from '@ngxs/store';
import { NgxsStoreModule } from 'app/core/store.module';
import { VideoCallDialogComponent } from './components/video-call-dialog/video-call-dialog.component';
import { AvatarComponent } from './components/avatar/avatar.component';

@NgModule({
    declarations: [
        ChatHeadComponent,
        ChatSearchComponent,
        ChatWrapperComponent,
        ChatBoxContentComponent,
        ChatBoxComponent,
        ChatBoxWrapperComponent,
        VideoCallDialogComponent,
        AvatarComponent
    ],
    entryComponents: [
        VideoCallDialogComponent
    ],
    imports: [ 
        CommonModule,
        ReactiveFormsModule,
        NgxEmojiPickerModule,
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
        ChatWrapperComponent
    ],
    providers: [],
})
export class ChatModule {}