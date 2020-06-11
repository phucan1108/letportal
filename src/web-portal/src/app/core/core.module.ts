import { NgModule, ModuleWithProviders, Provider } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateConfigs } from './shell/translates/translate.configs'
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { CustomHttpService } from 'services/customhttp.service';
import { Translator } from './shell/translates/translate.pipe';
import { ConfigurationProvider } from './configs/configProvider';
import { ShellConfigProvider } from './shell/shellconfig.provider';
import { DatabasesClient, EntitySchemasClient, AppsClient, StandardComponentClient, PagesClient, DynamicListClient, ChartsClient, BackupsClient, LocalizationClient } from 'services/portal.service';
import { RouterExtService } from './ext-service/routerext.service';
import { SessionService } from 'services/session.service';
import { AccountsClient, UserSessionClient, RolesClient } from 'services/identity.service';
import { PageService } from 'services/page.service';
import { UploadFileService } from 'services/uploadfile.service';
import { TRANSLATOR_METHODS, CONTROL_EVENTS } from './core.config';
import { ControlEventExecution } from './events/control/control.event';
import { EventsProvider, EVENTS_CONFIG } from './events/event.provider';
import { UnlockScreenDialogComponent } from './security/components/unlock-screen.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SafeHtmlPipe } from './pipe/safeHtmlPipe';
import { AutoCompletePipe } from './pipe/autocomplete.pipe';
import { ExportService } from 'services/export.service';
import { SecurePipe } from './pipe/secure.pipe';
import { MatButtonModule } from '@angular/material/button'
import { MatIconModule } from '@angular/material/icon'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatDialogModule } from '@angular/material/dialog'
import { MatInputModule } from '@angular/material/input'
import { DownloadFileService } from 'services/downloadfile.service';
import { PAGE_INTERCEPTORS, InterceptorsProvider } from './interceptors/interceptor.provider';
import { Interceptor } from './interceptors/interceptor';
@NgModule({
  declarations: [
    UnlockScreenDialogComponent,
    SafeHtmlPipe,
    AutoCompletePipe,
    SecurePipe
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatDialogModule,
    MatInputModule
  ],
  entryComponents: [
    UnlockScreenDialogComponent
  ],
  exports: [
    SafeHtmlPipe,
    CommonModule,
    ReactiveFormsModule,
    AutoCompletePipe,
    SecurePipe
  ],
  providers: [
    TranslateConfigs,
    EventsProvider,
    DatasourceOptionsService,
    CustomHttpService,
    Translator,
    ShellConfigProvider,
    RolesClient,
    DynamicListClient,
    DatabasesClient,
    EntitySchemasClient,
    AppsClient,
    RouterExtService,
    SessionService,
    AccountsClient,
    UserSessionClient,
    StandardComponentClient,
    PagesClient,
    PageService,
    UploadFileService,
    ChartsClient,
    BackupsClient,
    ExportService,
    LocalizationClient,
    DownloadFileService
  ],
})
export class CoreModule {

  static forRoot(
    builtInMethods: any[] = null,
    controlEvents: ControlEventExecution[] = null): ModuleWithProviders {
    return {
      ngModule: CoreModule,
      providers: [
        {
          provide: TranslateConfigs, useValue: {
            builtInMethods: builtInMethods ? TRANSLATOR_METHODS.concat(builtInMethods) : TRANSLATOR_METHODS
          }
        },
        {
          provide: EVENTS_CONFIG, useValue: {
            events: controlEvents ? CONTROL_EVENTS.concat(controlEvents) : CONTROL_EVENTS
          }
        },
        {
          provide: EventsProvider, useClass: EventsProvider
        }
      ]
    };
  }
  static forChild(
    builtInMethods: any[] = [],
    controlEvents: ControlEventExecution[] = [],
    ): ModuleWithProviders<CoreModule> {  
    return {
      ngModule: CoreModule,
      providers: [
        {
          provide: TranslateConfigs, useValue: {
            builtInMethods: builtInMethods ? TRANSLATOR_METHODS.concat(builtInMethods) : TRANSLATOR_METHODS
          }
        },
        {
          provide: EVENTS_CONFIG, useValue: {
            events: controlEvents ? CONTROL_EVENTS.concat(controlEvents) : CONTROL_EVENTS
          }
        },
        {
          provide: EventsProvider, useClass: EventsProvider
        }
      ]
    };
  }
}