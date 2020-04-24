import {
  Component,
  EmbeddedViewRef,
  forwardRef,
  Inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  SimpleChanges,
  TemplateRef,
  ViewContainerRef
} from '@angular/core';
import { DataTable } from './datatable';
import { ColumnComponent } from './columns';

@Component({
  selector: 'm-columnBodyTemplateLoader',
  template: ``
})
export class ColumnBodyTemplateLoader implements OnInit, OnChanges, OnDestroy {
  @Input()
  column: any;

  @Input()
  row: any;

  @Input()
  rowIndex: number;

  view: EmbeddedViewRef<any>;

  constructor(public viewContainer: ViewContainerRef) {}

  ngOnInit() {
    this.view = this.viewContainer.createEmbeddedView(
      this.column.bodyTemplate,
      {
        $implicit: this.column,
        row: this.row,
        rowIndex: this.rowIndex
      }
    );
  }

  ngOnChanges(changes: SimpleChanges) {
    if (!this.view) {
      return;
    }

    if ('rowIndex' in changes) {
      this.view.context.rowIndex = changes['rowIndex'].currentValue;
    }
  }

  ngOnDestroy() {
    this.view.destroy();
  }
}

@Component({
  selector: 'm-rowExpansionLoader',
  template: ``
})
export class RowExpansionLoader implements OnInit, OnDestroy {
  @Input()
  template: TemplateRef<any>;

  @Input()
  rowData: any;

  @Input()
  rowIndex: any;

  view: EmbeddedViewRef<any>;

  constructor(public viewContainer: ViewContainerRef) {}

  ngOnInit() {
    this.view = this.viewContainer.createEmbeddedView(this.template, {
      $implicit: this.rowData,
      rowIndex: this.rowIndex
    });
  }

  ngOnDestroy() {
    this.view.destroy();
  }
}

@Component({
  selector: 'm-emptyTableLoader',
  template: ``
})
export class EmptyTableLoader implements OnInit, OnDestroy {
  @Input()
  template: TemplateRef<any>;

  view: EmbeddedViewRef<any>;

  constructor(public viewContainer: ViewContainerRef) {}

  ngOnInit() {
    this.view = this.viewContainer.createEmbeddedView(this.template);
  }

  ngOnDestroy() {
    this.view.destroy();
  }
}

@Component({
  selector: 'm-rowSettingsLoader',
  template: ``
})
export class RowSettingsLoader implements OnInit, OnDestroy {
  @Input()
  template: TemplateRef<any>;

  @Input()
  rowData: any;

  @Input()
  rowIndex: any;

  view: EmbeddedViewRef<any>;

  constructor(public viewContainer: ViewContainerRef) {}

  ngOnInit() {
    this.view = this.viewContainer.createEmbeddedView(this.template, {
      $implicit: this.rowData,
      rowIndex: this.rowIndex
    });
  }

  ngOnDestroy() {
    this.view.destroy();
  }
}

