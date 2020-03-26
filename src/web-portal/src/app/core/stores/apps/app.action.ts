import { App } from 'services/portal.service';

const AppSelectorPage = '[App Selector Page]'

export class AppAction{
    public static readonly type = '';
}

export class UserSelectAppAction implements AppAction{
    public static readonly type = `${AppSelectorPage}-User select app`
    constructor(public app: App){}
}

export type All =
    UserSelectAppAction