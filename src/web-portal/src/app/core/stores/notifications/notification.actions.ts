import { ClickedOnMessageGroupEvent, ClickedOnNotificationBoxEvent, ReceivedNewMessageGroupEvent, ReceivedNotificationEvent, SubcribeToServerEvent as SubcribedToServerEvent } from "./notification.events"

const NOTIFICATION_ACTION = '[Notification Action]'

export class NotificationAction {
    public static readonly type = ''
}

export class SubcribeToServer implements NotificationAction {
    public static readonly type = `${NOTIFICATION_ACTION} Subcribe to server`
    constructor(public event: SubcribedToServerEvent) { }
}

export class ReceivedNewNotification implements NotificationAction {
    public static readonly type = `${NOTIFICATION_ACTION} Receive notification from server`
    constructor(public event: ReceivedNotificationEvent) { }
}

export class ReceivedNewMessageGroup implements NotificationAction {
    public static readonly type = `${NOTIFICATION_ACTION} Receive new message group from server`
    constructor(public event: ReceivedNewMessageGroupEvent) { }
}

export class ClickedOnNotificationBox implements NotificationAction {
    public static readonly type = `${NOTIFICATION_ACTION} Clicked on notification box`
    constructor(public event: ClickedOnNotificationBoxEvent) { }
}

export class ClickedOnMessageGroup implements NotificationAction {
    public static readonly type = `${NOTIFICATION_ACTION} Clicked on message group`
    constructor(public event: ClickedOnMessageGroupEvent) { }
}

export type All =
    NotificationAction |
    SubcribeToServer |
    ReceivedNewNotification |
    ClickedOnNotificationBox | 
    ClickedOnMessageGroup |
    ReceivedNewMessageGroup 