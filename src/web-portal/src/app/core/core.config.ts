import { toJsonStringTranslator } from './shell/translates/methods/toJsonStringTranslator';
import { currentDateTranslator } from './shell/translates/methods/currentDateTranslator';
import { ChangeControlEvent } from './events/control/change.event';
import { ReboundControlEvent } from './events/control/rebound.event';
import { CleanControlEvent } from './events/control/clean.event';
import { ResetControlEvent } from './events/control/reset.event';
import { ControlEventExecution } from './events/control/control.event';

/**
 * Declare all classes which contains a function to translate a expression data in {{ }}
 * Class must be declared with annotaion @MethodTranslator and impletement ShellMethod
 */
export const TRANSLATOR_METHODS = [
    toJsonStringTranslator,
    currentDateTranslator
]

/**
 * Declare all listen events for page render
 * Class must be declared with annotation @ControlEvent and impletement ControlEventExecution 
 */
export const CONTROL_EVENTS: any[] = [
    ChangeControlEvent,
    ResetControlEvent,
    CleanControlEvent,
    ReboundControlEvent
]


