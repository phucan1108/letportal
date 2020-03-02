import { ControlEventExecution } from './control.event';
import { ControlEvent } from '../decorators/event.decorator';
import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';

@ControlEvent({
    name: 'rebound',
    allowedControlTypes: '*'
})
export class ReboundControlEvent implements ControlEventExecution {
    execute(
        control: PageRenderedControl<DefaultControlOptions>,        
        pageService: PageService,
        formControl: FormControl,
        defaultValue: any,
        newValue: any) {
        const foundData = pageService.getDataByBindName(control.defaultOptions.bindname)
        formControl.setValue(foundData)
    }
}