import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { PORTAL_BASE_URL, ResponseUploadFile } from './portal.service';
import { Observable, Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UploadFileService {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(PORTAL_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : 'http://localhost:53508';
    }

    uploadOne(file: File): Observable<ResponseUploadFile> {
        let url_ = this.baseUrl + '/api/files/upload';
        url_ = url_.replace(/[?&]$/, '');
        const formData: FormData = new FormData();
        formData.append('formFile', file, file.name);
        return this.http.post<ResponseUploadFile>(url_, formData)
    }

    upload(files: Set<File>): { [key: string]: { progress: Observable<number>, completed: Observable<DownloadableResponseFile> } } {
        let url_ = this.baseUrl + '/api/files/upload';
        url_ = url_.replace(/[?&]$/, '');
        const status: {
            [key: string]: {
                progress: Observable<number>,
                completed: Observable<DownloadableResponseFile>,
                error: Observable<any>
            }
        } = {};

        files?.forEach(file => {
            const formData: FormData = new FormData();
            formData.append('formFile', file, file.name);
            const req = new HttpRequest('POST', url_, formData, {
                reportProgress: true
            });
            const progress = new Subject<number>();
            const completed = new Subject<DownloadableResponseFile>();
            const error = new Subject<any>()
            this.http.request(req).subscribe(event => {
                if (event.type === HttpEventType.UploadProgress) {
                    const percentDone = Math.round(100 * event.loaded / event.total);
                    progress.next(percentDone);
                } else if (event instanceof HttpResponse) {
                    progress.next(100);
                    progress.complete();

                    completed.next({
                        fileName: file.name,
                        response: event.body as ResponseUploadFile
                    })
                    completed.complete()
                }
            },
                err => {
                    error.next(err)
                });
            status[file.name] = {
                progress: progress.asObservable(),
                completed: completed.asObservable(),
                error: error.asObservable()
            };
        });
        return status;
    }
}

export interface DownloadableResponseFile {
    fileName: string,
    response: ResponseUploadFile
}