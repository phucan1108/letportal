import { ControlEventExecution } from './control/control.event';
import { Injectable, Optional, InjectionToken, Inject } from '@angular/core';
import { ControlType } from 'services/portal.service';

export const EVENTS_CONFIG = new InjectionToken<any[]>('EVENTS_CONFIG');
@Injectable()
export class EventsProvider {

    constructor(@Optional() @Inject(EVENTS_CONFIG) private config: any){}

    getEvent(eventName: string): ControlEventExecution {
        return this.config.events.find(a => a.prototype.eventname === eventName).prototype
    }

    getAvailableEventsForControlType(controlType: ControlType, controlName: string, sectionName: string = null) {
        const availableEvents = []
        this.config.events?.forEach(e => {
            if (typeof e.prototype.allowedTypes === 'string' && e.prototype.allowedTypes === '*') {
                availableEvents.push(sectionName ? `${sectionName}_${controlName}_${e.prototype.eventname}`: `${controlName}_${e.prototype.eventname}`)
            }
            else if(e.prototype.allowedTypes in ControlType && e.prototype.allowedTypes === controlType){
                availableEvents.push(sectionName ? `${sectionName}_${controlName}_${e.prototype.eventname}`: `${controlName}_${e.prototype.eventname}`)
            }
        })

        return availableEvents
    }
}