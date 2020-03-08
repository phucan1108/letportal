import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';

export interface ControlEventExecution{
    execute(
        control: PageRenderedControl<DefaultControlOptions>,        
        pageService: PageService,
        formControl: FormControl,
        defaultValue: any,
        newValue: any);
}