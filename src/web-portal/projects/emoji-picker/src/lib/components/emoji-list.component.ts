import { Component, Input, ViewChildren, QueryList, forwardRef, Output, EventEmitter } from '@angular/core';
import { EmojiCategoryComponent } from './emoji-category.component';

@Component({
  selector: 'emoji-list',
  styleUrls: ['../styles/emoji-list.scss'],
  template: `
  <div class="emoji-list">
    <ng-container *ngFor="let emojiCategory of emojis | notEmptyEmojiCategory">
      <emoji-category [category]="emojiCategory"></emoji-category>
      <div class="emoji-buttons">
        <emoji-button 
        *ngFor="let emoji of emojiCategory.emojis"
        (selection)="emojiSelectionEmitter.emit($event)"
        [emoji]="emoji"></emoji-button>
      </div>
    </ng-container>
  </div>
  `
})

export class EmojiListComponent {
  @ViewChildren(forwardRef(() => EmojiCategoryComponent)) emojiCategoryComponents: QueryList<EmojiCategoryComponent>;
  @Input('emojis') emojis;
  @Output('emoji-selection') emojiSelectionEmitter = new EventEmitter<any>();

  constructor() { }

  public selectCategory(event) {
    this.emojiCategoryComponents.forEach((categoryCmp:EmojiCategoryComponent) => {
      if (categoryCmp['category'].name === event.name) {
        categoryCmp.scrollIntoView();
      }
    });
  }
}
