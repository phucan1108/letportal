import { BoundControl } from 'app/core/context/bound-control';
import { DefaultControlOptions, PageRenderedControl } from 'app/core/models/page.model';
import { PageService } from 'services/page.service';
import { ControlEvent } from '../decorators/event.decorator';
import { ControlEventExecution } from './control.event';

@ControlEvent({
    name: 'change',
    allowedControlTypes: '*'
})
export class ChangeControlEvent implements ControlEventExecution {
    public execute(
        control: PageRenderedControl<DefaultControlOptions>,
        pageService: PageService,
        boundControl: BoundControl,
        defaultValue: any,
        newValue: any) {
            boundControl.getForm().setValue(newValue)
    }

}