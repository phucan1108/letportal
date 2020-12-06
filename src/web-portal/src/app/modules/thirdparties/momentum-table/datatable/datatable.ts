import {
  AfterContentChecked,
  AfterContentInit,
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ContentChild,
  ContentChildren,
  DoCheck,
  EventEmitter,
  Input,
  IterableDiffers,
  NgModule,
  OnDestroy,
  OnInit,
  Output,
  QueryList,
  Renderer2,
  TemplateRef,
  ViewChild,
  ElementRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../material';
import { DomHandler } from '../services/domhandler.service';
import { ObjectUtils } from '../services/util.service';
import { FormsModule } from '@angular/forms';
import { ColumnComponent, ColumnEditorTemplateLoader } from './columns';
import {
  ColumnBodyTemplateLoader,
  EmptyTableLoader,
  RowExpansionLoader, RowSettingsLoader,
  TableBodyComponent
} from './body';
import { GlobalHeaderTemplateLoader, Header, HeaderComponent } from './header';
import { Footer, FooterComponent, GlobalFooterTemplateLoader } from './footer';
import {
  ColumnHeaderComponent,
  ColumnHeaderTemplateLoader
} from './column-header';
import {
  ColumnSubHeaderComponent,
  ColumnSubHeaderTemplateLoader
} from './column-sub-header';
import {
  ColumnFooterComponent,
  ColumnFooterTemplateLoader
} from './column-footer';
import { MomentumTemplate } from './template.directive';
import { Overlay } from '@angular/cdk/overlay';
import { Subscription } from 'rxjs';

@Component({
  selector: 'm-table',
  template: `
    <mat-card [ngClass]="{'m-datatable m-widget':true}" [ngStyle]="{'width': width, 'height': height}" class="card-wrapper">
      <div #cardHeader *ngIf="header" [mHeader]="header" (filterChange)="filterChange($event)"></div>
      <div class="table-container" #tableContainer (scroll)="tableContainerScrollX = $event.target.scrollLeft">
        <table>
          <thead #columnHeader class="column-header" [mColumnHeader]="columns"></thead>
          <thead [mColumnSubHeader]="columns" *ngIf="hasSubFooter()"></thead>
          <tfoot [mColumnFooter]="columns" *ngIf="hasFooter()"></tfoot>
          <tbody [tableContainerScrollX]="tableContainerScrollX" [headerHeight]="cardHeaderHeight" [mTableBody]="columns" [value]="dataToRender"></tbody>
        </table>
      </div>
      <div *ngIf="footer" [mFooter]="footer"></div>
    </mat-card>
  `,
  styles: [
    `
      :host {
        display: block;
      }
      .card-wrapper {
        padding: 0;
      }
      .table-container {
        overflow: auto;
        height: var(--table-height);
        background: #fff;
        font-size: 14px;
        color: rgba(0, 0, 0, 0.87);
      }
      table {
        width: 100%;
        border-collapse: collapse;
        border-spacing: 0;
      }
    `
  ],
  providers: [DomHandler, ObjectUtils]
})
export class DataTable
  implements
    OnInit,
    AfterContentInit,
    AfterViewInit,
    OnDestroy,
    DoCheck,
    AfterContentChecked {
  @Input()
  width = '100%';

  @Input()
  height = 'auto';

  @Input()
  defaultSortOrder = 1;

  @Output()
  onSort: EventEmitter<any> = new EventEmitter();

  @Input()
  sortLocal = true;

  @Input()
  selectable: boolean;

  @Input()
  rowSelectableKey: string;

  @Input()
  disableSelectAll = false;

  @Input()
  selectionMode = 'multiple';

  @Input()
  selectionHandler = true;

  @Output()
  selectionChange: EventEmitter<any> = new EventEmitter();

  @Output()
  onRowClick: EventEmitter<any> = new EventEmitter();

  @Output()
  onRowSelect: EventEmitter<any> = new EventEmitter();

  @Output()
  onRowUnselect: EventEmitter<any> = new EventEmitter();

  @Input()
  expandable = false;

  @Input()
  expandedRows: any[];

  @Input()
  expandMultiple = true;

  @Output()
  onRowExpand: EventEmitter<any> = new EventEmitter();

  @Output()
  onRowCollapse: EventEmitter<any> = new EventEmitter();

  @Output()
  onEditInit: EventEmitter<any> = new EventEmitter();

  @Output()
  onEditComplete: EventEmitter<any> = new EventEmitter();

  @Output()
  onEdit: EventEmitter<any> = new EventEmitter();

  @Output()
  onEditCancel: EventEmitter<any> = new EventEmitter();

  @Output()
  onColumnDelete: EventEmitter<any> = new EventEmitter();

  @Output()
  onColumnReset: EventEmitter<any> = new EventEmitter();

  @Output()
  valueChange: EventEmitter<any[]> = new EventEmitter<any[]>();

  @Output()
  onFilter: EventEmitter<any> = new EventEmitter();

  @Input()
  filterLocal = true;

  @Input()
  locale: any = {};

  @Output()
  onReload: EventEmitter<string> = new EventEmitter();

  @Output()
  onPage: EventEmitter<any> = new EventEmitter();

  @ContentChildren(ColumnComponent)
  cols: QueryList<ColumnComponent>;

  @ContentChildren(MomentumTemplate)
  templates: QueryList<MomentumTemplate>;

  @ContentChild(Header)
  header;

  @ContentChild(Footer)
  footer;

  @ViewChild('cardHeader', { read: ElementRef })
  cardHeader: ElementRef;

  @ViewChild('tableContainer')
  tableContainer: ElementRef;

  @ViewChild('columnHeader', { read: ElementRef })
  columnHeader: ElementRef;

  public cardHeaderHeight = 0;
  public tableContainerScrollX = 0;

  public _value: any[];

  public totalRecords = 0;

  public pageIndex = 0;

  public filteredValue: any[];

  public dataToRender: any[];

  public columns: ColumnComponent[];

  public sortColumn: ColumnComponent;

  public preventRowClickPropagation: boolean;

  public expansionTemplate: TemplateRef<any>;

  public emptyTableTemplate: TemplateRef<any>;

  public rowSettingsTemplate: TemplateRef<any>;

  public editorClick: boolean;

  public editingCell: any;

  public documentEditListener: Function;

  public editRowIndex: number;

  public editCellIndex: number;

  _sortField: string;

  _sortOrder = 1;

  _selection: any;

  editChanged: boolean;

  globalFilterString: string;

  filterTimeout: any;

  differ: any;

  columnsSubscription$: Subscription;

  verticalScrollHandler;
  propSet = false;
  overlayRef;

  constructor(
    public domHandler: DomHandler,
    public objectUtils: ObjectUtils,
    public renderer: Renderer2,
    public differs: IterableDiffers,
    public changeDetector: ChangeDetectorRef,
    private overlay: Overlay
  ) {
    this.differ = differs.find([]).create(null);
  }

  ngOnInit() {}

  ngAfterContentInit() {
    this.initColumns();

    this.columnsSubscription$ = this.cols.changes.subscribe(_ => {
      this.initColumns();
      this.changeDetector.markForCheck();
    });

    this.templates?.forEach(item => {
      switch (item.getType()) {
        case 'expansion':
          this.expansionTemplate = item.template;
          break;
        case 'emptyTable':
          this.emptyTableTemplate = item.template;
          break;
        case 'rowSettings':
          this.rowSettingsTemplate = item.template;
          break;
      }
    });
  }

  ngAfterViewInit() {
    if (this.cardHeader) {
      const el = this.cardHeader.nativeElement as HTMLDivElement;
      // duping and dumping on DOM se can have the real dimensions.
      const dupe = el.cloneNode(true) as HTMLDivElement;
      document.body.appendChild(dupe);
      this.cardHeaderHeight = dupe.clientHeight;
      // and immediately getting rid of it.
      document.body.removeChild(dupe);
      this.tableContainerScrollX = this.tableContainer.nativeElement.scrollLeft;
      this.changeDetector.detectChanges();
    }

  }

  public fixHeader(scrollElement, threshold) {
    const elemsToFixed = Array.from(this.columnHeader.nativeElement.children[0].children);
    if (!elemsToFixed || elemsToFixed.length === 0) {
      return;
    }
    scrollElement.removeEventListener('scroll', this.verticalScrollHandler);
    this.verticalScrollHandler = this.headScrollFn.bind(this, threshold, elemsToFixed);
    scrollElement.addEventListener('scroll', this.verticalScrollHandler, true);
  }

  headScrollFn(threshold, elemsToFixed) {
    const firstEl = elemsToFixed[0];
    window.requestAnimationFrame(() => {
      const top = firstEl.parentElement!.getBoundingClientRect().top;
      if (top > threshold) {
        if (!this.propSet) return;
        this.propSet = false;
        this.setElemsFixed(elemsToFixed, top, threshold, false);
        return;
      }
      this.propSet = true;
      this.setElemsFixed(elemsToFixed, top, threshold);
    });
  }

  setElemsFixed(elemsToFixed: HTMLHeadingElement[], top: number,
                         threshold: number, setFixed = true) {
    elemsToFixed?.forEach((elem) => {
      if (!setFixed) {
        elem.removeAttribute('style');
        elem.classList.remove('fixed-header');
        if (this.overlayRef) {
          this.overlayRef.dispose();
          this.overlayRef = undefined;
        }
        return;
      }
      if (!this.overlayRef) {
        this.overlayRef = this.overlay.create();
      }
      elem.classList.add('fixed-header');
      elem.style.setProperty('--translate', `translateY(${((top - threshold) * -1)}px)`);
    });
  }


  filterChange(event: any) {
    this.globalFilterString = event.value;
    if (this.filterTimeout) {
      clearTimeout(this.filterTimeout);
    }
    this.filterTimeout = setTimeout(() => {
      this._filter(event.type);
      this.filterTimeout = null;
    }, 300);
  }

  @Input()
  get value(): any[] {
    return this._value;
  }

  set value(val: any[]) {
    this._value = val ? [...val] : null;
    this.handleDataChange();

    this.valueChange.emit(this.value);
  }

  @Input()
  get sortField(): string {
    return this._sortField;
  }

  set sortField(val: string) {
    this._sortField = val;
    if (this.sortLocal) this.sortSingle();
  }

  @Input()
  get sortOrder(): number {
    return this._sortOrder;
  }
  set sortOrder(val: number) {
    this._sortOrder = val;
    if (this.sortLocal) this.sortSingle();
  }

  @Input()
  get selection(): any {
    return this._selection;
  }

  set selection(val: any) {
    this._selection = val;
  }

  initColumns(): void {
    this.columns = this.cols.toArray();
  }

  ngDoCheck() {}

  ngAfterContentChecked() {
    // let changes = this.differ.diff(this.value);
    // if (changes) {
    //   this.handleDataChange();
    // }
  }

  handleDataChange() {
    if (this.hasFilter()) {
      this._filter();
    }

    if (this.sortField) {
      if (!this.sortColumn && this.columns) {
        this.sortColumn = this.columns.find(
          col => col.field === this.sortField
        );
      }
      if (this.sortLocal) this.sortSingle();
    }

    this.updateDataToRender(this.filteredValue || this.value);
  }

  pageChange(pageData) {
    this.footer.pageSize = pageData.pageSize;
    this.updateDataToRender(
      this.filteredValue || this.value,
      pageData.pageIndex,
      pageData.pageSize
    );
    this.onPage.emit(pageData);
  }

  updateDataToRender(
    dataSource,
    pageIndex: number = 0,
    pageSize: number = this.footer ? this.footer.pageSize : 0
  ) {
    this.totalRecords = dataSource ? dataSource.length : 0;
    this.pageIndex = pageIndex;

    if (this.footer && this.footer.paginator) {
      this.dataToRender = [];
      const startIndex: number = pageIndex * pageSize;
      const endIndex: number = startIndex + pageSize;

      for (let i = startIndex; i < endIndex; i++) {
        if (i >= dataSource.length) {
          break;
        }
        this.dataToRender.push(dataSource[i]);
      }
    } else {
      this.dataToRender = dataSource;
    }
  }

  resolveFieldData(data: any, field: string): any {
    if (data && field) {
      if (field.indexOf('.') === -1) {
        return data[field];
      } else {
        const fields: string[] = field.split('.');
        let value = data;
        for (let i = 0, len = fields.length; i < len; ++i) {
          if (value == null) {
            return null;
          }
          value = value[fields[i]];
        }
        return value;
      }
    } else {
      return null;
    }
  }

  visibleColumns() {
    return this.columns ? this.columns.filter(c => !c.hidden) : [];
  }

  totalColumns() {
    return (
      this.visibleColumns().length +
      (this.expandable ? 1 : 0) +
      (this.selectionHandler ? 1 : 0)
    );
  }

  hasFooter() {
    if (this.columns) {
      for (let i = 0; i < this.columns.length; i++) {
        if (this.columns[i].footer || this.columns[i].footerTemplate) {
          return true;
        }
      }
    }
    return false;
  }

  hasSubFooter() {
    if (this.columns) {
      for (let i = 0; i < this.columns.length; i++) {
        if (this.columns[i].subHeader || this.columns[i].subHeaderTemplate) {
          return true;
        }
      }
    }
    return false;
  }

  isEmpty() {
    return !this.dataToRender || this.dataToRender.length === 0;
  }

  sort(event, column: ColumnComponent) {
    if (!column.sortable) {
      return;
    }
    const targetNode = event.target.nodeName;
    if (
      (targetNode === 'TH' &&
        this.domHandler.hasClass(event.target, 'm-sortable-column')) ||
      ((targetNode === 'SPAN' || targetNode === 'DIV') &&
        !this.domHandler.hasClass(event.target, 'm-clickable'))
    ) {
      this.sortColumn = column;
      // todo sort logic

      const columnSortField = column.field;
      this._sortOrder =
        this.sortField === columnSortField
          ? this.sortOrder * -1
          : this.defaultSortOrder;
      this._sortField = columnSortField;
      this.sortColumn = column;

      if (this.sortLocal) this.sortSingle();

      this.onSort.emit({
        field: this.sortField,
        order: this.sortOrder
      });
    }

    this.updateDataToRender(this.filteredValue || this.value);
  }

  sortSingle() {
    if (this.value) {
      this.value.sort((data1, data2) => {
        const value1 = this.resolveFieldData(data1, this.sortField);
        const value2 = this.resolveFieldData(data2, this.sortField);
        let result = null;

        if (value1 == null && value2 != null) result = -1;
        else if (value1 != null && value2 == null) result = 1;
        else if (value1 == null && value2 == null) result = 0;
        else if (typeof value1 === 'string' && typeof value2 === 'string')
          result = value1.localeCompare(value2);
        else result = value1 < value2 ? -1 : value1 > value2 ? 1 : 0;

        return this.sortOrder * result;
      });
    }
  }

  getSortOrder(column: ColumnComponent) {
    let order = 0;
    const columnSortField = column.field;
    if (this.sortField && columnSortField === this.sortField) {
      order = this.sortOrder;
    }
    return order;
  }

  handleRowClick(event: MouseEvent, rowData: any, index: number) {
    const targetNode = (<HTMLElement>event.target).nodeName;

    if (
      targetNode === 'INPUT' ||
      targetNode === 'BUTTON' ||
      targetNode === 'A' ||
      this.domHandler.hasClass(event.target, 'm-clickable')
    ) {
      return;
    }

    this.onRowClick.emit({ originalEvent: event, data: rowData });

    if (this.selectionHandler === true) return;

    if (this.selectable) {
      const selected = this.isSelected(rowData);
      if (this.selectionMode === 'single') {
        if (selected) {
          this._selection = [];
          this.onRowUnselect.emit({
            originalEvent: event,
            data: rowData,
            type: 'row'
          });
        } else {
          this._selection = [rowData];
          this.onRowSelect.emit({
            originalEvent: event,
            data: rowData,
            type: 'row'
          });
        }
      } else if (this.selectionMode === 'multiple') {
        if (selected) {
          const selectionIndex = this.findIndexInSelection(rowData);
          this._selection = this.selection.filter(
            (val, i) => i !== selectionIndex
          );
          this.onRowUnselect.emit({
            originalEvent: event,
            data: rowData,
            type: 'row'
          });
        } else {
          this._selection = [...(this.selection || []), rowData];
          this.onRowSelect.emit({
            originalEvent: event,
            data: rowData,
            type: 'row'
          });
        }
      }

      this.selectionChange.emit(this.selection);
    }
  }

  toggleRowsWithCheckbox(event) {
    if (event.checked) this.selection = this.value.slice();
    else this.selection = [];

    this.selectionChange.emit(this.selection);

    // this.onHeaderCheckboxToggle.emit({originalEvent: event, checked: event.checked});
  }

  selectCheckboxClick(event) {
    event.stopPropagation();
  }

  toggleRowWithCheckbox(event, rowData: any) {
    if (this.selectionMode === 'single') {
      if (!this.objectUtils.equalsByValue(this.selection, [rowData])) {
        this._selection = [rowData];
        this.onRowSelect.emit({
          originalEvent: event,
          data: rowData,
          type: 'checkbox'
        });
      } else {
        this._selection = [];
        this.onRowUnselect.emit({
          originalEvent: event,
          data: rowData,
          type: 'checkbox'
        });
      }
    } else if (this.selectionMode === 'multiple') {
      const selectionIndex = this.findIndexInSelection(rowData);
      this.selection = this.selection || [];

      if (selectionIndex !== -1) {
        this._selection = this.selection.filter(
          (val, i) => i !== selectionIndex
        );
        this.onRowUnselect.emit({
          originalEvent: event,
          data: rowData,
          type: 'checkbox'
        });
      } else {
        this._selection = [...this.selection, rowData];
        this.onRowSelect.emit({
          originalEvent: event,
          data: rowData,
          type: 'checkbox'
        });
      }
    }

    this.selectionChange.emit(this.selection);
  }

  findIndexInSelection(rowData: any) {
    let index = -1;
    if (this.selection && this.selection.length) {
      for (let i = 0; i < this.selection.length; i++) {
        if (this.equals(rowData, this.selection[i])) {
          index = i;
          break;
        }
      }
    }
    return index;
  }

  equals(data1, data2) {
    return JSON.stringify(data1) === JSON.stringify(data2);
  }

  isSelected(rowData) {
    if (rowData && this.selection) {
      if (this.selection instanceof Array)
        return this.findIndexInSelection(rowData) > -1;
    }
    return false;
  }

  itemsSelected() {
    if (this.selection instanceof Array) return this.selection.length;
    else {
      return 0;
    }
  }

  get allSelected() {
    let val = true;
    if (this.selection && this.selection.length) {
      for (const data of this.value) {
        if (!this.isSelected(data)) {
          val = false;
          break;
        }
      }
    } else {
      val = false;
    }
    return val;
  }

  toggleRow(row: any, event?: Event) {
    if (!this.expandedRows) {
      this.expandedRows = [];
    }

    const expandedRowIndex = this.findExpandedRowIndex(row);

    if (expandedRowIndex !== -1) {
      this.expandedRows.splice(expandedRowIndex, 1);
      this.onRowCollapse.emit({
        originalEvent: event,
        data: row
      });
    } else {
      if (!this.expandMultiple) {
        this.expandedRows = [];
      }

      this.expandedRows.push(row);
      this.onRowExpand.emit({
        originalEvent: event,
        data: row
      });
    }

    if (event) {
      event.preventDefault();
    }
  }

  findExpandedRowIndex(row: any): number {
    let index = -1;
    if (this.expandedRows) {
      for (let i = 0; i < this.expandedRows.length; i++) {
        if (this.expandedRows[i] === row) {
          index = i;
          break;
        }
      }
    }
    return index;
  }

  isRowExpanded(row: any): boolean {
    return this.findExpandedRowIndex(row) !== -1;
  }

  findCell(element) {
    if (element) {
      let cell = element;
      while (cell && cell.tagName !== 'TD') {
        cell = cell.parentElement;
      }

      return cell;
    } else {
      return null;
    }
  }

  switchCellToEditMode(
    cell: any,
    column: ColumnComponent,
    rowData: any,
    ri,
    ci
  ) {
    if (column.editable && !this.editorClick) {
      this.editorClick = true;
      this.bindDocumentEditListener();

      if (cell !== this.editingCell) {
        if (
          this.editingCell &&
          this.domHandler.find(this.editingCell, '.ng-invalid.ng-dirty')
            .length === 0
        ) {
          this.domHandler.removeClass(this.editingCell, 'm-cell-editing');
        }

        this.editingCell = cell;
        this.onEditInit.emit({ column: column, data: rowData });
        this.domHandler.addClass(cell, 'm-cell-editing');
        const focusable = this.domHandler.findSingle(
          cell,
          '.m-cell-editor input'
        );
        if (focusable) {
          setTimeout(
            () => this.domHandler.invokeElementMethod(focusable, 'focus'),
            50
          );
        }
        this.editRowIndex = ri;
        this.editCellIndex = ci;
      }
    }
  }

  switchCellToViewMode(element: any) {
    this.editingCell = null;
    const cell = this.findCell(element);
    this.domHandler.removeClass(cell, 'm-cell-editing');
    this.unbindDocumentEditListener();
  }

  closeCell() {
    if (this.editingCell) {
      this.domHandler.removeClass(this.editingCell, 'm-cell-editing');
      this.editingCell = null;
      this.editRowIndex = null;
      this.editCellIndex = null;
      this.unbindDocumentEditListener();
    }
  }

  bindDocumentEditListener() {
    if (!this.documentEditListener) {
      this.documentEditListener = this.renderer.listen(
        'document',
        'click',
        event => {
          if (!this.editorClick) {
            this.closeCell();
          }
          this.editorClick = false;
        }
      );
    }
  }

  unbindDocumentEditListener() {
    if (this.documentEditListener) {
      this.documentEditListener();
      this.documentEditListener = null;
    }
  }

  onCellEditorKeydown(
    event,
    column: ColumnComponent,
    rowData: any,
    rowIndex: number
  ) {
    if (column.editable) {
      // enter
      if (event.keyCode === 13) {
        if (
          this.domHandler.find(this.editingCell, '.ng-invalid.ng-dirty')
            .length === 0
        ) {
          this.switchCellToViewMode(event.target);
          event.preventDefault();
        }
      } else if (event.keyCode === 27) {
        // escape
        this.switchCellToViewMode(event.target);
        event.preventDefault();
      }
    }
  }

  onCellEditorInput(
    event,
    column: ColumnComponent,
    rowData: any,
    rowIndex: number
  ) {
    if (column.editable) {
      this.onEdit.emit({
        originalEvent: event,
        column: column,
        data: rowData,
        index: rowIndex
      });
    }
  }

  onCellEditorChange(
    event,
    column: ColumnComponent,
    rowData: any,
    rowIndex: number
  ) {
    if (column.editable) {
      this.editChanged = true;

      this.onEditComplete.emit({
        column: column,
        data: rowData,
        index: rowIndex
      });
    }
  }

  onCellEditorBlur(
    event,
    column: ColumnComponent,
    rowData: any,
    rowIndex: number
  ) {
    if (column.editable) {
      if (this.editChanged) this.editChanged = false;
      else
        this.onEditCancel.emit({
          column: column,
          data: rowData,
          index: rowIndex
        });

      // this.closeCell();
    }
  }

  _filter(type?: string) {
    if (!this.value || !this.columns) {
      return;
    }

    this.filteredValue = [];

    for (let i = 0; i < this.value.length; i++) {
      let globalMatch = false;

      for (let j = 0; j < this.columns.length; j++) {
        const col = this.columns[j];

        if (this.header.globalSearch && !globalMatch) {
          globalMatch = this.filterContains(
            this.resolveFieldData(this.value[i], col.field),
            this.globalFilterString
          );
        }
      }

      if (globalMatch) {
        this.filteredValue.push(this.value[i]);
      }
    }

    if (this.filteredValue.length === this.value.length) {
      this.filteredValue = null;
    }

    if (this.filterLocal)
      this.updateDataToRender(this.filteredValue || this.value);

    this.onFilter.emit({
      filterQuery: this.globalFilterString,
      filteredValue: this.filteredValue || this.value,
      type: type
    });
  }

  filterContains(value, filter): boolean {
    if (
      filter === undefined ||
      filter === null ||
      (typeof filter === 'string' && filter.trim() === '')
    ) {
      return true;
    }

    if (value === undefined || value === null) {
      return false;
    }

    return (
      value
        .toString()
        .toLowerCase()
        .indexOf(filter.toLowerCase()) !== -1
    );
  }

  hasFilter() {
    return this.globalFilterString && this.globalFilterString.trim().length;
  }

  public reload() {
    this.onReload.emit();
  }

  public columnDelete(event, col, row, rowIndex, colIndex) {
    this.onColumnDelete.emit({
      column: col,
      data: row,
      rowIndex: rowIndex,
      colIndex: colIndex
    });
    event.stopPropagation();
  }

  public columnReset(event, col, row, rowIndex, colIndex) {
    this.onColumnReset.emit({
      column: col,
      data: row,
      rowIndex: rowIndex,
      colIndex: colIndex
    });
    event.stopPropagation();
  }

  public exportCSV(
    csvSeparator: string,
    exportFilename: string,
    selectionOnly: boolean
  ) {
    let data = this.filteredValue || this.value;
    let csv = '\ufeff';

    if (selectionOnly) {
      data = [];
      if (this.filteredValue) {
        if (this.selection && this.selection instanceof Array)
          this.selection?.forEach(selectedRow => {
            if (this.filteredValue.indexOf(selectedRow) !== -1)
              data.push(selectedRow);
          });
      } else {
        if (this.selection && this.selection instanceof Array)
          data = this.selection;
      }
    }

    // headers
    for (let i = 0; i < this.columns.length; i++) {
      if (this.columns[i].field) {
        csv += '"' + (this.columns[i].header || this.columns[i].field) + '"';
        csv += csvSeparator;
      }
    }
    csv = csv.slice(0, -1);

    // body
    data?.forEach((record, i) => {
      csv += '\n';
      for (let i = 0; i < this.columns.length; i++) {
        if (this.columns[i].field) {
          csv +=
            '"' + this.resolveFieldData(record, this.columns[i].field) + '"';
          csv += csvSeparator;

        }
      }
      csv = csv.slice(0, -1);
    });

    const blob = new Blob([csv], {
      type: 'text/csv;charset=utf-8;'
    });

    if (window.navigator.msSaveOrOpenBlob) {
      navigator.msSaveOrOpenBlob(blob, exportFilename + '.csv');
    } else {
      const link = document.createElement('a');
      link.style.display = 'none';
      document.body.appendChild(link);
      if (link.download !== undefined) {
        link.setAttribute('href', URL.createObjectURL(blob));
        link.setAttribute('download', exportFilename + '.csv');
        link.click();
      } else {
        csv = 'data:text/csv;charset=utf-8,' + csv;
        window.open(encodeURI(csv));
      }
      document.body.removeChild(link);
    }
  }

  ngOnDestroy() {
    if (this.columnsSubscription$) {
      this.columnsSubscription$.unsubscribe();
    }
  }
}

@NgModule({
  imports: [CommonModule, MaterialModule, FormsModule],
  exports: [DataTable, ColumnComponent, MomentumTemplate, Header, Footer],
  declarations: [
    DataTable,
    ColumnComponent,
    HeaderComponent,
    FooterComponent,
    ColumnHeaderComponent,
    ColumnSubHeaderComponent,
    ColumnFooterComponent,
    TableBodyComponent,
    EmptyTableLoader,
    RowSettingsLoader,
    RowExpansionLoader,
    MomentumTemplate,
    GlobalHeaderTemplateLoader,
    ColumnSubHeaderTemplateLoader,
    GlobalFooterTemplateLoader,
    Header,
    Footer,
    ColumnHeaderTemplateLoader,
    ColumnBodyTemplateLoader,
    ColumnFooterTemplateLoader,
    ColumnEditorTemplateLoader
  ]
})
export class MomentumTableModule {}
