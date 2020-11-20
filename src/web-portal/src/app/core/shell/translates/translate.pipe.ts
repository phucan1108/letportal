import { Injectable, Optional } from '@angular/core';
import { PageShellData } from 'app/core/models/page.model';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { PageParameterModel } from 'services/portal.service';
import { ShellConfig, ShellConfigType } from '../shell.model';
import { ShellConfigProvider } from '../shellconfig.provider';
import { TranslateConfigs } from './translate.configs';
 

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

        if (config) {
            this.methods = config.builtInMethods
        }
    }

    retrieveParameters(text: string, pageShellData: PageShellData) {
        let foundReplacingConfigs = ObjectUtils.getContentByDCurlyBrackets(text)
        if(foundReplacingConfigs){
            foundReplacingConfigs = foundReplacingConfigs.filter((val, index, selfArray) =>{
                return selfArray.indexOf(val) === index
            })
        }
        const foundReplacedConfigs: Array<ShellConfig> = [];
        foundReplacingConfigs?.forEach(config => {
            // New changes: we add one pipe for defining parameter type such as "data.ticks|long"
            // We need to detect this parameter has type
            const splitted = config.split('|')
            let configName = ''
            if(splitted.length == 2){
                configName = splitted[0]
            }
            else{
                configName = config
            }
            const shellConfig = this.getShellConfig(configName, pageShellData)
            if (shellConfig) {
                foundReplacedConfigs.push(shellConfig)
                shellConfig.key = config
            }
            else {
                const builtInMethod = this.isBuiltInMethod(configName)
                if (builtInMethod) {
                    // Find all parameters in built-in method
                    const padLeft = configName.indexOf('(')
                    const padRight = configName.indexOf(')')
                    if (padRight - 1 > padLeft) {
                        const allMid = configName.substr(padLeft + 1, padRight - padLeft - 1)
                        const builtInMethodExecute = new Function('user', 'claims', 'options', 'data', 'configs', 'appsettings', 'queryparams', 'builtInMethod', 'return builtInMethod.execute(' + allMid + ')')
                        foundReplacedConfigs.push({ key: config, value: builtInMethodExecute(pageShellData.user, pageShellData.claims, pageShellData.options, pageShellData.data, pageShellData.configs, pageShellData.appsettings, pageShellData.queryparams, builtInMethod), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                    }
                    else {
                        foundReplacedConfigs.push({ key: config, value: builtInMethod.execute(), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                    }
                }
                else {
                    foundReplacedConfigs.push(shellConfigs.find((shell: ShellConfig) => shell.key.indexOf(config) === 0))
                }
            }
        })

        const params: PageParameterModel[] = []
        foundReplacedConfigs?.forEach(config => {
            this.logger.debug('Replace config', config)
            params.push({
                name: config.key,
                removeQuotes: config.replaceDQuote,
                replaceValue: config.value
            })
        })

        return params
    }

    translateDataWithShell(text: string, pageShellData: PageShellData) {
        const foundReplacingConfigs = ObjectUtils.getContentByDCurlyBrackets(text)
        const foundReplacedConfigs: Array<ShellConfig> = [];
        foundReplacingConfigs?.forEach(config => {
            // New changes: we add one pipe for defining parameter type such as "data.ticks|long"
            // We need to detect this parameter has type
            const splitted = config.split('|')
            let configName = ''
            if(splitted.length == 2){
                configName = splitted[0]
            }
            else{
                configName = config
            }
            const shellConfig = this.getShellConfig(configName, pageShellData)
            if (shellConfig) {
                foundReplacedConfigs.push(shellConfig)
                shellConfig.key = config
            }
            else {
                const builtInMethod = this.isBuiltInMethod(configName)
                if (builtInMethod) {
                    // Find all parameters in built-in method
                    const padLeft = configName.indexOf('(')
                    const padRight = configName.indexOf(')')
                    if (padRight - 1 > padLeft) {
                        const allMid = configName.substr(padLeft + 1, padRight - padLeft - 1)
                        const builtInMethodExecute = new Function('user', 'claims', 'options', 'data', 'configs', 'appsettings', 'queryparams', 'builtInMethod', 'return builtInMethod.execute(' + allMid + ')')
                        foundReplacedConfigs.push({ key: config, value: builtInMethodExecute(pageShellData.user, pageShellData.claims, pageShellData.options, pageShellData.data, pageShellData.configs, pageShellData.appsettings, pageShellData.queryparams, builtInMethod), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                    }
                    else {
                        foundReplacedConfigs.push({ key: config, value: builtInMethod.execute(), type: ShellConfigType.Method, replaceDQuote: builtInMethod.replaceDQuote })
                    }
                }
                else {
                    foundReplacedConfigs.push(shellConfigs.find((shell: ShellConfig) => shell.key.indexOf(config) === 0))
                }
            }
        })

        foundReplacedConfigs?.forEach(config => {
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
        else if (config.indexOf('parent') === 0){
            return this.generateReplacedValue(config, 'parent', pageShellData.parent)
        }
        else {
            return null
        }
    }

    private generateReplacedValue(key: string, keyData: string, data: any): ShellConfig {
        try
        {
            let foundValue = null
            let requireDeleteUniqId = key.indexOf('.inserts') > 0 || key.indexOf('.updates') > 0
            // For all id or _id, we must to doublecheck two cases
            if(keyData == 'data' && (key.indexOf('.id') > 0 || key.indexOf('._id') > 0)){
                const tempKey = key.replace('.id', '._id');
                let extractValueTemp = new Function('data', 'return ' + tempKey)
                foundValue = extractValueTemp(data)
                if (foundValue === null || foundValue === undefined) {
                    // Case .id
                    const tempKeyId = key.replace('._id', '.id');
                    extractValueTemp = new Function('data', 'return ' + tempKeyId)
                    foundValue = extractValueTemp(data)
                }
            }
            else {
                const extractValue = new Function(keyData, `return ${key}`)
                foundValue = extractValue(data)
            }
            if (ObjectUtils.isObject(foundValue)) {
                return {
                    key,
                    value: JSON.stringify(foundValue),
                    replaceDQuote: true,
                    type: ShellConfigType.Constant
                }
            }
            else if(ObjectUtils.isArray(foundValue)){
    
                // Remove need to remove 'uniq_id' field
                if(requireDeleteUniqId){
                    foundValue = ObjectUtils.clone(foundValue)
                    if(foundValue[0]){
                        if(foundValue[0]['uniq_id']){
                            foundValue?.forEach(a => {
                                delete a['uniq_id']
                            })
                        }                    
                    }                
                }           
    
                return {
                    key,
                    value: JSON.stringify(foundValue),
                    replaceDQuote: true,
                    type: ShellConfigType.Constant
                }
            }
            else {
                return {
                    key,
                    value: foundValue,
                    replaceDQuote: ObjectUtils.isNumber(foundValue) || ObjectUtils.isBoolean(foundValue),
                    type: ShellConfigType.Constant
                }
            }
        }
        catch(err){
            this.logger.debug('There are something went wrongs with replace value: ' + key)
        }        
    }

    private isBuiltInMethod(text: string) {
        let builtInMethod: any = null;
        let isFound = false
        this.methods?.forEach(methodType => {
            if(!isFound){
                builtInMethod = new methodType();
                if (text.indexOf(builtInMethod.methodTranslatorName) >= 0) {
                    isFound = true
                }
                else{
                    builtInMethod = null
                }
            }
        })

        return builtInMethod
    }
}