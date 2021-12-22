import { NotificationMessage, OnlineSubcriber } from "app/core/models/notification.model";

export interface SubcribeToServerEvent{
    onlineSubcriber: OnlineSubcriber
}

export interface ClickedToNotificationBoxEvent{
    lastClickedTs: number
}

export interface ReceivedNotificationEvent{
    notificationMessage: NotificationMessage
}