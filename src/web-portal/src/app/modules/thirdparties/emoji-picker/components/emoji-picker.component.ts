import { Component, EventEmitter, Output, ElementRef, Renderer2 } from '@angular/core';
import { DIRECTIONS } from '../misc/picker-directions';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime} from 'rxjs/operators';

@Component({
  selector: 'emoji-picker',
  styles: [':host { position: absolute; z-index: 9999; }'],
  template: `
  <emoji-content (emoji-selection)="selectionEmitter.emit($event)"></emoji-content>
  `,
  host: {
    '(document:click)': 'onBackground($event)',
    '(click)': '_lastHostMousedownEvent = $event',
    '(window:resize)': '_windowResize.next($event)'
  }
})

export class EmojiPickerComponent {
  @Output('emoji-select') selectionEmitter = new EventEmitter();
  @Output('picker-close') pickerCloseEmitter = new EventEmitter(); 

  public _lastHostMousedownEvent;
  public _currentTarget: ElementRef;
  public _currentDirection: DIRECTIONS;

  public _windowResize = new Subject<any>();
  public _destroyed = new Subject<boolean>();

  constructor(private _renderer: Renderer2, private _el: ElementRef) {
    this._windowResize
      .pipe(takeUntil(this._destroyed), debounceTime(100))
      .subscribe(event => {
        this.setPosition(this._currentTarget, this._currentDirection);
      })
  }

  setPosition(target: ElementRef, directionCode: DIRECTIONS = DIRECTIONS.bottom) {
    if (!target) {
      return console.error('Emoji-Picker: positioning failed due to missing target element or direction code');
    }

    this._renderer.setStyle(this._el.nativeElement, 'transform', '');

    /** Store anchor and direction */
    this._currentTarget = target;
    this._currentDirection = directionCode;

    const targetBorders = target.nativeElement.getBoundingClientRect(),
      thisBorders = this._el.nativeElement.getBoundingClientRect();

    let heightCorrection = 0, widthCorrection = 0;

    /** Setting up centering of picker for all cases */
    switch (this._currentDirection) {
      case DIRECTIONS.top:
      case DIRECTIONS.bottom:
        widthCorrection = targetBorders.left - thisBorders.left + (targetBorders.width - thisBorders.width) / 2;
        break;
      case DIRECTIONS.left:
      case DIRECTIONS.right:
        heightCorrection = targetBorders.top - thisBorders.top + (targetBorders.height - thisBorders.height) / 2;
        break;
    }

    /** Setting up relative positioning for all cases */
    switch (this._currentDirection) {
      case DIRECTIONS.top:
        heightCorrection = targetBorders.top - thisBorders.bottom;
        break;
      case DIRECTIONS.left:
        widthCorrection = targetBorders.left - thisBorders.right;
        break;
      case DIRECTIONS.right:
        widthCorrection = targetBorders.right - thisBorders.left;
        break;
      case DIRECTIONS.bottom:
        heightCorrection = targetBorders.bottom - thisBorders.top;
        break;
    }

    /** Correcting positioning due to overflow problems */
    if (thisBorders.bottom + heightCorrection > window.innerHeight) {
      heightCorrection += window.innerHeight - (thisBorders.bottom + heightCorrection);
    }

    if (thisBorders.top + heightCorrection < 0) {
      heightCorrection -= (thisBorders.top + heightCorrection);
    }

    if (thisBorders.right + widthCorrection > window.innerWidth) {
      widthCorrection += window.innerWidth - (thisBorders.right + widthCorrection);
    }

    if (thisBorders.left + widthCorrection < 0) {
      widthCorrection -= (thisBorders.left + widthCorrection);
    }
    
    /** set the position adjustments to the emoji picker element */
    this._renderer.setStyle(this._el.nativeElement, 'transform', `translate(${widthCorrection}px,${heightCorrection}px)`);
  }

  onBackground(event) {
    /** internal mousedowns are ignored */
    if (event === this._lastHostMousedownEvent || event.emojiPickerExempt) {
      return;
    }

    this.pickerCloseEmitter.emit(event);
  }

  ngOnDestroy() {
    this._destroyed.next(true);
  }
}
