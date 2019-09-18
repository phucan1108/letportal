import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MatTree, MatDialog, MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ActivatedRoute, Router } from '@angular/router';
import { SessionService } from 'services/session.service';
import { RouterExtService } from 'app/core/ext-service/routerext.service';
import { SecurityService } from 'app/core/security/security.service';
import { PagesClient, ClaimValueType, AppsClient } from 'services/portal.service';
import { FlatTreeControl } from '@angular/cdk/tree';
import { SelectionModel } from '@angular/cdk/collections';
import { RolePortalClaimModel, RolesClient, PortalClaimModel } from 'services/identity.service';
import * as _ from 'lodash';
import { combineLatest } from 'rxjs';
import { NGXLogger } from 'ngx-logger';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { PageService } from 'services/page.service';
import { SelectablePortalClaim, ClaimNode } from 'portal/modules/models/role-claims.model';

@Component({
    selector: 'let-role-claims',
    templateUrl: './role-claims.page.html',
    styleUrls: ['./role-claims.page.scss']
})
export class RoleClaimsPage implements OnInit {
    @ViewChild('claimTree')
    claimTree: MatTree<any>;
    private transformer = (node: SelectablePortalClaim, level: number) => {
        let claimNode: ClaimNode = {
            id: node.name,
            name: node.displayName,
            checked: false,
            level: level,
            parentId: node.parentId,
            expandable: !!node.subClaims && node.subClaims.length > 0
        }
        if (!!this.selectedRolePortalClaims && this.selectedRolePortalClaims.length > 0 && level > 0) {
            const found = _.find(this.selectedRolePortalClaims, claim => claim.name === node.parentId)
            if (!!found) {
                claimNode.checked = found.claims.indexOf(claimNode.id) > -1
                if (claimNode.checked) {
                    this.checklistSelection.select(claimNode)
                }
            }
        }

        return claimNode;
    }
    hasChild = (_: number, node: ClaimNode) => node.expandable || node.level === 0;
    selectedRole = ''

    treeControl = new FlatTreeControl<ClaimNode>(
        node => node.level, node => node.expandable);
    treeFlattener = new MatTreeFlattener(
        this.transformer, node => node.level, node => node.expandable, (node: SelectablePortalClaim) => node.subClaims);

    dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

    allPortalClaims: SelectablePortalClaim[] = []
    checklistSelection = new SelectionModel<ClaimNode>(true)

    selectedRolePortalClaims: RolePortalClaimModel[] = []
    selectedClaims: SelectablePortalClaim[] = []

    private transformerSelectedClaim = (node: SelectablePortalClaim, level: number) => {
        let claimNode: ClaimNode = {
            id: node.name,
            name: node.displayName,
            checked: false,
            level: level,
            expandable: !!node.subClaims && node.subClaims.length > 0,
            parentId: node.parentId
        }

        return claimNode;
    }
    treeSelectedControl = new FlatTreeControl<ClaimNode>(
        node => node.level, node => node.expandable);
    treeSelectedFlattener = new MatTreeFlattener(
        this.transformerSelectedClaim, node => node.level, node => node.expandable, (node: SelectablePortalClaim) => node.subClaims);

    dataSourceSelectedClaims = new MatTreeFlatDataSource(this.treeSelectedControl, this.treeSelectedFlattener);

    constructor(
        private pageService: PageService,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
        private shortcutUtil: ShortcutUtil,
        private activatedRoute: ActivatedRoute,
        private session: SessionService,
        private routerExtService: RouterExtService,
        private router: Router,
        private pagesClient: PagesClient,
        private security: SecurityService,
        private appClient: AppsClient,
        private roleClient: RolesClient,
        private logger: NGXLogger
    ) {
    }

