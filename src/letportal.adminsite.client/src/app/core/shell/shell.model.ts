export interface ShellConfig {
    key: string
    value: string
    type: ShellConfigType,
    replaceDQuote?: boolean
}

export enum ShellConfigType {
    Constant,
    Method
}