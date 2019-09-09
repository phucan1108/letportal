import { ControlEventExecution } from './control.event';
import { ControlEvent } from '../decorators/event.decorator';
import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';

@ControlEvent({
    name: 'change'
})
export class ChangeControlEvent implements ControlEventExecution {
    public execute(control: FormControl, formGroup: FormGroup, pageService: PageService, bindName: string, defaultValue: any, oldValue: any, newValue: any) {
        control.setValue(newValue)
    }
    
}