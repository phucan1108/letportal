import { Component, OnInit, Input } from '@angular/core';
import { Route, PagesClient, ShortPageModel, RouteType } from 'services/portal.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { StaticResources } from 'portal/resources/static-resources';
import { Observable } from 'rxjs';
import { FormUtil } from 'app/core/utils/form-util';

@Component({
    selector: 'let-pageroute',
    templateUrl: './page-route.component.html'
})
export class PageRouteComponent implements OnInit {

    @Input()
    route: Route

    _routeTypes = StaticResources.routeTypes()
    routeForm: FormGroup
    routeType = RouteType
    page$: Observable<Array<ShortPageModel>>;
    constructor(
        private fb: FormBuilder,
        private pagesClient: PagesClient
    ) { }

    ngOnInit(): void {
        this.page$ = this.pagesClient.getAllShortPages()

        this.routeForm = this.fb.group({
            routeType: [this.route.routeType],
            targetPageId: [this.route.targetPageId],
            targetUrl: [this.route.targetUrl],
            passDataPath: [this.route.passDataPath],
            condition: [this.route.condition]
        })
    }

    get(){
        if(this.routeForm.valid){
            let formValues = this.routeForm.value
            let newRoute: Route = {
                routeType: formValues.routeType,
                targetPageId: formValues.targetPageId,
                targetUrl: formValues.targetUrl,
                passDataPath: formValues.passDataPath,
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
