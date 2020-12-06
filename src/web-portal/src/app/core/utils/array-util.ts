import { ObjectUtils } from './object-util';


export class ArrayUtils {
    public static updateOneItem<T>(arrays: Array<T>, updatingItem: T, predicate?: (value: T, index: number, obj: T[]) => unknown): Array<T> {
        const removingItemIndex = arrays.findIndex(predicate)
        arrays.splice(removingItemIndex, 1, updatingItem)
        return arrays
    }

    public static updateOneItemByIndex<T>(arrays: Array<T>, updatingItem: T, index: number){
        arrays.splice(index, 1, updatingItem)
        return arrays
    }

    public static insertAtIndex<T>(arrays: Array<T>, insertingItem: T, index: number){
        if(arrays.length < index){
            arrays.push(insertingItem)
        }
        else if(arrays.length >= index){
            arrays.splice(index, 0, insertingItem)
        }        
    }

    public static removeOneItem<T>(arrays: Array<T>, predicate?:  (value: T, index: number, obj: T[]) => unknown): Array<T> {
        const removingItemIndex = arrays.findIndex(predicate)
        arrays.splice(removingItemIndex, 1)
        return arrays
    }

    public static swapTwoItems<T>(arrays: Array<T>, first: number, second: number, identifierField: string = 'id', sortField: string = 'order'): Array<T> {
        let tempIdField = '';
        arrays?.forEach(elem => {
            if (elem[sortField] === first) {
                elem[sortField] = second
                tempIdField = elem[identifierField]
            }

            if (elem[sortField] === second && elem[identifierField] !== tempIdField) {
                elem[sortField] = first
            }
        })

        return arrays.sort(elem => { return elem[sortField] })
    }

    public static appendItemsDistinct<T>(arrays: Array<T>, appendItems: Array<T>, identifyField: string = 'id'): Array<T> {
        const temps: Array<any> = []
        appendItems?.forEach(item => {
            let notDuplicate = true
            arrays?.forEach(element => {
                if(ObjectUtils.isObject(element)){
                    if (element[identifyField] === item[identifyField]) {
                        temps.push({
                            key: element[identifyField],
                            update: item
                        })
                        notDuplicate = false
                        return false
                    }
                }
                else{
                    if(element === item){
                        temps.push({
                            key: element,
                            update: item
                        })
                        notDuplicate = false
                        return false
                    }
                }
            })
            if(notDuplicate){
                arrays.push(item)
            }
        })

        temps?.forEach(duplicate => {
            arrays = this.updateOneItem(arrays, duplicate.update, elem => elem[identifyField] === duplicate.key)
        })

        return arrays
    }

    public static sliceOneProp(arrays: Array<any>, propName: string): Array<any> {
        const reducedArrays: Array<any> = []

        arrays?.forEach(elem => {
            reducedArrays.push(elem[propName])
        })

        return reducedArrays
    }
}