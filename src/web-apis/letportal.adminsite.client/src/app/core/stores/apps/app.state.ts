import { App } from 'services/portal.service';
import * as AppActions from './app.action'
import { State, Selector, Action, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
export interface SelectedAppStateModel {
    selectedApp: App
    filterState: AppActions.AppAction
}

@State<SelectedAppStateModel>({
    name: 'selectedApp',
    defaults: {
        selectedApp: null,
        filterState: null
    }
})
@Injectable()
export class SelectedAppState {
    @Selector()
    public static getState(state: SelectedAppStateModel) {
        return state;
    }

    @Action(AppActions.UserSelectAppAction)
    public add(ctx: StateContext<SelectedAppStateModel>, { app }: AppActions.UserSelectAppAction) {
      const state = ctx.getState()
      return ctx.setState({
          ...state,
          selectedApp: app,
          filterState: AppActions.UserSelectAppAction
      })
    }
}