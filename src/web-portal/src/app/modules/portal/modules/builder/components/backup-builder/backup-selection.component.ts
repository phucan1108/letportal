import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatTree, MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { BackupNode, SelectableBackupNode } from 'portal/modules/models/backup.extended.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { ShortEntityModel } from 'services/portal.service';


@Component({
    selector: 'let-backup-selection',
    templateUrl: './backup-selection.component.html'
})
export class BackupSelectionComponent implements OnInit {

    constructor() { }
    @Input()
    data: BehaviorSubject<ShortEntityModel[]>

    @Input()
    name: string

    @Input()
    icon: string

    @Input()
    notifier: Observable<ShortEntityModel[]>

    @Output()
    searchChanged = new EventEmitter<any>()

    @Output()
    selectionEntitiesChanged = new EventEmitter<any>()

    @ViewChild('tree', { static: false })
    tree: MatTree<any>
    selectedEntities$: BehaviorSubject<ShortEntityModel[]> = new BehaviorSubject([])
    selectedEntities: ShortEntityModel[] = []
    selectableEntityNodes: SelectableBackupNode[] = []
    checkEntitieslistSelection = new SelectionModel<BackupNode>(true)

    private transformer = (node: SelectableBackupNode, level: number) => {
        const selectableNode: BackupNode = {
            id: node.id,
            name: node.displayName,
            checked: node.checked,
            level,
            expandable: !!node.subShortModels && node.subShortModels.length > 0,
            refModel: node
        }
        if (!!this.selectedEntities && this.selectedEntities.length > 0 && level > 0) {
            const found = this.selectedEntities.find(entity => entity.id === node.id)
            if (!!found) {
                selectableNode.checked = found.id == selectableNode.id
                if (selectableNode.checked) {
                    console.log('Hit selection ', selectableNode)
                    this.checkEntitieslistSelection.select(selectableNode)
                }
            }
        }

        return selectableNode;
    }

    treeControl = new FlatTreeControl<BackupNode>(
        node => node.level, node => node.expandable);
    treeFlattener = new MatTreeFlattener(
        this.transformer, node => node.level, node => node.expandable, (node: SelectableBackupNode) => node.subShortModels);

    dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

    hasChild = (_: number, node: BackupNode) => node.expandable || node.level === 0;
    hasSelected(node: BackupNode) {
        return this.checkEntitieslistSelection.isSelected(node)
    }
    onTreeChange($event, node: BackupNode) {
        this.checkEntitieslistSelection.toggle(node)
        const found = this.selectedEntities.find(a => a.id === node.id)
        const canRemove = !!found
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

    loadTree(models: ShortEntityModel[], selectedAll = false) {
        if (!ObjectUtils.isNotNull(models) || models.length === 0) {
            this.dataSource.data = []
        }
        else {
            const entitiesModel: SelectableBackupNode[] = []
            const rootNode: SelectableBackupNode = {
                id: this.name.toLowerCase(),
                allowSelected: false,
                parentId: '',
                displayName: this.name,
                subShortModels: [],
                checked: selectedAll
            }
            models?.forEach(app => {
                rootNode.subShortModels.push({
                    id: app.id,
                    displayName: app.displayName,
                    parentId: this.name.toLowerCase(),
                    allowSelected: true,
                    subShortModels: null,
                    checked: selectedAll
                })
            })
            entitiesModel.push(rootNode)
            this.dataSource.data = entitiesModel
            this.treeControl.expandAll()
        }
    }

    ngOnInit(): void {
        this.data.subscribe(
            res => {
                this.loadTree(res)
            }
        )

        this.notifier.subscribe(
            res => {
                this.selectedEntities = res   
                this.selectedEntities$.next(res)             
                this.loadTree(res, true)
                this.selectionEntitiesChanged.emit(this.selectedEntities)
            }
        )
    }
}
