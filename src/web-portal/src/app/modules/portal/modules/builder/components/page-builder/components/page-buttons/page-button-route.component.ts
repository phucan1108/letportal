import { Component, OnInit, Inject, ChangeDetectorRef, ViewChild } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material';
import { PageButtonGridComponent } from './page-button-grid.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PageButton, RouteType, Route } from 'services/portal.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { Guid } from 'guid-typescript';
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
    displayedListColumns = ['condition', 'routeType', 'actions'];

    isShowRouteForm = false
    isEditRouteForm = false
    isCreateRouteForm = false
    currentRoute: Route
    routeFormGroup: FormGroup
    @ViewChild(PageRouteComponent) pageRouteForm: PageRouteComponent
    constructor(
        public dialogRef: MatDialogRef<PageButtonGridComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.currentCommand = this.data.command
        this.routes = this.currentCommand.buttonOptions.routeOptions.routes
        this.isEnable = this.currentCommand.buttonOptions.routeOptions.isEnable
        if (!this.routes) {
            this.routes = []
        }

        this.routes$.next(this.routes)
    }

    translateRouteType(routeType: RouteType) {
        switch (routeType) {
            case RouteType.ThroughPage:
                return 'Through Page'
            case RouteType.ThroughUrl:
                return 'Through Url'
        }
    }

    addRoute() {
        let newRoute: Route = {
            condition: 'true',
            passDataPath: '',
            routeType: RouteType.ThroughPage,
            targetPageId: '',
            targetUrl: ''
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
                    this.routes[index] = {
                        ...this.pageRouteForm.get()
                    }
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
