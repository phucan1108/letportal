import { PageService } from 'services/page.service';
import { BoundControl } from '../context/bound-control';
import { BoundSection } from '../context/bound-section';
import { PageContext } from '../context/page-context';

export interface InterceptorContext{
    pageService: PageService
    pageContext: PageContext
    sectionRef: BoundSection
    controlRef: BoundControl
}