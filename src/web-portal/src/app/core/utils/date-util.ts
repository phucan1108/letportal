export class DateUtils {
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