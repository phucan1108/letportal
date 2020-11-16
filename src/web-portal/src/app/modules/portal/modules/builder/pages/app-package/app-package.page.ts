import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PreviewPackageModel, AppsClient } from 'services/portal.service';
import { MatTree, MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material/tree';
import { FlatTreeControl } from '@angular/cdk/tree';
import { ObjectUtils } from 'app/core/utils/object-util';
import { SecurityService } from 'app/core/security/security.service';
import { tap } from 'rxjs/operators';
import { DownloadFileService } from 'services/downloadfile.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { TranslateService } from '@ngx-translate/core';
import { ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { FormUtil } from 'app/core/utils/form-util';
import { PageService } from 'services/page.service';

@Component({
    selector: 'let-app-package',
    templateUrl: './app-package.page.html'
})
export class AppPackagePage implements OnInit {

    previewApp: PreviewPackageModel

    packageFormGroup: FormGroup

    @ViewChild(MatTree, { static: true })
    tree: MatTree<any>;

    private transformer = (node: any, level: number) => {
        return {
            expandable: !!node.sub && node.sub.length > 0,
            name: node.name,
            level,
            id: node.name,
            numberChildren: !!node.sub ? node.sub.length : 0
        };
    }

    treeControl = new FlatTreeControl<PackageNode>(
        node => node.level, node => node.expandable);
    treeFlattener = new MatTreeFlattener(
        this.transformer, node => node.level, node => node.expandable, (node: any) => node.sub);

    dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
    hasChild = (_: number, node: PackageNode) => node.expandable || node.level === 0;

    constructor(
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private appsClient: AppsClient,
        private security: SecurityService,
        private downloadFileService: DownloadFileService,
        private fb: FormBuilder,
        private translate: TranslateService,
        private shortcut: ShortcutUtil,
        private pageService: PageService
    ) { 

    }

    ngOnInit(): void { 
        this.pageService.init('app-package').subscribe()
        this.previewApp = this.activatedRoute.snapshot.data.previewApp
        this.packageFormGroup = this.fb.group({
            description: ['', [Validators.required, Validators.maxLength(500)]]
        })
        let tree: Array<any> = []
        if(ObjectUtils.isNotNull(this.previewApp.standards)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.standards')
            treeNode['sub'] = []
            this.previewApp.standards?.forEach(a => {
                const subName = new Object()
                subName['name'] = a
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        if(ObjectUtils.isNotNull(this.previewApp.dynamicLists)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.dynamicLists')
            treeNode['sub'] = []
            this.previewApp.dynamicLists?.forEach(a => {
                const subName = new Object()
                subName['name'] = a
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        if(ObjectUtils.isNotNull(this.previewApp.charts)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.charts')
            treeNode['sub'] = []
            this.previewApp.charts?.forEach(a => {
                const subName = new Object()
                subName['name'] = a
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        if(ObjectUtils.isNotNull(this.previewApp.pages)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.pages')
            treeNode['sub'] = []
            this.previewApp.pages?.forEach(a => {
                const subName = new Object()
                subName['name'] = a
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        if(ObjectUtils.isNotNull(this.previewApp.locales)){
            const treeNode = new Object()
            treeNode['name'] = this.translate.instant('common.locale')
            treeNode['sub'] = []
            this.previewApp.locales?.forEach(a => {
                const subName = new Object()
                subName['name'] = a
                treeNode['sub'].push(subName)
            })
            tree.push(treeNode)
        }

        this.dataSource.data = tree
    }

    onCreate(){
        FormUtil.triggerFormValidators(this.packageFormGroup)
        if(this.packageFormGroup.valid){
            this.appsClient.package(this.previewApp.app.id, {
                appId: this.previewApp.app.id,
                creator: this.security.getAuthUser().username,
                description: this.packageFormGroup.controls.description.value
            }).pipe(
                tap(
                    res => {
                        this.downloadFileService.download(res.downloadableUrl)
                        this.shortcut.toastMessage(this.translate.instant('common.createSuccessfully'), ToastType.Success) 
                    },
                    err => {
                       this.shortcut.toastMessage(this.translate.instant('common.somethingWentWrong'), ToastType.Error) 
                    }
                )
            ).subscribe()
        }        
    }

    onCancel(){
        this.router.navigateByUrl('portal/page/apps-management')
    }
}

interface PackageNode{
    numberChildren: number
    expandable: boolean
    name: string
    level: number
    id: string
}
