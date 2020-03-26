import { ControlEventExecution } from './control.event';
import { ControlEvent } from '../decorators/event.decorator';
import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import { ObjectUtils } from 'app/core/utils/object-util';

@ControlEvent({
    name: 'change',
    allowedControlTypes: '*'
})
export class ChangeControlEvent implements ControlEventExecution {
    public execute(
        control: PageRenderedControl<DefaultControlOptions>,
        pageService: PageService,
        formControl: FormControl,
        defaultValue: any,
        newValue: any) {
            formControl.setValue(newValue)
    }

}