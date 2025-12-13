import { MessageGroup, NotificationMessage, OnlineSubcriber } from "app/core/models/notification.model";

export interface SubcribeToServerEvent{
    onlineSubcriber: OnlineSubcriber
}

export interface ClickedOnNotificationBoxEvent{
    lastClickedTs: number
}

export interface ClickedOnMessageGroupEvent{
    messageGroup: MessageGroup
}

export interface ReceivedNotificationEvent{
    notificationMessage: NotificationMessage
}

export interface ReceivedNewMessageGroupEvent{
    messageGroup: MessageGroup
}