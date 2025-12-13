import { Component, EventEmitter, Output, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';


@Component({
    selector: 'emoji-search',
    styleUrls: ['../styles/emoji-search.scss'],
    template: `
  <input type="text" autocomplete="off" #input (input)="onInputChange($event)" placeholder="Search"/>
  `,
    standalone: false
})

export class EmojiSearchComponent implements OnDestroy {
  @Output('search') searchEmitter: EventEmitter<string> = new EventEmitter();
  @ViewChild('input') input: ElementRef;

  private _searchValue: Subject<string> = new Subject();
  private _destroyed = new Subject<boolean>();

  constructor() {
    this._searchValue
      .pipe(takeUntil(this._destroyed))
      .subscribe(value => {
        this.searchEmitter.emit(value);
      });
  }

  onInputChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.handleInputChange(value);
  }

  handleInputChange(value: string) {
    this._searchValue.next(value);
  }

  ngOnDestroy() {
    this._destroyed.next(true);
  }
}
