import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfigurationProvider } from '../configs/configProvider';
import { Translator } from './translates/translate.pipe';
import { HttpClientModule } from '@angular/common/http';
import { ShellConfigProvider } from './shellconfig.provider';

@NgModule({
    declarations: [
    ],
    imports: [
        CommonModule,
        HttpClientModule
    ],
    exports: [        
    ],
    providers: [
        Translator,
        ConfigurationProvider,
        ShellConfigProvider
    ],
})
export class ShellModule { }