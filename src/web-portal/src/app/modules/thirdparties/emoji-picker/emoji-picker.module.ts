import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EmojiButtonComponent } from './components/emoji-button.component';
import { EmojiCategoriesComponent } from './components/emoji-categories.component';
import { EmojiCategoryComponent } from './components/emoji-category.component';
import { EmojiContentComponent } from './components/emoji-content.component';
import { EmojiFooterComponent } from './components/emoji-footer.component';
import { EmojiHeaderComponent } from './components/emoji-header.component';
import { EmojiListComponent } from './components/emoji-list.component';
import { EmojiSearchComponent } from './components/emoji-search.component';

import { EmojiPickerApiDirective } from './directives/emoji-picker-api.directive';
import { EmojiPickerCaretDirective } from './directives/emoji-picker-caret.directive';
import { EmojiEmptyCategoryPipe } from './pipes/emoji-empty-category.pipe';
import { EmojiPickerComponent } from './components/emoji-picker.component';


@NgModule({
  declarations: [
    EmojiPickerApiDirective,
    EmojiPickerCaretDirective,
    EmojiButtonComponent,
    EmojiContentComponent,
    EmojiPickerComponent,
    EmojiListComponent,
    EmojiHeaderComponent,
    EmojiSearchComponent,
    EmojiCategoriesComponent,
    EmojiCategoryComponent,
    EmojiFooterComponent,
    EmojiEmptyCategoryPipe
  ],
  imports: [
    CommonModule
  ],
  exports: [
    EmojiPickerApiDirective,
    EmojiPickerCaretDirective,
    EmojiButtonComponent,
    EmojiContentComponent,
    EmojiPickerComponent,
    EmojiListComponent,
    EmojiHeaderComponent,
    EmojiSearchComponent,
    EmojiCategoriesComponent,
    EmojiCategoryComponent,
    EmojiFooterComponent
  ],
  providers: [],
  entryComponents: [
    EmojiPickerComponent
  ]
})
export class EmojiPickerModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: EmojiPickerModule,
      providers: []
    };
  }
 }
