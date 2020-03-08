import { ControlEventExecution } from './control.event';
import { ControlEvent } from '../decorators/event.decorator';
import { FormControl, FormGroup } from '@angular/forms';
import { PageService } from 'services/page.service';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import { ControlType } from 'services/portal.service';

@ControlEvent({
    name: 'clean',
    allowedControlTypes: '*'
})
export class CleanControlEvent implements ControlEventExecution{
    execute(
        control: PageRenderedControl<DefaultControlOptions>,        
        pageService: PageService,
        formControl: FormControl,
        defaultValue: any,
        newValue: any) {
            if(control.type === ControlType.Checkbox
                || control.type === ControlType.Slide){
                    formControl.setValue(control.defaultOptions.allowZero ? 0 : control.defaultOptions.allowYesNo ? 'N' : false)
                }
                else{
                    formControl.setValue(null)
                }
            
    }
}