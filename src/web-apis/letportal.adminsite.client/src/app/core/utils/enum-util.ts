export class EnumUtils{
    public static getEnumKeysAsString(enumType: object) {
        const members = Object.keys(enumType);
        return members.filter((x) => Number.isNaN(parseInt(x, 10)))
    }

    public static toKeyValueArray(enumType: object) {
        return this.getEnumKeysAsString(enumType)
            .map((key) => {
                return { key, value: enumType[key] };
             });
     }

    public static getEnumKeyByValue(enumType: object, value: any){
        const keys = this.toKeyValueArray(enumType)
        return keys.find(k => k.value === value).key
    }
}