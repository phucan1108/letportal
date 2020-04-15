import { Pipe, PipeTransform } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SecurityService } from '../security/security.service';

@Pipe({
    name: 'secureFile'
})
export class SecurePipe implements PipeTransform {
  constructor(
      private http: HttpClient,
      private security: SecurityService) {}

    async transform(url: string): Promise<string> {
        try{
            const fileBlob = await this.http.get(url, {
                responseType: 'blob'
            }).toPromise()

            const reader = new FileReader();
            return new Promise((resolve, reject) => {
                reader.onloadend = () => resolve(reader.result as string);
                reader.readAsDataURL(fileBlob);
              });
        }
        catch{
            return ''
        }
    }
}