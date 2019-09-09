import { ControlEventExecution } from './control/control.event';
import { Injectable } from '@angular/core';

@Injectable()
export class EventsProvider{
    events: any[]

    getEvent(eventName: string): ControlEventExecution{
        return this.events.find(a => a.name === eventName)
    }
}