import * as moment from 'moment'

export class DateUtils {
    public static getUTCNow() {
        const date = new Date();
        const now_utc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
            date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
        return new Date(now_utc);
    }

    public static getUTCNowByDate(date: Date) {
        const now_utc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
            date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
        return new Date(now_utc);
    }

    public static toDateFormat(date: Date, format: string){
        return moment(date).format(format)
    }

    public static toDateMMDDYYYYString(date: Date) {
        const dd = date.getDate();
        let dayStr = ''
        const mm = date.getMonth() + 1;
        let monthStr = ''
        const yyyy = date.getFullYear();
        if (dd < 10) {
            dayStr = '0' + dd;
        }

        if (mm < 10) {
            monthStr = '0' + mm;
        }

        const hrs = date.getHours()
        const minutes = date.getMinutes()
        const seconds = date.getSeconds()
        return `${monthStr}/${dayStr}/${yyyy} ${hrs}:${minutes}:${seconds}`
    }
}