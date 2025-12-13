
import {NgModule} from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatPaginatorModule } from '@angular/material/paginator';

@NgModule({
  imports: [MatCardModule, MatIconModule, MatCheckboxModule, MatInputModule, MatFormFieldModule, MatButtonModule, MatListModule, MatPaginatorModule],
  exports: [MatCardModule, MatIconModule, MatCheckboxModule, MatInputModule, MatFormFieldModule, MatButtonModule, MatListModule, MatPaginatorModule],
})
export class MaterialModule { }
