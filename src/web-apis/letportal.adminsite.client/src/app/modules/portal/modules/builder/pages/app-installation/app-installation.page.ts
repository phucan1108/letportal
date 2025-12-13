import { Component, OnInit, ViewChild } from '@angular/core';
import { AppsClient, UnpackResponseModel } from 'services/portal.service';
import { ObjectUtils } from 'app/core/utils/object-util';
import { TranslateService } from '@ngx-translate/core';
import { tap } from 'rxjs/internal/operators/tap';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { FormGroup } from '@angular/forms';
import { DateUtils } from 'app/core/utils/date-util';
import { MatDialog } from '@angular/material/dialog';
import { AppInstallationDialog } from '../../components/app-installation/app-installation.dialog';
import { Router } from '@angular/router';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatTreeFlattener, MatTree, MatTreeFlatDataSource } from '@angular/material/tree';
import { PageService } from 'services/page.service';

@Component({
    selector: 'let-app-installation',
    templateUrl: './app-installation.page.html',
    styleUrls: ['./app-installation.page.scss']
})
export class AppInstallationPage implements OnInit {
    unpackModel : UnpackResponseModel
    isUploaded = false
    createdDate: string

    @ViewChild(MatTree, { static: true })
    tree: MatTree<any>;

    private transformer = (node: any, level: number) => {
        return {
            expandable: !!node.sub && node.sub.length > 0,
            name: node.name,
            level,
            id: node.name,
            numberChildren: !!node.sub ? node.sub.length : 0,
            isExisted: ObjectUtils.isNotNull(node.isExisted) ? <boolean>node.isExisted : false
        };
    }

    treeControl = new FlatTreeControl<UnPackageNode>(
        node => node.level, node => node.expandable);
    treeFlattener = new MatTreeFlattener(
        this.transformer, node => node.level, node => node.expandable, (node: any) => node.sub);

    dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
    hasChild = (_: number, node: UnPackageNode) => node.expandable || node.level === 0;
    constructor(
        private appsClient: AppsClient,
        private dialog: MatDialog,
        private translate: TranslateService,
        private router: Router,
        private shortcut: ShortcutUtil,
        private pageService: PageService
    ) { }

    ngOnInit(): void { 
        this.pageService.init('app-installation').subscribe()
    }

    onFileChange($event){
        const latestFile: File = $event.target.files[$event.target.files.length - 1]
        if (ObjectUtils.isNotNull(latestFile)) {
            if (latestFile.name.indexOf('.zip') < 0) {
                window.alert(this.translate.instant('pageBuilder.page.appInstallation.uploadFile.errors.fileType'))
            }
            else {
                this.appsClient.unpack({
                    data: latestFile,
                    fileName: latestFile.name
                }).pipe(
                    tap(
                        res => {
                            this.unpackModel = res
                            this.createdDate = DateUtils.toDateFormat(this.unpackModel.packagedDate, 'MMM DD YYYY HH:mm')
                            this.isUploaded = true
                            this.buildTreeData()
                        },
                        err => {
                            this.shortcut.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error) 
                        }
                    )
                ).subscribe()
            }
        }
    }

    onInstall(){
        const dialogRef = this.dialog.open(AppInstallationDialog,{
            data: {
                unpack: this.unpackModel
            }
        })

        dialogRef.afterClosed().subscribe(
            res => {
                if(!!res){
                    this.shortcut.toastMessage('Install successfully!!!', ToastType.Success)
                    this.router.navigateByUrl('portal/page/apps-management')
                }
            }
        )
    }

    private buildTreeData(){
        let tree: Array<any> = []
        if(ObjectUtils.isNotNull(this.unpackModel.standards)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.standards')
            treeNode['isExisted'] = false
            treeNode['sub'] = []
            this.unpackModel.standards?.forEach(a => {
                const subName = new Object()
                subName['name'] = a.name
                subName['isExisted'] = a.isExisted
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        if(ObjectUtils.isNotNull(this.unpackModel.dynamicLists)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.dynamicLists')
            treeNode['isExisted'] = false
            treeNode['sub'] = []
            this.unpackModel.dynamicLists?.forEach(a => {
                const subName = new Object()
                subName['name'] = a.name
                subName['isExisted'] = a.isExisted
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        if(ObjectUtils.isNotNull(this.unpackModel.charts)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.charts')
            treeNode['isExisted'] = false
            treeNode['sub'] = []
            this.unpackModel.charts?.forEach(a => {
                const subName = new Object()
                subName['name'] = a.name
                subName['isExisted'] = a.isExisted
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        if(ObjectUtils.isNotNull(this.unpackModel.pages)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.pages')
            treeNode['isExisted'] = false
            treeNode['sub'] = []
            this.unpackModel.pages?.forEach(a => {
                const subName = new Object()
                subName['name'] = a.name
                subName['isExisted'] = a.isExisted
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        if(ObjectUtils.isNotNull(this.unpackModel.locales)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.locale')
            treeNode['isExisted'] = false
            treeNode['sub'] = []
            this.unpackModel.locales?.forEach(a => {
                const subName = new Object()
                subName['name'] = a.name
                subName['isExisted'] = a.isExisted
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        this.dataSource.data = tree
    }

    onCancel(){
        this.router.navigateByUrl('portal/page/apps-management')
    }
}


interface UnPackageNode{
    numberChildren: number
    expandable: boolean
    name: string
    level: number
    isExisted: boolean
    id: string
}