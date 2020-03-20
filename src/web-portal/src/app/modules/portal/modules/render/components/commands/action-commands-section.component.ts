import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { NGXLogger } from 'ngx-logger';
import { ShellConfigProvider } from 'app/core/shell/shellconfig.provider';
import { ExtendedPageButton } from 'app/core/models/extended.models';
import * as _ from 'lodash';
import { PageService } from 'services/page.service';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { Subscription } from 'rxjs';

@Component({
    selector: 'action-commands',
    templateUrl: './action-commands-section.component.html'
})
export class ActionCommandsSectionComponent implements OnInit, OnDestroy {

    @Input()
    actionCommands: Array<ExtendedPageButton>

    queryparams: any
    options: any
    data: any
    readyToRender = false
    @Output()
    onClick = new EventEmitter<ExtendedPageButton>()

    listenParamChanges$: Subscription
    listenDataChanges$: Subscription
    constructor(
        private pageService: PageService,
        private shellConfigProvider: ShellConfigProvider,
        private shortcutUtil: ShortcutUtil,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {

        this.listenParamChanges$ = this.pageService.listenOptionsAndParamsChange$().subscribe(
            res => {
                this.options = res.options
                this.queryparams = res.queryparams                
            }
        )

        const sub$ = this.pageService.listenDataChange$().subscribe(
            data => {
                this.data = data
                _.forEach(this.actionCommands, (command: ExtendedPageButton) => {
                    command.isHidden = this.pageService.evaluatedExpression(command.allowHidden)
                })
                this.readyToRender = true
                sub$.unsubscribe()
            }
        )
    }
    
    ngOnDestroy(): void {
        this.listenParamChanges$.unsubscribe()
    }

    onCommandClick(command: ExtendedPageButton) {
        this.pageService.executeCommand(command)
    }
}
