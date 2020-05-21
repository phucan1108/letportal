import {
  AfterContentInit,
  Component,
  ContentChildren,
  EmbeddedViewRef,
  Input,
  OnDestroy,
  OnInit,
  QueryList,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { MomentumTemplate } from './template.directive';

@Component({
  selector: 'm-columnEditorTemplateLoader',
  template: ``,
})
export class ColumnEditorTemplateLoader implements OnInit, OnDestroy {
  @Input() column: any;

  @Input() row: any;

  @Input() rowIndex: any;

  view: EmbeddedViewRef<any>;

  constructor(public viewContainer: ViewContainerRef) {}

  ngOnInit() {
    this.view = this.viewContainer.createEmbeddedView(
      this.column.editorTemplate,
      {
        $implicit: this.column,
        row: this.row,
        rowIndex: this.rowIndex,
      },
    );
  }

  ngOnDestroy() {
    this.view.destroy();
  }
}

@Component({
  selector: 'm-column',
  template: ``,
})
export class ColumnComponent implements AfterContentInit {
  @Input() field: string;
  @Input() header: string;
  @Input() subHeader: string;
  @Input() footer: string;
  @Input() sortable: boolean;
  @Input() editable: boolean;
  @Input() editTrigger: string = 'cell';
  @Input() deletable: boolean;
  @Input() resettable: boolean;
  @Input() hidden: boolean;
  @Input() frozen: boolean;
  @Input() colHeadClass: any;
  @Input() colSubHeadClass: any;
  @Input() colBodyClass: any;

  @ContentChildren(MomentumTemplate) templates: QueryList<any>;

  public headerTemplate: TemplateRef<any>;
  public subHeaderTemplate: TemplateRef<any>;
  public bodyTemplate: TemplateRef<any>;
  public footerTemplate: TemplateRef<any>;
  public editorTemplate: TemplateRef<any>;

  constructor() {}

  ngAfterContentInit() {
    this.templates.forEach(item => {
      switch (item.getType()) {
        case 'header':
          this.headerTemplate = item.template;
          break;

        case 'subHeader':
          this.subHeaderTemplate = item.template;
          break;

        case 'body':
          this.bodyTemplate = item.template;
          break;

        case 'footer':
          this.footerTemplate = item.template;
          break;

        case 'editor':
          this.editorTemplate = item.template;
          break;

        default:
          this.bodyTemplate = item.template;
          break;
      }
    });
  }
}
