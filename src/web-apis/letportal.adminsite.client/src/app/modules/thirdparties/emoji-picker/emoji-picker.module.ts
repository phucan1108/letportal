import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { EmojiButtonComponent } from './components/emoji-button.component';
import { EmojiCategoriesComponent } from './components/emoji-categories.component';
import { EmojiCategoryComponent } from './components/emoji-category.component';
import { EmojiContentComponent } from './components/emoji-content.component';
import { EmojiFooterComponent } from './components/emoji-footer.component';
import { EmojiHeaderComponent } from './components/emoji-header.component';
import { EmojiListComponent } from './components/emoji-list.component';
import { EmojiPickerComponent } from './components/emoji-picker.component';
import { EmojiSearchComponent } from './components/emoji-search.component';
import { EmojiPickerApiDirective } from './directives/emoji-picker-api.directive';
import { EmojiPickerCaretDirective } from './directives/emoji-picker-caret.directive';
import { EmojiEmptyCategoryPipe } from './pipes/emoji-empty-category.pipe';

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
  providers: []
})
export class EmojiPickerModule {
  static forRoot(): ModuleWithProviders<EmojiPickerModule> {
    return {
      ngModule: EmojiPickerModule,
      providers: []
    };
  }
 }
