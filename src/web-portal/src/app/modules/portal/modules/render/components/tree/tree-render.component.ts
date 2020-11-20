import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { Store } from '@ngxs/store';
import { StandardBoundSection, TreeBoundSection } from 'app/core/context/bound-section';
import { ExtendedPageSection, GroupControls } from 'app/core/models/extended.models';
import { DefaultControlOptions, MapDataControl, PageLoadedDatasource, PageRenderedControl } from 'app/core/models/page.model';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ObjectUtils } from 'app/core/utils/object-util';
import { Guid } from 'guid-typescript';
import { NGXLogger } from 'ngx-logger';
import { TreeStandardOptions } from 'portal/modules/models/standard.extended.model';
import { Observable, Subscription } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import { PageService } from 'services/page.service';
import { ControlType, StandardComponent } from 'services/portal.service';
import { AddSectionBoundDataForTree, BeginBuildingBoundData, EndRenderingPageSectionsAction, GatherSectionValidations, SectionValidationStateAction, UpdateTreeData } from 'stores/pages/page.actions';
import { PageStateModel } from 'stores/pages/page.state';
import { StandardSharedService } from '../standard/services/standard-shared.service';
import { StandardArrayDialog } from '../standard/standard-array-dialog.component';
import { TreeDataNode, TreeFlatNode } from './models/tree-node';

@Component({
    selector: 'let-tree-render',
    templateUrl: './tree-render.component.html',
    styleUrls: ['./tree-render.component.scss']
})
export class TreeRenderComponent implements OnInit, OnDestroy {
    @Input()
    section: ExtendedPageSection

    @Input()
    boundSection: TreeBoundSection

    standard: StandardComponent
    treeOptions: TreeStandardOptions
    boundData: any[]
    treeLocalData: TreeDataNode[]
    readyToRender = false
    maximumLevel = 2

    controlsGroups: Array<GroupControls>
    controls: Array<PageRenderedControl<DefaultControlOptions>>
    pageState$: Observable<PageStateModel>
    subscription: Subscription
    datasources: PageLoadedDatasource[]
    cloneOneItem: any

    sectionMap: MapDataControl[] = []
    formGroup: FormGroup

    flatMapNode = new Map<TreeFlatNode, TreeDataNode>()
    nestedNodeMap = new Map<TreeDataNode, TreeFlatNode>()
    dataMapNode = new Map<TreeDataNode, any>()
    treeControl: FlatTreeControl<TreeFlatNode>
    treeFlattener: MatTreeFlattener<TreeDataNode, TreeFlatNode>
    dataSource: MatTreeFlatDataSource<TreeDataNode, TreeFlatNode>

    storeName: string
    constructor(
        private dialog: MatDialog,
        private logger: NGXLogger,
        private fb: FormBuilder,
        private store: Store,
        private pageService: PageService,
        private standardSharedService: StandardSharedService
    ) { }

    ngOnDestroy(): void {
        this.subscription.unsubscribe()
    }

