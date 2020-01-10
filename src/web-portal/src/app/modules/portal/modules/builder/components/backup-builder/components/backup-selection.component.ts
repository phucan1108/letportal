import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { ShortEntityModel } from 'services/portal.service';
import { MatTree, MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material';
import { SelectableBackupNode, BackupNode } from 'portal/modules/models/backup.extended.model';
import { SelectionModel } from '@angular/cdk/collections';
import * as _ from 'lodash';
import { FlatTreeControl } from '@angular/cdk/tree';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';

@Component({
    selector: 'let-backup-selection',
    templateUrl: './backup-selection.component.html'
})
export class BackupSelectionComponent implements OnInit {
    @Input()
    data: BehaviorSubject<ShortEntityModel[]>

    @Input()
    name: string

    @Input()
    icon: string

    @Output()
    searchChanged = new EventEmitter<any>()

    @Output()
    selectionEntitiesChanged = new EventEmitter<any>()
    
    @ViewChild('tree')
    tree: MatTree<any>
    selectedEntities$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    selectedEntities: ShortEntityModel[] = []
    selectableEntityNodes: SelectableBackupNode[] = []
    checkEntitieslistSelection = new SelectionModel<BackupNode>(true)
    private transformer = (node: SelectableBackupNode, level: number) => {
        let selectableNode: BackupNode = {
            id: node.id,
            name: node.displayName,
            checked: false,
            level: level,
            expandable: !!node.subShortModels && node.subShortModels.length > 0,
            refModel: node
        }
        if (!!this.selectedEntities && this.selectedEntities.length > 0 && level > 0) {
            const found = _.find(this.selectedEntities, entity => entity.id === node.parentId)
            if (!!found) {
                selectableNode.checked = found == selectableNode.id
                if (selectableNode.checked) {
                    this.checkEntitieslistSelection.select(selectableNode)
                }
            }
        }

        return selectableNode;
    }
    hasChild = (_: number, node: BackupNode) => node.expandable || node.level === 0;
    treeControl = new FlatTreeControl<BackupNode>(
        node => node.level, node => node.expandable);
    treeFlattener = new MatTreeFlattener(
        this.transformer, node => node.level, node => node.expandable, (node: SelectableBackupNode) => node.subShortModels);

    dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
    hasSelected(node: BackupNode) {
        return this.checkEntitieslistSelection.isSelected(node)
    }
    onTreeChange($event, node: BackupNode) {
        this.checkEntitieslistSelection.toggle(node)
        const found = this.selectedEntities.find(a => a.id === node.id)
        let canRemove = !!found
        if (canRemove) {
            this.selectedEntities = ArrayUtils.removeOneItem(this.selectedEntities, a => a.id === node.id)
            this.selectedEntities$.next(this.selectedEntities)
        }
        else {
            this.selectedEntities = ArrayUtils.appendItemsDistinct(this.selectedEntities, [node.refModel], 'id')
            this.selectedEntities$.next(this.selectedEntities)
        }

        this.selectionEntitiesChanged.emit(this.selectedEntities)
    }
    filter(filterText: string) {
        this.searchChanged.emit(filterText)
    }

    loadTree(models: ShortEntityModel[]) {
        if (!ObjectUtils.isNotNull(models) || models.length === 0) {
            this.dataSource.data = []
        }
        else {
            let entitiesModel: SelectableBackupNode[] = []
            let rootNode: SelectableBackupNode = {
                id: this.name.toLowerCase(),
                allowSelected: false,
                parentId: '',
                displayName: this.name,
                subShortModels: []
            }
            _.forEach(models, app => {
                rootNode.subShortModels.push({
                    id: app.id,
                    displayName: app.displayName,
                    parentId: this.name.toLowerCase(),
                    allowSelected: true,
                    subShortModels: null
                })
            })
            entitiesModel.push(rootNode)
            this.dataSource.data = entitiesModel
            this.treeControl.expandAll()
        }
    }

    constructor() { }

    ngOnInit(): void { 
        this.data.subscribe(
            res => {
                this.loadTree(res)
            }
        )
    }
}
