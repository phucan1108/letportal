import { BoundControl } from 'app/core/context/bound-control';
import { DefaultControlOptions, PageRenderedControl } from 'app/core/models/page.model';
import { PageService } from 'services/page.service';
import { ControlType } from 'services/portal.service';
import { ControlEvent } from '../decorators/event.decorator';
import { ControlEventExecution } from './control.event';

@ControlEvent({
    name: 'clean',
    allowedControlTypes: '*'
})
export class CleanControlEvent implements ControlEventExecution{
    execute(
        control: PageRenderedControl<DefaultControlOptions>,
        pageService: PageService,
        boundControl: BoundControl,
        defaultValue: any,
        newValue: any) {
            if(boundControl.type === ControlType.Checkbox
                || boundControl.type === ControlType.Slide){
                    boundControl.getForm().setValue(control.defaultOptions.allowZero ? 0 : control.defaultOptions.allowYesNo ? 'N' : false)
                }
                else{
                    boundControl.getForm().setValue(null)
                }

    }
}