    ngOnInit(): void {
        this.standard = this.section.relatedTreeStandard
        this.treeOptions = TreeStandardOptions.getStandardOptions(this.section.relatedTreeStandard.options)
        this.maximumLevel = this.treeOptions.limitlevel - 1
        this.controls = this.standardSharedService
            .buildControlOptions(this.section.relatedTreeStandard.controls as PageRenderedControl<DefaultControlOptions>[])
            .filter(control => {
                return control.defaultOptions.checkRendered
            })
        this.section.relatedTreeStandard.controls = this.controls
        this.boundSection = new TreeBoundSection(this.section.name)
        this.storeName = ObjectUtils.isNotNull(this.section.sectionDatasource.dataStoreName) ? this.section.sectionDatasource.dataStoreName : this.section.name
        this.pageState$ = this.store.select<PageStateModel>(state => state.page)
        this.subscription = this.pageState$.pipe(
            filter(state => state.filterState
                && (state.filterState === EndRenderingPageSectionsAction
                    || state.filterState === BeginBuildingBoundData
                    || state.filterState === GatherSectionValidations)),
            tap(
                state => {
                    switch (state.filterState) {
                        case EndRenderingPageSectionsAction:
                            this.datasources = state.datasources
                            this.boundData =
                                ObjectUtils.clone(this.standardSharedService
                                    .buildTreeData(this.section.sectionDatasource.datasourceBindName, this.datasources, this.treeOptions))

                            this.cloneOneItem = this.buildEmptyData(this.controls)
                            // Add nodeid and nodeparentid
                            if (this.treeOptions.allowgenerateid) {
                                this.cloneOneItem[this.treeOptions.nodeidfield] = ''
                                this.cloneOneItem[this.treeOptions.nodeparentfield] = ''
                            }
                            this.controlsGroups = this.standardSharedService
                                .buildControlsGroup(
                                    this.controls,
                                    2)                            
                            break
                        case BeginBuildingBoundData:
                            this.initialTree()                            
                            this.store.dispatch(new AddSectionBoundDataForTree({
                                data: ObjectUtils.clone(this.boundData),
                                name: this.storeName
                            }))                            
                            this.readyToRender = true
                            break
                        case GatherSectionValidations:
                            if (state.specificValidatingSection === this.section.name
                                || !ObjectUtils.isNotNull(state.specificValidatingSection)) {
                                this.store.dispatch(new SectionValidationStateAction(this.section.name, true))
                            }
                            break
                    }
                }
            )
        ).subscribe()
    }

    getLevel = (node: TreeFlatNode) => node.level

    isExpandable = (node: TreeFlatNode) => node.expandable

    getChildren = (node: TreeDataNode): TreeDataNode[] => node.children

    hasChild = (_: number, _nodeData: TreeFlatNode) => _nodeData.expandable

    hasNoContent = (_: number, _nodeData: TreeFlatNode) => {
        this.logger.debug('current node data', _nodeData)
        return !ObjectUtils.isNotNull(_nodeData.name)
    }
    private initialTree() {
        // Transform Bound Data to TreeDataNode as nest
        // Note: boundData must be nested data, not array data
        this.treeFlattener = new MatTreeFlattener(this.transformer, this.getLevel,
            this.isExpandable, this.getChildren);
        this.treeControl = new FlatTreeControl<TreeFlatNode>(this.getLevel, this.isExpandable);
        this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
        this.treeLocalData = this.buildTreeDataNode(this.boundData, this.treeOptions)
        this.dataSource.data = this.treeLocalData
    }

    private transformer = (node: TreeDataNode, level: number) => {
        const existingNode = this.nestedNodeMap.get(node);
        let flatNode: TreeFlatNode = existingNode && existingNode.key === node.key
            ? existingNode
            : {
                key: node.key,
                level: level,
                expandable: !!node.children?.length,
                name: node.name
            } as TreeFlatNode

        // We need to set again expandable and name
        flatNode.name = node.name
        flatNode.expandable = !!node.children?.length
        this.flatMapNode.set(flatNode, node);
        this.nestedNodeMap.set(node, flatNode);
        return flatNode;
    }

    private buildTreeDataNode(data: any[], treeOptions: TreeStandardOptions, level: number = 0): TreeDataNode[] {
        // Ensure maximum level must have in the tree
        if (treeOptions.limitlevel < level) {
            return null
        }
        let returned: TreeDataNode[] = []
        data?.forEach(item => {
            let dataNode: TreeDataNode = {
                key: treeOptions.allowgenerateid ? item[treeOptions.nodeidfield] : Guid.create().toString(),
                name: item[treeOptions.displayname],
                children: ObjectUtils.isNotNull(item[treeOptions.inchildren]) ?
                    this.buildTreeDataNode(item[treeOptions.inchildren], treeOptions, level + 1) : null
            }
            this.dataMapNode.set(dataNode, item)
            returned.push(dataNode)
        })

        return returned
    }

