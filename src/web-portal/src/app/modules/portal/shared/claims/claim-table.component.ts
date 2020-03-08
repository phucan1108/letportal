import { Component, OnInit, Input, ChangeDetectorRef, ViewChild } from '@angular/core';
import { PortalClaim, ClaimValueType } from 'services/portal.service';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';
import { MatDialog, MatTable } from '@angular/material';
import { ClaimDialogComponent } from './claim-dialog.component';
import { ArrayUtils } from 'app/core/utils/array-util';
import * as _ from 'lodash';
import { MessageType, ToastType } from 'app/modules/shared/components/shortcuts/shortcut.models';
import { PortalStandardClaims } from 'app/core/security/portalClaims';
import { StaticResources } from 'portal/resources/static-resources';

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
        private shortcutUtil: ShortcutUtil,
        private cd: ChangeDetectorRef,
        public dialog: MatDialog,
    ) { }

    ngOnInit(): void { 

    }

    addClaim(){
        let claim: PortalClaim = {
            name: '',
            displayName: '',
            claimValueType: ClaimValueType.Boolean
        }

        const dialogRef = this.dialog.open(ClaimDialogComponent, {
            data: {
                claim: claim
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
                claim: claim
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
        this.claims = _.filter(this.claims, (elem) => {
            return elem.name !== claim.name
        })
        this.shortcutUtil.toastMessage("Delete claim successfully!", ToastType.Success);

        this.refreshTable()
    }

    refreshTable(){
        this.table.renderRows()
    }

    translateClaimValueType(claimValueType: ClaimValueType){
        return _.find(StaticResources.claimValueTypes(), claim => claim.value === claimValueType).name
    }

    isDefaultClaim(claim: PortalClaim){
        return claim.name === PortalStandardClaims.AllowAccess.name
    }
}
