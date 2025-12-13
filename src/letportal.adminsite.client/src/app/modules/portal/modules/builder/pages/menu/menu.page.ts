import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { App, Menu, AppsClient, PagesClient } from 'services/portal.service';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Guid } from 'guid-typescript';
import { MenuDialogComponent } from '../../components/menu/components/menu-dialog.component';
import { NGXLogger } from 'ngx-logger';
import { ArrayUtils } from 'app/core/utils/array-util';
 
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { SecurityService } from 'app/core/security/security.service';
import { RouterExtService } from 'app/core/ext-service/routerext.service';
import { PageService } from 'services/page.service';
import { ExtendedMenu, MenuNode } from 'portal/modules/models/menu.model';
import { SessionService } from 'services/session.service';
import { ObjectUtils } from 'app/core/utils/object-util';
import { MatDialog } from '@angular/material/dialog';
import { MatTree, MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material/tree';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-menu',
    templateUrl: './menu.page.html',
    styleUrls: ['./menu.page.scss']
})
export class MenuPage implements OnInit, AfterViewInit {
    constructor(
        private pageService: PageService,
        private activetedRouter: ActivatedRoute,
        private dialog: MatDialog,
        private logger: NGXLogger,
        private appClient: AppsClient,
        private shortcutUtil: ShortcutUtil,
        private router: Router,
        private routerExtService: RouterExtService,
        private translate: TranslateService
    ) {
    }

    @ViewChild(MatTree, { static: true })
    tree: MatTree<any>;

    app: App
    menus: Array<ExtendedMenu> = []
    private transformer = (node: ExtendedMenu, level: number) => {
        return {
            expandable: !!node.subMenus && node.subMenus.length > 0,
            name: node.displayName,
            level,
            id: node.id,
            extMenu: node
        };
    }

    treeControl = new FlatTreeControl<MenuNode>(
        node => node.level, node => node.expandable);
    treeFlattener = new MatTreeFlattener(
        this.transformer, node => node.level, node => node.expandable, (node: ExtendedMenu) => node.subMenus);

    dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
    hasChild = (_: number, node: MenuNode) => node.expandable || node.level === 0;

    ngOnInit(): void {
        this.pageService.init('menus').subscribe()
        this.app = this.activetedRouter.snapshot.data.app
        this.menus = this.app.menus as ExtendedMenu[]
        this.dataSource.data = this.menus
    }

    ngAfterViewInit(): void {
        // Ensure we have nested node with more than 1
        if(this.menus.some(a => ObjectUtils.isNotNull(a.subMenus) && a.subMenus.length > 0)){
            this.tree.treeControl.expandAll()
        }
    }

    cancel(){
        this.router.navigateByUrl(this.routerExtService.getPreviousUrl())
    }

    saveMenu(){
        this.appClient.updateMenu(this.app.id, this.menus).subscribe(
            result => {
                this.shortcutUtil.toastMessage(this.translate.instant('common.updateSuccessfully'), ToastType.Success)
            }
        )
    }

    addMenu() {
        const menu: ExtendedMenu = {
            id: Guid.create().toString(),
            displayName: '',
            icon: '',
            menuPath: '~',
            url: '',
            parentId: '',
            order: this.menus.length,
            hide: false,
            subMenus: [],
            level: 0
        }

        const dialogRef = this.dialog.open(MenuDialogComponent, {
            data: {
                menu,
                isEditMode: false,
                appId: this.app.id,
                level: 0
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
        const menu: ExtendedMenu = {
            id: Guid.create().toString(),
            displayName: '',
            icon: '',
            menuPath: `${node.extMenu.menuPath}/${node.extMenu.id}`,
            url: '',
            order: node.extMenu.parentId ? this.findParent(node.extMenu).subMenus.length : this.menus.length,
            hide: false,
            parentId: `${node.id}`,
            subMenus: [],
            level: this.treeControl.getLevel(node) + 1
        }

        const dialogRef = this.dialog.open(MenuDialogComponent, {
            data: {
                menu,
                isEditMode: false,
                appId: this.app.id,
                level: menu.level
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
        if(menu.extMenu.menuPath == '~'){
            const foundMenu = this.menus.find(a => a.id == menu.extMenu.id)
            this.menus.splice(this.menus.indexOf(foundMenu), 1)
        }
        else{
            const parentMenu = this.findParent(menu.extMenu);
            parentMenu.subMenus = ArrayUtils.removeOneItem(parentMenu.subMenus, m => m.id === menu.id)
        }
        this.refreshTree()
    }

    editMenu(menu: MenuNode){
        const dialogRef = this.dialog.open(MenuDialogComponent, {
            data: {
                menu: menu.extMenu,
                isEditMode: true,
                appId: this.app.id,
                level: menu.level
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                if(menu.level === 0){
                    this.menus?.forEach(menuTemp => {
                        if(menuTemp.id === result.id){
                            menuTemp.displayName = result.displayName
                            menuTemp.url = result.url
                            return false
                        }
                    })
                }
                else{
                    this.menus?.forEach(menuTemp => {
                        if(menuTemp.id === result.parentId){
                            menuTemp.subMenus?.forEach(menuSub => {
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
        const addMenu: ExtendedMenu = {
            id: Guid.create().toString(),
            displayName: '',
            icon: '',
            menuPath: menu.extMenu.menuPath,
            url: '',
            order: menu.extMenu.order + 1,
            hide: false,
            parentId: menu.extMenu.parentId,
            subMenus: [],
            level: this.treeControl.getLevel(menu)
        }

        const dialogRef = this.dialog.open(MenuDialogComponent, {
            data: {
                menu: addMenu,
                isEditMode: false,
                appId: this.app.id,
                level: menu.level
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                if(menu.level === 0){
                    this.menus.push(result)
                }
                else{
                    const parentNode = this.findParent(result)
                    parentNode.subMenus?.forEach(subMenu => {
                        if(subMenu.order >= result.order){
                            subMenu.order += 1
                        }
                    })

                    parentNode.subMenus.push(result)
                    parentNode.subMenus = parentNode.subMenus.sort(sub => sub.order)
                    this.menus?.forEach(menuTemp => {
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
        const parentMenuTemp: ExtendedMenu = this.findParent(menu)
        parentMenuTemp.subMenus.push(menu)
    }

    findParent(menu: ExtendedMenu){
        const menuPaths = menu.menuPath.split('/')
        let parentMenuTemp: ExtendedMenu = null;
        menuPaths?.forEach(path => {
            if(path != '~'){
                const lookingMenus = parentMenuTemp ? parentMenuTemp.subMenus : this.menus
                parentMenuTemp = {
                    ...lookingMenus.find(menu => menu.id === path),
                    level: 0
                }
            }
        })
        return parentMenuTemp
    }

    private refreshTree() {
        this.logger.debug('current tree', this.menus)
        this.dataSource.data = this.menus
        // Ensure we have nested node with more than 1
        if(this.menus.some(a => ObjectUtils.isNotNull(a.subMenus) && a.subMenus.length > 0)){
            this.tree.treeControl.expandAll()
        }
    }

    private sortMenus(){
        this.menus = this.menus.sort(a => a.order)
        this.refreshTree()
    }

    private sortSubMenus(menu: ExtendedMenu){
        menu.subMenus = menu.subMenus.sort(a => a.order)
        return menu
    }
}
