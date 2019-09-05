import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from './core/core.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ShortcutModule } from './shared/components/shortcuts/shortcut.module';
import { NgxsStoreModule } from 'stores/store.module';
import { LoggerModule, NgxLoggerLevel } from 'ngx-logger';
import { ConfigurationService } from 'services/configuration.service';
import { ConfigurationProvider } from './core/configs/configProvider';
import { ShellModule } from './core/shell/shell.module';
import { environment } from 'environments/environment';
import { RouterExtService } from './core/ext-service/routerext.service';
import { LoginComponent } from './modules/login/login.component';
import { PORTAL_BASE_URL, DatabasesClient, DatasourceClient, EntitySchemasClient, AppsClient, PagesClient, DynamicListClient, StandardComponentClient } from 'services/portal.service';
import { SessionService } from 'services/session.service';
import { ErrorComponent } from './modules/error/error.component';
import { MatFormFieldModule, MatInputModule, MatCardModule, MatButtonModule, MatCheckboxModule, MatIconModule, MatToolbarModule, MatDialogModule, MatProgressBarModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserSessionClient, AccountsClient, IDENTITY_BASE_URL, RolesClient } from 'services/identity.service';
import { ForgotPasswordComponent } from './modules/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './modules/reset-password/reset-password.component';
import { JwtTokenInterceptor } from './core/security/jwtToken.interceptor';
import { ToastrModule } from 'ngx-toastr';
import { HttpExceptionInterceptor } from './core/https/httpException.interceptor';
import {MomentumTableModule} from 'momentum-table';
import { NgJsonEditorModule } from 'ang-jsoneditor';
import { ClipboardModule } from 'ngx-clipboard';
import { toJsonStringTranslator } from './core/shell/translates/methods/toJsonStringTranslator';
import { currentDateTranslator } from './core/shell/translates/methods/currentDateTranslator';
import { MatProgressButtonsModule } from 'mat-progress-buttons';

let portalBaseUrl = (configProvider: ConfigurationProvider) => {
  return configProvider.getCurrentConfigs().portalBaseEndpoint
}

let identityBaseUrl = (configProvider: ConfigurationProvider) => {
  return configProvider.getCurrentConfigs().identityBaseEndpoint
}
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ErrorComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatProgressButtonsModule.forRoot(),
    MatProgressBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule,
    MatToolbarModule,
    FormsModule,
		ReactiveFormsModule,
    ShortcutModule,
    CoreModule.forRoot([
      toJsonStringTranslator,
      currentDateTranslator
    ]),
    NgxsStoreModule,
    MomentumTableModule,
    ShellModule,
    LoggerModule.forRoot({
      serverLoggingUrl: "",
      level: environment.production ? NgxLoggerLevel.OFF: NgxLoggerLevel.DEBUG,
      serverLogLevel: NgxLoggerLevel.OFF
    }),
    ToastrModule.forRoot(),
    ClipboardModule
  ],
  providers: [
    ConfigurationService,
    // Portal Clients
    DynamicListClient,
    {
      provide: PORTAL_BASE_URL,
      useFactory: portalBaseUrl,
      deps: [ConfigurationProvider]
    },
    DatabasesClient,
    DatasourceClient,
    EntitySchemasClient,
    AppsClient,
    RouterExtService,
    SessionService,
    AccountsClient,
    UserSessionClient,    
    StandardComponentClient,
    PagesClient,
    {
      provide: IDENTITY_BASE_URL,
      useFactory: identityBaseUrl,
      deps: [ConfigurationProvider]
    },
    RolesClient,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtTokenInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpExceptionInterceptor,
      multi: true
    }
  ],
  entryComponents: [

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
