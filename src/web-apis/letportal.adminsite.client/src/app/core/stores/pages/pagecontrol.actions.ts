import { TriggeredControlEvent } from 'app/core/models/page.model';

const PAGE_CONTROL_EVENTS = '[Page Control Events]'

export class PageControlEvent {
    public static readonly type = ''
}

export class TriggerControlChangeValueEvent implements PageControlEvent {
    public static readonly type = `${PAGE_CONTROL_EVENTS} Trigger Control Change Value`
    constructor(public event: TriggeredControlEvent) { }
}

export type All =
    PageControlEvent |
    TriggerControlChangeValueEvent