import { ClickedToNotificationBoxEvent, ReceivedNotificationEvent, SubcribeToServerEvent as SubcribedToServerEvent } from "./notification.events"

const NOTIFICATION_ACTION = '[Notification Action]'

export class NotificationAction {
    public static readonly type = ''
}

export class SubcribeToServer implements NotificationAction{
    public static readonly type = `${NOTIFICATION_ACTION} Subcribe to server`
    constructor(public event: SubcribedToServerEvent){ }
} 

export class ReceiveNewNotification implements NotificationAction{
    public static readonly type = `${NOTIFICATION_ACTION} Receive notification from server`
    constructor(public event: ReceivedNotificationEvent){}
}

export class ClickedOnNotificationBox implements NotificationAction{
    public static readonly type = `${NOTIFICATION_ACTION} Clicked on notification box`
    constructor(public event: ClickedToNotificationBoxEvent){}
}

export type All = 
    NotificationAction |
    SubcribeToServer |
    ReceiveNewNotification |
    ClickedOnNotificationBox