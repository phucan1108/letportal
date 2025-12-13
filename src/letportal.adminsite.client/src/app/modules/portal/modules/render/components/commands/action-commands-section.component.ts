import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ExtendedPageButton } from 'app/core/models/extended.models';
import { ShellConfigProvider } from 'app/core/shell/shellconfig.provider';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { NGXLogger } from 'ngx-logger';
import { Subscription } from 'rxjs';
import { PageService } from 'services/page.service';
 

@Component({
    selector: 'action-commands',
    templateUrl: './action-commands-section.component.html'
})
export class ActionCommandsSectionComponent implements OnInit, OnDestroy {

    @Input()
    actionCommands: Array<ExtendedPageButton>

    @Input()
    isInSection: boolean = false

    queryparams: any
    options: any
    data: any
    readyToRender = false

    listenParamChanges$: Subscription = new Subscription()
    listenDataChanges$: Subscription = new Subscription()
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

        this.listenDataChanges$.add(this.pageService.listenDataChange$().subscribe(
            data => {
                this.data = data
                this.actionCommands?.forEach((command: ExtendedPageButton) => {
                    command.isHidden = this.pageService.evaluatedExpression(command.allowHidden)
                })
                this.readyToRender = true
            }
        ))
    }
    
    ngOnDestroy(): void {
        this.listenParamChanges$.unsubscribe()
        this.listenDataChanges$.unsubscribe()
    }

    onCommandClick(command: ExtendedPageButton) {
        this.pageService.executeCommand(command)
    }
}
