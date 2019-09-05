import { Injectable, Injector, Optional } from '@angular/core';
import { ShellConfigProvider } from '../shellconfig.provider';
import { ShellConfig, ShellConfigType } from '../shell.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import * as _ from 'lodash';
import { Store } from '@ngxs/store';
import { filter, tap } from 'rxjs/operators';
import { AppendShellConfigsAction } from 'stores/shell/shell.actions';
import { NGXLogger } from 'ngx-logger';
import { TranslateConfigs } from './translate.configs';
import { ShellMethod } from './methods/shellmethod';
import { PageShellData } from 'app/core/models/page.model';

let shellConfigs: Array<ShellConfig> = []
@Injectable({
    providedIn: 'root'
})
export class Translator {

    private methods: any[]

    constructor(
        @Optional() config: TranslateConfigs,
        private shellConfigProvider: ShellConfigProvider,
        private logger: NGXLogger) {
        this.shellConfigProvider.shellConfigs$.subscribe(shells => {
            shellConfigs = null
            shellConfigs = shells
        })

        if(config){
            this.methods = config.builtInMethods
        }
    }

    translate(text: string, data: any) {
        let foundReplacingConfigs = ObjectUtils.getContentByDCurlyBrackets(text)
        let foundReplacedConfigs: Array<ShellConfig> = [];
        _.forEach(foundReplacingConfigs, config => {
            if (config.indexOf('data') === 0) {
                // Important hack: Because Mongodb uses '_id' instead of 'id' to indicate the primary key 
                // so we need to doublecheck the data here
                let extractedValue = null
                if (config === 'data.id' || config === 'data._id') {
                    let extractValue = new Function('data', 'return data._id')
                    extractedValue = extractValue(data)
                    if (extractedValue === null || extractedValue === undefined) {
                        extractValue = new Function('data', 'return data.id')
                        extractedValue = extractValue(data)
                    }
                }
                else {
                    let extractValue = new Function('data', `return ${config}`)
                    extractedValue = extractValue(data)
                }

                foundReplacedConfigs.push({ key: config, value: extractedValue, type: ShellConfigType.Constant })
            }
            else {
                let builtInMethod = this.isBuiltInMethod(config)
                if (builtInMethod) {
                    foundReplacedConfigs.push({ key: config, value: builtInMethod.execute(data), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                }
                else {
                    foundReplacedConfigs.push(_.find(shellConfigs, (shell: ShellConfig) => shell.key.indexOf(config) === 0))
                }
            }
        })

        _.forEach(foundReplacedConfigs, config => {
            switch (config.type) {
                case ShellConfigType.Constant:
                    text = text.replace(`{{${config.key}}}`, config.value)
                    break
                case ShellConfigType.Method:
                    if (config.replaceDQuote) {
                        text = text.replace(`"{{${config.key}}}"`, config.value)
                    }
                    else {
                        text = text.replace(`{{${config.key}}}`, config.value)
                    }
                    break
            }
        })

        return text
    }

    translateData(text: string, translatorFactors: TranslatorFactors) {
        let foundReplacingConfigs = ObjectUtils.getContentByDCurlyBrackets(text)
        let foundReplacedConfigs: Array<ShellConfig> = [];
        _.forEach(foundReplacingConfigs, config => {
            if (config.indexOf('data') === 0) {                
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'data', translatorFactors.data))
            }
            else if (config.indexOf('user') === 0) {
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'user', translatorFactors.user))
            }
            else if (config.indexOf('options') === 0) {
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'options', translatorFactors.options))
            }
            else if (config.indexOf('queryparams') === 0) {
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'queryparams', translatorFactors.queryparams))
            }
            else if (config.indexOf('claims') === 0) {
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'claims', translatorFactors.claims))
            }
            else {
                let builtInMethod = this.isBuiltInMethod(config)
                if (builtInMethod) {
                    foundReplacedConfigs.push({ key: config, value: builtInMethod.execute(translatorFactors.data), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                }
                else {
                    foundReplacedConfigs.push(_.find(shellConfigs, (shell: ShellConfig) => shell.key.indexOf(config) === 0))
                }
            }
        })

        _.forEach(foundReplacedConfigs, config => {
            switch (config.type) {
                case ShellConfigType.Constant:
                    if(config.replaceDQuote){
                        text = text.replace(`"{{${config.key}}}"`, config.value)
                    }
                    else{
                        text = text.replace(`{{${config.key}}}`, config.value)
                    }                    
                    break
                case ShellConfigType.Method:
                    if (config.replaceDQuote) {
                        text = text.replace(`"{{${config.key}}}"`, config.value)
                    }
                    else {
                        text = text.replace(`{{${config.key}}}`, config.value)
                    }
                    break
            }
        })

        return text
    }

    translateDataWithShell(text: string, pageShellData: PageShellData) {
        let foundReplacingConfigs = ObjectUtils.getContentByDCurlyBrackets(text)
        let foundReplacedConfigs: Array<ShellConfig> = [];
        _.forEach(foundReplacingConfigs, config => {
            if (config.indexOf('data') === 0) {                
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'data', pageShellData.data))
            }
            else if (config.indexOf('user') === 0) {
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'user', pageShellData.user))
            }
            else if (config.indexOf('options') === 0) {
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'options', pageShellData.options))
            }
            else if (config.indexOf('queryparams') === 0) {
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'queryparams', pageShellData.queryparams))
            }
            else if (config.indexOf('claims') === 0) {
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'claims', pageShellData.claims))
            }
            else if (config.indexOf('appsettings') === 0){
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'appsettings', pageShellData.appsettings))
            }
            else if (config.indexOf('configs') === 0){
                foundReplacedConfigs.push(this.generateReplacedValue(config, 'configs', pageShellData.configs))
            }
            else {
                let builtInMethod = this.isBuiltInMethod(config)
                if (builtInMethod) {
                    foundReplacedConfigs.push({ key: config, value: builtInMethod.execute(pageShellData.data), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                }
                else {
                    foundReplacedConfigs.push(_.find(shellConfigs, (shell: ShellConfig) => shell.key.indexOf(config) === 0))
                }
            }
        })

        _.forEach(foundReplacedConfigs, config => {
            switch (config.type) {
                case ShellConfigType.Constant:
                    if(config.replaceDQuote){
                        text = text.replace(`"{{${config.key}}}"`, config.value)
                    }
                    else{
                        text = text.replace(`{{${config.key}}}`, config.value)
                    }                    
                    break
                case ShellConfigType.Method:
                    if (config.replaceDQuote) {
                        text = text.replace(`"{{${config.key}}}"`, config.value)
                    }
                    else {
                        text = text.replace(`{{${config.key}}}`, config.value)
                    }
                    break
            }
        })

        return text
    }

    private generateReplacedValue(key: string, keyData: string, data: any): ShellConfig{
        let extractValue = new Function(keyData, `return ${key}`)
        let foundValue = extractValue(data)
        if(ObjectUtils.isObject(foundValue)){
            return {
                key: key,
                value: JSON.stringify(foundValue),
                replaceDQuote: true,
                type: ShellConfigType.Constant
            }
        }
        else{
            return {
                key: key,
                value: foundValue,
                replaceDQuote: ObjectUtils.isNumber(foundValue) || ObjectUtils.isBoolean(foundValue),
                type: ShellConfigType.Constant
            }
        }
    }

    private isBuiltInMethod(text: string) {
        let builtInMethod: any = null;
        _.forEach(this.methods, methodType => {
            builtInMethod = new methodType();
            if (text.indexOf(builtInMethod.methodTranslatorName) >= 0) {
                return false
            }
            builtInMethod = null
        })

        return builtInMethod
    }
}

export class TranslatorFactors{
    user: any
    configs: any
    queryparams: any
    claims: any
    options: any
    data: any
}