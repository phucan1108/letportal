import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateConfigs } from './shell/translates/translate.configs'
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { CustomHttpService } from 'services/customhttp.service';
import { Translator } from './shell/translates/translate.pipe';
import { ConfigurationProvider } from './configs/configProvider';
import { ShellConfigProvider } from './shell/shellconfig.provider';
import { DatabasesClient, DatasourceClient, EntitySchemasClient, AppsClient, StandardComponentClient, PagesClient, DynamicListClient } from 'services/portal.service';
import { RouterExtService } from './ext-service/routerext.service';
import { SessionService } from 'services/session.service';
import { AccountsClient, UserSessionClient, RolesClient } from 'services/identity.service';
import { PageService } from 'services/page.service';
import { UploadFileService } from 'services/uploadfile.service';
@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule
  ],
  entryComponents: [
  ],
  exports: [
  ],
  providers: [
    TranslateConfigs,
    DatasourceOptionsService,
    CustomHttpService,
    Translator,
    ConfigurationProvider,
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
		UploadFileService
  ],
})
export class CoreModule {

  static forRoot(builtInMethods: any[]): ModuleWithProviders {
    return {
      ngModule: CoreModule,
      providers: [
        {
          provide: TranslateConfigs, useValue: {
            builtInMethods: builtInMethods
          }
        }
      ]
    };
  }
}