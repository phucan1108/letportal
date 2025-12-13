import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CommandButtonInList, CommandPositionType } from 'services/portal.service';
import { CommandClicked } from '../models/commandClicked';

@Component({
    selector: 'dynamic-list-command',
    templateUrl: './dynamic-list.command.component.html'
})
export class DynamicListCommandComponent implements OnInit {

    @Input()
    data: any;

    @Input()
    command: CommandButtonInList

    @Input()
    isHansetDisplay = false

    @Output()
    onClick = new EventEmitter<CommandClicked>();

    isCommandInList = false;

    constructor() { }

    ngOnInit(): void {
        this.isCommandInList = this.command.commandPositionType === CommandPositionType.InList;
    }

    onCommandClick() {
        this.onClick.emit({
            command: this.command,
            data: this.data
        })
    }
}
