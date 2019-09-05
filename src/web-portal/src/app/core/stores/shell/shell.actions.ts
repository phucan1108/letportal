import { ShellConfig } from 'app/core/shell/shell.model';

const SHELL_DATA = '[Shell-Data]'

export class ShellAction {
    public static readonly type = '';
}

export class AppendShellConfigsAction implements ShellAction {
    public static readonly type = `${SHELL_DATA} Append Shel Configs`
    constructor(public appendingConfigs: Array<ShellConfig>){}
}

export type All = ShellAction | AppendShellConfigsAction