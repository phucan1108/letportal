import { Injectable, Optional } from '@angular/core';
import { ShellConfigProvider } from '../shellconfig.provider';
import { ShellConfig, ShellConfigType } from '../shell.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import * as _ from 'lodash';
import { TranslateConfigs } from './translate.configs';
import { PageShellData } from 'app/core/models/page.model';
import { PageParameterModel } from 'services/portal.service';

let shellConfigs: Array<ShellConfig> = []
@Injectable({
    providedIn: 'root'
})
export class Translator {

    private methods: any[]

    constructor(
        @Optional() config: TranslateConfigs,
        private shellConfigProvider: ShellConfigProvider) {
        this.shellConfigProvider.shellConfigs$.subscribe(shells => {
            shellConfigs = null
            shellConfigs = shells
        })

        if (config) {
            this.methods = config.builtInMethods
        }
    }

    retrieveParameters(text: string, pageShellData: PageShellData) {
        let foundReplacingConfigs = ObjectUtils.getContentByDCurlyBrackets(text)
        let foundReplacedConfigs: Array<ShellConfig> = [];
        _.forEach(foundReplacingConfigs, config => {
            let shellConfig = this.getShellConfig(config, pageShellData)
            if (shellConfig) {
                foundReplacedConfigs.push(shellConfig)
            }
            else {
                let builtInMethod = this.isBuiltInMethod(config)
                if (builtInMethod) {
                    // Find all parameters in built-in method
                    let padLeft = config.indexOf('(')
                    let padRight = config.indexOf(')')
                    if (padRight - 1 > padLeft) {
                        let allMid = config.substr(padLeft + 1, padRight - padLeft - 1)
                        let builtInMethodExecute = new Function('user', 'claims', 'options', 'data', 'configs', 'appsettings', 'queryparams', 'builtInMethod', 'return builtInMethod.execute(' + allMid + ')')
                        foundReplacedConfigs.push({ key: config, value: builtInMethodExecute(pageShellData.user, pageShellData.claims, pageShellData.options, pageShellData.data, pageShellData.configs, pageShellData.appsettings, pageShellData.queryparams, builtInMethod), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                    }
                    else {
                        foundReplacedConfigs.push({ key: config, value: builtInMethod.execute(), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                    }
                }
                else {
                    foundReplacedConfigs.push(_.find(shellConfigs, (shell: ShellConfig) => shell.key.indexOf(config) === 0))
                }
            }
        })

        let params: PageParameterModel[] = []
        _.forEach(foundReplacedConfigs, config => {
            params.push({
                name: config.key,
                removeQuotes: config.replaceDQuote,
                replaceValue: config.value
            })
        })

        return params
    }

    translateDataWithShell(text: string, pageShellData: PageShellData) {
        let foundReplacingConfigs = ObjectUtils.getContentByDCurlyBrackets(text)
        let foundReplacedConfigs: Array<ShellConfig> = [];
        _.forEach(foundReplacingConfigs, config => {
            let shellConfig = this.getShellConfig(config, pageShellData)
            if (shellConfig) {
                foundReplacedConfigs.push(shellConfig)
            }
            else {
                let builtInMethod = this.isBuiltInMethod(config)
                if (builtInMethod) {
                    // Find all parameters in built-in method
                    let padLeft = config.indexOf('(')
                    let padRight = config.indexOf(')')
                    if (padRight - 1 > padLeft) {
                        let allMid = config.substr(padLeft + 1, padRight - padLeft - 1)
                        let builtInMethodExecute = new Function('user', 'claims', 'options', 'data', 'configs', 'appsettings', 'queryparams', 'builtInMethod', 'return builtInMethod.execute(' + allMid + ')')
                        foundReplacedConfigs.push({ key: config, value: builtInMethodExecute(pageShellData.user, pageShellData.claims, pageShellData.options, pageShellData.data, pageShellData.configs, pageShellData.appsettings, pageShellData.queryparams, builtInMethod), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                    }
                    else {
                        foundReplacedConfigs.push({ key: config, value: builtInMethod.execute(), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                    }
                }
                else {
                    foundReplacedConfigs.push(_.find(shellConfigs, (shell: ShellConfig) => shell.key.indexOf(config) === 0))
                }
            }
        })

        _.forEach(foundReplacedConfigs, config => {
            switch (config.type) {
                case ShellConfigType.Constant:
                    if (config.replaceDQuote) {
                        text = text.replace(`"{{${config.key}}}"`, config.value)
                    }
                    else {
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

    private getShellConfig(config: string, pageShellData: PageShellData) {
        if (config.indexOf('data') === 0) {
            return this.generateReplacedValue(config, 'data', pageShellData.data)
        }
        else if (config.indexOf('user') === 0) {
            return this.generateReplacedValue(config, 'user', pageShellData.user)
        }
        else if (config.indexOf('options') === 0) {
            return this.generateReplacedValue(config, 'options', pageShellData.options)
        }
        else if (config.indexOf('queryparams') === 0) {
            return this.generateReplacedValue(config, 'queryparams', pageShellData.queryparams)
        }
        else if (config.indexOf('claims') === 0) {
            return this.generateReplacedValue(config, 'claims', pageShellData.claims)
        }
        else if (config.indexOf('appsettings') === 0) {
            return this.generateReplacedValue(config, 'appsettings', pageShellData.appsettings)
        }
        else if (config.indexOf('configs') === 0) {
            return this.generateReplacedValue(config, 'configs', pageShellData.configs)
        }
        else {
            return null
        }
    }

    private generateReplacedValue(key: string, keyData: string, data: any): ShellConfig {
        let foundValue = null
        if (keyData == 'data' && (key === 'data.id' || key === 'data._id')) {
            let extractValueTemp = new Function('data', 'return data._id')
            foundValue = extractValueTemp(data)
            if (foundValue === null || foundValue === undefined) {
                extractValueTemp = new Function('data', 'return data.id')
                foundValue = extractValueTemp(data)
            }
        }
        else {
            let extractValue = new Function(keyData, `return ${key}`)
            foundValue = extractValue(data)
        }
        if (ObjectUtils.isObject(foundValue)) {
            return {
                key: key,
                value: JSON.stringify(foundValue),
                replaceDQuote: true,
                type: ShellConfigType.Constant
            }
        }
        else {
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