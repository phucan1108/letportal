import * as PageControlActions from './pagecontrol.actions'
import { TriggeredControlEvent } from 'app/core/models/page.model';
import { State, Selector, Action, StateContext } from '@ngxs/store';

export interface PageControlEventStateModel {
    effectedControlEvent: TriggeredControlEvent,
    filterState: PageControlActions.PageControlEvent
}

@State<PageControlEventStateModel>({
    name: 'controlevents',
    defaults: {
        effectedControlEvent: null,
        filterState: null
    }
})
export class PageControlEventState {
    constructor(
    ) { }

    @Selector()
    public static getState(state: PageControlEventStateModel) {
        return state;
    }

    @Action(PageControlActions.TriggerControlChangeValueEvent)
    public triggeredControlEvent(ctx: StateContext<PageControlEventStateModel>, { event }: PageControlActions.TriggerControlChangeValueEvent) {
        const state = ctx.getState()
        ctx.setState({
            ...state,
            effectedControlEvent: event,
            filterState: PageControlActions.TriggerControlChangeValueEvent
        })
    }
}