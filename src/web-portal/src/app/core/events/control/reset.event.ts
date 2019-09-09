import { ControlEventExecution } from './control.event';
import { ControlEvent } from '../decorators/event.decorator';
import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';

@ControlEvent({
    name: 'reset'
})
export class ResetControlEvent implements ControlEventExecution{
    execute(
        control: FormControl, 
        formGroup: FormGroup, 
        pageService: PageService, 
        bindName: string,
        defaultValue: any, 
        oldValue: any, 
        newValue: any) {
        control.setValue(defaultValue)
    }
}