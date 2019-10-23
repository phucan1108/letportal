export class ObjectUtils {

    /**
     * This function helps to convert object to key value list
     * Ex: { foo: { bar: '1' }} => 'foo.bar': 1
     */
    public static flattenObjects = (object: any, allowLowerCase = false, prefix = '', separator = '.') => {
        return Object.keys(object).reduce((prev, element) => {
            return object[element] && typeof object[element] == 'object' && !Array.isArray(element)
                ? { ...prev, ...ObjectUtils.flattenObjects(object[element], allowLowerCase, `${prefix}${element}${separator}`, separator) }
                : { ...prev, ...{ [allowLowerCase ? `${prefix}${element}` : `${prefix}${element}`]: object[element] } }
        }, {})
    }

    /**
     * This function helps to do unflatten objects
     * @param object unflatten object
     * @param separator separator char, default '.'
     */
    public static unflattenObjects(object: any, separator = '.') {
        var result = {}
        for (var i in object) {
            var keys = i.split(separator)
            keys.reduce(function (r, e, j) {
                return r[e] || (r[e] = isNaN(Number(keys[j + 1])) ? (keys.length - 1 == j ? object[i] : {}) : [])
            }, result)
        }
        return result
    }

    public static getContentByDCurlyBrackets(text: string): string[] {
        let found = [],
            rxp = /{{([^}]+)}/g,
            mat: any[] | RegExpExecArray
        while (mat = rxp.exec(text)) {
            found.push(mat[1]);
        }

        return found
    }

    public static isObject(value) {
        return value && typeof value === 'object' && value.constructor === Object;
    }

    public static isNumber(value){
        return !isNaN(value)
    }

    public static isBoolean(value){
        return typeof value === 'boolean'
    }

    public static isArray(value){
        return Array.isArray(value);
    }

    public static isNotNull(value){
        return typeof value !== 'undefined' && value != null
    }

    public static clone(source: any): any {
        // Prefer to Mozila docs: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/assign
        let jsonStr = JSON.stringify(source)
        return JSON.parse(jsonStr)
    }

    
}
