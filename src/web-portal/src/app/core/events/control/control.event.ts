import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';

export interface ControlEventExecution{
    execute(
        control: FormControl, 
        formGroup: FormGroup,        
        pageService: PageService,
        bindName: string,
        defaultValue: any,
        oldValue: any,
        newValue: any);
}