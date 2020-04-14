export default class StringUtils {
    public static b64EncodeUnicode(str: string): string {
        if (window
            && 'btoa' in window
            && 'encodeURIComponent' in window) {
            return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, (match, p1) => {
                return String.fromCharCode(('0x' + p1) as any);
            }));
        } else {
            console.warn('b64EncodeUnicode requirements: window.btoa and window.encodeURIComponent functions');
            return null;
        }

    }

    public static b64DecodeUnicode(str: string): string {
        if (window
            && 'atob' in window
            && 'decodeURIComponent' in window) {
            return decodeURIComponent(Array.prototype.map.call(atob(str), (c) => {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));
        } else {
            console.warn('b64DecodeUnicode requirements: window.atob and window.decodeURIComponent functions');
            return null;
        }
    }

    public static getHashCode(str: string): string {
        let h;
        for (let i = 0; i < str.length; i++)
            h = Math.imul(31, h) + str.charCodeAt(i) | 0;
        return h;
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

    public static isAllUpperCase(text: string): boolean {
        return text === text.toUpperCase()
    }

    public static replaceAllOccurences(text: string, replaceStr: string, replacedStr: string) {
        return text.split(replaceStr).join(replacedStr)
    }

    public static getNumberOccurencesOfStr(text: string, occurStr: string) {
        return text.split(occurStr).length - 1
    }
}