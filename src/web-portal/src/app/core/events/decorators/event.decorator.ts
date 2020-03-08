import { ControlType } from 'services/portal.service'

export interface ControlEventOptions{
    name: string,
    allowedControlTypes: string | ControlType
}

export function ControlEvent(options: ControlEventOptions){
    return (constructor: Function) => {
        constructor.prototype.eventname = options.name
        constructor.prototype.allowedTypes = options.allowedControlTypes
    }
}