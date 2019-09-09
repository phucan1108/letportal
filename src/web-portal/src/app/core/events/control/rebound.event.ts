import { ControlEventExecution } from './control.event';
import { ControlEvent } from '../decorators/event.decorator';
import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';

@ControlEvent({
    name: 'rebound'
})
export class ReboundControlEvent implements ControlEventExecution {
    execute(
        control: FormControl,
        formGroup: FormGroup,
        pageService: PageService,
        bindName: string,
        defaultValue: any,
        oldValue: any,
        newValue: any) {
        const foundData = pageService.getDataByBindName(bindName)
        control.setValue(foundData)
    }
}