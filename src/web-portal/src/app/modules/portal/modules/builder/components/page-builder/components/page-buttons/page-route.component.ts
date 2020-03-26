import { Component, OnInit, Input } from '@angular/core';
import { Route, PagesClient, ShortPageModel } from 'services/portal.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StaticResources } from 'portal/resources/static-resources';
import { Observable, BehaviorSubject } from 'rxjs';
import { FormUtil } from 'app/core/utils/form-util';
import { tap } from 'rxjs/operators';

@Component({
    selector: 'let-pageroute',
    templateUrl: './page-route.component.html'
})
export class PageRouteComponent implements OnInit {

    @Input()
    route: Route

    routeForm: FormGroup

    page$: BehaviorSubject<Array<ShortPageModel>> = new BehaviorSubject([])
    pages: Array<ShortPageModel> = []

    constructor(
        private fb: FormBuilder,
        private pagesClient: PagesClient
    ) { }

    ngOnInit(): void {
        this.pagesClient.getAllShortPages()
            .pipe(
                tap(
                    res => {
                        this.page$.next(res)
                    }
                )
            ).subscribe()

        this.page$.subscribe(res => {
            this.pages = res
        })

        this.routeForm = this.fb.group({
            redirectUrl: [this.route.redirectUrl, Validators.required],
            isSameDomain: [this.route.isSameDomain],
            condition: [this.route.condition, Validators.required],
            targetPageId: ['']
        })

        this.routeForm.get('targetPageId').valueChanges.subscribe(newValue => {
            if(newValue){
                const found = this.pages.find(a => a.id == newValue)
                this.routeForm.get('redirectUrl').setValue(found.urlPath)
            }
        })
    }

    get(){
        if(this.routeForm.valid){
            const formValues = this.routeForm.value
            const newRoute: Route = {
                redirectUrl: formValues.redirectUrl,
                isSameDomain: formValues.isSameDomain,
                condition: formValues.condition
            }
            return newRoute
        }
    }

    valid(){
        FormUtil.triggerFormValidators(this.routeForm)
        return this.routeForm.valid
    }
}
