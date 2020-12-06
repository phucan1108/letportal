import {
  Component,
  EmbeddedViewRef,
  forwardRef,
  Inject,
  Input,
  OnDestroy,
  OnInit,
  ViewContainerRef,
} from '@angular/core';
import { DataTable } from './datatable';
import { ColumnComponent } from './columns';

@Component({
  selector: 'm-columnHeaderTemplateLoader',
  template: ``,
})
export class ColumnHeaderTemplateLoader implements OnInit, OnDestroy {
  @Input() column: any;

  view: EmbeddedViewRef<any>;

  constructor(public viewContainer: ViewContainerRef) {}

  ngOnInit() {
    this.view = this.viewContainer.createEmbeddedView(
      this.column.headerTemplate,
      {
        $implicit: this.column,
      },
    );
  }

  ngOnDestroy() {
    this.view.destroy();
  }
}

@Component({
  selector: '[mColumnHeader]',
  template: `
    <tr>
      <th *ngIf="dt.selectionHandler" class="m-checkbox-header">
        <div class="m-header-th m-header-th--checkbox">
          <mat-checkbox [disabled]="dt.disableSelectAll" *ngIf="dt.selectionMode === 'multiple'" [checked]="dt.allSelected" (change)="dt.toggleRowsWithCheckbox($event)"></mat-checkbox>
        </div>
      </th>
      <th [hidden]="col.hidden" *ngFor="let col of columns;" (click)="dt.sort($event,col)"
          [ngClass]="[col.colHeadClass ? col.colHeadClass : '', col.sortable ? 'm-sortable-column': '']">
        <div class="m-header-th">
          <span *ngIf="!col.headerTemplate">{{ col.header }}</span>
          <span *ngIf="col.headerTemplate">
            <m-columnHeaderTemplateLoader [column]="col"></m-columnHeaderTemplateLoader>
          </span>
          <span class="m-sortable-column-icon material-icons" *ngIf="dt.getSortOrder(col) == -1">arrow_downward</span>
          <span class="m-sortable-column-icon material-icons" *ngIf="dt.getSortOrder(col) == 1">arrow_upward</span>
        </div>
      </th>
      <th *ngIf="dt.expandable == true" class="m-expand-header">
      </th>
    </tr>
  `,
  styles: [
    `
    tr {
      text-align: left;
      font-size: 12px;
      height: var(--table-header-height, 55px);
      color: rgba(0, 0, 0, 0.54);
    }
    tr:hover{
      background: #fff;
    }
    th:not(:first-child){
      padding: var(--column-padding, 0 28px);
    }
    th:first-child{
      padding: var(--first-column-padding, 0 0 0 24px);
    }
    th:last-child{
      padding: var(--last-column-padding, 0 24px 0 0);
    }
    th{
      top: 0;
      background: #fff;
      z-index: 2 !important;
    }
    .fixed-header {
      transform: var(--translate);
      position: relative;
      box-shadow: rgb(224, 224, 224) 0 2px 0 0;
      transition-timing-function: ease-in;
    }
    .m-checkbox-header, .m-expand-header{
      width: 1%;
    }
    .m-sortable-column {
      cursor: pointer;
    }

    .m-sortable-column-icon {
      font-size: 12px;
      vertical-align: middle;
    }
    ::ng-deep .single-select-table .mat-checkbox-frame {
      border-radius: 50% !important;
    }

    ::ng-deep .single-select-table .mat-checkbox-background {
      border-radius: 50% !important;
    }
    ::ng-deep .single-select-table .mat-checkbox-background svg {
      display: none;
    }

    ::ng-deep .single-select-table
    .mat-checkbox-checked.mat-accent
    .mat-checkbox-background {
      border: 4px solid #fff !important;
      transform: scale(0.8);
    }
  `,
  ],
})
export class ColumnHeaderComponent {
  constructor(
    @Inject(forwardRef(() => DataTable))
    public dt: DataTable,
  ) {}
  @Input('mColumnHeader') columns: ColumnComponent[];

}
