import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { ShellOption } from 'services/portal.service';
 

export class ArrayStandardOptions {
    
    public static IdentityField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Array - Identity field must be used to indicate one element. Default: id',
        key: 'identityfield',
        value: 'id'
    }

    public static NameField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Array - Names field must be used to display one element, can be multiple by ;. Default: name',
        key: 'namefield',
        value: 'name'
    }

    public static AllowAdjustment: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Array - Allow user to add or remove on element. Default: false',
        key: 'allowadjustment',
        value: 'false'
    }

    public static AllowUpdateParts: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Array - Allow to update one changed element instead of removing all and then adding all. Default: false',
        key: 'allowupdateparts',
        value: 'false'
    }

    identityfield: string
    namefield: string
    allowadjustment: boolean
    allowupdateparts: boolean

    public static getStandardOptions(options: ShellOption[]): ArrayStandardOptions {
        const identityfield = options.find(opt => opt.key === 'identityfield')
        const namefield = options.find(opt => opt.key === 'namefield')
        const allowadjustment = options.find(opt => opt.key === 'allowadjustment')
        const allowupdateparts = options.find(opt => opt.key === 'allowupdateparts')
        return {
            identityfield: identityfield ? identityfield.value : '',
            namefield: namefield ? namefield.value : '',
            allowadjustment: allowadjustment ? JSON.parse(allowadjustment.value) : false,
            allowupdateparts: allowupdateparts ? JSON.parse(allowupdateparts.value) : false
        }
    }

    public static getDefaultShellOptionsForArray(): ExtendedShellOption[] {
        return [
            this.IdentityField,
            this.NameField,
            this.AllowAdjustment,
            this.AllowUpdateParts
        ]
    }

    public static combinedDefaultShellOptions(opts: ExtendedShellOption[]) {
        const defaultOpts = this.getDefaultShellOptionsForArray()
        opts?.forEach(a => {
            const found = defaultOpts.find(b => b.key === a.key)
            if (found) {
                a.description = found.description
                a.allowDelete = found.allowDelete
                a.id = found.id
            }
        })
    }
}

export class TreeStandardOptions{
    

    public static InDataStructureType: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Defines the input structure as flat|nest. Flat is array data and nest is subset data. Default: nest',
        key: 'indatastructure',
        value: 'nest'
    }

    public static OutDataStructureType: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Defines the output structure as flat|nested. Flat is array data and nest is subset data. Default: nest',
        key: 'outdatastructure',
        value: 'nest'
    }

    public static LimitLevel: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Defines maximum node level in tree, it should be greater than 2. Default: 2',
        key: 'limitlevel',
        value: '2'
    }
    
    public static AllowGenerateId: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Allow to generate GUID for NodeId and NodeParentId. Default: false',
        key: 'allowgenerateid',
        value: 'false'
    }

    public static DisplayNameField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Defines a display name field which is used to be node name. Default: name',
        key: 'displayname',
        value: 'name'
    }

    public static InChildrenPropertyField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Property name of children in the nested input data. Default: children',
        key: 'inchildren',
        value: 'children'
    }

    public static OutChildrenPropertyField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Property name of children in the nested output data. Default: children',
        key: 'outchildren',
        value: 'children'
    }

    public static NodeIdField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Property name of node which indicates distinct node. Default: id',
        key: 'nodeidfield',
        value: 'id'
    }

    public static NodeParentIdField: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Tree - Property name of node which indicates parent field. Default: parentId',
        key: 'nodeparentfield',
        value: 'parentId'
    }

    public static DefaultArrayStandardOptions: ArrayStandardOptions = {
        identityfield: 'id',
        namefield: 'name',
        allowadjustment: false,
        allowupdateparts: false
    }

    public static DefaultTreeStandardOptions: TreeStandardOptions = {
        indatastructure: 'nest',
        outdatastructure: 'nest',
        displayname: 'name',
        limitlevel: 2,
        allowgenerateid: false,
        inchildren: 'children',
        outchildren: 'children',
        nodeidfield: 'id',
        nodeparentfield: 'parentId'
    }

    indatastructure: string
    outdatastructure: string
    displayname: string
    limitlevel: number
    allowgenerateid: boolean
    inchildren: string
    outchildren: string
    nodeidfield: string
    nodeparentfield: string

    public static getStandardOptions(options: ShellOption[]): TreeStandardOptions {
        const indatastructure = options.find(opt => opt.key === 'indatastructure')
        const outdatastructure = options.find(opt => opt.key === 'outdatastructure')
        const limitlevel = options.find(opt => opt.key === 'limitlevel')
        const displayname = options.find(opt => opt.key === 'displayname')
        const allowgenerateid = options.find(opt => opt.key === 'allowgenerateid')
        const inchildren = options.find(opt => opt.key === 'inchildren')
        const outchildren = options.find(opt => opt.key === 'outchildren')
        const nodeidfield = options.find(opt => opt.key === 'nodeidfield')
        const nodeparentfield = options.find(opt => opt.key === 'nodeparentfield')
        return {
            indatastructure: indatastructure ? indatastructure.value : 'nest',
            outdatastructure: outdatastructure ? outdatastructure.value : 'nest',
            displayname: displayname ? displayname.value : 'name',
            limitlevel: limitlevel ? parseInt(limitlevel.value) : 2,
            allowgenerateid: allowgenerateid ? JSON.parse(allowgenerateid.value) : false,
            inchildren: inchildren ? inchildren.value : 'children',
            outchildren: outchildren ? outchildren.value : 'children',
            nodeidfield: nodeidfield ? nodeidfield.value : 'id',
            nodeparentfield: nodeparentfield ? nodeparentfield.value : 'parentId',
        }
    }

    public static getDefaultShellOptionsForTreeStandard(): ExtendedShellOption[] {
        return [
            this.InDataStructureType,
            this.OutDataStructureType,
            this.DisplayNameField,
            this.AllowGenerateId,
            this.InChildrenPropertyField,
            this.OutChildrenPropertyField,
            this.NodeIdField,
            this.NodeParentIdField
        ]
    }

    public static combinedDefaultShellOptions(opts: ExtendedShellOption[]) {
        const defaultOpts = this.getDefaultShellOptionsForTreeStandard()
        opts?.forEach(a => {
            const found = defaultOpts.find(b => b.key === a.key)
            if (found) {
                a.description = found.description
                a.allowDelete = found.allowDelete
                a.id = found.id
            }
        })
    }
}