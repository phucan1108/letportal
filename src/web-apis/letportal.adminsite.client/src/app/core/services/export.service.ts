import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

@Injectable()
export class ExportService {
    constructor() {

    }

    public exportExcelFile(
        excelFileName: string,
        sheetName: string,
        orderedCols: string[],
        colNames: string[],
        data: any[]) {
        const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(data, {
            header: orderedCols
        });

        // By default, XLSX doesn't support to change header name
        // We need to read loop one by one A1 - An to replace name
        const range = XLSX.utils.decode_range(worksheet['!ref']);
        let indexName = 0;
        for(let C = range.s.r; C <= range.e.c; ++C) {
            const address = XLSX.utils.encode_col(C) + '1';
          if(!worksheet[address]) continue;
          worksheet[address].v = colNames[indexName]
          indexName++
        }

        const workbook: XLSX.WorkBook = XLSX.utils.book_new()
        XLSX.utils.book_append_sheet(workbook, worksheet, sheetName)

        const excelBuffer: any = XLSX.write(workbook, {
            bookType: 'xlsx',
            type: 'array'
        })

        const fileData: Blob = new Blob([excelBuffer], {
            type: EXCEL_TYPE
        });

        FileSaver.saveAs(fileData, excelFileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
    }

    public exportJsonFile(jsonString: string, fileName: string){
        var blob = new Blob([jsonString], {type: "application/json"});
        FileSaver.saveAs(blob, fileName)
    }
}