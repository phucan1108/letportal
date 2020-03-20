import { ListIterateeCustom } from 'lodash';
import * as _ from 'lodash';
import { ObjectUtils } from './object-util';


export class ArrayUtils {
    public static updateOneItem<T>(arrays: Array<T>, updatingItem: T, predicate?: ListIterateeCustom<T, boolean>): Array<T> {
        const removingItemIndex = _.findIndex(arrays, predicate)
        arrays.splice(removingItemIndex, 1, updatingItem)
        return arrays
    }

    public static updateOneItemByIndex<T>(arrays: Array<T>, updatingItem: T, index: number){
        arrays.splice(index, 1, updatingItem)
        return arrays
    }

    public static removeOneItem<T>(arrays: Array<T>, predicate?: ListIterateeCustom<T, boolean>): Array<T> {
        const removingItemIndex = _.findIndex(arrays, predicate)
        arrays.splice(removingItemIndex, 1)
        return arrays
    }

    public static swapTwoItems<T>(arrays: Array<T>, first: number, second: number, identifierField: string = 'id', sortField: string = 'order'): Array<T> {
        let tempIdField = '';
        _.forEach(arrays, elem => {
            if (elem[sortField] === first) {
                elem[sortField] = second
                tempIdField = elem[identifierField]
            }

            if (elem[sortField] === second && elem[identifierField] !== tempIdField) {
                elem[sortField] = first
            }
        })

        return _.sortBy<T>(arrays, [function (elem) { return elem[sortField] }])
    }

    public static appendItemsDistinct<T>(arrays: Array<T>, appendItems: Array<T>, identifyField: string = 'id'): Array<T> {
        const temps: Array<any> = []
        _.forEach(appendItems, item => {
            let notDuplicate = true
            _.forEach(arrays, element => {
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

        _.forEach(temps, duplicate => {
            arrays = this.updateOneItem(arrays, duplicate.update, elem => elem[identifyField] === duplicate.key)
        })

        return arrays
    }

    public static sliceOneProp(arrays: Array<any>, propName: string): Array<any> {
        const reducedArrays: Array<any> = []

        _.forEach(arrays, elem => {
            reducedArrays.push(elem[propName])
        })

        return reducedArrays
    }
}