@Component({
  selector: '[mTableBody]',
  template: `
    <ng-template ngFor let-row [ngForOf]="value" let-even="even" let-odd="odd" let-rowIndex="index">
      <tr (click)="dt.handleRowClick($event, row, rowIndex)" (mouseenter)="onRowHover(rowIndex, true)" (mouseleave)="onRowHover(rowIndex, false)" [ngClass]="[dt.isSelected(row)? 'm-row-selected': '']">
        <td *ngIf="dt.selectionHandler == true">
          <div class="m-body-td m-body-td--checkbox" [ngClass]="[dt.selectionMode === 'single' ? 'single-select-table' : '']">
            <mat-checkbox [disabled]="row[dt.rowSelectableKey] !== undefined && row[dt.rowSelectableKey] === false" (click)="dt.selectCheckboxClick($event)" (change)="dt.toggleRowWithCheckbox($event, row)" [checked]="dt.isSelected(row)"></mat-checkbox>
          </div>
        </td>
        <td #cell (mouseenter)="onHover(colIndex, true)" (mouseleave)="onHover(colIndex, false)" [hidden]="col.hidden" *ngFor="let col of columns; let colIndex = index"  [ngClass]="[col.colBodyClass ? col.colBodyClass : '', col.editable ? 'm-editable-column': '', (col.editable && col.editTrigger === 'cell') ? 'm-clickable' : '']" (click)="col.editTrigger === 'cell' && dt.switchCellToEditMode(cell,col,row,rowIndex,colIndex)">
            <div class="m-cell-data" *ngIf="!col.bodyTemplate" [ngClass]="{'m-clickable':col.editable && col.editTrigger !== 'button'}">
              {{row[col.field]}}
              <button type="button" mat-icon-button [ngStyle]="{visibility: (colIndex == hoverCellIndex && rowIndex == hoverRowIndex) ? 'visible' : 'hidden'}" *ngIf="col.editable && col.editTrigger === 'button'" class="action-icon m-clickable" (click)="dt.switchCellToEditMode(cell,col,row,rowIndex,colIndex);"><mat-icon class="m-clickable">mode_edit</mat-icon></button>
              <button type="button" mat-icon-button [ngStyle]="{visibility: (colIndex == hoverCellIndex && rowIndex == hoverRowIndex) ? 'visible' : 'hidden'}" *ngIf="col.deletable" class="action-icon m-clickable" (click)="dt.columnDelete($event, col, row, rowIndex, colIndex);"><mat-icon class="m-clickable">delete</mat-icon></button>
              <button type="button" mat-icon-button [ngStyle]="{visibility: (colIndex == hoverCellIndex && rowIndex == hoverRowIndex) ? 'visible' : 'hidden'}" *ngIf="col.resettable" class="action-icon m-clickable" (click)="dt.columnReset($event, col, row, rowIndex, colIndex);"><mat-icon class="m-clickable">sync</mat-icon></button>
            </div>
            <div class="m-cell-data" *ngIf="col.bodyTemplate">
              <m-columnBodyTemplateLoader [column]="col" [row]="row" [rowIndex]="rowIndex"></m-columnBodyTemplateLoader>
              <button type="button" mat-icon-button [ngStyle]="{visibility: (colIndex == hoverCellIndex && rowIndex == hoverRowIndex) ? 'visible' : 'hidden'}" *ngIf="col.editable && col.editTrigger === 'button'" class="action-icon m-clickable" (click)="dt.switchCellToEditMode(cell,col,row,rowIndex,colIndex);"><mat-icon class="m-clickable">mode_edit</mat-icon></button>
              <button type="button" mat-icon-button [ngStyle]="{visibility: (colIndex == hoverCellIndex && rowIndex == hoverRowIndex) ? 'visible' : 'hidden'}" *ngIf="col.deletable" class="action-icon m-clickable" (click)="dt.columnDelete($event, col, row, rowIndex, colIndex);"><mat-icon class="m-clickable">delete</mat-icon></button>
              <button type="button" mat-icon-button [ngStyle]="{visibility: (colIndex == hoverCellIndex && rowIndex == hoverRowIndex) ? 'visible' : 'hidden'}" *ngIf="col.resettable" class="action-icon m-clickable" (click)="dt.columnReset($event, col, row, rowIndex, colIndex);"><mat-icon class="m-clickable">sync</mat-icon></button>
            </div>
            <div [ngStyle]="getOffsetStyles(cell)" class="m-cell-editor" (click)="$event.stopPropagation()" *ngIf="col.editable && rowIndex === dt.editRowIndex && colIndex === dt.editCellIndex">
              <mat-card matInput class="m-input-card" *ngIf="!col.editorTemplate">
                <mat-form-field [floatLabel]="'never'" class="m-input-form">
                  <input matInput placeholder="{{col.header}}" [(ngModel)]="row[col.field]" (change)="dt.onCellEditorChange($event, col, row, rowIndex)"
                         (keydown)="dt.onCellEditorKeydown($event, col, row, rowIndex)" (blur)="dt.onCellEditorBlur($event, col, row, rowIndex)"
                         (input)="dt.onCellEditorInput($event, col, row, rowIndex)">
                </mat-form-field>
              </mat-card>
              <m-columnEditorTemplateLoader *ngIf="col.editorTemplate" (click)="$event.stopPropagation()" [column]="col" [row]="row" [rowIndex]="rowIndex"></m-columnEditorTemplateLoader>
            </div>

        </td>
        <td *ngIf="dt.expandable == true">
          <span class="m-expand-icon material-icons" (click)="dt.toggleRow(row, $event)">
            <i class="material-icons m-clickable" *ngIf="!dt.isRowExpanded(row)">keyboard_arrow_right</i>
            <i class="material-icons m-clickable" *ngIf="dt.isRowExpanded(row)">keyboard_arrow_down</i>
          </span>
        </td>
        <div class="row-settings" *ngIf="dt.rowSettingsTemplate && hoverRowIndex === rowIndex">
          <m-rowSettingsLoader [rowData]="row" [rowIndex]="rowIndex" [template]="dt.rowSettingsTemplate"></m-rowSettingsLoader>
        </div>
      </tr>
      <tr *ngIf="dt.expandable && dt.isRowExpanded(row)" class="m-expanded-row-content">
        <td [attr.colspan]="dt.totalColumns()">
          <m-rowExpansionLoader [rowData]="row" [rowIndex]="rowIndex" [template]="dt.expansionTemplate"></m-rowExpansionLoader>
        </td>
      </tr>
    </ng-template>

    <tr *ngIf="dt.isEmpty()" class="m-empty-row">
      <td [attr.colspan]="dt.totalColumns()">
        <m-emptyTableLoader *ngIf="dt.emptyTableTemplate" [template]="dt.emptyTableTemplate"></m-emptyTableLoader>
      </td>
    </tr>
  `,
  styles: [
    `
      tr {
        border-top: 1px solid #e0e0e0;
        height: var(--row-height, 50px);
        transition: all 0.2s;
      }
      tr:hover {
        background: #eeeeee;
      }
      td:not(:first-child) {
        padding: var(--column-padding, 0px 28px);
      }
      td:first-child {
        padding: var(--first-column-padding, 0 0 0 24px);
      }
      td:last-of-type  {
        padding: var(--last-column-padding, 0 24px 0 0);
      }
      .m-row-selected {
        background: #eeeeee;
      }
      .checkbox-container {
        overflow: hidden;
      }
      .m-cell-data {
        position: relative;
      }
      .m-expand-icon {
        font-size: 12px;
        vertical-align: middle;
        cursor: pointer;
        color: #757575;
      }
      .m-input-card {
        background: #f7f7f7;
        padding: 0px 0px !important;
        top: 0px !important;
      }
      .m-input-form {
        width: 150px;
        padding: 0px 12px;
      }
      .m-cell-editor {
        position: absolute !important;
        z-index: 1000 !important;
      }
      .m-editable-column > .m-cell-editor {
        display: none;
      }
      .m-editable-column.m-cell-editing > .m-cell-editor {
        display: block;
      }
      .m-editable-column.m-cell-editing > .m-cell-data {
        visibility: hidden;
      }
      .action-icon {
        color: #757575;
        cursor: pointer;
        width: 30px;
        height: 30px;
        line-height: 30px;
      }
      .action-icon mat-icon {
        font-size: 16px;
      }
      .row-settings {
        height: calc(var(--row-height, 48px) - 1px);
        background: linear-gradient(to right, #eeeeeef0, #eeeeee);
        position: absolute;
        right: 0;
        padding: 0 1rem;
        line-height: calc(var(--row-height, 48px) - 1px);
      }
    `
  ]
})
export class TableBodyComponent {
  constructor(@Inject(forwardRef(() => DataTable)) public dt: DataTable) {}
  @Input('mTableBody')
  columns: ColumnComponent[];
  @Input()
  value;
  @Input()
  headerHeight = 0;

  @Input()
  tableContainerScrollX = 0;

  hoverRowIndex;
  hoverCellIndex;

  onHover(ci, hover) {
    this.hoverCellIndex = hover ? ci : undefined;
  }

  onRowHover(ri, hover) {
    this.hoverRowIndex = hover ? ri : undefined;
  }
  getOffsetStyles(cell) {
    return {
      'top.px': cell.offsetTop + this.headerHeight,
      'left.px': cell.offsetLeft - this.tableContainerScrollX
    };
  }
}
