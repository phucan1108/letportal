import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { App, Menu, AppsClient, PagesClient } from 'services/portal.service';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MenuNode, ExtendedMenu } from './models/extended.model';
import { MatTreeFlattener, MatTreeFlatDataSource, MatDialog, MatTree } from '@angular/material';
import { Guid } from 'guid-typescript';
import { MenuDialogComponent } from './components/menu-dialog.component';
import { NGXLogger } from 'ngx-logger';
import { ArrayUtils } from 'app/core/utils/array-util';
import * as _ from 'lodash';
import { ShortcutUtil } from 'app/shared/components/shortcuts/shortcut-util';
import { MessageType, ToastType } from 'app/shared/components/shortcuts/shortcut.models';
import { SecurityService } from 'app/core/security/security.service';
import { RouterExtService } from 'app/core/ext-service/routerext.service';
import { PageService } from 'services/page.service';

@Component({
    selector: 'let-menu',
    templateUrl: './menu.component.html',
    styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit, AfterViewInit {
   
    @ViewChild(MatTree) 
    tree: MatTree<any>;

    private transformer = (node: ExtendedMenu, level: number) => {
        return {
            expandable: !!node.subMenus && node.subMenus.length > 0,
            name: node.displayName,
            level: level,
            id: node.id,
            extMenu: node
        };
    }
    hasChild = (_: number, node: MenuNode) => node.expandable || node.level === 0;

    app: App
    menus: Array<ExtendedMenu> = []
    treeControl = new FlatTreeControl<MenuNode>(
        node => node.level, node => node.expandable);
    treeFlattener = new MatTreeFlattener(
        this.transformer, node => node.level, node => node.expandable, (node: ExtendedMenu) => node.subMenus);

    dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
    constructor(
        private pageService: PageService,
        private activetedRouter: ActivatedRoute,
        private dialog: MatDialog,
        private logger: NGXLogger,
        private appClient: AppsClient,
        private shortcutUtil: ShortcutUtil,
        private pagesClient: PagesClient,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private routerExtService: RouterExtService,
        private security: SecurityService
    ) {
    }

    ngOnInit(): void {
        this.pageService.init('menus').subscribe()   
        this.app = this.activetedRouter.snapshot.data.app
        this.menus = this.app.menus as ExtendedMenu[]
        this.dataSource.data = this.menus
    }

    ngAfterViewInit(): void {
        this.tree.treeControl.expandAll()
    }

    cancel(){
        this.router.navigateByUrl(this.routerExtService.getPreviousUrl())
    }

    saveMenu(){
        this.appClient.updateMenu(this.app.id, {
            appId: this.app.id,
            menus: this.menus
        }).subscribe(
            result => {
                this.shortcutUtil.notifyMessage('Update menu successfully!', ToastType.Success)
            }
        )
    }

    addMenu() {
        let menu: ExtendedMenu = {
            id: Guid.create().toString(),
            displayName: '',
            icon: '',
            menuPath: '~',
            url: '',
            parentId: '',
            order: this.menus.length,
            hide: false,
            subMenus: []
        }

        const dialogRef = this.dialog.open(MenuDialogComponent, {
            data: {
                menu: menu,
                isEditMode: false,
                appId: this.app.id
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.menus.push(result)
                this.sortMenus()
            }
        })
    }

    addChild(node: MenuNode) {
        let menu: ExtendedMenu = {
            id: Guid.create().toString(),
            displayName: '',
            icon: '',
            menuPath: `${node.extMenu.menuPath}/${node.extMenu.id}`,
            url: '',
            order: node.extMenu.parentId ? this.findParent(node.extMenu).subMenus.length : this.menus.length, 
            hide: false,
            parentId: `${node.id}`,
            subMenus: []
        }

        const dialogRef = this.dialog.open(MenuDialogComponent, {
            data: {
                menu: menu,
                isEditMode: false,
                appId: this.app.id
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {                
                this.insertChild(result)
                this.refreshTree()
            }
        })
    }

    removeMenu(menu: MenuNode) {
        let parentMenu = this.findParent(menu.extMenu);
        parentMenu.subMenus = ArrayUtils.removeOneItem(parentMenu.subMenus, m => m.id === menu.id)
        this.refreshTree()
    }

    editMenu(menu: MenuNode){
        const dialogRef = this.dialog.open(MenuDialogComponent, {
            data: {
                menu: menu.extMenu,
                isEditMode: true,
                appId: this.app.id
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                if(menu.level === 0){
                    _.forEach(this.menus, menuTemp => {
                        if(menuTemp.id === result.id){
                            menuTemp.displayName = result.displayName
                            menuTemp.url = result.url
                            return false
                        }
                    })                    
                }
                else{
                    _.forEach(this.menus, menuTemp => {
                        if(menuTemp.id === result.parentId){
                            _.forEach(menuTemp.subMenus, menuSub => {
                                if(menuSub.id === result.id){
                                    menuSub.displayName = result.displayName
                                    menuSub.url = result.url
                                    return false
                                }
                            })

                            return false
                        }
                    })
                }
                this.refreshTree()
            }
        })
    }

    addBelow(menu: MenuNode){
        let addMenu: ExtendedMenu = {
            id: Guid.create().toString(),
            displayName: '',
            icon: '',
            menuPath: menu.extMenu.menuPath,
            url: '',
            order: menu.extMenu.order + 1,
            hide: false,
            parentId: menu.extMenu.parentId,
            subMenus: []
        }

        const dialogRef = this.dialog.open(MenuDialogComponent, {
            data: {
                menu: addMenu,
                isEditMode: false,
                appId: this.app.id
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                if(menu.level === 0){
                    this.menus.push(result)
                }
                else{
                    const parentNode = this.findParent(result)
                    _.forEach(parentNode.subMenus, subMenu => {
                        if(subMenu.order >= result.order){
                            subMenu.order += 1
                        }
                    })
    
                    parentNode.subMenus.push(result)
                    parentNode.subMenus = _.orderBy(parentNode.subMenus, sub => sub.order)
                    _.forEach(this.menus, menuTemp => {
                        if(menuTemp.id === parentNode.id){
                            menuTemp.subMenus = parentNode.subMenus
                            return false
                        }
                    })
                }
                
                this.refreshTree()
            }
        })
    }

    insertChild(menu: ExtendedMenu){
        let parentMenuTemp: ExtendedMenu = this.findParent(menu)
        parentMenuTemp.subMenus.push(menu)
    }

    findParent(menu: ExtendedMenu){
        let menuPaths = menu.menuPath.split('/')
        let parentMenuTemp: ExtendedMenu = null;
        _.forEach(menuPaths, path => {
            if(path != '~'){
                let lookingMenus = parentMenuTemp ? parentMenuTemp.subMenus : this.menus
                parentMenuTemp = _.find(lookingMenus, subMenu => subMenu.id === path)
            }
        })
        return parentMenuTemp
    }

    private refreshTree() {
        this.logger.debug('current tree', this.menus)
        this.dataSource.data = this.menus
        this.tree.treeControl.expandAll()
    }

    private sortMenus(){
        this.menus = _.orderBy(this.menus, a => a.order)
        this.refreshTree()
    }

    private sortSubMenus(menu: ExtendedMenu){
        menu.subMenus = _.orderBy(menu.subMenus, a => a.order)
        return menu
    }
}
