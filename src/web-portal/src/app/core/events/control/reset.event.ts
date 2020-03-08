import { ControlEventExecution } from './control.event';
import { ControlEvent } from '../decorators/event.decorator';
import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';

@ControlEvent({
    name: 'reset',
    allowedControlTypes: '*'
})
export class ResetControlEvent implements ControlEventExecution{
    execute(
        control: PageRenderedControl<DefaultControlOptions>,        
        pageService: PageService,
        formControl: FormControl,
        defaultValue: any,
        newValue: any) {
            formControl.setValue(defaultValue)
    }
}