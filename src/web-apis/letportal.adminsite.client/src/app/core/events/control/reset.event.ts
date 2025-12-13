import { BoundControl } from 'app/core/context/bound-control';
import { DefaultControlOptions, PageRenderedControl } from 'app/core/models/page.model';
import { PageService } from 'services/page.service';
import { ControlEvent } from '../decorators/event.decorator';
import { ControlEventExecution } from './control.event';

@ControlEvent({
    name: 'reset',
    allowedControlTypes: '*'
})
export class ResetControlEvent implements ControlEventExecution{
    execute(
        control: PageRenderedControl<DefaultControlOptions>,
        pageService: PageService,
        boundControl: BoundControl,
        defaultValue: any,
        newValue: any) {
            boundControl.getForm().setValue(defaultValue)
    }
}