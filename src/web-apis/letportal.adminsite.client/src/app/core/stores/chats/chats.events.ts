import { ChatOnlineUser, ChatRoom, DoubleChatRoom, ChatSession, Message, ExtendedMessage, ParticipantVideo, VideoRoomModel, RtcIceServer } from 'app/core/models/chat.model';

export interface TakeUserOnlineEvent{
    user: ChatOnlineUser
}

export interface ActiveDoubleChatActionEvent{
    chatSession: ChatSession
}

export interface ClickedOnChatUserEvent{
    inviee: ChatOnlineUser
}

export interface InitialReceivedChatRoomEvent{
    inviee: ChatOnlineUser
}

export interface NotifyNewIncomingMessageEvent{
    chatRoomId: string
}

export interface ClickedOnChatBoxIconEvent{
    chatRoomId: string
}

export interface OpenNewDoubleChatRoomEvent{
    chatRoom: DoubleChatRoom
}

export interface SendNewMessageEvent{
    chatRoomId: string,
    chatSessionId: string
    receiver: string
    message: ExtendedMessage
    lastSentHashCode: string
}

export interface ReceivedMessageEvent{
    chatRoomId: string,
    chatSessionId: string
    message: ExtendedMessage   
}

export interface ReceivedMessageFromAnotherDeviceEvent{
    chatRoomId: string,
    chatSessionId: string,
    message: ExtendedMessage,
    lastHashCode: string
}

export interface LoadingMoreSessionEvent{
    chatRoomId: string,
    chatSessionId: string,
    previousSessionId: string
}

export interface LoadedMoreSessionEvent {
    chatSession: ChatSession
}

export interface AddedNewSessionEvent{
    chatSession: ChatSession
}

export interface LoadedAllAvailableUsersEvent{
    availableUsers: ChatOnlineUser[]
}

export interface IncomingOnlineUserEvent{
    onlineUser: ChatOnlineUser
}

export interface IncomingOfflineUserEvent{
    offlineUser: string
}

export interface IncomingMessageFromUnloadUserEvent{
    sender: string
}

export interface IncomingVideoCallEvent{
    caller: ParticipantVideo
}

export interface HandshakedVideoCallEvent{
    videoRoom: VideoRoomModel
}

export interface ReceivedIceServerEvent{
    iceServers: RtcIceServer[]
}