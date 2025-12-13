import { ShellOption } from 'services/portal.service';

export interface ExtendedShellOption extends ShellOption {
    id: string
    description: string,
    allowDelete: boolean
}
