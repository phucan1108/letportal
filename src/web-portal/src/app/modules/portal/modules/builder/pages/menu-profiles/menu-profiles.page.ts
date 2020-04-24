import { Component, OnInit, ViewChild } from '@angular/core';
import { App, AppsClient, MenuProfile } from 'services/portal.service';
import { FlatTreeControl } from '@angular/cdk/tree';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { Role, RolesClient } from 'services/identity.service';
import * as _ from 'lodash';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { RouterExtService } from 'app/core/ext-service/routerext.service';
import { SelectionModel } from '@angular/cdk/collections';
import { ExtendedMenu, MenuNode } from 'portal/modules/models/menu.model';
import { MatDialog } from '@angular/material/dialog';
import { MatTree, MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material/tree';

@Component({
    selector: 'let-menu-profiles',
    templateUrl: './menu-profiles.page.html'
})
export class MenuProfilesPage implements OnInit {
    constructor(
        private activetedRouter: ActivatedRoute,
        private dialog: MatDialog,
        private logger: NGXLogger,
        private appClient: AppsClient,
        private roleClient: RolesClient,
        private shortcutUtil: ShortcutUtil,
        private router: Router,
        private routerService: RouterExtService
    ) {
        this.app = this.activetedRouter.snapshot.data.app
        this.menus = this.app.menus as ExtendedMenu[]
        this.dataSource.data = this.menus
        this.roleClient.getAllRoles().subscribe(
            result => {
                this.roles = result
            }
        )
    }
    @ViewChild('menuTree', { static: false })
    menuTree: MatTree<any>;

    app: App
    selectedRole = ''

    roles: Array<Role> = []
    menus: Array<ExtendedMenu> = []

    private transformer = (node: ExtendedMenu, level: number) => {
        let isChecked = false
        if(this.selectedRole && this.menuProfile){
            const index = this.menuProfile.menuIds.indexOf(node.id)
            isChecked = index > -1
        }
        const menuNode: MenuNode = {
            expandable: !!node.subMenus && node.subMenus.length > 0,
            name: node.displayName,
            level,
            id: node.id,
            extMenu: node,
            checked: isChecked
        }
        if(isChecked){
            this.checklistSelection.select(menuNode)
        }

        return menuNode;
    }

    treeControl = new FlatTreeControl<MenuNode>(
        node => node.level, node => node.expandable);
    treeFlattener = new MatTreeFlattener(
        this.transformer, node => node.level, node => node.expandable, (node: ExtendedMenu) => node.subMenus);

    dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

    checklistSelection = new SelectionModel<MenuNode>(true)
    menuProfile: MenuProfile

    
    hasChild = (_: number, node: MenuNode) => node.expandable || node.level === 0;

    ngOnInit(): void {
    }

    onRoleChange() {
        // Refill the claim list based on role
        this.menuProfile = _.find(this.app.menuProfiles, profile => profile.role === this.selectedRole)
        if (this.menuProfile) {

        }
        else {
            this.menuProfile = {
                role: this.selectedRole,
                menuIds: []
            }
        }
        this.checklistSelection = new SelectionModel<MenuNode>(true)
        this.dataSource.data = this.menus
    }

    hasSelected(menu: MenuNode) {
        return this.checklistSelection.isSelected(menu)
    }

    saveChange() {
        if (this.menuProfile && this.selectedRole) {
            this.appClient.asssignRolesToMenu( this.app.id, this.menuProfile).subscribe(
                result => {
                    this.shortcutUtil.toastMessage('Update successfully', ToastType.Success)
                },
                err => {
                })
        }
        else {
            this.shortcutUtil.toastMessage('Please select one role', ToastType.Warning)
        }
    }

    backToAppList() {
        this.router.navigateByUrl(this.routerService.getPreviousUrl())
    }

    onChange($event, node: MenuNode) {
        if (this.selectedRole) {
            this.checklistSelection.toggle(node)
            const descendants = this.treeControl.getDescendants(node);
            this.checklistSelection.isSelected(node)
                ? this.checklistSelection.select(...descendants)
                : this.checklistSelection.deselect(...descendants);

            this.menuProfile.menuIds = []
            _.forEach(this.checklistSelection.selected, a => {
                this.menuProfile.menuIds.push(a.extMenu.id)
            })
            this.logger.debug('Menu profile', this.menuProfile)
        }
        else {
            // node.checked = false
        }
    }

    private findParent(menu: ExtendedMenu) {
        const menuPaths = menu.menuPath.split('/')
        let parentMenuTemp: ExtendedMenu = null;
        _.forEach(menuPaths, path => {
            if (path != '~') {
                const lookingMenus = parentMenuTemp ? parentMenuTemp.subMenus : this.menus
                parentMenuTemp = {
                    ..._.find(lookingMenus, subMenu => subMenu.id === path),
                    level: 0
                }
            }
        })
        return parentMenuTemp
    }
}
