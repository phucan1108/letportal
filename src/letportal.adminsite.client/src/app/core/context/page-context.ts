import { Page } from 'services/portal.service';
import { BoundSection } from './bound-section';

export class PageContext{
    constructor(
        public page: Page,
        private sections: BoundSection[]){

    }

    get(sectionName: string): BoundSection{
        return this.sections.find(a => a.name === sectionName)
    }
}