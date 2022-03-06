import { Injectable } from "@angular/core";
import { Action, State, StateContext, StateToken } from "@ngxs/store";
import { MessageGroup, NotificationMessage, OnlineSubcriber } from "app/core/models/notification.model";
import { ArrayUtils } from "app/core/utils/array-util";
import { ObjectUtils } from "app/core/utils/object-util";
import * as NotificationActions from './notification.actions';
export const NOTIFICATION_STATE_TOKEN = new StateToken<NotificationStateModel>('notifications');

export interface NotificationStateModel{
    onlineSubcriber: OnlineSubcriber
    lastReceivedMessage: NotificationMessage
    lastReceivedMessageGroup: MessageGroup
}

@State<NotificationStateModel>({
    name: NOTIFICATION_STATE_TOKEN,
    defaults: {
        onlineSubcriber : null,
        lastReceivedMessage: null,
        lastReceivedMessageGroup: null
    }
})
@Injectable()
export class NotificationState{
    @Action(NotificationActions.SubcribeToServer)
    public subcribeToServer(ctx: StateContext<NotificationStateModel>, { event }: NotificationActions.SubcribeToServer) {
        return ctx.setState({
            onlineSubcriber: event.onlineSubcriber,
            lastReceivedMessage: null,
            lastReceivedMessageGroup: null
        })
    }
    @Action(NotificationActions.ReceivedNewNotification)
    public receiveNewNotification(ctx: StateContext<NotificationStateModel>, {event}: NotificationActions.ReceivedNewNotification){
        const state = ctx.getState()
        ctx.setState({
            onlineSubcriber: state.onlineSubcriber,
            lastReceivedMessage: event.notificationMessage,
            lastReceivedMessageGroup: state.lastReceivedMessageGroup
        })
    }

    @Action(NotificationActions.ReceivedNewMessageGroup)
    public receivedNewMessageGroup(ctx: StateContext<NotificationStateModel>, {event}: NotificationActions.ReceivedNewMessageGroup){
        const state = ctx.getState()
    }

    @Action(NotificationActions.ClickedOnNotificationBox)
    public clickedOnNotificationBox(ctx: StateContext<NotificationStateModel>, {event}: NotificationActions.ClickedOnNotificationBox){
        const state = ctx.getState()
        ctx.setState({
            onlineSubcriber: {
                ...state.onlineSubcriber,
                lastClickedTs: event.lastClickedTs
            },
            lastReceivedMessage: state.lastReceivedMessage,
            lastReceivedMessageGroup: state.lastReceivedMessageGroup
        })
    }

    @Action(NotificationActions.ClickedOnMessageGroup)
    public clickedOnMessageGroup(ctx: StateContext<NotificationStateModel>, {event}: NotificationActions.ClickedOnMessageGroup){
        const state = ctx.getState()
        let groups = ObjectUtils.clone(state.onlineSubcriber.groups)
        groups = ArrayUtils.updateOneItem(groups, event.messageGroup, a => a.id === event.messageGroup.id) 
        ctx.setState({
            onlineSubcriber: {
                ...state.onlineSubcriber,
                groups: groups
            },
            lastReceivedMessage: state.lastReceivedMessage,
            lastReceivedMessageGroup: state.lastReceivedMessageGroup
        })
    }
}