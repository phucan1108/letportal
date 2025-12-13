import { InterceptorContext } from './interceptorcontext';

export interface Interceptor{
    pageName: string
    executeControlEvent(
        section: string, 
        control: string, 
        event: string, context: InterceptorContext): boolean
    executeCommand(command: string, context: InterceptorContext): boolean
}