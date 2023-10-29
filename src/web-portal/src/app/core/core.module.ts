import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { CustomHttpService } from 'services/customhttp.service';
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { DownloadFileService } from 'services/downloadfile.service';
import { ExportService } from 'services/export.service';
import { AccountsClient, RolesClient, UserSessionClient } from 'services/identity.service';
import { PageService } from 'services/page.service';
import { AppsClient, BackupsClient, ChartsClient, DatabasesClient, DynamicListClient, EntitySchemasClient, LocalizationClient, PagesClient, StandardComponentClient } from 'services/portal.service';
import { SessionService } from 'services/session.service';
import { UploadFileService } from 'services/uploadfile.service';
import { CONTROL_EVENTS, TRANSLATOR_METHODS } from './core.config';
import { ControlEventExecution } from './events/control/control.event';
import { EventsProvider, EVENTS_CONFIG } from './events/event.provider';
import { RouterExtService } from './ext-service/routerext.service';
import { AutoCompletePipe } from './pipe/autocomplete.pipe';
import { SafeHtmlPipe } from './pipe/safeHtmlPipe';
import { SecurePipe } from './pipe/secure.pipe';
import { UnlockScreenDialogComponent } from './security/components/unlock-screen.component';
import { ShellConfigProvider } from './shell/shellconfig.provider';
import { TranslateConfigs } from './shell/translates/translate.configs';
import { Translator } from './shell/translates/translate.pipe';
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
    controlEvents: ControlEventExecution[] = null): ModuleWithProviders<CoreModule> {
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