    ngOnInit(): void {
        this.pageService.init('role-claims').subscribe()
        this.activatedRoute.paramMap.subscribe(params => {
            this.selectedRole = params.get('roleName')
        })
        this.selectedRolePortalClaims = this.activatedRoute.snapshot.data.roleClaims

        this.logger.debug('selected claims', this.selectedRolePortalClaims)
        combineLatest(this.pagesClient.getAllPortalClaims(), this.appClient.getAllApps(), (v1, v2) => ({ v1, v2 })).subscribe(
            pair => {
                let allclaims: SelectablePortalClaim[] = []
                let appClaim: SelectablePortalClaim = {
                    name: 'apps',
                    parentId: '',
                    displayName: 'Apps',
                    allowSelected: false,
                    claimValueType: ClaimValueType.Array,
                    subClaims: []
                }
                _.forEach(pair.v2, app => {
                    appClaim.subClaims.push({
                        name: app.id,
                        displayName: app.displayName,
                        claimValueType: ClaimValueType.Boolean,
                        parentId: appClaim.name,
                        allowSelected: true,
                        subClaims: null
                    })
                })
                allclaims.push(appClaim)
                _.forEach(pair.v1, page => {
                    let pageClaim: SelectablePortalClaim = {
                        parentId: '',
                        name: page.pageName,
                        displayName: page.pageDisplayName,
                        allowSelected: false,
                        subClaims: [],
                        claimValueType: ClaimValueType.Array
                    }
                    _.forEach(page.claims, subClaim => {
                        let sub: SelectablePortalClaim = {
                            name: subClaim.name,
                            displayName: subClaim.displayName,
                            allowSelected: true,
                            claimValueType: subClaim.claimValueType,
                            subClaims: null,
                            parentId: page.pageName
                        }
                        pageClaim.subClaims.push(sub)
                    })

                    allclaims.push(pageClaim)
                })

                this.allPortalClaims = allclaims
                this.dataSource.data = this.allPortalClaims
                this.selectedClaims = this.initSelectedClaims()
                this.dataSourceSelectedClaims.data = this.selectedClaims
                this.treeSelectedControl.expandAll()
            }
        )
    }

    hasSelected(claimNode: ClaimNode) {
        return this.checklistSelection.isSelected(claimNode)
    }
    onChange($event, node: ClaimNode) {
        if (this.selectedRole) {
            this.checklistSelection.toggle(node)
            this.selectedClaims = this.filterSelectedClaims()
            this.dataSourceSelectedClaims.data = this.selectedClaims
            this.treeSelectedControl.expandAll()
        }
        else {
        }
    }

    initSelectedClaims() {
        let selectedClaims: SelectablePortalClaim[] = []
        _.forEach(this.allPortalClaims, claim => {
            let selectedClaim: SelectablePortalClaim = _.cloneDeep(claim)
            selectedClaim.subClaims = []
            let canAdd = false
            _.forEach(claim.subClaims, sub => {
                const found = _.find(this.selectedRolePortalClaims, selected => selected.name === claim.name)
                if (found) {
                    const foundClaim = _.find(found.claims, portalclaim => portalclaim === sub.name)
                    if (foundClaim) {
                        canAdd = true
                        selectedClaim.subClaims.push(_.cloneDeep(sub))
                    }
                }
            })
            if (canAdd) {
                selectedClaims.push(selectedClaim)
            }
        })
        this.logger.debug('Init Selected Claims', selectedClaims)
        return selectedClaims
    }

    filterSelectedClaims() {
        let selectedClaims: SelectablePortalClaim[] = []
        _.forEach(this.allPortalClaims, claim => {
            let selectedClaim: SelectablePortalClaim = _.cloneDeep(claim)
            selectedClaim.subClaims = []
            let canAdd = false
            _.forEach(claim.subClaims, sub => {
                const found = _.find(this.checklistSelection.selected, selected => selected.id === sub.name && selected.parentId === claim.name)
                if (found) {
                    canAdd = true
                    selectedClaim.subClaims.push(_.cloneDeep(sub))
                }
            })
            if (canAdd) {
                selectedClaims.push(selectedClaim)
            }
        })
        return selectedClaims
    }

    public filter(filterText: string) {
        let filteredClaimsData: SelectablePortalClaim[] = []
        if(filterText){
            const filtered = this.allPortalClaims.filter(claim => claim.displayName.toLowerCase().indexOf(filterText.toLowerCase()) > -1)
            filteredClaimsData = _.cloneDeep(filtered)
            this.dataSource.data = filteredClaimsData
        }
        else{
            this.dataSource.data = this.allPortalClaims
        }
    }

    saveChange() {
        this.roleClient.addPortalClaims(this.selectedRole, this.mapToPortalClaimModel()).subscribe(
            result => {
                this.shortcutUtil.notifyMessage('Update successfully', ToastType.Success)
            },
            err => {
            })
    }

    mapToPortalClaimModel() {
        let portalClaimModels: PortalClaimModel[] = []
        this.logger.debug('Selected Claims', this.selectedClaims)
        _.forEach(this.selectedClaims, claim => {
            _.forEach(claim.subClaims, sub => {
                portalClaimModels.push({
                    name: claim.name,
                    value: sub.name
                })
            })
        })
        return portalClaimModels
    }

    cancel() {
        this.router.navigateByUrl('portal/page/roles-management')
    }
}
