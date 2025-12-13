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
        const result = {}
        for (const i in object) {
            const keys = i.split(separator)
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

    public static isNumber(value: number){
        return !isNaN(value)
    }

    public static isBoolean(value: any){
        return typeof value === 'boolean'
    }

    public static isArray(value: any){
        return Array.isArray(value);
    }

    public static isNotNull(value: any){
        return typeof value !== 'undefined' && value !== null && value !== ''
    }

    public static clone(source: any): any {
        // Prefer to Mozila docs: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/assign
        const jsonStr = JSON.stringify(source)
        return JSON.parse(jsonStr)
    }

    public static projection(outputProjection: string, data: any){
        const splitted = outputProjection.split(';')
        const fieldMaps: any = []
        splitted?.forEach(field => {
            if (field.indexOf('=') > 0) {
                const fieldSplitted = field.split('=')
                fieldMaps.push({
                    key: fieldSplitted[0],
                    map: fieldSplitted[1]
                })
            }
            else {
                fieldMaps.push({
                    key: field,
                    map: field
                })
            }
        })
        if (data instanceof Array) {
            const resData = new Array()
            data?.forEach(dt => {
                const obj = new Object()
                fieldMaps?.forEach(map => {
                    const evaluted = Function('data', 'return data.' + map.map)
                    obj[map.key] = evaluted(dt)
                })

                resData.push(obj)
            })

            return resData
        }
        else {
            const obj = new Object()
            fieldMaps?.forEach(map => {
                const evaluted = Function('data', 'return data.' + map.map)
                obj[map.key] = evaluted(data)
            })
            return obj
        }
    }
}
