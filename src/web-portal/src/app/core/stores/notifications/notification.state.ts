import { Injectable } from "@angular/core";
import { Action, State, StateContext, StateToken } from "@ngxs/store";
import { OnlineSubcriber } from "app/core/models/notification.model";
import * as NotificationActions from './notification.actions';
export const NOTIFICATION_STATE_TOKEN = new StateToken<NotificationStateModel>('notifications');

export interface NotificationStateModel{
    onlineSubcriber: OnlineSubcriber
}

@State<NotificationStateModel>({
    name: NOTIFICATION_STATE_TOKEN,
    defaults: {
        onlineSubcriber : null
    }
})
@Injectable()
export class NotificationState{
    @Action(NotificationActions.SubcribeToServer)
    public subcribeToServer(ctx: StateContext<NotificationStateModel>, { event }: NotificationActions.SubcribeToServer) {
        return ctx.setState({
            onlineSubcriber: event.onlineSubcriber
        })
    }
    @Action(NotificationActions.ReceiveNewNotification)
    public receiveNewNotification(ctx: StateContext<NotificationStateModel>, {event}: NotificationActions.ReceiveNewNotification){
        const state = ctx.getState()
        // return ctx.setState(
        //     patch({
        //         onlineSubcriber: {
        //             ...state.onlineSubcriber,
        //             messages: [
        //                 ...state.onlineSubcriber.messages,
        //                 event.notificationMessage
        //             ]
        //         }  
        //     }))
    }

    @Action(NotificationActions.ClickedOnNotificationBox)
    public clickedOnNotificationBox(ctx: StateContext<NotificationStateModel>, {event}: NotificationActions.ClickedOnNotificationBox){
        const state = ctx.getState()
        return ctx.setState({
            onlineSubcriber: {
                ...state.onlineSubcriber,
                lastClickedTs: event.lastClickedTs
            }
        })
    }
}