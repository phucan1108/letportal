import * as moment from 'moment'

export class DateUtils {
    public static getUTCNow() {
        var date = new Date();
        var now_utc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
            date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
        return new Date(now_utc);
    }

    public static getUTCNowByDate(date: Date) {
        var now_utc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
            date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
        return new Date(now_utc);
    }

    public static toDateFormat(date: Date, format: string){
        return moment(date).format(format)
    }

    public static toDateMMDDYYYYString(date: Date) {
        let dd = date.getDate();
        let dayStr = ''
        let mm = date.getMonth() + 1;
        let monthStr = ''
        let yyyy = date.getFullYear();
        if (dd < 10) {
            dayStr = '0' + dd;
        }

        if (mm < 10) {
            monthStr = '0' + mm;
        }

        let hrs = date.getHours()
        let minutes = date.getMinutes()
        let seconds = date.getSeconds()
        return `${monthStr}/${dayStr}/${yyyy} ${hrs}:${minutes}:${seconds}`
    }
}