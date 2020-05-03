import { MatPaginatorIntl } from '@angular/material/paginator';
import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
    providedIn: 'root'
})
export class MatPaginatorIntlCustom extends MatPaginatorIntl {
    itemsPerPageLabel = 'Items per page';
    nextPageLabel = 'Next';
    previousPageLabel = 'Previous';
    translateService: TranslateService

    addTranslateService(translate: TranslateService) {
        this.translateService = translate

        this.itemsPerPageLabel = this.translateService.instant('common.itemsPerPage')
        this.nextPageLabel = this.translateService.instant('common.nextText')
        this.previousPageLabel = this.translateService.instant('common.previous')
    }

}