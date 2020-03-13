import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateConfigs } from './shell/translates/translate.configs'
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { CustomHttpService } from 'services/customhttp.service';
import { Translator } from './shell/translates/translate.pipe';
import { ConfigurationProvider } from './configs/configProvider';
import { ShellConfigProvider } from './shell/shellconfig.provider';
import { DatabasesClient, DatasourceClient, EntitySchemasClient, AppsClient, StandardComponentClient, PagesClient, DynamicListClient, ChartsClient, BackupsClient } from 'services/portal.service';
import { RouterExtService } from './ext-service/routerext.service';
import { SessionService } from 'services/session.service';
import { AccountsClient, UserSessionClient, RolesClient } from 'services/identity.service';
import { PageService } from 'services/page.service';
import { UploadFileService } from 'services/uploadfile.service';
import { TRANSLATOR_METHODS, CONTROL_EVENTS } from './core.config';
import { ControlEventExecution } from './events/control/control.event';
import { EventsProvider, EVENTS_CONFIG } from './events/event.provider';
import { UnlockScreenDialogComponent } from './security/components/unlock-screen.component';
import { MatFormFieldModule, MatDialogModule, MatInputModule, MatIconModule, MatButtonModule } from '@angular/material';
import { ReactiveFormsModule } from '@angular/forms';
import { SafeHtmlPipe } from './pipe/safeHtmlPipe';
import { AutoCompletePipe } from './pipe/autocomplete.pipe';
import { ExportService } from 'services/export.service';
@NgModule({
  declarations: [
    UnlockScreenDialogComponent,        
    SafeHtmlPipe,
    AutoCompletePipe
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
    AutoCompletePipe
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
    DatasourceClient,
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
    ExportService
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
}