    private buildEmptyData(controls: PageRenderedControl<DefaultControlOptions>[]): any {
        // If there are no row data, we need to create an empty object
        let emptyObject = new Object()
        controls?.forEach(control => {
            switch (control.type) {
                case ControlType.Number:
                    emptyObject[control.defaultOptions.bindname] = 0
                    break
                case ControlType.Slide:
                case ControlType.Checkbox:
                    emptyObject[control.defaultOptions.bindname] = false
                    break
                default:
                    emptyObject[control.defaultOptions.bindname] = ''
                    break
            }
        })
        return emptyObject
    }

    private getParentNode(node: TreeFlatNode): TreeFlatNode | null {
        const currentLevel = this.getLevel(node);

        if (currentLevel < 1) {
            return null;
        }

        const startIndex = this.treeControl.dataNodes.indexOf(node) - 1;

        for (let i = startIndex; i >= 0; i--) {
            const currentNode = this.treeControl.dataNodes[i];

            if (this.getLevel(currentNode) < currentLevel) {
                return currentNode;
            }
        }
        return null;
    }

    private setDataInFormGroup(setterObj: any, formGroup: FormGroup, controls: PageRenderedControl<DefaultControlOptions>[]) {
        controls?.forEach(control => {
            setterObj[control.defaultOptions.bindname] = formGroup.get(control.defaultOptions.bindname).value
        })

        return setterObj
    }

    private refreshTree() {
        const tempData = this.dataSource.data
        this.dataSource.data = []
        this.dataSource.data = tempData
        this.store.dispatch(new UpdateTreeData({
            storeName: this.storeName,
            treeData: ObjectUtils.clone(this.boundData)
        }))
    }

    edit(node: TreeFlatNode) {
        const treeDataNode = this.flatMapNode.get(node)
        this.formGroup = null
        this.boundSection.setOpenedSection(this.standardSharedService
            .buildBoundSection(
                this.section.name,
                null,
                this.standard,
                this.dataMapNode.get(treeDataNode),
                null,
                null) as StandardBoundSection)
        this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
        const dialogRef = this.dialog.open(StandardArrayDialog, {
            data: {
                controlsGroups: this.controlsGroups,
                formGroup: this.formGroup,
                section: this.section,
                isEdit: false,
                boundSection: this.boundSection,
                ids: []
            }
        })
        this.boundSection.opened = true
        this.boundSection.close = () => {
            dialogRef.close()
        }
        dialogRef.afterClosed().subscribe(res => {
            if (!!res) {
                let dataNode = this.dataMapNode.get(treeDataNode)
                this.setDataInFormGroup(dataNode, this.formGroup, this.controls)
                node.name = dataNode[this.treeOptions.displayname]
                treeDataNode.name = dataNode[this.treeOptions.displayname]
                this.refreshTree()
            }
            this.boundSection.opened = false
            this.boundSection.close = null
        })
    }

