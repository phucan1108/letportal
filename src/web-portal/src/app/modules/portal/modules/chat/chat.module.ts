import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTreeModule } from '@angular/material/tree';
import { TranslateModule } from '@ngx-translate/core';
import { CoreModule } from 'app/core/core.module';
import { EmojiPickerModule } from 'app/modules/thirdparties/emoji-picker/emoji-picker.module';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { AvatarComponent } from './components/avatar/avatar.component';
import { ChatBoxContentComponent } from './components/chat-box-content/chat-box-content.component';
import { ChatBoxWrapperComponent } from './components/chat-box-wrapper/chat-box-wrapper.component';
import { ChatBoxComponent } from './components/chat-box/chat-box.component';
import { ChatHeadComponent } from './components/chat-head/chat-head.component';
import { ChatSearchComponent } from './components/chat-search/chat-search.component';
import { ChatWrapperComponent } from './components/chat-warpper/chat-wrapper.component';
import { VideoCallDialogComponent } from './components/video-call-dialog/video-call-dialog.component';
import { VideoStreamDialogComponent } from './components/video-stream-dialog/video-stream-dialog.component';

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
        MatTooltipModule,
        TranslateModule
     ],
    exports: [
        ChatWrapperComponent,
        VideoStreamDialogComponent
    ],
    providers: [],
})
export class ChatModule {}