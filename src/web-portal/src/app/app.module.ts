import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from './core/core.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { SharedModule } from './modules/shared/shortcut.module';
import { NgxsStoreModule } from 'app/core/store.module';
import { LoggerModule, NgxLoggerLevel } from 'ngx-logger';
import { ConfigurationService } from 'services/configuration.service';
import { ConfigurationProvider } from './core/configs/configProvider';
import { environment } from 'environments/environment';
import { PORTAL_BASE_URL } from 'services/portal.service';
import { ErrorComponent } from './modules/error/error.component';
import { MatCardModule, MatButtonModule, MatIconModule } from '@angular/material';
import { IDENTITY_BASE_URL } from 'services/identity.service';
import { JwtTokenInterceptor } from './core/security/jwtToken.interceptor';
import { ToastrModule } from 'ngx-toastr';
import { HttpExceptionInterceptor } from './core/https/httpException.interceptor';
import { ClipboardModule } from 'ngx-clipboard';
import { MatProgressButtonsModule } from 'mat-progress-buttons';

import pgsql from 'highlight.js/lib/languages/pgsql'
import sql from 'highlight.js/lib/languages/sql'
import { HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';
import { ChatService, CHAT_BASE_URL } from 'services/chat.service';
import { ChatModule } from 'portal/modules/chat/chat.module';
import { NgxEmojiPickerModule } from 'ngx-emoji-picker';
import { VideoCallService, VIDEO_BASE_URL } from 'services/videocall.service';
import { FullscreenOverlayContainer, OverlayContainer } from '@angular/cdk/overlay';
export function hlJSLang() {
  return [
    { name: 'sql', func: sql }
  ]
}
const portalBaseUrl = (configProvider: ConfigurationProvider) => {
  return configProvider.getCurrentConfigs().portalBaseEndpoint
}
const chatBaseUrl = (configProvider: ConfigurationProvider) => {
  console.log('current configs', configProvider.getCurrentConfigs())
  return configProvider.getCurrentConfigs().chatBaseEndpoint
}
const identityBaseUrl = (configProvider: ConfigurationProvider) => {
  return configProvider.getCurrentConfigs().identityBaseEndpoint
}
@NgModule({
  declarations: [
    AppComponent,
    ErrorComponent
  ],
  imports: [
    ChatModule,
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    BrowserAnimationsModule,
    MatProgressButtonsModule.forRoot(),
    SharedModule,
    NgxsStoreModule,
    LoggerModule.forRoot({
      serverLoggingUrl: '',
      level: environment.production ? NgxLoggerLevel.OFF : NgxLoggerLevel.DEBUG,
      serverLogLevel: NgxLoggerLevel.OFF
    }),
    ToastrModule.forRoot(),
    ClipboardModule,

    // Portal Module Sections
    CoreModule.forRoot(),

    NgxEmojiPickerModule.forRoot()
  ],
  providers: [
    ChatService,
    VideoCallService,
    ConfigurationService,
    {
      provide: PORTAL_BASE_URL,
      useFactory: portalBaseUrl,
      deps: [ConfigurationProvider]
    },
    {
      provide: IDENTITY_BASE_URL,
      useFactory: identityBaseUrl,
      deps: [ConfigurationProvider]
    },
    {
      provide: CHAT_BASE_URL,
      useFactory: chatBaseUrl,
      deps: [ConfigurationProvider]
    },
    {
      provide: VIDEO_BASE_URL,
      useFactory: chatBaseUrl,
      deps: [ConfigurationProvider]
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtTokenInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpExceptionInterceptor,
      multi: true
    },
    {
      provide: HIGHLIGHT_OPTIONS,
      useValue: {
        languages: hlJSLang
      }
    },
    ConfigurationProvider
  ],
  entryComponents: [

  ],
  exports: [
    CoreModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
