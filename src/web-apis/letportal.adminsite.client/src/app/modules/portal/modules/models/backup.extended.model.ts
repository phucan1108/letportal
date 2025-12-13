import { ShortEntityModel } from 'services/portal.service';

export interface BackupNode{
    id: string
    name: string
    level: number
    checked: boolean
    expandable: boolean
    refModel: ShortEntityModel
}

export interface SelectableBackupNode extends ShortEntityModel{
    allowSelected: boolean
    subShortModels: SelectableBackupNode[],
    parentId: string
    checked: boolean
}