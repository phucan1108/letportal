import { Injectable } from '@angular/core';
 
import { ShellContants } from './shell.contants';
import { ConfigurationProvider } from '../configs/configProvider';
import { ShellConfig } from './shell.model';
import { ArrayUtils } from '../utils/array-util';
import { NGXLogger } from 'ngx-logger';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ShellConfigProvider {

    private shellConfigs: Array<ShellConfig>;

    shellConfigs$: BehaviorSubject<Array<ShellConfig>> = new BehaviorSubject([])

    definitionVars = [
        'env',
        'session',
        ShellContants.FORM_DATA
    ]

    definitionConstants = [
        ShellContants.NAVIGATION_PREVIOUS
    ]

    definitionFuncs = [
        'toJsonString',
        'toEncryptionString',
        'toBase64String'
    ]

    constructor(
        private configProvider: ConfigurationProvider,
        private logger: NGXLogger) {
        this.shellConfigs = this.configProvider.getShellConfigs()
        this.shellConfigs$.next(this.shellConfigs)
    }

    appendShellConfigs(shellConfigs: Array<ShellConfig>) {
        this.shellConfigs = ArrayUtils.appendItemsDistinct(this.shellConfigs, shellConfigs, 'key')
        this.shellConfigs$.next(this.shellConfigs)
    }

    getAllAvailableShells() {
        return this.definitionVars.concat(this.definitionFuncs, this.definitionConstants, ArrayUtils.sliceOneProp(this.shellConfigs, 'key'));
    }
}
