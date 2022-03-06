export interface OnlineSubcriber{
    userId: string
    subcriberId: string
    userName: string
    roles: string[]
    lastClickedTs: number
    lastUnreadMessages: number
    groups: MessageGroup[]    
}

export interface MessageGroup{
    id: string
    name: string
    subcriberId: string
    channelCode: string
    mute: boolean
    createdDate: Date
    modifiedDate: Date
    icon: string
    lastVisitedTs: number
    numberOfUnreadMessages: number
    lastMessage: NotificationMessage
    messages: NotificationMessage[]
}

export interface NotificationMessage{
    notificationBoxId: string
    subcriberId: string
    messageGroupId: string
    messageId: string
    messageGroupName: string
    type: NotificationType
    shortMessage: string
    isDirty: boolean
    receivedDate: Date,
    clickedDate: Date,
    receivedDateTs: number
}

export enum NotificationType{
    Info = 0,
    Warning = 1,
    Critical = 2
}

export interface FetchedNotificationMessageRequest{
    subcriberId: string
    messageGroupId: string
    lastFectchedTs: number
    selectedTypes: NotificationType[]
}

export const NOTIFICATION_TYPE_RES = [
    { name: 'Info', value: NotificationType.Info, icon: 'info', color: 'primary' },
    { name: 'Warning', value: NotificationType.Warning, icon: 'warning', color: 'warn' },
    { name: 'Critical', value: NotificationType.Critical, icon: 'gpp_bad', color: 'accent' }
]