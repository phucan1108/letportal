import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { NgxsStoreModule } from 'app/core/store.module';
import { environment } from 'environments/environment';
import sql from 'highlight.js/lib/languages/sql';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { ClipboardModule } from 'ngx-clipboard';
import { HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';
import { LoggerModule, NgxLoggerLevel } from 'ngx-logger';
import { ToastrModule } from 'ngx-toastr';
import { ChatModule } from 'portal/modules/chat/chat.module';
import { ChatService, CHAT_BASE_URL } from 'services/chat.service';
import { ConfigurationService } from 'services/configuration.service';
import { IDENTITY_BASE_URL } from 'services/identity.service';
import { LocalizationService } from 'services/localization.service';
import { PORTAL_BASE_URL } from 'services/portal.service';
import { VideoCallService, VIDEO_BASE_URL } from 'services/videocall.service';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConfigurationProvider } from './core/configs/configProvider';
import { CoreModule } from './core/core.module';
import { MatPaginatorIntlCustom } from './core/custom-materials/matPaginatorIntlCustom';
import { HttpExceptionInterceptor } from './core/https/httpException.interceptor';
import { JwtTokenInterceptor } from './core/security/jwtToken.interceptor';
import { ErrorComponent } from './modules/error/error.component';
import { SharedModule } from './modules/shared/shortcut.module';
import { EmojiPickerModule } from './modules/thirdparties/emoji-picker/emoji-picker.module';

export function hlJSLang() {
  return [
    { name: 'sql', func: sql }
  ]
}
const portalBaseUrl = (configProvider: ConfigurationProvider) => {
  return configProvider.getCurrentConfigs().portalBaseEndpoint
}
const chatBaseUrl = (configProvider: ConfigurationProvider) => {
  return configProvider.getCurrentConfigs().chatBaseEndpoint
}
const identityBaseUrl = (configProvider: ConfigurationProvider) => {
  return configProvider.getCurrentConfigs().identityBaseEndpoint
}
// required for AOT compilation
function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/','.json');
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

    EmojiPickerModule.forRoot(),
    TranslateModule.forRoot(
      {
        defaultLanguage: environment.localization.defaultLanguage,
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        }
      }
    )
  ],
  providers: [
    ChatService,
    VideoCallService,
    LocalizationService,
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
    {
      provide: MatPaginatorIntl,
      useFactory: (translate: TranslateService) => {
        const service = new MatPaginatorIntlCustom();
        service.addTranslateService(translate);
        return service;
      },
      deps: [TranslateService]
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
