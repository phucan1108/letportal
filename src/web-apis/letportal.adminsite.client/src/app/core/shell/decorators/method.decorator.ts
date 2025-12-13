export interface MethodOptions{
    name: string,
    replaceDQuote: boolean
}

export function MethodTranslator(options: MethodOptions){
    return (constructor: Function) => {
        constructor.prototype.methodTranslatorName = options.name
        constructor.prototype.replaceDQuote = options.replaceDQuote
    }
}