export interface ControlEventOptions{
    name: string
}

export function ControlEvent(options: ControlEventOptions){
    return (constructor: Function) => {
        constructor.prototype.eventname = options.name
    }
}