    addChild(node: TreeFlatNode) {
        this.formGroup = null
        let childData = ObjectUtils.clone(this.cloneOneItem)
        let parentNode = this.flatMapNode.get(node)
        let boundDataNode = this.dataMapNode.get(parentNode)
        if (!ObjectUtils.isNotNull(boundDataNode[this.treeOptions.inchildren])) {
            boundDataNode[this.treeOptions.inchildren] = []
        }
        boundDataNode[this.treeOptions.inchildren].push(childData)

        let newTreeDataNode: TreeDataNode = {
            name: childData[this.treeOptions.displayname],
            children: [],
            key: Guid.create().toString(),
        }

        if (!ObjectUtils.isNotNull(parentNode.children)) {
            parentNode.children = []
        }
        parentNode.children.push(newTreeDataNode)
        this.dataMapNode.set(newTreeDataNode, childData)
        this.dataSource.data = this.treeLocalData
        this.treeControl.expand(node)
        this.boundSection.setOpenedSection(this.standardSharedService
            .buildBoundSection(
                this.section.name,
                null,
                this.standard,
                childData,
                null,
                null) as StandardBoundSection)
        this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
        const dialogRef = this.dialog.open(StandardArrayDialog, {
            data: {
                controlsGroups: this.controlsGroups,
                formGroup: this.formGroup,
                section: this.section,
                isEdit: false,
                boundSection: this.boundSection,
                ids: []
            },
            disableClose: true
        })
        this.boundSection.opened = true
        this.boundSection.close = () => {
            dialogRef.close()
        }
        dialogRef.afterClosed().subscribe(res => {
            if (!!res) {
                this.setDataInFormGroup(childData, this.formGroup, this.controls)
                let treeFlatNode = this.nestedNodeMap.get(newTreeDataNode)
                treeFlatNode.name = childData[this.treeOptions.displayname]
                newTreeDataNode.name = childData[this.treeOptions.displayname]
                this.dataSource.data = this.treeLocalData
                this.refreshTree()
            }
            else {
                // We need to remove temp node
                const indexTreeNode = parentNode.children.indexOf(newTreeDataNode)
                const indexDataNode = boundDataNode[this.treeOptions.inchildren].indexOf(childData)
                boundDataNode[this.treeOptions.inchildren].splice(indexDataNode, 1)
                parentNode.children.splice(indexTreeNode, 1)
                this.dataSource.data = this.treeLocalData
                this.refreshTree()
            }

            this.boundSection.opened = false
            this.boundSection.close = null
        })
    }

