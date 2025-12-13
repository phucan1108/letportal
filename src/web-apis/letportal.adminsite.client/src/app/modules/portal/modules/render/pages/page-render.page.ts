import { Component, OnInit, ChangeDetectorRef, AfterViewInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DynamicFormEventData } from '../../../../../core/models/extended.models';
 
import { Constants } from 'portal/resources/constants';
import { ShellConfigType } from 'app/core/shell/shell.model';
import { ShellConfigProvider } from 'app/core/shell/shellconfig.provider';
import { NGXLogger } from 'ngx-logger';
import { BehaviorSubject } from 'rxjs';
import { RouterExtService } from 'app/core/ext-service/routerext.service';
import { Page } from 'services/portal.service';
import { PageService } from 'services/page.service';
import { tap } from 'rxjs/operators';
import { PageReadyAction } from 'stores/pages/page.actions';

@Component({
    selector: 'let-page-render',
    templateUrl: './page-render.page.html',
    styleUrls: ['./page-render.page.scss']
})
export class PageRenderPage implements OnInit, AfterViewInit, OnDestroy {

    event$: BehaviorSubject<DynamicFormEventData> = new BehaviorSubject(null);

    page: Page;

    constructor(
        private pageService: PageService,
        private activatedRoute: ActivatedRoute,
        private routerService: RouterExtService,
        private shellConfigProvider: ShellConfigProvider,
        private logger: NGXLogger) {
    }

    ngOnInit(): void {
        this.pageService.initRender(this.activatedRoute.snapshot.data.page, this.activatedRoute)
            .subscribe()
        this.page = this.activatedRoute.snapshot.data.page
    }

    ngAfterViewInit(): void {
        // Move some events that relates to notify
        this.shellConfigProvider.appendShellConfigs([{ key: Constants.NAVIGATION_PREV_URL, value: this.routerService.getPreviousUrl(), type: ShellConfigType.Constant }])
    }

    ngOnDestroy(): void {
        this.pageService.destroy()
    }
}
