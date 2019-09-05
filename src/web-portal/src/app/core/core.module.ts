import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShellMethod } from './shell/translates/methods/shellmethod'
import { TranslateConfigs } from './shell/translates/translate.configs'
import { DatasourceOptionsService } from 'services/datasourceopts.service';
import { CustomHttpService } from 'services/customhttp.service';
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
      CustomHttpService     
    ],
})
export class CoreModule { 
    
static forRoot(builtInMethods: any[]): ModuleWithProviders {
    return {
      ngModule: CoreModule,
      providers: [
        {provide: TranslateConfigs, useValue: {
          builtInMethods: builtInMethods
        }}
      ]
    };
  }
}