    addBelow(node: TreeFlatNode) {
        this.formGroup = null
        let childData = ObjectUtils.clone(this.cloneOneItem)
        let newTreeDataNode: TreeDataNode = {
            name: childData[this.treeOptions.displayname],
            children: [],
            key: Guid.create().toString(),
        }
        if (node.level === 0) {
            const currentDataNode = this.flatMapNode.get(node)
            const currentData = this.dataMapNode.get(currentDataNode)
            const insertingIndexData = this.treeLocalData.indexOf(currentDataNode)
            const insertingIndex = this.boundData.indexOf(currentData)
            ArrayUtils.insertAtIndex(this.treeLocalData, newTreeDataNode, insertingIndexData + 1)
            ArrayUtils.insertAtIndex(this.boundData, childData, insertingIndex + 1)
            this.dataMapNode.set(newTreeDataNode, childData)
            this.dataSource.data = this.treeLocalData
            this.boundSection.setOpenedSection(this.standardSharedService
                .buildBoundSection(
                    this.section.name,
                    null,
                    this.standard,
                    childData,
                    null,
                    null) as StandardBoundSection)
            this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
            const dialogRef = this.dialog.open(StandardArrayDialog, {
                data: {
                    controlsGroups: this.controlsGroups,
                    formGroup: this.formGroup,
                    section: this.section,
                    isEdit: false,
                    boundSection: this.boundSection,
                    ids: []
                },
                disableClose: true
            })
            this.boundSection.opened = true
            this.boundSection.close = () => {
                dialogRef.close()
            }
            dialogRef.afterClosed().subscribe(res => {
                if (!!res) {
                    this.setDataInFormGroup(childData, this.formGroup, this.controls)
                    let treeFlatNode = this.nestedNodeMap.get(newTreeDataNode)
                    treeFlatNode.name = childData[this.treeOptions.displayname]
                    newTreeDataNode.name = childData[this.treeOptions.displayname]
                    this.dataSource.data = this.treeLocalData
                    this.refreshTree()
                }
                else {
                    // We need to remove temp node
                    this.boundData.splice(insertingIndex + 1, 1)
                    this.treeLocalData.splice(insertingIndexData + 1, 1)
                    this.dataSource.data = this.treeLocalData
                    this.refreshTree()
                }
                this.boundSection.opened = false
                this.boundSection.close = null
            })
        }
        else {
            let parentNode: TreeDataNode = null
            let boundDataNode: any
            let parentFlatNode: TreeFlatNode = null
            parentFlatNode = this.getParentNode(node)
            parentNode = this.flatMapNode.get(parentFlatNode)
            boundDataNode = this.dataMapNode.get(parentNode)
            const currentDataNode = this.flatMapNode.get(node)
            const insertingIndexNode = parentNode.children.indexOf(currentDataNode)
            const insertingIndex = boundDataNode[this.treeOptions.inchildren].indexOf(this.dataMapNode.get(currentDataNode))
            ArrayUtils.insertAtIndex(boundDataNode[this.treeOptions.inchildren], childData, insertingIndex + 1)
            ArrayUtils.insertAtIndex(parentNode.children, newTreeDataNode, insertingIndexNode + 1)
            this.dataMapNode.set(newTreeDataNode, childData)
            this.dataSource.data = this.treeLocalData
            this.treeControl.expand(parentFlatNode)
            this.boundSection.setOpenedSection(this.standardSharedService
                .buildBoundSection(
                    this.section.name,
                    null,
                    this.standard,
                    childData,
                    null,
                    null) as StandardBoundSection)
            this.formGroup = this.boundSection.getOpenedSection().getFormGroup()
            const dialogRef = this.dialog.open(StandardArrayDialog, {
                data: {
                    controlsGroups: this.controlsGroups,
                    formGroup: this.formGroup,
                    section: this.section,
                    isEdit: false,
                    boundSection: this.boundSection,
                    ids: []
                },
                disableClose: true
            })
            this.boundSection.opened = true
            this.boundSection.close = () => {
                dialogRef.close()
            }
            dialogRef.afterClosed().subscribe(res => {
                if (!!res) {
                    this.setDataInFormGroup(childData, this.formGroup, this.controls)
                    let treeFlatNode = this.nestedNodeMap.get(newTreeDataNode)
                    treeFlatNode.name = childData[this.treeOptions.displayname]
                    newTreeDataNode.name = childData[this.treeOptions.displayname]
                    this.dataSource.data = this.treeLocalData
                    this.refreshTree()
                }
                else {
                    // We need to remove temp node
                    const indexTreeNode = parentNode.children.indexOf(newTreeDataNode)
                    const indexDataNode = boundDataNode[this.treeOptions.inchildren].indexOf(childData)
                    boundDataNode[this.treeOptions.inchildren].splice(indexDataNode, 1)
                    parentNode.children.splice(indexTreeNode, 1)
                    this.dataSource.data = this.treeLocalData
                    this.refreshTree()
                }
                this.boundSection.opened = false
                this.boundSection.close = null
            })
        }
    }

    remove(node: TreeFlatNode) {
        // Removed node is top
        if (node.level === 0) {
            const removedDataNode = this.flatMapNode.get(node)          
            const removedIndex = this.treeLocalData.indexOf(removedDataNode)              
            const boundDataNode = this.dataMapNode.get(removedDataNode)
            const boundDataIndex = this.boundData.indexOf(boundDataNode)
            // Remove on boundata
            this.boundData.splice(boundDataIndex, 1)
            this.treeLocalData.splice(removedIndex, 1)
        }
        else {
            const parentNode = this.getParentNode(node)
            const removedDataNode = this.flatMapNode.get(node)
            const parentDataNode = this.flatMapNode.get(parentNode)
            const removedIndex = parentDataNode.children.indexOf(removedDataNode)
            const parentBoundData = this.dataMapNode.get(parentDataNode)
            const removedBoundData = this.dataMapNode.get(removedDataNode)
            const removedBoundDataIndex = parentBoundData[this.treeOptions.inchildren].indexOf(removedBoundData)
            parentBoundData[this.treeOptions.inchildren].splice(removedBoundDataIndex, 1)
            parentDataNode.children.splice(removedIndex, 1)
        }
        this.dataSource.data = this.treeLocalData
        this.refreshTree()
    }
}
