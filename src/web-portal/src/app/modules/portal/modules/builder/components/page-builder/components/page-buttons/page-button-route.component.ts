import { ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ArrayUtils } from 'app/core/utils/array-util';
import { BehaviorSubject } from 'rxjs';
import { PageButton, Route } from 'services/portal.service';
import { PageButtonGridComponent } from './page-button-grid.component';
import { PageRouteComponent } from './page-route.component';

@Component({
    selector: 'let-page-button-route',
    templateUrl: './page-button-route.component.html'
})
export class PageButtonRouteDialogComponent implements OnInit {

    currentCommand: PageButton
    isEnable = false
    routes$: BehaviorSubject<Route[]> = new BehaviorSubject([])
    routes: Route[]
    displayedListColumns = ['condition', 'redirectUrl', 'actions'];

    isShowRouteForm = false
    isEditRouteForm = false
    isCreateRouteForm = false
    currentRoute: Route
    routeFormGroup: FormGroup
    @ViewChild(PageRouteComponent, { static: false }) pageRouteForm: PageRouteComponent
    constructor(
        public dialogRef: MatDialogRef<PageButtonGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.currentCommand = this.data.command
        this.routes = this.currentCommand.buttonOptions.routeOptions?.routes
        this.isEnable = this.currentCommand.buttonOptions.routeOptions?.isEnable
        if (!this.routes) {
            this.routes = []
        }

        this.routes$.next(this.routes)
    }

    addRoute() {
        const newRoute: Route = {
            condition: 'true',
            redirectUrl: '',
            isSameDomain: true
        }
        this.currentRoute = newRoute
        this.isShowRouteForm = true
        this.cd.detectChanges()
        this.isCreateRouteForm = true
        this.isEditRouteForm = false
    }

    editRoute(route: Route){
        this.currentRoute = route
        this.isShowRouteForm = true
        this.isCreateRouteForm = false
        this.isEditRouteForm = true
        this.cd.detectChanges()
    }

    deleteRoute(route: Route){
        const index = this.routes.indexOf(route)
        if(index  > -1){
            this.routes.splice(index, 1)
            this.routes$.next(this.routes)
        }
    }

    save() {
        if (this.isShowRouteForm) {
            if (this.pageRouteForm.valid()) {
                if (this.isEditRouteForm) {
                    const index = this.routes.indexOf(this.currentRoute)
                    this.routes = ArrayUtils.updateOneItemByIndex(this.routes, this.pageRouteForm.get(), index)
                    this.routes$.next(this.routes)
                    this.isShowRouteForm = false
                    this.cd.detectChanges()
                }
                else if(this.isCreateRouteForm){
                    this.routes.push(this.pageRouteForm.get())
                    this.routes$.next(this.routes)
                    this.isShowRouteForm = false
                    this.cd.detectChanges()
                }
            }
        }
        else {
            this.dialogRef.close({
                routes: this.routes,
                isEnable: true
            })
        }
    }

    cancel(){
        this.isShowRouteForm = false
        this.cd.detectChanges()
    }
}
