import { ShellConfig } from 'app/core/shell/shell.model';
import { State, Selector, Action, StateContext } from '@ngxs/store';
import * as ShellActions from './shell.actions'
import * as _ from 'lodash';


export interface ShellStateModel {
    shellConfigs: Array<ShellConfig>
    filterState: ShellActions.ShellAction
}

@State<ShellStateModel>({
    name: 'shelldata',
    defaults: {
        shellConfigs: [],
        filterState: ShellActions.ShellAction
    }
})
export class ShellDataState {
    @Selector()
    public static getState(state: ShellStateModel) {
        return state;
    }

    @Action(ShellActions.AppendShellConfigsAction)
    public add(ctx: StateContext<ShellStateModel>, { appendingConfigs }: ShellActions.AppendShellConfigsAction) {
        const state = ctx.getState()
        const distinctConfigs = _.uniq(_.concat(state.shellConfigs, appendingConfigs))
        ctx.setState({
            ...state,
            shellConfigs: distinctConfigs,
            filterState: ShellActions.AppendShellConfigsAction
        })
    }
}