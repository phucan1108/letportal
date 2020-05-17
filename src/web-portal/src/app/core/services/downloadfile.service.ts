import { Injectable, Inject, Optional } from "@angular/core";
import { HttpClient, HttpResponse } from '@angular/common/http';
import * as FileSaver from 'file-saver';
import { PORTAL_BASE_URL } from './portal.service';
import { map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class DownloadFileService {
    private http: HttpClient;
    private baseUrl: string;

    constructor(@Inject(HttpClient) http: HttpClient) {
        this.http = http;
    }

    download(url: string) {
        let fileName = ''
        let fileType = ''
        this.http.get(url,
            {observe: 'response', responseType: 'blob'})
            .pipe(
                map((res: HttpResponse<Blob>) => {
                    fileName = this.getFileName(res.headers.get('content-disposition'))
                    fileType = res.headers.get('content-type')
                    return res
                })
            )
            .subscribe(
                res => { 
                    this.downLoadFile(res.body, fileName, fileType)
                }
            )
    }

    private downLoadFile(data: any, fileName: string, fileType: string) {
        const blob = new Blob([data], { type: fileType });
        FileSaver.saveAs(blob, fileName);
    }

    private getFileName(content: string) {
        const contentDisposition = content || '';
        const matches = /filename=([^;]+)/ig.exec(contentDisposition);
        const fileName = (matches[1] || 'untitled').trim();
        return fileName;
    }
}