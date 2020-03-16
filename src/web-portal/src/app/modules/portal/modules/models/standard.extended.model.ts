import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { ShellOption } from 'services/portal.service';
import * as _ from 'lodash';

export class StandardOptions {

    identityfield: string
    namefield: string
    allowadjustment: boolean
    allowupdateparts: boolean

    public static getStandardOptions(options: ShellOption[]): StandardOptions {
        let identityfield = _.find(options, opt => opt.key === 'identityfield')
        let namefield = _.find(options, opt => opt.key === 'namefield')
        let allowadjustment = _.find(options, opt => opt.key === 'allowadjustment')
        let allowupdateparts = _.find(options, opt => opt.key === 'allowupdateparts')
        return {
            identityfield: identityfield ? identityfield.value : '',
            namefield: namefield ? namefield.value : '',
            allowadjustment: allowadjustment ? JSON.parse(allowadjustment.value) : false,
            allowupdateparts: allowupdateparts ? JSON.parse(allowupdateparts.value) : false
        }
    }

    public static IdentityField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Array Standard: Identity field must be used to indicate one element. Default: id',
        key: 'identityfield',
        value: 'id'
    }

    public static NameField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Array Standard: Name field must be used to display one element. Default: name',
        key: 'namefield',
        value: 'name'
    }
    
    public static AllowAdjustment: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Array Standard: Allow user to add or remove on element. Default: false',
        key: 'allowadjustment',
        value: 'false'
    }

    public static AllowUpdateParts: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Allow to update one changed element instead of removing all and then adding all. Default: false',
        key: 'allowupdateparts',
        value: 'false'
    }

    public static getDefaultShellOptionsForStandard(): ExtendedShellOption[] {
        return [
            this.IdentityField,
            this.NameField,
            this.AllowAdjustment,
            this.AllowUpdateParts
        ]
    }

    public static combinedDefaultShellOptions(opts: ExtendedShellOption[]) {
        const defaultOpts = this.getDefaultShellOptionsForStandard()
        opts.forEach(a => {
            const found = defaultOpts.find(b => b.key === a.key)
            if (found) {
                a.description = found.description
                a.allowDelete = found.allowDelete
                a.id = found.id
            }
        })
    }

    public static DefaultStandardOptions: StandardOptions = {
        identityfield: 'id',
        namefield: 'name',
        allowadjustment: false,
        allowupdateparts: false
    }
}