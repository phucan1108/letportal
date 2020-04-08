import { OnlineUser } from 'services/chat.service';

export interface ChatOnlineUser extends OnlineUser{
    incomingMessages: number
}

export interface ChatRoom{
    chatRoomId: string
    roomName: string
    type: RoomType
    participants: ChatOnlineUser[]
    chatSessions: ChatSession[]
    currentSession: ChatSession
    lastVisited: number 
}

export enum RoomType{
    Double,
    Group
}

export interface DoubleChatRoom extends ChatRoom{
    invitee: ChatOnlineUser
}

export interface ChatSession{
    sessionId: string
    chatRoomId: string
    messages: ExtendedMessage[]
    previousSessionId: string
    nextSessionId: string
}

export interface Message{
    message: string
    formattedMessage: string
    attachmentFiles: AttachmentFile[]
    userName: string
    timeStamp: number
    createdDate: Date
}

export interface ExtendedMessage extends Message{
    isReceived: boolean
    hasAttachmentFile: boolean
    chatSessionId: string
}

export interface AttachmentFile{
    downloadUrl: string
    fileType: string
    fileName: string
}