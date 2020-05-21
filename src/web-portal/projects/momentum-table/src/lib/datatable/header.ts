import {
  AfterViewInit,
  Component,
  ContentChild,
  ElementRef,
  EmbeddedViewRef,
  EventEmitter,
  forwardRef,
  Inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  Renderer2,
  SimpleChanges,
  TemplateRef,
  ViewChild,
  ViewContainerRef,
} from '@angular/core';
import { DataTable } from './datatable';

@Component({
  selector: 'm-header',
  template: ``,
})
export class Header {
  @Input() title: string;
  @Input() globalSearch: boolean = false;
  @Input() searchField: string = 'line';
  @Input() colSetting: boolean = true;
  @Input() export: boolean = false;
  @Input() csvSeparator: string = ',';
  @Input() exportFilename: string = 'download';
  @Input() exportSelectionOnly: boolean = false;
  @Input() reload: boolean = false;

  @ContentChild(TemplateRef) template: TemplateRef<any>;
  constructor() {}
}

@Component({
  selector: 'm-globalHeaderTemplateLoader',
  template: ``,
})
export class GlobalHeaderTemplateLoader
  implements OnInit, OnChanges, OnDestroy {
  @Input() header: any;

  view: EmbeddedViewRef<any>;

  constructor(public viewContainer: ViewContainerRef) {}

  ngOnInit() {
    this.view = this.viewContainer.createEmbeddedView(this.header.template, {
      $implicit: this.header,
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (!this.view) {
      return;
    }
  }

  ngOnDestroy() {
    this.view.destroy();
  }
}

@Component({
  selector: '[mHeader]',
  template: `
    <div *ngIf="header.template" class="card-header">
      <m-globalHeaderTemplateLoader [header]="header"></m-globalHeaderTemplateLoader>
    </div>

    <div *ngIf="!header.template" class="card-header">
      <div *ngIf="header.title && !dt.itemsSelected()" class="card-header-title">{{header.title}}</div>
      <div *ngIf="dt.itemsSelected()" class="card-header-selection-count">{{dt.itemsSelected()}} item(s) selected</div>
      <div class="tool-box">
        <div class="search-setting-wrapper" *ngIf="header.globalSearch">
          <div *ngIf="header.searchField == 'line'" class="line-search">
            <mat-form-field class="m-search-form" [floatLabel]="'never'" [ngClass]="[searchOpen ? 'search-open' : 'search-close']">
              <input matInput #globalFilterField placeholder="{{dt.locale.searchLabel ? dt.locale.searchLabel : 'Search...'}}">
            </mat-form-field>
            <button mat-icon-button *ngIf="!searchOpen" class="search-icon" (click)="toggleSearch(true)">
              <mat-icon class="mat-24" aria-label="Search icon">search</mat-icon>
            </button>
            <button mat-icon-button *ngIf="searchOpen" class="search-icon" (click)="toggleSearch(false)">
              <mat-icon class="mat-24" aria-label="Search icon">clear</mat-icon>
            </button>
          </div>
          <div *ngIf="header.searchField == 'box'" class="box-search">
            <div class="box-search-icon">
              <button mat-icon-button (click)="emitChange()">
                <mat-icon class="mat-24" aria-label="Search icon">search</mat-icon>
              </button>
            </div>
            <div class="box-search-input-wrapper">
              <input type="text" #globalFilterField placeholder="{{dt.locale.searchLabel ? dt.locale.searchLabel : 'Search...'}}">
            </div>
          </div>
        </div>

        <button mat-icon-button *ngIf="header.colSetting" class="col-setting-btn" (click)="openColSetting()">
          <mat-icon class="mat-24" aria-label="column">view_column</mat-icon>
        </button>
        <mat-card class="col-setting-wrapper" *ngIf="colSettingOpen" (click)="$event.stopPropagation()">
          <mat-selection-list>
            <mat-list-option [selected]="!col.hidden" [value]="col.header" (click)="toggleColumn(col)" checkboxPosition="'before'" *ngFor="let col of dt.columns">
              {{col.header}}
            </mat-list-option>
          </mat-selection-list>
        </mat-card>

        <button mat-icon-button *ngIf="header.export" class="col-setting-btn"  (click)="dt.exportCSV(header.csvSeparator, header.exportFilename, header.exportSelectionOnly)">
          <mat-icon class="mat-24" aria-label="download">file_download</mat-icon>
        </button>

        <button mat-icon-button *ngIf="header.reload" class="col-setting-btn" (click)="dt.reload()">
          <mat-icon class="mat-24" aria-label="refresh">refresh</mat-icon>
        </button>
      </div>

    </div>
  `,
  styles: [
    `
    .card-header{
      height: var(--card-header-height, 64px);
      padding: var(--card-header-padding, 0 14px 0 24px);
      border-bottom: 1px solid #e0e0e0;
      position: relative;
      display: flex;
    }
    .card-header-title{
      width: 50%;
      line-height: 64px;
      font-size: 22px;
    }
    .card-header-selection-count{
      width: 50%;
      line-height: 64px;
    }
    .tool-box{
      display: flex;
      justify-content: flex-end;
      width: 50%;
      right: 0px;
      color: #757575;
    }
    .search-setting-wrapper{
      width: 100%;
      position: relative;
    }
    .col-setting-btn{
      top: 12px;
    }
    .m-search-form{
      position: absolute !important;
      right: 0;
      -webkit-transition: width 0.2s ease-in-out;
      transition: width 0.2s ease-in-out;
      margin-top: 5px;
    }
    .col-setting-wrapper{
      position: absolute !important;
      top: 65px;
      right: 0px;
      padding: 0px !important;
      z-index: 3000;
      max-height: 250px;
      overflow: auto;
    }
    .search-icon{
      position: absolute !important;
      top: 12px !important;
      right: 0px !important;
    }
    .search-open{
      width: 100%;
    }
    .search-close{
      width: 0%;
    }
    .box-search{
      background-color: #fff;
      height: 44px;
      position: inherit;
      top: 10px;
      vertical-align: top;
      border-radius: 2px;
      box-shadow: 0 2px 2px 0 rgba(0,0,0,0.16), 0 0 0 1px rgba(0,0,0,0.08);
      transition: box-shadow 200ms cubic-bezier(0.4, 0.0, 0.2, 1);
    }
    .box-search-icon{
      float: right;
    }
    .box-search-icon button{
      margin-top: 2px;
    }
    .box-search-input-wrapper{
      overflow: hidden;
      height: auto;
      padding: 5px 12px;
    }
    .box-search input{
      border: none;
      padding: 0px;
      margin: 0px;
      height: 34px;
      line-height: 34px;
      width: 100%;
      z-index: 6;
      outline: none;
      font-size: 16px;
    }
  `,
  ],
})
export class HeaderComponent implements AfterViewInit, OnDestroy {
  @Input('mHeader') header: Header;

  @Output() filterChange: EventEmitter<any> = new EventEmitter();

  @ViewChild('globalFilterField') globalFilterField: ElementRef;

  searchOpen = false;
  colSettingOpen = false;

  globalFilterFunction: any;

  documentEditListener: Function;

  colToggleClick: boolean = false;

  constructor(
    @Inject(forwardRef(() => DataTable))
    public dt: DataTable,
    public renderer: Renderer2,
  ) {}

  ngAfterViewInit() {
    if (this.globalFilterField) {
      this.globalFilterFunction = this.renderer.listen(
        this.globalFilterField.nativeElement,
        'keyup',
        () => {
          this.filterChange.emit({
            value: this.globalFilterField.nativeElement.value,
            type: 'input',
          });
        },
      );
    }
  }

  emitChange() {
    this.filterChange.emit({
      value: this.globalFilterField.nativeElement.value,
      type: 'click',
    });
  }

  toggleSearch(state: boolean) {
    if (!state) {
      this.globalFilterField.nativeElement.value = '';
      this.filterChange.emit({ value: '', type: 'input' });
    } else {
      this.globalFilterField.nativeElement.focus();
    }
    this.searchOpen = state;
  }

  openColSetting() {
    this.colSettingOpen = true;
    this.colToggleClick = true;
    this.bindDocumentEditListener();
  }

  closeColSetting() {
    this.colSettingOpen = false;
    if (!this.colToggleClick) this.unbindDocumentEditListener();
  }

  toggleColumn(col) {
    col.hidden = !col.hidden;
  }

  bindDocumentEditListener() {
    if (!this.documentEditListener) {
      this.documentEditListener = this.renderer.listen(
        'document',
        'click',
        event => {
          this.closeColSetting();
        },
      );
    }
    setTimeout(() => {
      this.colSettingOpen = true;
      this.colToggleClick = false;
    }, 0);
  }

  unbindDocumentEditListener() {
    if (this.documentEditListener) {
      this.documentEditListener();
      this.documentEditListener = null;
    }
  }

  ngOnDestroy() {
    if (this.globalFilterFunction) {
      this.globalFilterFunction();
    }

    this.unbindDocumentEditListener();
  }
}
