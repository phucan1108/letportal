import { BoundControl } from 'app/core/context/bound-control';
import { DefaultControlOptions, PageRenderedControl } from 'app/core/models/page.model';
import { PageService } from 'services/page.service';

export interface ControlEventExecution{
    execute(
        control: PageRenderedControl<DefaultControlOptions>,
        pageService: PageService,
        boundControl: BoundControl,
        defaultValue: any,
        newValue: any);
}