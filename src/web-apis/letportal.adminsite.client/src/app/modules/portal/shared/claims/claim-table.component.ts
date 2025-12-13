import { Component, OnInit, Input, ChangeDetectorRef, ViewChild } from '@angular/core';
import { PortalClaim, ClaimValueType } from 'services/portal.service';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table'
import { ClaimDialogComponent } from './claim-dialog.component';
import { ArrayUtils } from 'app/core/utils/array-util';
 
import { MessageType, ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { PortalStandardClaims } from 'app/core/security/portalClaims';
import { StaticResources } from 'portal/resources/static-resources';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'let-claim-table',
    templateUrl: './claim-table.component.html'
})
export class ClaimTableComponent implements OnInit {
    @ViewChild('table', { static: true }) table: MatTable<PortalClaim>;
    @Input()
    claims: Array<PortalClaim> = []

    displayedClaimListColumns = [ 'name', 'displayName', 'claimValueType', 'actions']

    constructor(
        private translate: TranslateService,
        private shortcutUtil: ShortcutUtil,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
    ) { }

    ngOnInit(): void {

    }

    addClaim(){
        const claim: PortalClaim = {
            name: '',
            displayName: '',
            claimValueType: ClaimValueType.Boolean
        }

        const dialogRef = this.dialog.open(ClaimDialogComponent, {
            data: {
                claim
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.claims.push(result)
                this.refreshTable()
            }
        })
    }

    editClaim(claim: PortalClaim){
        const dialogRef = this.dialog.open(ClaimDialogComponent, {
            data: {
                claim
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.claims = ArrayUtils.updateOneItem(this.claims, result, (claim: PortalClaim) => { return claim.name === claim.name })
            this.refreshTable()
        })
    }

    deleteClaim(claim: PortalClaim){
        this.claims = this.claims.filter((elem) => {
            return elem.name !== claim.name
        })
        this.shortcutUtil.toastMessage(this.translate.instant('common.deleteSuccessfully'), ToastType.Success);

        this.refreshTable()
    }

    refreshTable(){
        this.table.renderRows()
    }

    translateClaimValueType(claimValueType: ClaimValueType){
        return StaticResources.claimValueTypes().find(claim => claim.value === claimValueType).name
    }

    isDefaultClaim(claim: PortalClaim){
        return claim.name === PortalStandardClaims.AllowAccess.name
    }
}
